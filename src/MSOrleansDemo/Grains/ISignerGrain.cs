using Orleans.Runtime;

namespace MSOrleansDemo.Grains
{
    public interface ISignerGrain : IGrainWithStringKey
    {
        Task SignAgreementAsync(IAgreementGrain agreement);

        Task SignAgreementAsync(string agreementId);
    }

    public class SignerGrain : Grain, ISignerGrain
    {
        public async Task  SignAgreementAsync(IAgreementGrain agreement)
        {
            await agreement.SignAsync(this.GetPrimaryKeyString());
        }

        public async Task SignAgreementAsync(string agreementId)
        {
            var agreementGrain = this.GrainFactory.GetGrain<IAgreementGrain>(agreementId);

            await agreementGrain.SignAsync(this.GetPrimaryKeyString());
        }
    }
}