using FluentAssertions;
using TechChallenge.Common.Infrastructure.Dto;
using TechChallenge.Exporters.Infrastructure.Services;

namespace TechChallenge.Exporters.Tests
{
    public class JPMorganPaymentExporterTests
    {
        [Fact]
        public async Task SelectPaymentsForBankAsync_PassedNullPaymentsReturnsNull()
        {
            // Arrange
            var exporter = new JPMorganPaymentExporter();

            // Act
            var selectedPayments = await exporter.SelectPaymentsForBankAsync(null);

            // Assert
            selectedPayments.Should().BeNull();
        }

        [Fact]
        public async Task SelectPaymentsForBankAsync_PassedEmptyPaymentsReturnsEmpty()
        {
            // Arrange
            var exporter = new JPMorganPaymentExporter();

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
                new Payment { Currency = "USD", Amount = 50000 },
                new Payment { Currency = "GBP", Amount = 20000 },
                new Payment { Currency = "USD", Amount = 200000 },
                new Payment { Currency = "EUR", Amount = 2000 },
            };

            var exporter = new JPMorganPaymentExporter();

            // Act
            var selectedPayments = await exporter.SelectPaymentsForBankAsync(payments);

            // Assert
            selectedPayments.Should().HaveCount(2); 
            selectedPayments.Should().Contain(p => p.Currency == "USD" && p.Amount <= 100000);
            selectedPayments.Should().Contain(p => p.Currency == "GBP");
        }

        [Fact]
        public async Task ValidatePaymentsAsync_PassedNullPaymentsReturnsNull()
        {
            // Arrange
            var validationDictionary = new Dictionary<int, List<string>>();
            var exporter = new JPMorganPaymentExporter();

            // Act
            var validPayments = await exporter.ValidatePaymentsAsync(null, validationDictionary);

            // Assert
            validPayments.Should().BeNull();
        }

        [Fact]
        public async Task ValidatePaymentsAsync_PassedNullDictionaryReturnsNull()
        {
            // Arrange
            var exporter = new JPMorganPaymentExporter();

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
            var exporter = new JPMorganPaymentExporter();

            // Act
            var validPayments = await exporter.ValidatePaymentsAsync(payments, validationDictionary);

            // Assert
            validPayments.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidatePaymentsAsync_ShouldValidatePayments()
        {
            // Arrange
            var payments = new List<Payment>
            {
                new Payment {Number = 1, Amount = 50000, Country = "USA", BeneficiaryFirstName = "John", BeneficiaryLastName = "Doe", Postcode = "12345" },
                new Payment {Number = 2, Amount = 0, Country = "US", BeneficiaryFirstName = "ABC", BeneficiaryLastName = "XYZ", Postcode = "23141" },
                new Payment {Number = 3, Amount = 100, Country = "", BeneficiaryFirstName = "ABC", BeneficiaryLastName = "XYZ", Postcode = "23141" },
                new Payment {Number = 4, Amount = 99, Country = "GB", BeneficiaryFirstName = "My name is very big", BeneficiaryLastName = "My surname is also big", Postcode = "BR1 1LT" },
                new Payment {Number = 5, Amount = 11, Country = "IT", BeneficiaryFirstName = "ABC", BeneficiaryLastName = "456", Postcode = "" },
                new Payment {Number = 6, Amount = 0, Country = "", BeneficiaryFirstName = "My name is very big", BeneficiaryLastName = "My surname is also big", Postcode = "" },
            };

            var expectedErrors = new Dictionary<int, List<string>>
            {
                { 2, new List<string> { "Payment amount cannot be 0." } },
                { 3, new List<string> { "Country is mandatory." } },
                { 4, new List<string> { "BeneficiaryName cannot exceed 30 characters." } },
                { 5, new List<string> { "Postcode is mandatory." } },
                { 6, new List<string> { "Payment amount cannot be 0.", "Country is mandatory.", "BeneficiaryName cannot exceed 30 characters.", "Postcode is mandatory." } }
            };

            var validationDictionary = new Dictionary<int, List<string>>();
            var exporter = new JPMorganPaymentExporter();

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
            var exporter = new JPMorganPaymentExporter();

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
                new Payment { Currency = "USD", Amount = 50000, BeneficiaryFirstName = "John", BeneficiaryLastName = "Doe", Address = "123 Main St", Postcode = "12345" },
                new Payment { Currency = "GBP", Amount = 20000, BeneficiaryFirstName = "Alice", BeneficiaryLastName = "Smith", Address = "456 Elm St", Postcode = "67890" },
            };

            var exporter = new JPMorganPaymentExporter();

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