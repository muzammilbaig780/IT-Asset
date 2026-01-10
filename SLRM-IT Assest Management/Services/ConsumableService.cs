using AssetManagement.Data;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;
using System;
using System.Threading.Tasks;

namespace SLRM_IT_Assest_Management.Services
{
    public class ConsumableService : IConsumableService
    {
        private readonly ApplicationDbContext _context;

        public ConsumableService(ApplicationDbContext context)
        {
            _context = context;
        }

        /* ===================== STOCK IN ===================== */
        public async Task StockInAsync(int consumableId, decimal quantity, string referenceNo, string performedBy)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var stock = await _context.ConsumableStocks
                    .FirstOrDefaultAsync(s => s.ConsumableId == consumableId);

                if (stock == null)
                    throw new Exception("Stock record not found.");

                stock.TotalQuantity += quantity;
                stock.AvailableQuantity += quantity;
                stock.LastUpdatedOn = DateTime.Now;

                var ledger = new ConsumableTransaction
                {
                    ConsumableId = consumableId,
                    TransactionType = ConsumableTransactionType.StockIn,
                    Quantity = quantity,
                    TransactionDate = DateTime.Now,
                    PerformedBy = performedBy,
                    ReferenceNo = referenceNo,
                    Remarks = "Stock in"
                };

                _context.ConsumableTransactions.Add(ledger);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /* ===================== ISSUE ===================== */
        public async Task IssueAsync(int consumableId, decimal quantity, int assetId, string performedBy)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var stock = await _context.ConsumableStocks
                    .FirstOrDefaultAsync(s => s.ConsumableId == consumableId);

                if (stock == null)
                    throw new Exception("Stock record not found.");

                if (stock.AvailableQuantity < quantity)
                    throw new Exception("Insufficient available stock.");

                stock.AvailableQuantity -= quantity;
                stock.LastUpdatedOn = DateTime.Now;

                var ledger = new ConsumableTransaction
                {
                    ConsumableId = consumableId,
                    AssetId = assetId,
                    TransactionType = ConsumableTransactionType.Issue,
                    Quantity = quantity,
                    TransactionDate = DateTime.Now,
                    PerformedBy = performedBy,
                    Remarks = "Issued to asset"
                };

                _context.ConsumableTransactions.Add(ledger);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /* ===================== RETURN ===================== */
        public async Task ReturnAsync(int consumableId, decimal quantity, int assetId, string performedBy)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var stock = await _context.ConsumableStocks
                    .FirstOrDefaultAsync(s => s.ConsumableId == consumableId);

                if (stock == null)
                    throw new Exception("Stock record not found.");

                stock.AvailableQuantity += quantity;
                stock.LastUpdatedOn = DateTime.Now;

                var ledger = new ConsumableTransaction
                {
                    ConsumableId = consumableId,
                    AssetId = assetId,
                    TransactionType = ConsumableTransactionType.Return,
                    Quantity = quantity,
                    TransactionDate = DateTime.Now,
                    PerformedBy = performedBy,
                    Remarks = "Returned from asset"
                };

                _context.ConsumableTransactions.Add(ledger);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
