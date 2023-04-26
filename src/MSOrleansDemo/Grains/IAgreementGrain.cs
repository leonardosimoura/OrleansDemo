
using Orleans.Concurrency;

namespace MSOrleansDemo.Grains
{
    public interface IAgreementGrain : IGrainWithStringKey
    {
        Task<string> GeneratePdfAsync();

        /// <summary>
        /// GetState convention for Orleans Dashboard to retrive grain state dynamically
        /// </summary>
        /// <returns></returns>
        Task<AgreementDetails> GetState();

        Task SignAsync(string signerId);

        [OneWay]
        Task GeneratePdfOneRequestAsync();


        Task<string> LongProcessAsync(string callerId);
    }

}