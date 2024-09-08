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
            var orders = await _context.Foods.ToListAsync();
            return View(orders);
        }
        #endregion
        //create new order
        #region Create
        public IActionResult Create()
        {
            var nextId = GeneratefoodId();
            ViewData["NextId"] = nextId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Food food, IFormFile Apphich)
        {
            if (ModelState.IsValid)
            {
                // Handle the file upload
                if (Apphich != null && Apphich.Length > 0)
                {
                    // Ensure that the directory exists
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/Order");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Create a unique file name for the uploaded file to prevent overwriting
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Apphich.FileName);
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Apphich.CopyToAsync(stream);
                    }

                    // Save the filename to the Apphich property of the Food model
                    food.Apphich = fileName;
                }

                // Save the food record to the database
                _context.Add(food);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(food);
        }

        private int GeneratefoodId()
        {
            // Logic to generate the next Idfood based on the last Idfood in the database
            var lastfood = _context.Foods.OrderByDescending(p => p.IdSanPham).FirstOrDefault();
            if (lastfood != null)
            {
                int nextId = lastfood.IdSanPham + 1;
                return nextId;
            }
            return 1; // Return 1 as the default value if there is no last Id found or parsing fails
        }

        #endregion



        // GET: Food/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods
                .FirstOrDefaultAsync(m => m.IdSanPham == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        // POST: Food/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            try
            {
                // Delete associated image from the server (optional)
                if (!string.IsNullOrEmpty(food.Apphich))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/Order", food.Apphich);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath); // Delete the image file
                    }
                }

                // Remove the food record from the database
                _context.Foods.Remove(food);
                await _context.SaveChangesAsync();

                // Log the deletion
                _logger.LogInformation($"Food item with ID {id} was deleted successfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting Food item with ID {IdSanPham}", id);
                return View(food); // If an error occurs, reload the delete page
            }
        }
        //edit food
        #region Edit
        [Route("Admin/OrderManagement/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            // Check if the food item exists
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            // Prepare view data for dropdowns or related data (if needed)
            // Example: ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

            return View(food);
        }

        [Route("Admin/OrderManagement/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSanPham,TenSanPham,Gia,MoTa,PLoai,Apphich")] Food food, IFormFile newApphich)
        {
            if (id != food.IdSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload if a new file is selected
                    if (newApphich != null && newApphich.Length > 0)
                    {
                        // Ensure that the directory exists
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/Order");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Delete the old image file if it exists
                        if (!string.IsNullOrEmpty(food.Apphich))
                        {
                            var oldFilePath = Path.Combine(directoryPath, food.Apphich);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Create a unique file name for the new uploaded file
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(newApphich.FileName);
                        var filePath = Path.Combine(directoryPath, fileName);

                        // Save the new file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await newApphich.CopyToAsync(stream);
                        }

                        // Update the food model with the new file name
                        food.Apphich = fileName;
                    }

                    // Update the food record in the database
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

        private bool FoodExists(int id)
        {
            return _context.Foods.Any(e => e.IdSanPham == id);
        }

        #endregion




    }
}
