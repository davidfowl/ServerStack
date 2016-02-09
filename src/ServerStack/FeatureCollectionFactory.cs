using System;

namespace ServerStack
{
    public class FeatureCollectionFactory : IContextFactory<IFeatureCollection>
    {
        public IFeatureCollection CreateContext(IFeatureCollection features)
        {
            return features;
        }
    }
}