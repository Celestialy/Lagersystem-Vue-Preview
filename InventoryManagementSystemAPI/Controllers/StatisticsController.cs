using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using InventoryManagementSystemAPI.Database;
using InventoryManagementSystemAPI.DTOs.Response;
using InventoryManagementSystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public StatisticsController(DatabaseContext context)
        {
            this._context = context;
        }

        // GET: api/statistics
        [HttpGet]
        [Route("get_inventory_statistics")]
        public async Task<IActionResult> GetInventoryStatistics()
        {
            InventoryStatisticsDTO inventoryStatistics = new InventoryStatisticsDTO();

            if (AnyItems(InventoryType.loan))
            {
                foreach (var item in _context.LoanItems.Include(b => b.Barcodes))
                {
                    inventoryStatistics.LoanAmount += item.Barcodes.Count;
                    inventoryStatistics.LoanedAmount += item.Barcodes.Count - item.AmountLeft;
                }
            }

            if (AnyItems(InventoryType.consumption))
            {
                foreach (var item in _context.ConsumptionItems)
                {
                    inventoryStatistics.ConsumptionAmount += item.AmountLeft;
                }
            }

            await Task.Delay(1);
            return Ok(inventoryStatistics);
        }

        // GET: api/statistics
        [HttpGet]
        [Route("get_most_loaned_item")]
        public async Task<IActionResult> GetMostLoandItem()
        {
            var itemId = await _context.UserLoans.Where(x => x.IsHandedIn == false).GroupBy(x => x.LoanItem.Item.Id).OrderByDescending(x => x.Count()).Take(1).Select(x => x.Key).FirstOrDefaultAsync();
            var item = await _context.LoanItems.Include(c => c.Category).Include(i => i.Image).Include(b => b.Barcodes).Where(x => x.Id == itemId).Select(x => new LowItemAmountDTO
            {
                Brand = x.Brand,
                Model = x.Model,
                Category = x.Category.CategoryName,
                ImageUri = x.Image.ImageUri,
                AmountLeft = x.AmountLeft,
                TotalAmount = x.Barcodes.Count
            }).FirstOrDefaultAsync();

            return Ok(item);
        }

        // GET: api/statistics
        [HttpGet]
        [Route("get_most_used_item")]
        public async Task<IActionResult> GetMostUsedItems()
        {
            List<Tuple<int, ConsumptionItemModel>> items = new List<Tuple<int, ConsumptionItemModel>>();
            var test = await _context.ConsumptionItems.Include(c => c.Category).Include(i => i.Image).ToListAsync();
            foreach (var item in test)
            {
                int amount = 0;
                foreach (var uc in await _context.UserConsumptions.Where(x => x.ConsumptionItem.Item == item).ToListAsync())
                {
                    amount += uc.Amount;
                }
                items.Add(new Tuple<int, ConsumptionItemModel>(amount, item));
            }

            var mostUsedItems = items.OrderByDescending(x => x.Item1).Take(10).Select(x => new LowItemAmountDTO
            {
                Brand = x.Item2.Brand,
                Model = x.Item2.Model,
                Category = x.Item2.Category.CategoryName,
                ImageUri = x.Item2.Image.ImageUri,
                AmountLeft = x.Item2.AmountLeft,
                TotalAmount = x.Item1 + x.Item2.AmountLeft
            });
            return Ok(mostUsedItems);
        }



        // GET: api/statistics
        [HttpGet]
        [Route("get_low_stock_loaned_items")]
        public async Task<IActionResult> GetLowStockLoanedItems()
        {
            var lowItemAmounts = await _context.LoanItems.Include(b => b.Barcodes).Where(x => x.AmountLeft <= x.Barcodes.Count * 0.1).Select(x => new LowItemAmountDTO
            {
                Brand = x.Brand,
                Model = x.Model,
                Category = x.Category.CategoryName,
                ImageUri = x.Image.ImageUri,
                AmountLeft = x.AmountLeft,
                TotalAmount = x.Barcodes.Count
            }).ToListAsync();

            return Ok(lowItemAmounts);
        }

        // GET: api/statistics
        [HttpGet]
        [Route("get_low_stock_items")]
        public async Task<IActionResult> GetLowStockItems()
        {
            List<Tuple<int, ConsumptionItemModel>> items = new List<Tuple<int, ConsumptionItemModel>>();

            foreach (var item in await _context.ConsumptionItems.Include(c => c.Category).Include(i => i.Image).Where(x => x.AmountLeft < 26).Take(10).ToListAsync())
            {
                int amount = 0;
                foreach (var uc in await _context.UserConsumptions.Where(x => x.ConsumptionItem.Item == item).ToListAsync())
                {
                    amount += uc.Amount;
                }
                items.Add(new Tuple<int, ConsumptionItemModel>(amount, item));
            }

            var lowItemAmounts = items.OrderByDescending(x => x.Item1).Take(10).Select(x => new LowItemAmountDTO
            {
                Brand = x.Item2.Brand,
                Model = x.Item2.Model,
                Category = x.Item2.Category.CategoryName,
                ImageUri = x.Item2.Image.ImageUri,
                AmountLeft = x.Item2.AmountLeft,
                TotalAmount = x.Item1 + x.Item2.AmountLeft
            });

            return Ok(lowItemAmounts);
        }

        private bool AnyItems(InventoryType type)
        {
            if (type == InventoryType.loan)
                return _context.LoanItems.Any();
            else
                return _context.ConsumptionItems.Any();
        }
    }

    enum InventoryType
    {
        loan = 1,
        consumption = 2,
    }
}