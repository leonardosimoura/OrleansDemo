using Orleans.Runtime;

namespace MSOrleansDemo.Grains
{
    public interface IAgreementGrain : IGrainWithStringKey
    {
        Task<string> GetPdfAsync();

        Task<AgreementDetails> GetState();

        Task SignAsync(string signerId);
    }

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

        public async Task<string> GetPdfAsync()
        {
            var grainId = this.GetPrimaryKeyString();
            _state.State.PdfFileLocation = grainId.ToString();
            await _state.WriteStateAsync();
            return "https://www.google.com.br/search?q=" + _state.State.PdfFileLocation;
        }

        public async Task<AgreementDetails> GetState()
        {
            return _state.State;
        }

        public async Task SignAsync(string signerId)
        {
             _state.State.SignerId = signerId;
             await _state.WriteStateAsync();
        }
    }

   [GenerateSerializer, Immutable,Serializable]
    public record AgreementDetails
    {
        public string PdfFileLocation { get; set; } = "";

        public string SignerId { get; set; } = "";
    }

}