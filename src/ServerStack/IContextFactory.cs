namespace ServerStack
{
    public interface IContextFactory<TContext>
    {
        TContext CreateContext(IFeatureCollection features);
    }
}