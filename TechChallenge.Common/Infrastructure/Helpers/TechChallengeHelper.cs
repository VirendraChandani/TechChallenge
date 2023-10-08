using TechChallenge.Common.Infrastructure.Dto;

namespace TechChallenge.Common.Infrastructure.Helpers
{
    public static class TechChallengeHelper
    {
        public static void AddPayment(List<Payment> payments, string currency, decimal amount, string beneficiaryFirstName, string beneficiaryLastName, 
            string address, string country, string postcode = "")
        {
            var payment = new Payment()
            {
                BeneficiaryFirstName = beneficiaryFirstName, 
                BeneficiaryLastName = beneficiaryLastName,
                Amount = amount, 
                Address = address, 
                Postcode = postcode, 
                Currency = currency, 
                Country = country
            };

            payments.Add(payment);
            payment.Number = payments.Count;
        }

        public static List<Payment> Payments
        {
            get
            {
                var result = new List<Payment>();
                AddPayment(result, "USD", 0, "First", "Chunn", "Address Line 1\r\nAddress Line 2", "IT", "");
                AddPayment(result, "USD", 112, "Rossi", "Leiper", "VIA GARIBALDI 26", "IT", "10238");
                AddPayment(result, "GBP", 223, "Nicci", "Osbourne", "25 Eastcheap, London", "GB", "BR1 1LT");
                AddPayment(result, "USD", 500000, "Fred", "Everitt", "248 Burnt Ash Lane", "IT", "10238");
                AddPayment(result, "USD", 70000, "Gemma", "Harris", "1 Sunset Street", "IT", "10250");
                AddPayment(result, "EUR", 234, "Lucy", "Readman", "3 Adelaide Court", "IT", "23042");
                AddPayment(result, "EUR", 2234, "1234567", "123145352", "8 Range Villas", "IT", "BR1 1LT");
                AddPayment(result, "GBP", 23, "1234567", "123145352", "2 Croydon Road", "", "BR1 1LT");
                AddPayment(result, "EUR", 2223, "Seb", "Ontario", "VIA GARIBALDI 26", "IT", "10238");
                AddPayment(result, "USD", 2341, "Joe", "Bloggs", "231 Sunset Road", "US", "41093");
                AddPayment(result, "USD", 231, "Janet", "Jones", "4 Turners Way", "US", "23141");
                AddPayment(result, "USD", 44, "Joe", "Smith", "78 Lakeside Avenue", "US", "11431");
                AddPayment(result, "USD", 233142, "Fred", "Jones", "99 Ampare Way", "US", "42442");
                AddPayment(result, "USD", 3212421, "Amy", "Smith", "91 Festival Road", "US", "45323");
                AddPayment(result, "USD", 2312441, "Robert Testing This", "Payment Is ok", "334 Summercourt Road", "US", "22345");
                AddPayment(result, "USD", 2312441, "Amy", "Brown", "213 Hayes Road", "US", "42345");

                return result;
            }
        }
    }
}
