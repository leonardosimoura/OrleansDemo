namespace MSOrleansDemo.Grains
{
    public interface IAgreementGrain : IGrainWithStringKey
    {
        Task<string> GeneratePdfAsync();

        Task<AgreementDetails> GetState();

        Task SignAsync(string signerId);
    }

}