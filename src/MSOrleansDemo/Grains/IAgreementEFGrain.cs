namespace MSOrleansDemo.Grains
{
    public interface IAgreementEFGrain : IGrainWithStringKey
    {
        Task<string> GeneratePdfAsync();

        /// <summary>
        /// GetState convention for Orleans Dashboard to retrive grain state dynamically
        /// </summary>
        /// <returns></returns>
        Task<AgreementDetails> GetState();

        Task SignAsync(string signerId);
    }

}