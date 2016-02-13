namespace ServerStack.Serialization
{
    public interface IOutputProducer
    {
        void Produce(object value);
    }
}
