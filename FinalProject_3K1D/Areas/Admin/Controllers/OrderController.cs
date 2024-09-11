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
            var Foods = await _context.Foods.ToListAsync();
            return View(Foods);
        }
        #endregion

        #region Details
        [Route("Admin/FoodManagement/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Food = await _context.Foods.FirstOrDefaultAsync(m => m.IdSanPham == id);

            if (Food == null)
            {
                return NotFound();
            }

            return View(Food);
        }
        #endregion

        #region Create
        [Route("Admin/FoodManagement/Create")]
        public IActionResult Create()
        {
            // Assuming you have a database or collection to get the last Food ID.
            var lastFood = _context.Foods.OrderByDescending(f => f.IdSanPham).FirstOrDefault(); // Get the last Food
            var nextId = (lastFood != null) ? lastFood.IdSanPham + 1 : 1; // Generate the next ID (auto-increment)

            ViewData["NextId"] = nextId.ToString("D5"); // Formatting the ID as needed (e.g., 00001)
          
            return View();
        }

        [Route("Admin/FoodManagement/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSanPham,TenSanPham,Gia,MoTa,Apphich,PLoai")] Food Food)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Food);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Food has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(Food);
        }
        #endregion
        

        #region Edit
        [Route("Admin/FoodManagement/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Food = await _context.Foods.FindAsync(id);
            if (Food == null)
            {
                return NotFound();
            }

            return View(Food);
        }

        [Route("Admin/FoodManagement/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSanPham,TenSanPham,Gia,MoTa,Apphich,PLoai")] Food food)
        {
            if (id != food.IdSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(food);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodExists(food.IdSanPham))
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
            return View(food);
        }
        #endregion

        #region Delete
        [Route("Admin/FoodManagement/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var food = await _context.Foods.FirstOrDefaultAsync(m => m.IdSanPham == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        [Route("Admin/FoodManagement/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food != null)
            {
                _context.Foods.Remove(food);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FoodExists(int id)
        {
            return _context.Foods.Any(e => e.IdSanPham == id);
        }
        #endregion

    }

}
