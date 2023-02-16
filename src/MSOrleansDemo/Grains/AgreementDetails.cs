namespace MSOrleansDemo.Grains
{
    [GenerateSerializer, Immutable,Serializable]
    public record AgreementDetails
    {
        public string PdfFileLocation { get; set; } = "";

        public string SignerId { get; set; } = "";
    }

}