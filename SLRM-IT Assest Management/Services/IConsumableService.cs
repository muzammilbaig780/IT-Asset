using System.Threading.Tasks;

namespace SLRM_IT_Assest_Management.Services
{
    public interface IConsumableService
    {
        Task StockInAsync(int consumableId, decimal quantity, string referenceNo, string performedBy);
        Task IssueAsync(int consumableId, decimal quantity, int assetId, string performedBy);
        Task ReturnAsync(int consumableId, decimal quantity, int assetId, string performedBy);
    }
}
