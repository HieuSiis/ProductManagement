using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.AppData;
using ProductManagement.Models;

namespace ProductManagement.Components
{
	public class CategoryViewComponent: ViewComponent
	{
		private readonly AppDBContext _db;
		public CategoryViewComponent(AppDBContext db) {
			this._db = db;
		}
		public async Task<IViewComponentResult> InvokeAsync() {

			// List<Category> cateList = await this._db.Categories.ToListAsync();

			var cateList = await _db.Categories
			   .Select(c => new Category
			   {
				   Id = c.Id,
				   Name = c.Name,
				   ProductCount = _db.Products.Count(p => p.CategoryId == c.Id)
			   })
			   .ToListAsync();

			List<Product> cheapPro = await this._db.Products
				// Ascending
				.OrderBy(p => p.Price)
				.Take(2)
				.ToListAsync();

			ViewBag.CheapPro = cheapPro;

			return View(cateList);
		}
	}
}
