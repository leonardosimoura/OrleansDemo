using Orleans.Runtime;

namespace MSOrleansDemo.Grains
{
    public interface ISignerGrain : IGrainWithStringKey
    {
        Task SignAgreementAsync(IAgreementGrain agreement);

        Task SignAgreementAsync(string agreementId);
    }
}