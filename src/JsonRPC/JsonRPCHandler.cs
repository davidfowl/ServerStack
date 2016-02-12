using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServerStack.Dispatch;
using ServerStack.Serialization;

namespace JsonRPC
{
    public class JsonRPCHandler : IFrameHandler<JObject>
    {
        private readonly Dictionary<string, Func<JObject, JObject>> _callbacks = new Dictionary<string, Func<JObject, JObject>>(StringComparer.OrdinalIgnoreCase);
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JsonRPCHandler> _logger;

        private bool _isBound;
        private readonly IFrameOutput _output;

        public JsonRPCHandler(ILogger<JsonRPCHandler> logger, 
                              IEnumerable<RpcEndpoint> endpoints, 
                              IFrameOutput output,
                              IServiceProvider serviceProvider)
        {
            _logger = logger;
            _output = output;
            _serviceProvider = serviceProvider;

            foreach (var endpoint in endpoints)
            {
                Bind(endpoint.GetType());
            }
        }

        public Task OnFrame(Stream output, JObject request)
        {
            if (_logger.IsEnabled(LogLevel.Verbose))
            {
                _logger.LogVerbose("Received JSON RPC request: {request}", request);
            }
            JObject response = null;

            Func<JObject, JObject> callback;
            if (_callbacks.TryGetValue(request.Value<string>("method"), out callback))
            {
                response = callback(request);
            }
            else
            {
                // If there's no method then return a failed response for this request
                response = new JObject();
                response["id"] = request["id"];
                response["error"] = string.Format("Unknown method '{0}'", request.Value<string>("method"));
            }

            _logger.LogVerbose("Sending JSON RPC response: {data}", response);
            return _output.WriteAsync(output, response);
        }

        private void Bind(Type type)
        {
            if (_isBound)
            {
                throw new NotSupportedException("Can't bind to different objects");
            }

            _isBound = true;

            var methods = new List<string>();

            foreach (var m in type.GetTypeInfo().DeclaredMethods.Where(m => m.IsPublic))
            {
                var methodName = m.GetCustomAttribute<JsonRPCMethodAttribute>()?.MethodName ?? type.FullName + "." + m.Name;

                methods.Add(methodName);

                var parameters = m.GetParameters();

                if (_callbacks.ContainsKey(methodName))
                {
                    throw new NotSupportedException(String.Format("Duplicate definitions of {0}. Overloading is not supported.", m.Name));
                }

                if (_logger.IsEnabled(LogLevel.Verbose))
                {
                    _logger.LogVerbose("RPC method '{methodName}' is bound", methodName);
                }

                _callbacks[methodName] = request =>
                {
                    var response = new JObject();
                    response["id"] = request["id"];

                    var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

                    // Scope per call so that deps injected get disposed
                    using (var scope = scopeFactory.CreateScope())
                    {
                        object value = scope.ServiceProvider.GetService(type);

                        try
                        {
                            var args = request.Value<JArray>("params").Zip(parameters, (a, p) => a.ToObject(p.ParameterType))
                                                                      .ToArray();

                            var result = m.Invoke(value, args);

                            if (result != null)
                            {
                                response["result"] = JToken.FromObject(result);
                            }
                        }
                        catch (TargetInvocationException ex)
                        {
                            response["error"] = ex.InnerException.Message;
                        }
                        catch (Exception ex)
                        {
                            response["error"] = ex.Message;
                        }
                    }

                    return response;
                };
            };
        }
    }
}
