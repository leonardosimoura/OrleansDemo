namespace MSOrleansDemo.Grains
{
    public class SignerGrain : Grain, ISignerGrain
    {
        public async Task  SignAgreementAsync(IAgreementGrain agreement)
        {
            await agreement.SignAsync(this.GetPrimaryKeyString());
        }

        public async Task SignAgreementAsync(string agreementId)
        {
            var agreementGrain = GrainFactory.GetGrain<IAgreementGrain>(agreementId);

            await agreementGrain.SignAsync(this.GetPrimaryKeyString());
        }
    }
}