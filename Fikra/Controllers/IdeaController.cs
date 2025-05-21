using AutoMapper;
using Fikra.Models;
using Fikra.Models.Dto;
using Fikra.Service.Implementation;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OpenAI;
using Org.BouncyCastle.Crypto.Digests;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using Stripe;
using System.Diagnostics.Metrics;
using static QuestPDF.Helpers.Colors;
using static System.Reflection.Metadata.BlobBuilder;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeaController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdeaService _IdeaService;
        private readonly IMapper _mapper;
        private readonly IIdentityServices _identityServices;
        private readonly IIdeaRating _IdeaRating;
        public IdeaController(UserManager<ApplicationUser> userManager, IIdeaService ideaService, IMapper mapper, IIdentityServices identityServices,IIdeaRating ideaRating)
        {
            _userManager = userManager;
            _IdeaService = ideaService;
            _mapper = mapper;
            _identityServices = identityServices;
            _IdeaRating = ideaRating;

        }
        //[HttpPost]
        //public async Task<IActionResult>CreateIdea([FromBody]CreateIdeaDto createIdeaDto)
        //{
        //    if (createIdeaDto == null)
        //    {
        //        return BadRequest("cannot Add Empty fields for idea");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        await _IdeaService.AddAsync(createIdeaDto);
        //        await _IdeaService.SaveChangesAsync():

        //        return Ok(createIdeaDto);
        //    }


        //}
        //[HttpPost]
        //public async Task<IActionResult> GetSwot([FromBody] SwotRequestDto request)
        //{
        //    var swot = await _IdeaService.SwotAnalysis(request.Budget, request.ProblemStatement, request.ProposedSolution);
        //    return Ok(swot);
        //}
        //public class SwotRequestDto
        //{
        //    public string Budget { get; set; }
        //    public string ProblemStatement { get; set; }
        //    public string ProposedSolution { get; set; }
        //}
        [HttpGet("test")]
        public async Task<IActionResult> TestAnalysis()
        {



            //var productivityAppIdea = new Idea
            //{
            //    Title = "Remote Team Productivity Suite",
            //    ProblemStatement = "Remote teams struggle to manage workflows and track productivity.",
            //    ShortDescription = "A SaaS platform combining task management, time tracking, and team analytics.",
            //    Category = "SaaS",
            //    TargetAudience = "Remote startups and SMEs",
            //    CompetitiveAdvantage = "Built-in AI productivity recommendations and cross-platform support",
            //    Features = new List<string> { "Time tracking", "Task boards", "Automated productivity reports", "Cross-device sync" },
            //    Country = "United States",
            //    ExpectedUserCount = 10000,
            //    EstimatedMonthlyRevenuePerUser = 15,
            //    RealisticConversionRate = 12,
            //    RetentionMonths = 18,
            //    Tools = new List<string> { "ci/cd", "monitoring", "analytics", "version control" },
            //    RequiresRealTimeFeatures = true,
            //    RequiresMobileApp = true,
            //    RequiresDevOpsSetup = true,
            //    FrontendComplexity = "Medium",
            //    SecurityCriticalLevel = "Sensitive",
            //    DeploymentFrequency = "Weekly"
            //};


            //case 2 
            //var financeAppIdea = new Idea
            //{
            //    Title = "Smart Financial Health Manager",
            //    ProblemStatement = "Individuals and small businesses lack easy tools to monitor, forecast, and optimize their financial health in real-time.",
            //    ShortDescription = "An AI-powered finance platform offering budgeting, cash flow forecasting, investment insights, and credit monitoring.",
            //    Category = "Fintech",
            //    TargetAudience = "Young professionals, freelancers, and small business owners",
            //    CompetitiveAdvantage = "Real-time personalized financial coaching and predictive cash flow modeling with open banking integrations",
            //    Features = new List<string> { "Budget planner", "Cash flow forecasting", "Credit score monitoring", "AI-based financial advice", "Automated investment tracking" },
            //    Country = "United States",
            //    ExpectedUserCount = 25000,
            //    EstimatedMonthlyRevenuePerUser = 10,
            //    RealisticConversionRate = 15,
            //    RetentionMonths = 24,
            //    Tools = new List<string> { "open banking APIs", "data analytics", "machine learning", "secure authentication" },
            //    RequiresRealTimeFeatures = true,
            //    RequiresMobileApp = true,
            //    RequiresDevOpsSetup = true,
            //    FrontendComplexity = "High",
            //    SecurityCriticalLevel = "Highly Sensitive",
            //    DeploymentFrequency = "Bi-Weekly"
            //};




            //case 3

            //        var profitableFinanceAppIdea = new Idea
            //        {
            //            Title = "Business Finance Master",
            //            ProblemStatement = "Small businesses struggle with real-time cash flow management, smart investment planning, and financial risk forecasting.",
            //            ShortDescription = "An AI-driven financial platform offering cash flow forecasting, investment guidance, risk analysis, and tax optimization tailored for SMEs.",
            //            Category = "Fintech",
            //            TargetAudience = "Small and Medium Enterprises (SMEs), Business Owners",
            //            CompetitiveAdvantage = "AI-driven predictive analytics, tax optimization modules, and investment guidance — integrated with open banking APIs for real-time data.",
            //            Features = new List<string>
            //{
            //    "Cash Flow Forecasting",
            //    "Investment Insights",
            //    "Risk Analysis & Alerts",
            //    "Real-Time Financial Dashboard",
            //    "Automated Tax Optimization",
            //    "Open Banking Integration"
            //},
            //            Country = "United States",
            //            ExpectedUserCount = 15_000, // focused realistic number
            //            EstimatedMonthlyRevenuePerUser = 35, // much higher price point
            //            RealisticConversionRate = 25, // stronger because of focused business clients
            //            RetentionMonths = 30, // stickier product
            //            Tools = new List<string>
            //{
            //    "open banking APIs",
            //    "analytics",
            //    "machine learning",
            //    "tax software integrations",
            //    "secure authentication"
            //},
            //            RequiresRealTimeFeatures = true,
            //            RequiresMobileApp = false, // Businesses usually desktop → cheaper dev
            //            RequiresDevOpsSetup = true,
            //            FrontendComplexity = "Medium", // Not "High" like before, keep MVP
            //            SecurityCriticalLevel = "Highly Sensitive",
            //            DeploymentFrequency = "Weekly"
            //        };



            //case 4 simpleIdea

            //var simpleIdea = new Idea
            //{
            //    Title = "Online Study Habit Tracker",
            //    ProblemStatement = "Students struggle to build consistent study habits without motivation and tracking tools.",
            //    ShortDescription = "A simple web/mobile app that helps students set study goals, track their time, and receive daily motivation.",
            //    Category = "EdTech",
            //    TargetAudience = "High school and college students",
            //    CompetitiveAdvantage = "Gamification elements and daily AI-generated motivational nudges",
            //    Features = new List<string> { "Daily goal setting", "Study timer", "Progress tracking", "Streak rewards", "AI motivational messages" },
            //    Country = "United States",
            //    ExpectedUserCount = 3000,
            //    EstimatedMonthlyRevenuePerUser = 5,
            //    RealisticConversionRate = 18, // 18% realistic for simple, free-to-premium apps
            //    RetentionMonths = 12,
            //    Tools = new List<string> { "analytics", "push notifications", "ci/cd" },
            //    RequiresRealTimeFeatures = false,
            //    RequiresMobileApp = true,
            //    RequiresDevOpsSetup = false,
            //    FrontendComplexity = "Simple",
            //    SecurityCriticalLevel = "Normal",
            //    DeploymentFrequency = "Monthly"
            //};

            //case 5 not much profitable 
            //        var midIdea = new Idea
            //        {
            //            Title = "Freelancer Contract Manager",
            //            ProblemStatement = "Freelancers and small agencies struggle to easily create, sign, and manage client contracts online without expensive legal teams.",
            //            ShortDescription = "A SaaS platform allowing freelancers to create, send, e-sign, and store client contracts legally and securely.",
            //            Category = "SaaS",
            //            TargetAudience = "Freelancers, consultants, small agencies",
            //            CompetitiveAdvantage = "Prebuilt customizable contract templates + built-in AI to suggest clauses based on project type",
            //            Features = new List<string>
            //{
            //    "Contract template library",
            //    "Drag & drop contract builder",
            //    "Legally binding e-signatures",
            //    "AI clause suggestions",
            //    "Payment milestone tracking",
            //    "Client portal"
            //},
            //            Country = "United States",
            //            ExpectedUserCount = 8000,
            //            EstimatedMonthlyRevenuePerUser = 12, // SaaS model
            //            RealisticConversionRate = 20, // very realistic for SaaS tools
            //            RetentionMonths = 18,
            //            Tools = new List<string> { "analytics", "payments integration", "ci/cd", "monitoring" },
            //            RequiresRealTimeFeatures = false,
            //            RequiresMobileApp = false,
            //            RequiresDevOpsSetup = true,
            //            FrontendComplexity = "Medium",
            //            SecurityCriticalLevel = "Sensitive",
            //            DeploymentFrequency = "Weekly"
            //        };


            //var fitapp = new Idea
            //{
            //    Title = "DailyFit - Personalized Fitness Tracker",
            //    ProblemStatement = "Many people fail to maintain fitness goals due to lack of personalized tracking.",
            //    Category = "Fitness",
            //    TargetAudience = "General public (18-45 years old)",
            //    CompetitiveAdvantage = "Uses AI to auto-adjust plans based on daily behavior",
            //    Features = new List<string> { "Step counter", "Calorie tracker", "Heart monitor integration", "AI fitness plans" },
            //    Country = "Syria",
            //    ExpectedUserCount = 30000,
            //    EstimatedMonthlyRevenuePerUser = 6,
            //    RealisticConversionRate = 15,
            //    RetentionMonths = 18,
            //    Tools = new List<string> { "analytics", "notifications", "ci/cd" },
            //    RequiresRealTimeFeatures = true,

            //    RequiresDevOpsSetup = false,
            //    FrontendComplexity = "Medium",
            //    SecurityCriticalLevel = "Normal",
            //    DeploymentFrequency = "Bi-Weekly"
            //};


            var flashcart = new Idea
            {
                Title = "FlashCart",
                ProblemStatement = "Consumers miss out on limited-time deals because platforms are too slow.",
                Category = "E-commerce",
                TargetAudience = "Price-sensitive shoppers (18-40 years old)",
                CompetitiveAdvantage = "Ultra-fast checkout + exclusive deals",
                Features = new List<string> { "Daily deals", "Fast checkout", "Personalized recommendations" },
                Country = "United States",
                ExpectedUserCount = 25000,
                EstimatedMonthlyRevenuePerUser = 4,
                RealisticConversionRate = 30,
                RetentionMonths = 12,
                Tools = new List<string> { "payments", "notifications", "analytics" },
                RequiresRealTimeFeatures = false,

                RequiresDevOpsSetup = false,
                FrontendComplexity = "Simple",
                SecurityCriticalLevel = "Normal",
                DeploymentFrequency = "Monthly"
            };
            var cointPilot = new Idea
            {
                Title = "CoinPilot",
                ProblemStatement = "Crypto investors struggle to manage scattered assets across exchanges.",
                Category = "Fintech",
                TargetAudience = "Crypto traders",
                CompetitiveAdvantage = "Real-time syncing from major exchanges + profit/loss tracking",
                Features = new List<string> { "Portfolio tracking", "Real-time updates", "Profit alerts", "Tax reports" },
                Country = "United States",
                ExpectedUserCount = 8000,
                EstimatedMonthlyRevenuePerUser = 15,
                RealisticConversionRate = 10,
                RetentionMonths = 10,
                Tools = new List<string> { "api integrations", "ci/cd", "monitoring" },
                RequiresRealTimeFeatures = true,
                RequiresDevOpsSetup = true,
                FrontendComplexity = "Complex",
                SecurityCriticalLevel = "Highly Sensitive",
                DeploymentFrequency = "Weekly"
            };
            var eventLink = new Idea
            {
                Title = "EventLink",
                ProblemStatement = "Finding and booking small events (like workshops, meetups) is messy.",
                Category = "Marketplace",
                TargetAudience = "Young adults, professionals",
                CompetitiveAdvantage = "Better small event discovery + instant booking",
                Features = new List<string> { "Event search", "Instant booking", "Reviews", "Payment splitting" },
                Country = "Syria",
                ExpectedUserCount = 12000,
                EstimatedMonthlyRevenuePerUser = 9,
                RealisticConversionRate = 15,
                RetentionMonths = 14,
                Tools = new List<string> { "payments", "ci/cd", "monitoring" },
                RequiresRealTimeFeatures = false,

                RequiresDevOpsSetup = true,
                FrontendComplexity = "Medium",
                SecurityCriticalLevel = "Normal",
                DeploymentFrequency = "Bi-Weekly",
                BigServerNeeded = true,
                HaveBigFiles= true,
            };




            var analysis = await _IdeaService.AnalyzeIdeaCompletely(eventLink);
            return Ok(analysis);




        }
        [HttpPost]
       [Route("CreateNewIdea")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "IdeaOwner")]
        public async Task<IActionResult> CreateIdeaPost([FromBody] AddIdeaDto ideaDto)
        {
           var idea=await _IdeaService.GetTableAsNoTracking().FirstOrDefaultAsync(x=>x.Title.Equals(ideaDto.Title));
            if (idea != null)
            {
                return BadRequest("This Idea has Been taken And Registered Before");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid check the fields");
            }
            var currentUserName = await _identityServices.GetCurrentUserName();
            var IdeaDomainModel = _mapper.Map<Idea>(ideaDto);
            var currentIdeaOwner = await _userManager.FindByNameAsync(currentUserName);
            IdeaDomainModel.IdeaOwnerName = currentUserName;
            IdeaDomainModel.IdeaOwner = currentIdeaOwner;
            IdeaDomainModel.CreatedAt = DateTime.Now;
            IdeaDomainModel.RatingCount = 0;
            IdeaDomainModel.AverageRating = 0;
            IdeaDomainModel.Confirmed = false;
           
            var result = await _IdeaService.AddAsync(IdeaDomainModel);
            await _IdeaService.SaveChangesAsync();
            return Ok();




        }
        
       
        [HttpPost]
        [Route("GetFinanceStudy")]
        [Authorize(AuthenticationSchemes ="Bearer",Roles ="Investor")]
        public async Task<IActionResult> GetFinanceStudy([FromBody] GetIdeaFinanceDto getIdeaFinanceDto)
        {

            var allIdea=await _IdeaService.GetTableAsNoTracking().ToListAsync();
            var ideaToStudy=allIdea.FirstOrDefault(x=>x.Title.Equals(getIdeaFinanceDto.Title));
            if (ideaToStudy == null)
            {
                return NotFound($" Idea With Title :{getIdeaFinanceDto.Title}NotFound:");
            }
           var analysis=await  _IdeaService.AnalyzeIdeaCompletely(ideaToStudy);
            return Ok(analysis);
        }
        [HttpGet]
        [Route("GetAllIdeas")]
        [Authorize(AuthenticationSchemes ="Bearer",Roles ="Investor")]
        public async Task<IActionResult> GetAllIdeas([FromQuery] string? Category, [FromQuery] string? Country )
        {
            IEnumerable<GetIdeaDto> ideasDto = null; 
           
           
        var ideas = await _IdeaService.GetTableAsNoTracking().ToListAsync();
            ideas=ideas.Where(x=>x.Confirmed==false).ToList();
            if (ideas.Any())
            {
               ideasDto= _mapper.Map<List<GetIdeaDto>>(ideas);
                if (Category != null)
                {
                    ideasDto=ideasDto.Where(x=>x.Category.Equals(Category,StringComparison.OrdinalIgnoreCase));
                }
                if (Country != null)
                {
                    ideasDto = ideasDto.Where(x => x.Country.Equals(Country,StringComparison.OrdinalIgnoreCase));
                }
               ideasDto= ideasDto.OrderByDescending(x => x.AverageRating);
                return Ok(ideasDto);

            }
            return BadRequest("Failed While Fetching the Ideas From Db!");

        }
        [HttpGet]
        [Route("GetIdeasCountRelatedToIdeaOwner")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "IdeaOwner")]
        public async Task<IActionResult> GetIdeaCount()
        {
            var currentUserName = await _identityServices.GetCurrentUserName();
            var result=await _IdeaService.GetIdeasRelatedToSpecificIdeaOwner(currentUserName);
            if (result != null)
            {
                return Ok(new
                {
                    IdeasCount = result.Count()
                });
            }
            return Ok();

        }
       
        [HttpPost]
        [Route("RateIdea")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Investor")] 
        public async Task<IActionResult> RateIdea([FromBody] IdeaRatingDto ratingDto)
        {

            var currentUserName =await  _identityServices.GetCurrentUserName();
            var currentUser = await _userManager.FindByNameAsync(currentUserName);
 
            var ideas = await _IdeaService.GetIdeasWithRating();
            var idea= ideas.FirstOrDefault(x=>x.Id==ratingDto.IdeaId);
            if (idea == null)
                return NotFound("Idea not found.");

         
           
                var newRating = new IdeaRating
                {
                    IdeaId = ratingDto.IdeaId,
                    UserId = currentUser.Id,
                    Rating = ratingDto.Rating,
                    RatedAt = DateTime.UtcNow
                };
              await _IdeaRating.AddAsync(newRating);
      


            idea.RatingCount++;
        
            var updatedRatings = await _IdeaRating.GetTableAsNoTracking().Where(x => x.IdeaId == ratingDto.IdeaId).ToListAsync();   

           var sumofRatings = updatedRatings.Sum(r => r.Rating);
            idea.AverageRating=sumofRatings/idea.RatingCount;

            await _IdeaRating.SaveChangesAsync();
            await _IdeaService.SaveChangesAsync();

            return Ok( );
        }
        [HttpGet]
        [Route("GetIdeasPostsRelatedToFreelancer")]
        [Authorize(AuthenticationSchemes ="Bearer",Roles ="Freelancer")]
        public async Task<IActionResult> GetIdeasPostsRealtedToFreelancer()
        {
            var Ideas=await _IdeaService.GetTableAsNoTracking().Where(x=>x.Confirmed==true).ToListAsync();
            if (Ideas.Any())
            {
                var ideasAftermappingforfreelancer=_mapper.Map<List<GetFreelancerIdeasDto>>(Ideas);
                return Ok(ideasAftermappingforfreelancer);

            }
            return Ok(new
            {
                message = "Db Does not Contian any Ideas right now!:"
            });

        }

        
    }
    public class GetIdeaFinanceDto
    {
        public string Title { get; set; }
    }
}
//total += tool.ToLower() switch
//{
//    "ci/cd" => 120m,           // e.g., GitHub Actions, CircleCI
//    "monitoring" => 150m,      // e.g., Datadog, New Relic
//    "analytics" => 100m,       // e.g., Mixpanel, Amplitude
//    "testing" => 60m,          // e.g., BrowserStack, Postman Pro
//    "version control" => 40m,  // e.g., GitHub Pro
//    "project management" => 50m, // e.g., Jira, Trello Premium
//    "design" => 35m,           // e.g., Figma, Adobe XD
//    "ai tool" => 90m,          // e.g., OpenAI Pro, Copilot
//    _ => 25m                   // Default/fallback
//};
