using TechChallenge.Common.Infrastructure.Dto;
using TechChallenge.Exporters.Infrastructure.Interfaces;

namespace TechChallenge.Exporters.Infrastructure.Services
{
    public class BankOfIrelandPaymentExporter : IPaymentExporter
    {
        public async Task<List<Payment>> SelectPaymentsForBankAsync(List<Payment> payments)
        {
            if (payments == null)
            {
                return null;
            }

            var selectedPayments = payments
                .Where(payment =>
                    (payment.Currency == "EUR" && payment.Country == "IT") ||
                    (payment.Currency == "USD" && payment.Amount > 100000))
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

                if (string.IsNullOrWhiteSpace(payment.BeneficiaryFirstName) || payment.BeneficiaryFirstName.All(char.IsDigit))
                {
                    errors.Add("BeneficiaryFirstName is invalid.");
                }

                if (string.IsNullOrWhiteSpace(payment.BeneficiaryLastName) || payment.BeneficiaryLastName.All(char.IsDigit))
                {
                    errors.Add("BeneficiaryLastName is invalid.");
                }

                if (payment.Currency == "EUR" && payment.Amount < 320)
                {
                    errors.Add("EUR Payments must be at least 320 EUR.");
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
                writer.WriteLine("Bank Of Ireland");
                writer.WriteLine("Currency,Amount,BeneficiaryFirstName,BeneficiaryLastName,Address");

                foreach (var payment in payments)
                {
                    if (payment == null)
                    {
                        continue;
                    }

                    writer.WriteLine($"{payment.Currency},{payment.Amount},{payment.BeneficiaryFirstName},{payment.BeneficiaryLastName},{payment.Address}");
                }
            }

            return fileName;
        }
    }
}
