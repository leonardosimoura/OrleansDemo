namespace MSOrleansDemo.Grains
{
    public interface IAgreementEFGrain : IGrainWithStringKey
    {
        Task<string> GeneratePdfAsync();

        Task<AgreementDetails> GetState();

        Task SignAsync(string signerId);
    }

}