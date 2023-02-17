using Orleans.Concurrency;

namespace MSOrleansDemo.Grains
{
    [StatelessWorker]
    public class BrokerGrain : Grain, IBrokerGrain
    {
        public Task SendMessageAsync(object message)
        {
            return Task.Delay(100);
        }
    }
}
