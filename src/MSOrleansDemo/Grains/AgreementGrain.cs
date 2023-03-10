using Orleans.Runtime;

namespace MSOrleansDemo.Grains
{
    public class AgreementGrain : Grain, IAgreementGrain
    {
        private readonly IPersistentState<AgreementDetails> _state;
        public AgreementGrain([PersistentState(
                stateName: "agreementDetail",
                storageName: "agreementDetail")]
                IPersistentState<AgreementDetails> state)
        {
            _state = state;
        }

        public async Task<string> GeneratePdfAsync()
        {
            _state.State.PdfFileLocation = Guid.NewGuid().ToString() + ".pdf";
            await _state.WriteStateAsync();
            return _state.State.PdfFileLocation;
        }

        public async Task<AgreementDetails> GetState()
        {
            return _state.State;
        }

        public async Task SignAsync(string signerId)
        {
             _state.State.SignerId = signerId;
             await _state.WriteStateAsync();

            var brokerGrain = GrainFactory.GetGrain<IBrokerGrain>(0);

            await brokerGrain.SendMessageAsync(new AgreementSignedEvent
            {
                AgreementId = this.GetPrimaryKeyString(),
                PdfFileLocation = _state.State.PdfFileLocation,
                SignerId = signerId
            });
        }
    }
}