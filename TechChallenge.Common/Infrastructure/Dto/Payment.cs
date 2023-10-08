using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechChallenge.Common.Infrastructure.Dto
{
    public class Payment
    {
        public string BeneficiaryFirstName { get; set; }
        public string BeneficiaryLastName { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public int Number { get; set; }
    }
}
