using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.AppData;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _db;

        public HomeController(ILogger<HomeController> logger, AppDBContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
			//List<Product> products = _db.Products
			//	.Take(6).ToList();

			List<Product> latestProducts = _db.Products
                .OrderByDescending(p => p.CreatedAt)
				.Take(6)
                .ToList();

			List<Product> featuredProducts = _db.Products
				// Descending
				.OrderByDescending(p => p.Sold)
                .Take(12)
				.ToList();

			ViewBag.FeaturedCount = featuredProducts.Count;

			//ViewBag.LatestProducts = latestProducts;

			var productGroups = new List<List<Product>>();

			for (int i = 0; i < featuredProducts.Count; i += 4)
			{
				productGroups.Add(featuredProducts.Skip(i).Take(4).ToList());
			}

			ViewBag.ProductGroups = productGroups;

			return View(latestProducts);

		}

		public IActionResult Privacy()
        {
            List<Category> categories = _db.Categories.ToList();
            return View(categories);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
