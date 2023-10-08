using TechChallenge.Common.Infrastructure.Dto;
using TechChallenge.Exporters.Infrastructure.Interfaces;
using TechChallenge.Exporters.Infrastructure.Services;

namespace TechChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var allPayments = Common.Infrastructure.Helpers.TechChallengeHelper.Payments;
            var validationDictionary = new Dictionary<int, List<string>>();

            // Change this to change the bank being used
            const string selectedBank = "BankOfIreland";
            //const string selectedBank = "JPMorgan";

            Console.WriteLine($"Selected Bank: {selectedBank}");

            IPaymentExporter paymentExporter = CreatePaymentExporter(selectedBank);

            if (paymentExporter == null)
            {
                Console.WriteLine("Invalid bank selection.");
                return;
            }

            List<Payment> paymentsForBank = await paymentExporter.SelectPaymentsForBankAsync(allPayments);
            if (paymentsForBank == null)
            {
                Console.WriteLine("No payments to process.");
                return;
            }
            List<Payment> validPaymentsForBank = await paymentExporter.ValidatePaymentsAsync(paymentsForBank, validationDictionary);

            OutputValidationDictionary(validationDictionary);

            Console.WriteLine($"Total payments with validation errors: {validationDictionary.Count}");
            Console.WriteLine($"Total payments to be exported: {validPaymentsForBank.Count}");

            string filename = await paymentExporter.ExportPaymentsAsync(validPaymentsForBank);
            Console.WriteLine($"Destination file: {filename}");

            var text = await File.ReadAllTextAsync(filename);
            Console.WriteLine(text);

            PressAKeyToContinue();
        }

        private static IPaymentExporter CreatePaymentExporter(string selectedBank)
        {
            switch (selectedBank)
            {
                case "BankOfIreland":
                    return new BankOfIrelandPaymentExporter();
                case "JPMorgan":
                    return new JPMorganPaymentExporter();
                // Add more cases for other banks as needed
                default:
                    return null;
            }
        }

        private static void OutputValidationDictionary(Dictionary<int, List<string>> validationDictionary)
        {
            foreach (var entry in validationDictionary)
            {
                var payment = entry.Key;

                Console.WriteLine($"Validation Errors for payment number: {payment}");
                foreach (var text in entry.Value)
                {
                    Console.WriteLine(text);
                }

                Console.WriteLine();
            }

            PressAKeyToContinue();
        }

        private static void PressAKeyToContinue()
        {
            Console.WriteLine("Press a key to continue");
            Console.ReadKey();
        }
    }
}
