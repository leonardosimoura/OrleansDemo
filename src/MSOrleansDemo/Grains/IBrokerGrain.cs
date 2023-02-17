namespace MSOrleansDemo.Grains
{
    public interface IBrokerGrain : IGrainWithIntegerKey
    {
        Task SendMessageAsync(object message);
    }
}
