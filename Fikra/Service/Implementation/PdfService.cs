using AutoMapper;
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
        private readonly IStripeService _stripeService;
        private readonly IStripeCustomer _stripeCustomerRepo;
        private readonly IStripeAccountsRepo _stripeAccountsRepo;
        private readonly IIdeaService _IdeaService;
        private readonly ICVService _cvService;
        private readonly IMapper _mapper;
        private readonly IMoneyTransferRepo _moneyTransferRepo;

        private readonly ITransictionRepo _transictionRepo;
        public PdfService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IRSAService rsaService,ISignatureRepo signatureRepo,UserManager<ApplicationUser>UserManager,IContractRepo contractRepo,IConfiguration configuration,IStripeService stripeService,IStripeAccountsRepo stripeAccountsRepo,IStripeCustomer stripeCustomer,ITransictionRepo transictionRepo,IIdeaService ideaService,ICVService cVService,IMapper mapper,IMoneyTransferRepo moneyTransferRepo)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _rsaService = rsaService;
            _signatureRepo = signatureRepo;
            _userManager = UserManager;
            _contractRepo = contractRepo;
            _configuration = configuration;
            _stripeService = stripeService;
            _stripeCustomerRepo = stripeCustomer;
            _stripeAccountsRepo = stripeAccountsRepo;
            _transictionRepo = transictionRepo;
            _IdeaService= ideaService;
            _cvService = cVService;
            _mapper = mapper;
            _moneyTransferRepo = moneyTransferRepo;
        }

        public async Task<string> GenerateContract(string ideaOwnerName, string investorName, double budget, DateTime date, string IdeaownerSignature, string investorSignature, byte[] logoBytes,string ideaTitle, double ideaOwnerPercentage)
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
                        col.Item().PaddingTop(10).Text(ideaTitle)
       .FontSize(16).Bold().AlignCenter();

                        col.Item().LineHorizontal(1).LineColor(Colors.Black);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Date: {date:MMMM dd, yyyy}").AlignRight();
                        col.Item().Text($"Idea Owner: {firstPeerNameToCheck.UserName}").FontSize(12);
                        col.Item().Text($"Investor: {secondPeerNametoCheck.UserName}").FontSize(12);
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
                                column.Item().Text(firstUserFullName).FontSize(12).Italic();
                            });

                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Investor Signature:").SemiBold();
                                column.Item().Text(secondUserFullName).FontSize(12).Italic();
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
                IdeaTitle=ideaTitle,
                IdeaOwnerpercentage = ideaOwnerPercentage,

            };
           
            var stripeCustomersaccounts =  _stripeCustomerRepo.GetTableAsNoTracking().Include(x => x.ApplicationUser);
            var checkInvestor = await stripeCustomersaccounts.FirstOrDefaultAsync(x => x.ApplicationUser.UserName == investorName);
            if (checkInvestor == null)
            {
                var result = await _stripeService.CreateCustomer(secondPeerNametoCheck.Email, secondPeerNametoCheck.UserName);
                var newCustomerAccount = new StripeCustomer()
                {
                    ApplicationUser = secondPeerNametoCheck,
                    ApplicationUserId =secondPeerNametoCheck.Id,
                    CreatedAt = DateTime.Now,
                    StripeCustomerId = result,
                };
                await _stripeCustomerRepo.AddAsync(newCustomerAccount);
                await _stripeCustomerRepo.SaveChangesAsync();
            }


            // start fixing

            //start adminSection
            // var Admin = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == "KaoudAdmin");



            //end AdminSection
            // start for IDeaOwnerSection
            var stripeAccounts =  _stripeAccountsRepo.GetTableAsNoTracking().Include(x => x.ApplicationUser);
            var checkIdeaOwner=await stripeAccounts.FirstOrDefaultAsync(x=>x.ApplicationUser.UserName== "KaoudAdmin");
            var Admin = await _userManager.FindByNameAsync("KaoudAdmin");
            if (checkIdeaOwner == null)
            {
               

                var stripeAccountId = await _stripeService.CreateConnectedAccount(Admin.Email, Admin.FirstName, Admin.LastName, Admin.UserName, PostCode: null, city: null, state: null);


                var stripeAccount = new StripeAccount
                {
                    ApplicationUserId = Admin.Id,
                    StripeAccountId = stripeAccountId,
                    BusinessType = "Personal",
                    CreatedAt = DateTime.UtcNow
                };

                await _stripeAccountsRepo.AddAsync(stripeAccount);
                await _stripeAccountsRepo.SaveChangesAsync();
            }
            var getTableofCustomersAfter =await  _stripeCustomerRepo.GetTableAsNoTracking().Include(x => x.ApplicationUser).ToListAsync();
            var getInvestorCustomerAccount = getTableofCustomersAfter.FirstOrDefault(x => x.ApplicationUserId == secondPeerNametoCheck.Id);
            var getTablesofAccountsAfter=await _stripeAccountsRepo.GetTableAsNoTracking().Include(x=>x.ApplicationUser).ToListAsync();
            var getAdminAcount =  getTablesofAccountsAfter.FirstOrDefault(x => x.ApplicationUserId == Admin.Id);

            try
            {
                var result = await _stripeService.SimulateInvestmentToAdmin(getInvestorCustomerAccount.StripeCustomerId, getAdminAcount.StripeAccountId, budget);
                var newTransaction = new Transaction()
                {
                    Amount = budget,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending",
                    StripePaymentIntentId = result,
                    InvestorId = secondPeerNametoCheck.Id,
                    IdeaOwnerId = firstPeerNameToCheck.Id,
                };
                await _transictionRepo.AddAsync(newTransaction);

                await _transictionRepo.SaveChangesAsync();

               

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            var AmountAfterPercentageIt = ((long)budget) * ideaOwnerPercentage / 100;
            var moneyTransferRequest = new MoneyTransferRequest
            {
                Amount = AmountAfterPercentageIt,
                ReceiverUserName = firstPeerNameToCheck.UserName,
                Statue = "Pending"
            };
           await  _moneyTransferRepo.AddAsync(moneyTransferRequest);
           await  _moneyTransferRepo.SaveChangesAsync();
           



            var IdeatoFind =await  _IdeaService.GetTableAsNoTracking().FirstOrDefaultAsync(x=>x.Title==ideaTitle);
            IdeatoFind.Confirmed=true;
           await  _IdeaService.UpdateAsync(IdeatoFind);
            await _IdeaService.SaveChangesAsync();







            await  _contractRepo.AddAsync(contract);
            await _contractRepo.SaveChangesAsync();

            
            return PdfUrl;
        }

        public async  Task<string> GenerateCV(GenerateCVDto freelancerCvDto,string UserName)
        {
            var freelancer=await _userManager.FindByNameAsync(UserName);
            string cvpath = Path.Combine(_webHostEnvironment.ContentRootPath, "cvs");
            if (!Directory.Exists(cvpath))
                Directory.CreateDirectory(cvpath);

            string fileName = $"CV_{UserName}_.pdf";
            string outputFilePath = Path.Combine(cvpath, fileName);
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(12).FontColor(Colors.Black));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(freelancer.FirstName+freelancer.FatherName+freelancer.LastName).FontSize(18).Bold();
                            col.Item().Text(freelancerCvDto.Title).FontSize(14).Italic().FontColor(Colors.Grey.Darken1);
                            col.Item().Text($"Country: {freelancer.Country}");
                            col.Item().PaddingTop(5);
                            col.Item().Text($"Email: {freelancer.Email}");
                            col.Item().Text($"Phone: {freelancerCvDto.Phone}");
                            if (!string.IsNullOrWhiteSpace(freelancer.LinkedinUrl))
                                col.Item().Text($"LinkedIn: {freelancer.LinkedinUrl}");
                        });

                        //if (freelancerCvDto.ProfileIma != null)
                        //{
                        //    row.ConstantItem(80).Height(80).Image(freelancerCvDto.ProfileImageBytes).WithCompressionQuality(ImageCompressionQuality.Medium);
                        //}
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingVertical(10);

                        col.Item().Text("Professional Summary").FontSize(14).Bold().Underline();
                        col.Item().Text(freelancerCvDto.Summary ?? "N/A").FontSize(12);

                        col.Item().PaddingVertical(10);

                        col.Item().Text("Technologies").FontSize(14).Bold().Underline();
                        foreach (var tech in freelancerCvDto.Technologies ?? new List<SkillLevelDto>())
                            col.Item().Text($"• {tech.Technology} - {tech.Level}");

                        col.Item().PaddingVertical(10);

                        col.Item().Text("Education").FontSize(14).Bold().Underline();
                        foreach (var edu in freelancerCvDto.Education ?? new List<string>())
                            col.Item().Text($"• {edu}");

                        col.Item().PaddingVertical(10);

                        col.Item().Text("Experience").FontSize(14).Bold().Underline();
                        foreach (var exp in freelancerCvDto.Experience ?? new List<string>())
                            col.Item().Text($"• {exp}");
                    });

                    page.Footer().AlignCenter()
                        .Text("Generated by Fikra Freelancer Platform")
                        .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                });
            }).GeneratePdf(outputFilePath);
            var customUrl = _configuration["AppSettings:BaseUrl"];
            var request = _httpContextAccessor.HttpContext.Request;
            var PdfUrl = $"{customUrl}cvs/{fileName}";
            var mappedSkills = _mapper.Map <List<SkillLevel>>(freelancerCvDto.Technologies);
            var cv = new CV()
            {
                ApplicationUser = freelancer,
                ApplicationUserId = freelancer.Id,
                Country = freelancer.Country,
                Experience = freelancerCvDto.Experience,
                Education = freelancerCvDto.Education,
                Phone = freelancerCvDto.Phone,
                Summary = freelancerCvDto.Summary,
                Technologies = mappedSkills,
                Title = freelancerCvDto.Title,
                CVPdfUrl = PdfUrl,


            };
           await  _cvService.AddAsync(cv);
            await _cvService.SaveChangesAsync();

          
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
