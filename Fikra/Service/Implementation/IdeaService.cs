using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using Fikra.Models;
using Fikra.Service.Interface;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using SparkLink.Data;
using SparkLink.Service.Implementation;
using static Fikra.Service.Implementation.IdeaService;

namespace Fikra.Service.Implementation
{
    public class IdeaService : GenericRepo<Idea>, IIdeaService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _openRouterApiKey;
        private readonly Dictionary<string, SalaryData> _salaryCache;
        public IdeaService(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _httpClient = new HttpClient();
            _openRouterApiKey = "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";


            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openRouterApiKey}");
      
        }
        public decimal Initinvestment { get; set; }
       
        IdeaSuggestionResult suggestionResult = new IdeaSuggestionResult();
        private async Task<SalaryData> GetDeveloperSalaries(string techReasioning, string country,string backendLevel,string frontendLevel,string devOpsLevel)
        {
            var prompt = $@"
                Estimate the average **annual salaries** in USD for the following developer roles based on the project requirements:

                - Backend Developer ({backendLevel})
                - Frontend Developer ({frontendLevel})
                - DevOps Engineer ({devOpsLevel})
                - QA Engineer (Mid Level assumed)

                You must extract and consider the used technologies from the following reasoning text:

                --- 
                {techReasioning}
                ---

                The project is located in {country}.

                Return ONLY a JSON object exactly matching this structure:

                {{
                  ""BackendDeveloper"": number,
                  ""FrontEndDeveloper"": number,
                  ""DevOpsEngineer"": number,
                  ""QAEngineer"": number,
                  ""Source"": string,
            
              
                }}

                ⚠️ Important Rules:
                - Read the technologies from the provided reasoning text.
                - Fill all salary fields with realistic non-zero values.
                - Only use real-world salary sources (e.g., source you got url).
                -place in the source field the url of source
                - No extra text or markdown outside JSON.
                ";

            var aiResponse = await GetAIJsonResponse<SalaryData>(prompt);
            return aiResponse;

        }
        private (string backendLevel, string frontendLevel, string devOpsLevel) DetectDeveloperLevels(Idea idea)
        {
            string backendLevel = "Mid";
            string frontendLevel = "Mid";
            string devOpsLevel = "Mid";


            if (idea.RequiresRealTimeFeatures || idea.SecurityCriticalLevel == "Highly Sensitive")
                backendLevel = "Senior";

          
            if (idea.FrontendComplexity == "Complex")
                frontendLevel = "Senior";
            else if (idea.FrontendComplexity == "Simple")
                frontendLevel = "Junior";

            
            if (idea.RequiresDevOpsSetup && idea.DeploymentFrequency == "Daily")
                devOpsLevel = "Senior";
            else if (idea.RequiresDevOpsSetup && idea.DeploymentFrequency == "Weekly")
                devOpsLevel = "Mid";
            else
                devOpsLevel = "Junior";

            return (backendLevel, frontendLevel, devOpsLevel);
        }



        public async Task<FullProjectAnalysis> AnalyzeIdeaCompletely(Idea idea)
        {

            try
            {
      
                var investmentNeeds = await CalculateRequiredInvestment(idea);
                decimal initialInvestment = investmentNeeds.Item1.Total;
                this.Initinvestment = initialInvestment;
                var marketData = await GetTechMarketData(idea.Category,idea.Country);
               

   
                var revenueProjection = ProjectRevenue(idea);

                var financialAnalysis  =CalculateFinancialMetrics(investmentNeeds.Item1, revenueProjection, initialInvestment, idea, marketData,investmentNeeds.Item2);

            
                var successProbability = CalculateRealisticSuccessProbability(
                    financialAnalysis);
                var swotAnalysis = await  GenerateCompleteSWOTAnalysis(idea, marketData);
                var MonthEstimator = SmartDevelopmentMonthEstimator.Estimate(idea);
                var DeveloperEstimator=TeamSizeEstimator.Estimate(idea);
                return new FullProjectAnalysis
                {
                    InitialInvestmentRequired = initialInvestment,
                    MarketAnalysis = marketData,
                    SWOTAnalysis = swotAnalysis,
                    FinancialAnalysis = financialAnalysis,
                    technologyRecommendation = investmentNeeds.Item2,
                    CostEstimation=investmentNeeds.Item1,
                    revenueProjection = revenueProjection,
                    InfrastructureCostEstimate = this.infrastructureCostAnalysis,
                    GeneratedAt = DateTime.UtcNow,
                    salaryData=investmentNeeds.Item1.SalaryData,
                    initWorkingMonthDevelopment=MonthEstimator.months,
                    BackEndDevelopers=DeveloperEstimator.backend,
                    FrontEndDevelopers=DeveloperEstimator.frontend,
                    QA=DeveloperEstimator.qa,
                    DevOps=DeveloperEstimator.devops,
                };
            }
            catch (Exception ex)
            {
                return new FullProjectAnalysis
                {
                    Error = $"Analysis failed: {ex.Message}",
                    GeneratedAt = DateTime.UtcNow
                };
            }
        }





        public class IdeaSuggestionResult
        {
            public string TeamSize { get; set; }


        }
       


        private async Task<TechnologyRecommendation> GetTechnologyRecommendation(Idea idea,string backendLevel,string frontendLevel,string DevopsLevel)
        {
  

            var prompt = $@"Analyze this software project and recommend appropriate technologies:
            Project Title: {idea.Title}
            Problem: {idea.ProblemStatement}
            Expected Users: {idea.ExpectedUserCount}
            Key Features: {string.Join(", ", idea.Features)}
            Category :{idea.Category}
            Budget: {idea.InitialInvestment}
            Tools:{idea.Tools}
            CompetitiveAdvantage:{idea.CompetitiveAdvantage}
            ConversionRate:{idea.RealisticConversionRate}
            backendDeveloperLevel:{backendLevel}
            frontendDeveloperLevel:{frontendLevel}
            DevOpsLevel:{DevopsLevel}
             
            

            Return JSON response with:
            {{
                ""RecommendedTechnology"": ""..."",
                ""ComplexityLevel"": ""Low/Medium/High"",
                ""Reasoning"": ""...""
            }}";

            var response = await GetAIJsonResponse<TechnologyRecommendation>(prompt);
            return response;
        }




        private TeamCostBreakdown CalculateTeamCosts(int BackendDevelopers,int FrontEndDevelopers,int DevOpsEngineers,int QASpecialists, SalaryData salaries, int months)
        {
            decimal backendDevCost = BackendDevelopers * salaries.BackendDeveloper / 12 * months;
            decimal frontendDevCost = FrontEndDevelopers * salaries.FrontEndDeveloper / 12 * months;
            decimal devOpsCost = DevOpsEngineers * salaries.DevOpsEngineer / 12 * months;
            decimal qaCost = QASpecialists * salaries.QAEngineer / 12 * months;

            return new TeamCostBreakdown
            {
                BackEndDeveloperCost = backendDevCost,
                FrontEndDeveloperCost = frontendDevCost,
                DevOpsCost = devOpsCost,
                QACost = qaCost,
                TotalCost = backendDevCost + frontendDevCost + devOpsCost + qaCost
            };
        }



        public RevenueProjection ProjectRevenue(Idea idea)
        {
            int payingUsers = (int)(idea.ExpectedUserCount * (idea.RealisticConversionRate / 100.0));
            decimal mrr = payingUsers * idea.EstimatedMonthlyRevenuePerUser;
            return new RevenueProjection
            {
                Year1 = mrr * 12 * 0.7m,
                Year2 = mrr * 12 * 0.9m * 1.1m,
                Year3 = mrr * 12 * 1.2m,
                PricingModel = new PricingModel
                {
                    Model = "User-Provided",
                    MonthlyPrice = idea.EstimatedMonthlyRevenuePerUser,
                    ExpectedConversionRate = idea.RealisticConversionRate
                }
            };
        }



        private async Task<SWOTAnalysis> GenerateCompleteSWOTAnalysis(Idea idea, MarketData marketData)
        {

            var aiSwot = await GetAISWOTAnalysis(idea);


            return new SWOTAnalysis
            {
                Strengths = aiSwot.Strengths
                    .Select(s => new SWOTFactor
                    {
                        Description = s.Description,

                        Probability = s.Probability
                    }).ToList(),

                Weaknesses = aiSwot.Weaknesses
                    .Select(w => new SWOTFactor
                    {
                        Description = w.Description,
                        Probability = w.Probability
                    }).ToList(),

                Opportunities = aiSwot.Opportunities
                    .Select(o => new SWOTFactor
                    {
                        Description = o.Description,
                        Probability = o.Probability
                    }).ToList(),

                Threats = aiSwot.Threats
                    .Select(t => new SWOTFactor
                    {
                        Description = t.Description,
                        Probability = t.Probability
                    }).ToList()
            };
        }
        private async Task<SWOTAnalysis> GetAISWOTAnalysis(Idea idea)
        {

            var prompt = $@"
            Return ONLY valid JSON. No extra text, no markdown, just a JSON object exactly matching this schema:

            {{
              ""Strengths"": [{{""Description"": ""..."", ""Probability"": 0-1}}],
              ""Weaknesses"": [{{""Description"": ""..."", ""Probability"": 0-1}}],
              ""Opportunities"": [{{""Description"": ""..."", ""Probability"": 0-1}}],
              ""Threats"": [{{""Description"": ""..."", ""Probability"": 0-1}}]
            }}

            Analyze the following business idea and return the SWOT in the format above:
            no extra words lines dashes dots only the json object as FOLLOWING ABOVE
            no extra comment or characters STRONLY FOLLOW MY INSTRUCTIONS
            Assume These Entred Data
            - Idea Title: {idea.Title}
            - Problem Statement: {idea.ProblemStatement}
            - Solution: {idea.ShortDescription}
            - Industry: {idea.Category}
            - Target Market: {idea.TargetAudience}
            - Differentiation: {idea.CompetitiveAdvantage}
            -Country:{idea.Country}
            -ExpectedUserCount:{idea.ExpectedUserCount}
            _FRONTENDComplexity:{idea.FrontendComplexity}
            -RealsticConversionRate:{idea.RealisticConversionRate}
            _DevelopmentComplexity :{idea.DeploymentFrequency}
            ";

            var response = await GetAIJsonResponse<SWOTAnalysis>(prompt);
            return response;

        }





        private decimal CalculateMRR(int expectedUsers, PricingModel pricing)
        {

            int payingUsers = (int)(expectedUsers * (pricing.ExpectedConversionRate / 100.0));

            return payingUsers * pricing.MonthlyPrice;
        }


        private FinancialAnalysis CalculateFinancialMetrics(
     CalculatedInvestment costs,
     RevenueProjection revenue,
     decimal initialInvestment,
       Idea idea,
       MarketData market,
       TechnologyRecommendation technologyRecommendation
            )
        {
            var baseAnnualMaintenance = costs.MaintenanceCost / 3;

            var cashFlows = new List<CashFlow>
{
    new() { Year = 0, Amount = -initialInvestment },
    new() { Year = 1, Amount = revenue.Year1 - baseAnnualMaintenance },
    new() { Year = 2, Amount = revenue.Year2 - baseAnnualMaintenance * 1.10m },
    new() { Year = 3, Amount = revenue.Year3 - baseAnnualMaintenance * 1.21m }
};
            decimal cumulative = 0;
            foreach (var flow in cashFlows)
            {
                cumulative += flow.Amount;
                flow.Cumulative = cumulative;
            }
            double discountRate = CalculateDiscountRate(idea,technologyRecommendation, revenue, market);
            var PaybackPeriodYears = CalculatePaybackPeriod(cashFlows);
            return new FinancialAnalysis
            {
                NPV = CalculateNPV(cashFlows, discountRate),
                IRR = CalculateIRR(cashFlows),
                PaybackPeriodYears = Math.Round(PaybackPeriodYears.TotalDays / 365.0, 2),
                ROI = CalculateROI(initialInvestment, cashFlows),
                BreakEvenPointYear = CalculateBreakEven(cashFlows),
                DiscountRate = discountRate,
                CashFlows = cashFlows,


            };
        }

        private double CalculateDiscountRate(Idea idea, TechnologyRecommendation costs, RevenueProjection revenue, MarketData market)
        {
            double baseRate = market.RiskFreeRate > 0 ? market.RiskFreeRate : 0.03;
            var DevelopmentMonth = SmartDevelopmentMonthEstimator.Estimate(idea);

            double techRisk = DevelopmentMonth.months switch
            {
                > 18 => 0.08,
                > 12 => 0.06,
                > 6 => 0.04,
                _ => 0.02
            };

            double conversionRisk = revenue.PricingModel.ExpectedConversionRate switch
            {
                < 2 => 0.06,
                < 5 => 0.04,
                _ => 0.02
            };

            double countryRisk = idea.Country.ToLower() switch
            {
                "united states" => 0.01,
                "uae" or "germany" or "canada" => 0.02,
                "syria" or "egypt" or "nigeria" => 0.06,
                _ => 0.03
            };


            double volatilityRisk = market.Volatility switch
            {
                > 0.4 => 0.06,
                > 0.25 => 0.04,
                _ => 0.02
            };


            return baseRate + techRisk + conversionRisk + countryRisk + volatilityRisk;
        }

        private double CalculateNPV(List<CashFlow> flows, double discountRate)
        {


            double npv = 0;
            foreach (var flow in flows)
            {
                double denominator = Math.Pow(1 + discountRate, flow.Year);
                npv += Convert.ToDouble(flow.Amount) / denominator;
            }
            return npv;
        }

        private double CalculateIRR(List<CashFlow> flows, double minRate = -0.5, double maxRate = 1.0, double tolerance = 0.0001, int maxIterations = 1000)
        {
            double midRate = 0.0;

            for (int i = 0; i < maxIterations; i++)
            {
                midRate = (minRate + maxRate) / 2.0;
                double npv = CalculateNPV(flows, midRate);

                if (Math.Abs(npv) < tolerance)
                    return midRate * 100;

                if (npv > 0)
                    minRate = midRate;
                else
                    maxRate = midRate;
            }

            return midRate * 100;
        }



        public async Task<(CalculatedInvestment,TechnologyRecommendation)> CalculateRequiredInvestment(Idea idea)
        {


      
            var scopePrompt = $@"
            Analyze this software project to determine resource requirements:
            Project: {idea.Title}
            Category: {idea.Category}
            ProbmlemStatment: {idea.ProblemStatement} 
            Features: {string.Join(", ", idea.Features ?? new List<string>())}
            Target Users: {idea.TargetAudience}
           

            Return JSON with:
            {{
                ""developmentComplexity"": ""low/medium/high"",
                ""infrastructureNeeds"": [""server"", ""database"", ...],
      
            }}";

            var scopeAnalysis = await GetAIJsonResponse<ScopeAnalysis>(scopePrompt);
            var DeveloperLevels = DetectDeveloperLevels(idea);


   
        
            var techRecommendation = await GetTechnologyRecommendation(idea, DeveloperLevels.backendLevel, DeveloperLevels.frontendLevel, DeveloperLevels.devOpsLevel);
          var DevelopmentMonth =  SmartDevelopmentMonthEstimator.Estimate(idea);
            var DeveloperNumberNeeded=TeamSizeEstimator.Estimate(idea);
            var salaryData = await GetDeveloperSalaries(techRecommendation.Reasoning, idea.Country, DeveloperLevels.backendLevel, DeveloperLevels.frontendLevel, DeveloperLevels.devOpsLevel);
            var calculateTeamcost = CalculateTeamCosts(DeveloperNumberNeeded.backend,DeveloperNumberNeeded.frontend,DeveloperNumberNeeded.devops,DeveloperNumberNeeded.qa, salaryData, DevelopmentMonth.months);
            var infrastructre = await CalculateInfrastructureCost(scopeAnalysis, idea);



            var costs = new CalculatedInvestment
            {
                Development = calculateTeamcost.TotalCost,
                Infrastructure = infrastructre.totalMonthly * 36,
                Tooling = CalculateToolingCost(idea.Tools),
                Marketing = MarketingCost(idea.Category, idea.TargetAudience, idea.ExpectedUserCount, idea.RealisticConversionRate),

                SalaryData = salaryData,

                TeamCostBreakdown = calculateTeamcost,
                Contingency = 0,
                InfrastructureCostEstimate = infrastructre

            };
            costs.baseCost = costs.Development + costs.Infrastructure + costs.Tooling + costs.Marketing;

            costs.Contingency =costs.baseCost * 0.25m;
            costs.MaintenanceCost = (costs.TeamCostBreakdown.TotalCost * 0.18m * 3) + (infrastructre.totalMonthly * 36);

            return (costs,techRecommendation);
        }
        private decimal CalculateToolingCost(List<string> tools)
        {
            decimal total = 0;

            foreach (var tool in tools.Distinct())
            {
                total += tool.ToLower() switch
                {
                    "ci/cd" => 120m,           // e.g., GitHub Actions, CircleCI
                    "monitoring" => 150m,      // e.g., Datadog, New Relic
                    "analytics" => 100m,       // e.g., Mixpanel, Amplitude
                    "testing" => 60m,          // e.g., BrowserStack, Postman Pro
                    "version control" => 40m,  // e.g., GitHub Pro
                    "project management" => 50m, // e.g., Jira, Trello Premium
                    "design" => 35m,           // e.g., Figma, Adobe XD
                    "ai tool" => 90m,          // e.g., OpenAI Pro, Copilot
                    _ => 25m                   // Default/fallback
                };
            }

            return total * 12; // Annual cost
        }


        private TimeSpan CalculatePaybackPeriod(List<CashFlow> flows)
        {
            decimal cumulative = 0;
            decimal previousCumulative = 0;
            int previousYear = 0;

            foreach (var flow in flows.OrderBy(f => f.Year))
            {
                previousCumulative = cumulative;
                cumulative += flow.Amount;

                if (cumulative >= 0)
                {
                    int currentYear = flow.Year;


                    decimal remaining = -previousCumulative;
                    decimal gainThisYear = cumulative - previousCumulative;
                    double fraction = (double)(remaining / gainThisYear);

                    double paybackYears = previousYear + fraction;

                    return TimeSpan.FromDays(paybackYears * 365);
                }

                previousYear = flow.Year;
            }

            return TimeSpan.MaxValue;
        }

        private decimal MarketingCost(string category, string targetAudience, int expectedUsers, double conversionRate)
        {
            // Estimate CAC per category based on real benchmarks:
            // - SaaS: ProfitWell, HubSpot
            // - E-commerce: Shopify, WordStream
            // - Education: EdSurge, LearnPlatform
            // - Mobile: Appsflyer, Statista
            // - Fintech & HealthTech: McKinsey, Rock Health
            decimal cac = category.ToLower() switch
            {
                "saas" => 40m,          // $30–$50 average CAC
                "ecommerce" => 55m,     // $45–$65 average CAC
                "education" => 22m,     // $15–$25 average CAC
                "mobile" => 18m,        // $10–$25 subscriptions
                "fintech" => 45m,       // $30–$60 average CAC
                "healthtech" => 50m,    // $35–$70 average CAC
                "gaming" => 30m,        // $10–$40 depending on platform
                _ => 28m                // General average fallback
            };

            // Adjust CAC for enterprise-focused projects (usually higher acquisition cost)
            if (targetAudience.ToLower().Contains("enterprise"))
                cac *= 1.5m;

            // Calculate the number of users expected to convert
            int payingUsers = (int)(expectedUsers * (conversionRate / 100.0));

            // Total direct acquisition cost
            decimal acquisitionCost = payingUsers * cac;

            // Add 20% for brand awareness, remarketing, and retention strategies
            decimal retentionMarketing = acquisitionCost * 0.2m;

            return acquisitionCost + retentionMarketing;
        }

        private double CalculateROI(decimal initialInvestment, List<CashFlow> cashFlows)
        {
            if (initialInvestment == 0) return 0;
            decimal totalReturns = cashFlows.Where(cf => cf.Year != 0).Sum(cf => cf.Amount);
            decimal netProfit = totalReturns - initialInvestment;
            return (double)(netProfit / initialInvestment);
        }

        private int CalculateBreakEven(List<CashFlow> cashFlows)
        {
            decimal cumulative = 0;
            foreach (var flow in cashFlows.OrderBy(f => f.Year))
            {
                cumulative += flow.Amount;
                if (cumulative >= 0)
                    return flow.Year;
            }
            return int.MaxValue;
        }



        private double CalculateFinancialScore(FinancialAnalysis financial)
        {
            double score = 0;

            double paybackYears = financial.PaybackPeriodYears;

            if ((decimal)financial.NPV > 0)
                score += 0.3;
            else if ((decimal)financial.NPV > financial.CashFlows[0].Amount * -0.5m)
                score += 0.15;

            if (financial.IRR > financial.DiscountRate + 0.05)
                score += 0.2;
            else if (financial.IRR > financial.DiscountRate)
                score += 0.1;

            if (financial.ROI >= 0.5)
                score += 0.2;
            else if (financial.ROI > 0.2)
                score += 0.1;

            if (paybackYears < 2)
                score += 0.3;
            else if (paybackYears < 4)
                score += 0.15;

            return Math.Round(score, 3);
        }
        private SuccessProbability CalculateRealisticSuccessProbability(FinancialAnalysis financial)


        {
            double financialScore = CalculateFinancialScore(financial);

            return new SuccessProbability
            {
                FinancialScore = financialScore,

            };
        }



        public class DevelopmentCostAnalysis
        {
            public double seniorDevMonthly { get; set; }
            public double midDevMonthly { get; set; }
            public double juniorDevMonthly { get; set; }
            public RecommendedTeam recommendedTeam { get; set; }
        }
        public class RecommendedTeam
        {
            public double seniors { get; set; }
            public double mids { get; set; }
            public double juniors { get; set; }
        }
        public InfrastructureCostAnalysis infrastructureCostAnalysis { get; set; }
        private async Task<InfrastructureCostAnalysis> CalculateInfrastructureCost(ScopeAnalysis scope, Idea idea)
        {
            var services = string.Join(", ", scope.InfrastructureNeeds);
            var prompt = $@"
            Estimate monthly  infrastructure costs for:
            Services needed: {services}
            Expected users: {idea.ExpectedUserCount}
            Idea Title:{idea.Title}
            Idea SecurityCriticalLevel{idea.SecurityCriticalLevel}
            Idea RequiresRealTimeFeatures:{idea.RequiresRealTimeFeatures}
            Idea Tools:{idea.Tools}
          
            Return JSON with:
            {{
                ""serverCost"": number,
                ""databaseCost"": number,
                ""storageCost"": number,
                ""bandwidthCost"": number,
                ""totalMonthly"": number
                ""source"":string
            }}
            ⚠️ Important Rules:
              - Read the Requirement from the provided Data.
                - Fill all salary fields with realistic non-zero values.
                - Only use real-world Prices sources (e.g., Amazon).
                _fill the source property from where did you got the prices
                - No extra text or markdown outside JSON.
                "";
";


            var costs = await GetAIJsonResponse<InfrastructureCostAnalysis>(prompt);
            infrastructureCostAnalysis = costs;
            return infrastructureCostAnalysis;
           
        }
        public class InfrastructureCostAnalysis
        {
            public int serverCost { get; set; }
            public int databaseCost { get; set; }
            public int storageCost { get; set; }
            public int bandwidthCost { get; set; }
            public int totalMonthly { get; set; }
            public string Source    { get; set; }

        }

        private string GenerateFinancialRecommendation(FinancialAnalysis financial)
        {
            var npv = financial.NPV;
            var irr = financial.IRR;
            var payback = financial.PaybackPeriodYears;
            var roi = financial.ROI;
            var breakEvenYear = financial.BreakEvenPointYear;
            var discountRate = financial.DiscountRate;

            // 1️⃣ **Strong Buy (Exceptional Performance)**
            if (npv >= 0 && irr >= discountRate + 10 && payback <= 2 && roi >= 25 && breakEvenYear <= 2)
            {
                return "🚀 **Strong Buy** - Exceptional returns (High NPV, IRR beats hurdle by 10%+, fast payback & breakeven)";
            }

            // 2️⃣ **Buy (Solid Investment)**
            if (npv >= 0 && irr >= discountRate + 5 && payback <= 4 && roi >= 15 && breakEvenYear <= 3)
            {
                return "📈 **Buy** - Strong fundamentals (Positive NPV, good IRR, reasonable payback)";
            }

            // 3️⃣ **Hold/Neutral (Acceptable but Weak)**
            if (npv >= 0 && irr >= discountRate && payback <= 6 && roi >= 10)
            {
                return "🤔 **Hold/Neutral** - Meets minimum criteria (NPV ≥ 0, IRR > discount rate) but lacks upside";
            }

            // 4️⃣ **High Risk (Borderline, Needs Improvement)**
            if (npv < 0 || irr < discountRate || payback > 6 || roi < 10)
            {
                string warning = npv < 0 ? "Negative NPV" : "";
                warning += irr < discountRate ? (warning != "" ? ", IRR < hurdle" : "IRR < hurdle") : "";
                warning += payback > 6 ? (warning != "" ? ", slow payback" : "Slow payback") : "";
                warning += roi < 10 ? (warning != "" ? ", low ROI" : "Low ROI") : "";

                return $"⚠️ **High Risk** - {warning}. Needs cost cuts or revenue boost.";
            }

            // 5️⃣ **Reject (Financially Unviable)**
            if (npv < 0 && irr < 0 && payback > 10 && roi < 5)
            {
                return "❌ **Reject** - Financially unviable (Negative NPV/IRR, very slow payback)";
            }

            // Default case (if none match)
            return "🔍 **Review Manually** - Unclear case (Check assumptions or sensitivity analysis)";
        }
        private async Task<T> GetAIJsonResponse<T>(string prompt)
        {
            var strictPrompt = $@"{prompt}

        IMPORTANT: Your response MUST:
        1. Be EXACTLY in this JSON schema With scheme
        2. Contain NO other text, markdown, or formatting
        3. Have ALL required fields populated
        4. Escape any special characters ";
            var payload = new
            {
                model = "mistralai/mistral-7b-instruct",
                messages = new[] { new { role = "user", content = strictPrompt } },
                response_format = new { type = "json_object" },
                temperature = 0.3
            };

            var response = await _httpClient.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(content);
            var rawContent = json["choices"]?[0]?["message"]?["content"]?.ToString();


            var match = Regex.Match(rawContent ?? "", @"\{[\s\S]*\}");
            if (!match.Success)
            {
                throw new Exception("Failed to extract JSON from the AI response.");
            }

            var jsonOnly = match.Value;
            return JsonConvert.DeserializeObject<T>(jsonOnly);

        }

        private static readonly Dictionary<string, string> CountryNameToCode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "United States", "USA" },
        { "Germany", "DEU" },
        { "France", "FRA" },
        { "Syria", "SYR" },
        { "United Kingdom", "GBR" },
        { "Canada", "CAN" },
        { "Australia", "AUS" },
        { "India", "IND" },
        { "Brazil", "BRA" },
        { "Japan", "JPN" }
   
    };

        public static string GetCountryCode(string countryName)
        {
            if (CountryNameToCode.TryGetValue(countryName, out var code))
                return code;

            throw new Exception($"Country code not found for: {countryName}");
        }

        private async Task<MarketData> GetTechMarketData(string industryCategory, string countryCode)
        {
            if (countryCode.Length > 3)
            {
              countryCode=  GetCountryCode(countryCode);
            }
            try
            {
                var response = await _httpClient.GetAsync(
                    $"https://api.worldbank.org/v2/country/{countryCode}/indicator/IT.NET.USER.ZS?format=json");

                if (response.IsSuccessStatusCode)
                {
                    var data = await ParseInternetPenetration(response);
                    return new MarketData
                    {
                        GrowthRate = data.InternetGrowthRate,
                        RiskFreeRate = 0.02,
                        MarketReturn = data.InternetGrowthRate * 2,
                        Volatility = 0.3
                    };
                }

                return new MarketData { GrowthRate = 0.12 };
            }
            catch
            {
                return new MarketData { GrowthRate = 0.1 };
            }
        }



        private async Task<InternetPenetrationData> ParseInternetPenetration(HttpResponseMessage response)
        {

            var content = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(content);
            var latestData = json[1]?[0]?["value"]?.ToString();

            return new InternetPenetrationData
            {
                InternetUsersPercent = double.TryParse(latestData, out var percent) ? percent : 30.0,
                InternetGrowthRate = 0.08
            };
        }
        private async Task<decimal> EstimateInfrastructureCosts(int expectedUsers, string technology, string IdeaTitle, string problemStatement, List<string> Features)
        {
            try
            {
                var prompt = $@"
                Estimate the monthly infrastructure cost ONLY in JSON using this structure:

                {{
                  ""ServerCost"": number,
                  ""DatabaseCost"": number,
                  ""StorageCost"": number,
                  ""BandwidthCost"": number
                }}

                Assume:
                - {expectedUsers} expected users
                - Technology stack: {technology}
                _Idea Title:{IdeaTitle}
                _Problem Statement :{problemStatement}
               _Recommendation Technology Reasioning :{technology}
                _list of Features :{string.Join(", ", Features ?? new List<string>())}
                - Output ONLY valid JSON, with no explanations.
                ";

                var response = await GetAIJsonResponse<InfrastructureCostEstimate>(prompt);
                response.TotalMonthly = response.ServerCost + response.BandwidthCost + response.StorageCost + response.DatabaseCost;
                return response.TotalMonthly;
            }
            catch
            {
                Console.WriteLine("There Was Aproblem While Evaluate the infrastructure cost");
                return 0;
            }
        }

        public Task<double> TestNvp(List<CashFlow> flows, double discountRate)
        {
            double npv = 0;
            foreach (var flow in flows)
            {
                npv += (double)flow.Amount / Math.Pow(1 + discountRate, flow.Year);
            }
            return Task.FromResult(npv);
        }

        public Task<double> TestIRR(List<CashFlow> flows, double minRate = -0.5, double maxRate = 1.0, double tolerance = 0.0001, int maxIterations = 1000)
        {
            double midRate = 0.0;

            for (int i = 0; i < maxIterations; i++)
            {
                midRate = (minRate + maxRate) / 2.0;
                double npv = CalculateNPV(flows, midRate);

                if (Math.Abs(npv) < tolerance)
                    return Task.FromResult(midRate * 100);

                if (npv > 0)
                    minRate = midRate;
                else
                    maxRate = midRate;
            }

            return Task.FromResult(midRate * 100);
        }
    }




    public class ScopeAnalysis
    {
        public string DevelopmentComplexity { get; set; }
        public List<string> InfrastructureNeeds { get; set; } = new();
       
    }
    public class InfrastructureCostEstimate
    {
        public decimal ServerCost { get; set; }
        public decimal DatabaseCost { get; set; }
        public decimal StorageCost { get; set; }
        public decimal BandwidthCost { get; set; }
        public string Source { get; set; }
        public decimal TotalMonthly { get; set; }
    }



    public class TechnologyRecommendation
    {
        public string RecommendedTechnology { get; set; }
        public string ComplexityLevel { get; set; }
      
        public string Reasoning { get; set; }
    }

    public class TeamRequirements
    {
        public int FrontEndDevelopers { get; set; }
        public int BackendDevelopers { get; set; }
     
        public int DevOpsEngineers { get; set; }
        public int QASpecialists { get; set; }
    }

    public class SalaryData
    {
        public decimal FrontEndDeveloper { get; set; }
        public decimal BackendDeveloper { get; set; }
       
        public decimal DevOpsEngineer{ get; set; }
        public decimal QAEngineer { get; set; }
        public string Source { get; set; }

    }

    public class ProjectCostEstimation
    {
        public int DevelopmentMonths { get; set; }
        public TeamCostBreakdown TeamCosts { get; set; }
        public decimal InfrastructureCosts { get; set; }
        public decimal MaintenanceCosts { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class TeamCostBreakdown
    {
        public decimal FrontEndDeveloperCost { get; set; }
        public decimal BackEndDeveloperCost { get; set; }
        public decimal DevOpsCost { get; set; }
        public decimal QACost { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class RevenueProjection
    {
        public decimal Year1 { get; set; }
        public decimal Year2 { get; set; }
        public decimal Year3 { get; set; }
        public PricingModel PricingModel { get; set; }
    }

    public class PricingModel
    {
        public string Model { get; set; }
        public decimal MonthlyPrice { get; set; }
        public double ExpectedConversionRate { get; set; }
    }
    public class CalculatedInvestment
    {
        public decimal Development { get; set; }
        public decimal Infrastructure { get; set; }
        public decimal Tooling { get; set; }
        public decimal Marketing { get; set; }
        public decimal Contingency { get; set; }
        public decimal baseCost { get; set; }
        public SalaryData SalaryData { get; set; }
        public TeamCostBreakdown TeamCostBreakdown { get; set; }
        public InfrastructureCostAnalysis InfrastructureCostEstimate { get; set; }
        public decimal MaintenanceCost { get; set; } 

        
        public decimal Total => Development + Infrastructure + Tooling + Marketing + Contingency;
    }


    public class SuccessProbability
    {
        public double FinancialScore { get; set; }

        public double TotalScore { get; set; }
    }
    public class InternetPenetrationData
    {
        public double InternetUsersPercent { get; set; }
        public double InternetGrowthRate { get; set; }
    }
    public static class SmartDevelopmentMonthEstimator
    {
   
        public static (int months, string notes) Estimate(Idea idea)
        {
            int months = 6; // Base months for MVP
            List<string> notes = new List<string> { "Base 6 months (default)" };

            if (idea.Features != null)
            {
                if (idea.Features.Count > 10)
                {
                    months += 6;
                    notes.Add("+6 months for >10 features");
                }
                else if (idea.Features.Count > 7)
                {
                    months += 4;
                    notes.Add("+4 months for 8-10 features");
                }
                else if (idea.Features.Count >= 5)
                {
                    months += 2;
                    notes.Add("+2 months for 5-7 features");
                }
            }

            if (idea.RequiresRealTimeFeatures)
            {
                months += 2;
                notes.Add("+2 months for real-time features");
            }

            if (idea.RequiresMobileApp)
            {
                months += 3;
                notes.Add("+3 months for mobile app");
            }

            if (idea.SecurityCriticalLevel == "Highly Sensitive")
            {
                months += 3;
                notes.Add("+3 months for highly sensitive security");
            }

            if (idea.ExpectedUserCount > 50000)
            {
                months += 4;
                notes.Add("+4 months for >50,000 users");
            }
            else if (idea.ExpectedUserCount > 10000)
            {
                months += 2;
                notes.Add("+2 months for >10,000 users");
            }

            if (idea.Category?.Contains("Fintech", StringComparison.OrdinalIgnoreCase) == true)
            {
                months += 3;
                notes.Add("+3 months for Fintech category");
            }

            if (months > 18) months = 18; 

            return (months, string.Join(" | ", notes));
        }
    }

    public static class TeamSizeEstimator
    {
       
        public static (int frontend, int backend, int devops, int qa, string notes) Estimate(Idea idea)
        {
            int frontend = 1, backend = 1, devops = 1, qa = 1;
            List<string> notes = new List<string> { "Base team: 1 Frontend, 1 Backend, 1 DevOps, 1 QA" };

            if (idea.ExpectedUserCount > 5000)
            {
                frontend++;
                backend++;
                notes.Add("+1 Frontend, +1 Backend for >5000 users");
            }
            if (idea.ExpectedUserCount > 20000)
            {
                backend++;
                devops++;
                notes.Add("+1 Backend, +1 DevOps for >20000 users");
            }

            if (idea.Features != null && idea.Features.Count > 7)
            {
                frontend++;
                backend++;
                notes.Add("+1 Frontend, +1 Backend for >7 features");
            }

            if (idea.RequiresRealTimeFeatures)
            {
                backend++;
                devops++;
                notes.Add("+1 Backend, +1 DevOps for real-time");
            }

            if (idea.RequiresMobileApp)
            {
                frontend++;
                notes.Add("+1 Frontend for mobile app");
            }

            if (idea.SecurityCriticalLevel == "Highly Sensitive")
            {
                devops++;
                qa++;
                notes.Add("+1 DevOps, +1 QA for highly sensitive security");
            }

            return (frontend, backend, devops, qa, string.Join(" | ", notes));
        }
    }

}










