using Fikra.Models;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SparkLink.Models.Identity;
using System.Reflection.Metadata.Ecma335;


namespace Fikra.Service.Implementation
{
    public class PdfService:IPdfService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRSAService _rsaService;
        private readonly ISignatureRepo _signatureRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContractRepo _contractRepo;
        private readonly IConfiguration _configuration;

        public PdfService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IRSAService rsaService,ISignatureRepo signatureRepo,UserManager<ApplicationUser>UserManager,IContractRepo contractRepo,IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _rsaService = rsaService;
            _signatureRepo = signatureRepo;
            _userManager = UserManager;
            _contractRepo = contractRepo;
        }

        public async Task<string> GenerateContract(string ideaOwnerName, string investorName, double budget, DateTime date, string IdeaownerSignature, string investorSignature, byte[] logoBytes)
        {
            string contractsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "contracts");
            if (!Directory.Exists(contractsPath))
                Directory.CreateDirectory(contractsPath);
            var firstPeerNameToCheck = await _userManager.FindByNameAsync(ideaOwnerName);
            var secondPeerNametoCheck = await _userManager.FindByNameAsync(investorName);
            if (firstPeerNameToCheck == null)
            {
                return $"IdeaOwner: {ideaOwnerName} is not Found";
            }
            if(secondPeerNametoCheck == null)
            {
                return $"Investor:{investorName} is not Found";
            }
            string fileName = $"Contract_{ideaOwnerName} X {investorName}.pdf";
            string outputFilePath = Path.Combine(contractsPath, fileName);
  
            var encryptedIdeaOwnerSignture = _rsaService.SignData(IdeaownerSignature);
            var encryptedInvestorSignture = _rsaService.SignData(investorSignature);
            if (firstPeerNameToCheck != null && secondPeerNametoCheck != null)
            {
                var resultofCheckingExsiting = await _signatureRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x=>x.ApplicationUserId==firstPeerNameToCheck.Id);
                
                if (resultofCheckingExsiting!=null&& !resultofCheckingExsiting.Sign.Equals(encryptedIdeaOwnerSignture))
                {
                    return $"IdeaOwner{ideaOwnerName} Signature is not Verified!!";
                }
                var resultofcheckingInvestor=await _signatureRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x=>x.ApplicationUserId==secondPeerNametoCheck.Id);
                
                if (resultofcheckingInvestor != null && !resultofcheckingInvestor.Sign.Equals(encryptedInvestorSignture))
                {
                    return $"Investor {investorName} Signature is not Verified!!";
                }
                if (firstPeerNameToCheck != null && secondPeerNametoCheck != null)
                {
                    if (resultofCheckingExsiting == null)
                    {
                        var IdeaOwnernewSignature = new Signature()
                        {
                            ApplicationUserId = firstPeerNameToCheck.Id,
                            ApplicationUser = firstPeerNameToCheck,
                            Sign = encryptedIdeaOwnerSignture,
                        };
                        await _signatureRepo.AddAsync(IdeaOwnernewSignature);
                        await _signatureRepo.SaveChangesAsync();
                    }
                    if (resultofcheckingInvestor == null)
                    {
                        var InvestorNewSignature = new Signature()
                        {
                            ApplicationUserId = secondPeerNametoCheck.Id,
                            ApplicationUser = secondPeerNametoCheck,
                            Sign = encryptedInvestorSignture,
                        };
                        await _signatureRepo.AddAsync(InvestorNewSignature);
                        await _signatureRepo.SaveChangesAsync();
                    }

                }
            }
            var firstUserFullName=firstPeerNameToCheck.FirstName+firstPeerNameToCheck.FatherName+firstPeerNameToCheck.LastName;
            var secondUserFullName=secondPeerNametoCheck.FirstName+secondPeerNametoCheck.FatherName+secondPeerNametoCheck.LastName;
            


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
                        col.Item().Text($"Idea Owner: {firstUserFullName}").FontSize(12);
                        col.Item().Text($"Investor: {secondUserFullName}").FontSize(12);
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
                                column.Item().Text(IdeaownerSignature).FontSize(12).Italic();
                            });

                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Investor Signature:").SemiBold();
                                column.Item().Text(investorSignature).FontSize(12).Italic();
                            });
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text("This document is generated electronically and holds the same legal standing as a physical contract.")
                        .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                });
            })
            .GeneratePdf(outputFilePath);
            var customUrl = _configuration["AppSettings:BaseUrl"];
            var request = _httpContextAccessor.HttpContext.Request;
            var PdfUrl = $"{customUrl}contracts/{fileName}";
            var contract = new Contract()
            {
                ContractPdfUrl = PdfUrl,
                CreateAt = DateTime.Now,
                InvestorId = secondPeerNametoCheck.Id,
                Investor = secondPeerNametoCheck,
                IdeaOwner = firstPeerNameToCheck,
                IdeaOwnerId = firstPeerNameToCheck.Id,
                Budget = budget,
                IdeaOwnerpercentage = 0.0,
            };
           await  _contractRepo.AddAsync(contract);
            await _contractRepo.SaveChangesAsync();

            
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
