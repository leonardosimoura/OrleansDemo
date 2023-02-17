namespace MSOrleansDemo.Grains
{

    [GenerateSerializer]
    public record AgreementSignedEvent
    {
        [Id(0)]
        public string PdfFileLocation { get; set; } 

        [Id(1)]
        public string SignerId { get; set; }
        [Id(2)]
        public string AgreementId { get; set; }

    }
}