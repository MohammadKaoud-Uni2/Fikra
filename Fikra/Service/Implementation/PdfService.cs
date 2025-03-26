using Fikra.Service.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Fikra.Service.Implementation
{
    public class PdfService:IPdfService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRSAService _rsaService;

        public PdfService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IRSAService rsaService)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _rsaService = rsaService;
        }

        public async Task<string> GenerateContract(string ideaOwnerName, string investorName, decimal budget, DateTime date, string ownerSignature, string investorSignature, byte[] logoBytes)
        {
            string contractsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "contracts");
            if (!Directory.Exists(contractsPath))
                Directory.CreateDirectory(contractsPath);

            string fileName = $"Contract_{ideaOwnerName} X {investorName}.pdf";
            string outputFilePath = Path.Combine(contractsPath, fileName);
            var encryptedIdeaOwnerSignture = _rsaService.SignData(ownerSignature);
            var encryptedInvestorSignture = _rsaService.SignData(investorSignature);


            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(12).FontColor(Colors.Black));

                    page.Header().AlignCenter().Column(col =>
                    {

                        col.Item().Container().Width(80).Height(80).Image(logoBytes).WithCompressionQuality(ImageCompressionQuality.High);
                        col.Item().Text("Fikra").FontSize(18).SemiBold();
                        col.Item().Text("Official Contract Document").FontSize(14).Italic().FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Date: {date:MMMM dd, yyyy}").AlignRight();
                        col.Item().Text($"Idea Owner: {ideaOwnerName}").FontSize(12);
                        col.Item().Text($"Investor: {investorName}").FontSize(12);
                        col.Item().Text($"Budget: ${budget:N2}").FontSize(12);

                        col.Item().PaddingVertical(10);

                        col.Item().Text("Terms and Conditions").FontSize(14).Bold().Underline();
                        col.Item().Text("1. The Idea Owner agrees to share the idea...");
                        col.Item().Text("2. The Investor agrees to invest the amount...");
                        col.Item().Text("3. This contract is legally binding...");

                        col.Item().PaddingVertical(10);
                        col.Item().Text("4. Consequences of Breach of Contract:").FontSize(14).Bold().FontColor(new Color().Red);
                        col.Item().Text("   a. In case of any party being found guilty of fraudulent actions, such as causing harm or death, the responsible party will face severe legal consequences including monetary penalties and imprisonment.").FontSize(12).FontColor(new Color().Red).Bold();
                        col.Item().Text("   b. If any party is found guilty of intentionally causing harm or taking another's life, the consequences will involve direct prosecution under the international criminal law.").FontSize(12).FontColor(new Color().Red).Bold();

                        col.Item().PaddingVertical(10);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Idea Owner Signature:").SemiBold();
                                column.Item().Text(encryptedIdeaOwnerSignture).FontSize(12).Italic();
                            });

                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Investor Signature:").SemiBold();
                                column.Item().Text(encryptedInvestorSignture).FontSize(12).Italic();
                            });
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text("This document is generated electronically and holds the same legal standing as a physical contract.")
                        .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                });
            })
            .GeneratePdf(outputFilePath);
            var test = _rsaService.VerifySignature(investorSignature, encryptedIdeaOwnerSignture);
            var request = _httpContextAccessor.HttpContext.Request;
            var PdfUrl = $"{request.Scheme}://{request.Host}/contracts/{fileName}";
            return PdfUrl;
        }

        public async Task<byte[]> ReciveImage()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var imageUrl = $"{request.Scheme}://{request.Host}/images/profilePictures/FikraLogo.jpg";


            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                return imageBytes;
            }
        }
    }
}
