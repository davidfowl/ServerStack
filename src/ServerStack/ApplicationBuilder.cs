using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public class ApplicationBuilder<TContext> : IApplicationBuilder<TContext>
    {
        internal static string ServerFeaturesKey = "server.Features";
        internal static string ApplicationServicesKey = "application.Services";

        private readonly IList<Func<Func<TContext, Task>, Func<TContext, Task>>> _components = new List<Func<Func<TContext, Task>, Func<TContext, Task>>>();

        public ApplicationBuilder(IServiceProvider serviceProvider)
        {
            Properties = new Dictionary<string, object>();
            ApplicationServices = serviceProvider;
        }

        public ApplicationBuilder(IServiceProvider serviceProvider, object server)
            : this(serviceProvider)
        {
            SetProperty(ServerFeaturesKey, server);
        }

        private ApplicationBuilder(ApplicationBuilder<TContext> builder)
        {
            Properties = builder.Properties;
        }

        public IServiceProvider ApplicationServices
        {
            get
            {
                return GetProperty<IServiceProvider>(ApplicationServicesKey);
            }
            set
            {
                SetProperty<IServiceProvider>(ApplicationServicesKey, value);
            }
        }

        public IFeatureCollection ServerFeatures
        {
            get
            {
                return GetProperty<IFeatureCollection>(ServerFeaturesKey);
            }
        }

        public IDictionary<string, object> Properties { get; }

        private T GetProperty<T>(string key)
        {
            object value;
            return Properties.TryGetValue(key, out value) ? (T)value : default(T);
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        public IApplicationBuilder<TContext> Use(Func<Func<TContext, Task>, Func<TContext, Task>> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public IApplicationBuilder<TContext> New()
        {
            return new ApplicationBuilder<TContext>(this);
        }

        public Func<TContext, Task> Build()
        {
            Func<TContext, Task> app = ctx => Task.FromResult(0);

            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }
    }
}
