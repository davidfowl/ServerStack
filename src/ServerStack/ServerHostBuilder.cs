using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServerStack.Dispatch;
using ServerStack.Protocols.Tcp;
using ServerStack.Serialization;
using ServerStack.Serialization.Json;

namespace ServerStack
{
    public class ServerHostBuilder<TContext> : IServerHostBuilder<TContext>
    {
        private Action<IServiceCollection> _configureServices;
        private IConfiguration _config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        private Type _startupType;
        private Type _serverFactoryType;

        public IServerHost<TContext> Build()
        {
            var services = new ServiceCollection();
            services.AddInstance<ILoggerFactory>(new LoggerFactory());
            services.AddLogging();
            services.AddOptions();
            services.AddSingleton<IApplicationLifetime, ApplicationLifetime>();

            services.AddSingleton<IFrameOutput, FrameOutput>();
            services.AddSingleton(typeof(Dispatcher<>));

            // known encoders
            services.AddCodec<JObject, JsonEncoder, JsonDecoder>();

            // Add known protocols
            services.AddSingleton(typeof(IContextFactory<TcpContext>), typeof(TcpContextFactory));

            // Add the startup type
            services.AddSingleton(_startupType);
            services.AddSingleton(typeof(IServerFactory), _serverFactoryType);

            _configureServices?.Invoke(services);

            var serviceProvider = services.BuildServiceProvider();

            var startup = serviceProvider.GetService(_startupType);

            var configure = FindConfigureDelegate(_startupType);
            var configureServices = FindConfigureServicesDelegate(_startupType);

            // ConfigureServices
            var applicationServices = configureServices?.Invoke(startup, new object[] { services }) as IServiceProvider ??
                                      services.BuildServiceProvider();

            // Configure (will support DI at some point)
            // configure.Invoke(startup, new object[] { });
            var parameters = configure.GetParameters();
            var args = new object[parameters.Length];

            if (parameters.Length == 0)
            {
                throw new InvalidOperationException("Invalid Configure signature");
            }

            var appBuilder = new ApplicationBuilder<TContext>(applicationServices);
            args[0] = appBuilder;

            for (int i = 1; i < parameters.Length; i++)
            {
                args[i] = applicationServices.GetRequiredService(parameters[i].ParameterType);
            }

            configure.Invoke(startup, args);

            var pipeline = appBuilder.Build();
            var serverFactory = applicationServices.GetRequiredService<IServerFactory>();
            var contextFactory = applicationServices.GetRequiredService<IContextFactory<TContext>>();
            var server = serverFactory.CreateServer(_config);

            return new ServerHost<TContext>(server, applicationServices, contextFactory, pipeline);
        }
        
        public IServerHostBuilder<TContext> Configure(Action<IApplicationBuilder<TContext>> configureApplication)
        {
            throw new NotImplementedException();
        }

        public IServerHostBuilder<TContext> ConfigureServices(Action<IServiceCollection> configureServices)
        {
            _configureServices = configureServices;
            return this;
        }

        public string GetSetting(string key)
        {
            return _config[key];
        }

        public IServerHostBuilder<TContext> UseServer<TServerFactory>()
        {
            _serverFactoryType = typeof(TServerFactory);
            return this;
        }

        public IServerHostBuilder<TContext> UseServer(IServerFactory factory)
        {
            throw new NotImplementedException();
        }

        public IServerHostBuilder<TContext> UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        public IServerHostBuilder<TContext> UseStartup(Type startupType)
        {
            _startupType = startupType;
            return this;
        }
        public IServerHostBuilder<TContext> UseStartup<TStartup>()
        {
            return UseStartup(typeof(TStartup));
        }

        private static MethodInfo FindConfigureDelegate(Type startupType)
        {
            return FindMethod(startupType, "Configure", typeof(void), required: true);
        }

        private static MethodInfo FindConfigureServicesDelegate(Type startupType)
        {
            var servicesMethod = FindMethod(startupType, "ConfigureServices", typeof(IServiceProvider), required: false)
                ?? FindMethod(startupType, "ConfigureServices", typeof(void), required: false);
            return servicesMethod;
        }

        private static MethodInfo FindMethod(Type startupType, string methodName, Type returnType = null, bool required = true)
        {
            var methodNameWithNoEnv = string.Format(CultureInfo.InvariantCulture, methodName, "");

            var methods = startupType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithNoEnv)).ToList();

            if (selectedMethods.Count > 1)
            {
                throw new InvalidOperationException(string.Format("Having multiple overloads of method '{0}' is not supported.", methodNameWithNoEnv));
            }

            var methodInfo = selectedMethods.FirstOrDefault();
            if (methodInfo == null)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("A public method named '{0}' could not be found in the '{2}' type.",
                        methodNameWithNoEnv,
                        startupType.FullName));

                }
                return null;
            }

            if (returnType != null && methodInfo.ReturnType != returnType)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("The '{0}' method in the type '{1}' must have a return type of '{2}'.",
                        methodInfo.Name,
                        startupType.FullName,
                        returnType.Name));
                }
                return null;
            }
            return methodInfo;
        }
    }
}
