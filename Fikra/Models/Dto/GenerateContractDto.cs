﻿using System.ComponentModel.DataAnnotations;

namespace Fikra.Models.Dto
{
    public class GenerateContractDto
    {
        public double Budget { get; set; }
        [Compare("Budget", ErrorMessage = "Budget And BudgetConfirmation from Another Peer Are not the same")]
        public  double BudgetConfirmation { get; set; }
        public double IdeaOwnerPercentage { get; set; }
        [Compare("IdeaOwnerPercentage", ErrorMessage = "IdeaOwner percentage  And IdeaOwnerPercentage from Another Peer Are not the Same")]

        public double IdeaOwnerPercentageConfirmation { get; set; }

        public string IdeaOwnerSignature { get; set; }
        public string InvestorSignature { get; set; }
        public string InvestorName {  get; set; }

        public string IdeaOwnerName { get; set; }
        public string IdeaTitle { get; set; }
        [Compare("IdeaTitle", ErrorMessage = "not The Same for The Another Side")]

        public string IdeaTitleConfirmation { get; set; }

    }
}
