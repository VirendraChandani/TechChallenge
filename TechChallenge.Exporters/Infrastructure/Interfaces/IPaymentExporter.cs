using TechChallenge.Common.Infrastructure.Dto;

namespace TechChallenge.Exporters.Infrastructure.Interfaces
{
    public interface IPaymentExporter
    {
        Task<List<Payment>> SelectPaymentsForBankAsync(List<Payment> payments);
        Task<List<Payment>> ValidatePaymentsAsync(List<Payment> payments, Dictionary<int, List<string>> validationDictionary);
        Task<string> ExportPaymentsAsync(List<Payment> payments);
    }
}
