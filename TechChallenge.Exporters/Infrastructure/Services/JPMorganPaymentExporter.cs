using TechChallenge.Common.Infrastructure.Dto;
using TechChallenge.Exporters.Infrastructure.Interfaces;

namespace TechChallenge.Exporters.Infrastructure.Services
{
    public class JPMorganPaymentExporter : IPaymentExporter
    {
        public async Task<List<Payment>> SelectPaymentsForBankAsync(List<Payment> payments)
        {
            if(payments == null) 
            {
                return null;
            }

            var selectedPayments = payments
                .Where(payment =>
                    (payment.Currency == "GBP") ||
                    (payment.Currency == "USD" && payment.Amount <= 100000))
                .ToList();

            return selectedPayments;
        }

        public async Task<List<Payment>> ValidatePaymentsAsync(List<Payment> payments, Dictionary<int, List<string>> validationDictionary)
        {
            if (payments == null || validationDictionary == null)
            {
                return null;
            }

            var validPayments = new List<Payment>();
            foreach (var payment in payments)
            {
                var errors = new List<string>();

                if (payment == null)
                {
                    errors.Add("Payment is null.");
                    continue;
                }

                if (payment.Amount == 0)
                {
                    errors.Add("Payment amount cannot be 0.");
                }

                if (string.IsNullOrEmpty(payment.Country))
                {
                    errors.Add("Country is mandatory.");
                }

                var beneficiaryName = $"{payment.BeneficiaryFirstName} {payment.BeneficiaryLastName}";
                if (beneficiaryName.Length > 30)
                {
                    errors.Add("BeneficiaryName cannot exceed 30 characters.");
                }

                if (string.IsNullOrEmpty(payment.Postcode))
                {
                    errors.Add("Postcode is mandatory.");
                }

                if (errors.Any())
                {
                    validationDictionary.Add(payment.Number, errors);
                }
                else
                {
                    validPayments.Add(payment);
                }
            }

            return validPayments;
        }

        public async Task<string> ExportPaymentsAsync(List<Payment> payments)
        {
            if (payments == null)
            {
                return null;
            }

            var fileName = Path.GetTempFileName();
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("JP Morgan");
                writer.WriteLine("Currency,Amount,BeneficiaryName,Address,Postcode");

                foreach (var payment in payments)
                {
                    if (payment == null)
                    {
                        continue;
                    }

                    var beneficiaryName = $"{payment.BeneficiaryFirstName} {payment.BeneficiaryLastName}";
                    writer.WriteLine($"{payment.Currency},{payment.Amount},{beneficiaryName},{payment.Address},{payment.Postcode}");
                }
            }

            return fileName;
        }
    }
}
