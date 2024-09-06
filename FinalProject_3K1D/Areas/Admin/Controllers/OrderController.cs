using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManagementController : Controller
    {
        private readonly QlrapPhimContext _context;
        private readonly ILogger<OrderManagementController> _logger;

        // Constructor with ILogger injected
        public OrderManagementController(QlrapPhimContext context, ILogger<OrderManagementController> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.ToListAsync();
            return View(orders);
        }
        #endregion

        #region Details
        [Route("Admin/OrderManagement/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Idsanpham == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        #endregion

        #region Create
        [Route("Admin/OrderManagement/Create")]
        public IActionResult Create()
        {
            // Assuming you have a database or collection to get the last order ID.
            var lastOrder = _context.Orders.OrderByDescending(o => o.Idsanpham).FirstOrDefault();
            var nextId = (lastOrder != null) ? lastOrder.Idsanpham + 1 : 1; // Generate the next ID (auto-increment)

            ViewData["NextId"] = nextId.ToString("D5"); // Formatting the ID as needed (e.g., 00001)
          
            return View();
        }

        [Route("Admin/OrderManagement/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idsanpham,Tensanpham,Mota,Apphich,PLoai")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }
        #endregion
        

        #region Edit
        [Route("Admin/OrderManagement/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Route("Admin/OrderManagement/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idsanpham,Tensanpham,Mota,Apphich,PLoai")] Order order)
        {
            if (id != order.Idsanpham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Idsanpham))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }
        #endregion

        #region Delete
        [Route("Admin/OrderManagement/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Idsanpham == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Route("Admin/OrderManagement/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Idsanpham == id);
        }
        #endregion
    }

}
