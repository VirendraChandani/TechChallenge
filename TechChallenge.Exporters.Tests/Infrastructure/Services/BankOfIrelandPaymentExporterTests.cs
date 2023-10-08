using FluentAssertions;
using TechChallenge.Common.Infrastructure.Dto;
using TechChallenge.Exporters.Infrastructure.Services;

namespace TechChallenge.Exporters.Tests.Infrastructure.Services
{
    public class BankOfIrelandPaymentExporterTests
    {
        [Fact]
        public async Task SelectPaymentsForBankAsync_PassedNullPaymentsReturnShouldBeNull()
        {
            // Arrange
            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var selectedPayments = await exporter.SelectPaymentsForBankAsync(null);

            // Assert
            selectedPayments.Should().BeNull();
        }

        [Fact]
        public async Task SelectPaymentsForBankAsync_PassedEmptyPaymentsReturnShouldBeEmpty()
        {
            // Arrange
            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var selectedPayments = await exporter.SelectPaymentsForBankAsync(new List<Payment>());

            // Assert
            selectedPayments.Should().BeEmpty();
        }

        [Fact]
        public async Task SelectPaymentsForBankAsync_ShouldSelectCorrectPayments()
        {
            // Arrange
            var payments = new List<Payment>
            {
                new Payment { Currency = "EUR", Country = "IT" },
                new Payment { Currency = "EUR", Country = "GE" },
                new Payment { Currency = "USD", Amount = 150000 },
                new Payment { Currency = "GBP" },
                new Payment { Currency = "USD", Amount = 90000 },
            };

            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var selectedPayments = await exporter.SelectPaymentsForBankAsync(payments);

            // Assert
            selectedPayments.Should().HaveCount(2);
            selectedPayments.Should().Contain(p => p.Currency == "EUR" && p.Country == "IT");
            selectedPayments.Should().Contain(p => p.Currency == "USD" && p.Amount > 100000);
        }

        [Fact]
        public async Task ValidatePaymentsAsync_PassedNullPaymentsReturnsNull()
        {
            // Arrange
            var validationDictionary = new Dictionary<int, List<string>>();
            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var validPayments = await exporter.ValidatePaymentsAsync(null, validationDictionary);

            // Assert
            validPayments.Should().BeNull();
        }

        [Fact]
        public async Task ValidatePaymentsAsync_PassedNullDictionaryReturnsNull()
        {
            // Arrange
            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var validPayments = await exporter.ValidatePaymentsAsync(new List<Payment>(), null);

            // Assert
            validPayments.Should().BeNull();
        }

        [Fact]
        public async Task ValidatePaymentsAsync_PassedEmptyPaymentsReturnsEmpty()
        {
            // Arrange
            var payments = new List<Payment>();
            var validationDictionary = new Dictionary<int, List<string>>();
            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var validPayments = await exporter.ValidatePaymentsAsync(payments, validationDictionary);

            // Assert
            validPayments.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidatePaymentsAsync_ShouldValidatePayment()
        {
            // Arrange
            var payments = new List<Payment>
            {
                new Payment {Number = 1, Amount = 50000, Country = "US", BeneficiaryFirstName = "John", BeneficiaryLastName = "Doe" },
                new Payment {Number = 2, Amount = 0, Country = "US", BeneficiaryFirstName = "ABC", BeneficiaryLastName = "XYZ" },
                new Payment {Number = 3, Amount = 100, Country = "", BeneficiaryFirstName = "ABC", BeneficiaryLastName = "XYZ" },
                new Payment {Number = 4, Amount = 99, Country = "GB", BeneficiaryFirstName = "123456", BeneficiaryLastName = "My surname is also big" },
                new Payment {Number = 5, Amount = 11, Country = "IT", BeneficiaryFirstName = "ABC", BeneficiaryLastName = "456" },
                new Payment {Number = 6, Currency = "EUR", Amount = 11, Country = "IT", BeneficiaryFirstName = "ABC", BeneficiaryLastName = "XYZ" },
                new Payment {Number = 7, Currency = "USD", Amount = 0, Country = "", BeneficiaryFirstName = "12345", BeneficiaryLastName = "6789" },
                new Payment {Number = 8, Currency = "EUR", Amount = 10, Country = "", BeneficiaryFirstName = "12345", BeneficiaryLastName = "6789" },
            };

            var expectedErrors = new Dictionary<int, List<string>>
            {
                { 2, new List<string> { "Payment amount cannot be 0." } },
                { 3, new List<string> { "Country is mandatory." } },
                { 4, new List<string> { "BeneficiaryFirstName is invalid." } },
                { 5, new List<string> { "BeneficiaryLastName is invalid." } },
                { 6, new List<string> { "EUR Payments must be at least 320 EUR." } },
                { 7, new List<string> { "Payment amount cannot be 0.", "Country is mandatory.", "BeneficiaryFirstName is invalid.", "BeneficiaryLastName is invalid." } },
                { 8, new List<string> { "Country is mandatory.", "BeneficiaryFirstName is invalid.", "BeneficiaryLastName is invalid.", "EUR Payments must be at least 320 EUR." } }
            };

            var validationDictionary = new Dictionary<int, List<string>>();
            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var validPayments = await exporter.ValidatePaymentsAsync(payments, validationDictionary);

            // Assert
            validPayments.Should().HaveCount(1);
            validationDictionary.Should().HaveCount(expectedErrors.Count);

            foreach (var kvp in expectedErrors)
            {
                validationDictionary.ContainsKey(kvp.Key).Should().BeTrue();
                validationDictionary[kvp.Key].Should().BeEquivalentTo(kvp.Value);
            }
        }

        [Fact]
        public async Task ExportPaymentsAsync_PassedNullPaymentsReturnsNull()
        {
            // Arrange
            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var selectedPayments = await exporter.ExportPaymentsAsync(null);

            // Assert
            selectedPayments.Should().BeNull();
        }

        [Fact]
        public async Task ExportPaymentsAsync_ShouldExportPaymentsToCSV()
        {
            // Arrange
            var payments = new List<Payment>
        {
            new Payment { Currency = "USD", Amount = 50000, BeneficiaryFirstName = "John", BeneficiaryLastName = "Doe", Address = "123 Main St" },
            new Payment { Currency = "GBP", Amount = 20000, BeneficiaryFirstName = "Alice", BeneficiaryLastName = "Smith", Address = "456 Elm St" },
        };

            var exporter = new BankOfIrelandPaymentExporter();

            // Act
            var fileName = await exporter.ExportPaymentsAsync(payments);

            // Assert
            fileName.Should().NotBeNullOrEmpty();
            File.Exists(fileName).Should().BeTrue();

            // Clean up: Delete the temporary file created during testing
            File.Delete(fileName);
        }
    }
}
