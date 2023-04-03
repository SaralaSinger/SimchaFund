using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class ViewModel
    {
        public List<Simcha> Simchas { get; set; }
        public List<Contributor> Contributors { get; set; }
        public List<Payment> Contributions { get; set; }
        public List<Payment> Actions { get; set; }
        public string ContributorName { get; set; }
        public decimal Balance { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
        public int TotalContributorCount { get; set; }
        public string Message { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
