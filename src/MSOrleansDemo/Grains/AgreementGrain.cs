using Orleans.Runtime;

namespace MSOrleansDemo.Grains
{
    public class AgreementGrain : Grain, IAgreementGrain
    {
        private readonly IPersistentState<AgreementDetails> _state;
        private readonly ILogger<AgreementGrain> _logger;
        public AgreementGrain([PersistentState(
                stateName: "agreementDetail",
                storageName: "agreementDetail")]
                IPersistentState<AgreementDetails> state, ILogger<AgreementGrain> logger)
        {
            _state = state;
            _logger = logger;
        }

        public async Task<string> GeneratePdfAsync()
        {
            _state.State.PdfFileLocation = Guid.NewGuid().ToString() + ".pdf";
            await _state.WriteStateAsync();
            return _state.State.PdfFileLocation;
        }

        public async Task GeneratePdfOneRequestAsync()
        {
            _logger.LogInformation($"{nameof(GeneratePdfOneRequestAsync)} Started");
            await Task.Delay(5000);
            _logger.LogInformation($"{nameof(GeneratePdfOneRequestAsync)} Ended");
        }

        /// <summary>
        /// GetState convention for Orleans Dashboard to retrive grain state dynamically
        /// </summary>
        /// <returns></returns>
        public async Task<AgreementDetails> GetState()
        {
            return _state.State;
        }

        public async Task<string> LongProcessAsync(string callerId)
        {
            _logger.LogInformation($"{nameof(LongProcessAsync)} Started - AgreementId: {this.GetPrimaryKeyString()} callerId {callerId}");
            await Task.Delay(10000);
            _logger.LogInformation($"{nameof(LongProcessAsync)} Ended - AgreementId: {this.GetPrimaryKeyString()} callerId {callerId}");
            return Guid.NewGuid().ToString();
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