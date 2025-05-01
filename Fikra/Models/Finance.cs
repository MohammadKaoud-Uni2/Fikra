using Fikra.Service.Implementation;
using static Fikra.Service.Implementation.IdeaService;

namespace Fikra.Models
{
    public class FullProjectAnalysis
    {
  
        public MarketData MarketAnalysis { get; set; }
        public SWOTAnalysis SWOTAnalysis { get; set; }
        public FinancialAnalysis FinancialAnalysis { get; set; }
        public decimal InitialInvestmentRequired {  get; set; }
        
        public string Error { get; set; }
        public DateTime GeneratedAt { get; set; }
        public TechnologyRecommendation technologyRecommendation { get; set; }
        public CalculatedInvestment CostEstimation { get; set; }
        public RevenueProjection revenueProjection { get; set; }

        public SalaryData salaryData { get; set; }
        public int initWorkingMonthDevelopment { get; set; }
        public int FrontEndDevelopers { get; set; }
        public int BackEndDevelopers { get; set; }
        public int QA {  get; set; }
        public int DevOps { get; set; }


    }

    public class IdeaBasicInfo
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string ProblemStatement { get; set; }
        public string SolutionSummary { get; set; }
        public decimal InitialInvestment { get; set; }
        public int TeamExperience { get; set; }
    }

    public class MarketData
    {
        public double GrowthRate { get; set; }
        public double RiskFreeRate { get; set; }
        public double MarketReturn { get; set; }
        public double Volatility { get; set; }

    }
    public class CostEstimation
    {
        public decimal Development { get; set; }
        public decimal Infrastructure { get; set; }
        public decimal Tooling { get; set; }
        public decimal Marketing { get; set; }
        public decimal Contingency { get; set; }

        // Ongoing Maintenance After Launch (not part of initial investment)
        public decimal Maintenance { get; set; }

        // Convenience Property to Auto-Calculate
        public decimal InitialInvestmentRequired => Development + Infrastructure + Tooling + Marketing + Contingency;
        public decimal TotalThreeYearCost => InitialInvestmentRequired + Maintenance;
    }

    public class SWOTAnalysis
    {
        public List<SWOTFactor> Strengths { get; set; } = new List<SWOTFactor>();
        public List<SWOTFactor> Weaknesses { get; set; } = new List<SWOTFactor>();
        public List<SWOTFactor> Opportunities { get; set; } = new List<SWOTFactor>();
        public List<SWOTFactor> Threats { get; set; } = new List<SWOTFactor>();
    }

    public class SWOTFactor
    {
        public string Description { get; set; }


    }

    public class FinancialAnalysis
    {
        public double NPV { get; set; }
        public double IRR { get; set; }
        public double  PaybackPeriodYears { get; set; }
        public double ROI { get; set; }
        public int BreakEvenPointYear { get; set; } 
        public double DiscountRate { get; set; }
        public List<CashFlow> CashFlows { get; set; }

    
    }

   



    public class CashFlow
    {
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public decimal Cumulative { get; set; }
    }

    public class WorldBankResponse
    {
        public List<WorldBankData> Data { get; set; }
    }

    public class WorldBankData
    {
        public double value { get; set; }
        public string date { get; set; }
    }
}
