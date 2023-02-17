namespace MSOrleansDemo.Grains
{
    [GenerateSerializer]
    public record AgreementDetails
    {
        [Id(0)]
        public string PdfFileLocation { get; set; } = "";

        [Id(1)]
        public string SignerId { get; set; } = "";
    }

}