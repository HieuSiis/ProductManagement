using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductManagement.AppData;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
	public class ProductController : Controller
	{
		private readonly AppDBContext _db;
        private readonly ICompositeViewEngine _viewEngine;
        // private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductController(
			AppDBContext db, 
			ICompositeViewEngine viewEngine
			// IHttpContextAccessor httpContextAccessor
			) 
		{ 
			_db = db;
            _viewEngine = viewEngine;
            //_httpContextAccessor = httpContextAccessor;

        }
		public IActionResult Index()
		{
			return View();
		}
		[Authorize]
		public IActionResult ProductDetails(int id)
		{
			Product? p = _db.Products.Find(id);

			List<Product> relatedProducts = _db.Products
				// .Include(c => c.Category)
				.Where(prod => prod.CategoryId == p.CategoryId && prod.Id != id)
				.ToList();
			ViewBag.RelatedProducts = relatedProducts;

			return View(p);
		}

        public IActionResult ProductCategories(int id)
        {
            List<Product> productList = _db.Products.Include(c => c.Category).Where(p => p.CategoryId == id).ToList();
            return View(productList);
        }

		public const string CARTKEY = "cart";

		// Get cart from Session (CartItem list)
		List<CartItem>? GetCartItems()
		{
			var session = HttpContext.Session;
			string? jsoncart = session.GetString(CARTKEY);
			if (jsoncart != null)
			{
				return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
			}
			return new List<CartItem>();
		}

		// Save Cart (CartItem List) to session
		void SaveCartSession(List<CartItem>? ls)
		{
			var session = HttpContext.Session;
			string jsoncart = JsonConvert.SerializeObject(ls);
			session.SetString(CARTKEY, jsoncart);
		}

		// Remove cart from session
		void ClearCart()
		{
			var session = HttpContext.Session;
			session.Remove(CARTKEY);
		}

		/// Add product to cart
		[Route("addcart/{productId:int}", Name = "Addcart")]
		public IActionResult AddToCart([FromRoute] int productId)
		{
			var product = _db.Products
				.Where(p => p.Id == productId)
				.FirstOrDefault();
			if (product == null)
                return Json(new { status = "ERR", message = "Product not found." });

			

			// Handle add to Cart ...
			var cart = GetCartItems();
			var cartItem = cart?.Find(p => p?.product?.Id == productId);

			if (cartItem != null)
			{
				// Already exists, increase by 1
				cartItem.quantity++;
				
			}
			else
			{
				//  New add
				cart?.Add(new CartItem() { quantity = 1, product = product });
			}

			// Save cart to Session
			SaveCartSession(cart);

			decimal? priceTotal = 0;
			decimal? discountTotal = 0;
			decimal? taxTotal = 0;
			decimal? grandTotal = 0;

			foreach (var item in cart)
			{
				if (item?.product != null)
				{
					var itemPriceCol = item.quantity * item.product.Price;
					var itemDiscountCol = itemPriceCol * item.product.Discount;
					var itemTaxCol = (itemPriceCol - itemDiscountCol) * item.product.Tax;
					var itemTotalCol = itemPriceCol - itemDiscountCol + itemTaxCol;

					priceTotal += itemPriceCol;
					discountTotal += itemDiscountCol;
					taxTotal += itemTaxCol;
					grandTotal += itemTotalCol;
				}
			}

			string cartHtml = RenderPartialViewToString("_CartTablePartial", cart);

			return Json(new
			{
				status = "OK",
				message = "Product added to cart successfully.",
				data = new
				{
					cartHtml = cartHtml,
					// cartHtml = PartialView("_CartTablePartial", cart),
					priceTotal = priceTotal,
					discountTotal = discountTotal,
					taxTotal = taxTotal,
					grandTotal = grandTotal
				}
			}); 
        }
		public string RenderPartialViewToString(string viewName, object model)
		{
			// var httpContext = _httpContextAccessor.HttpContext;

			if (string.IsNullOrEmpty(viewName))
			{
				viewName = ControllerContext.ActionDescriptor.ActionName;
			}

			ViewData.Model = model;

			using (var writer = new StringWriter())
			{
				var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

				if (viewResult.Success)
				{
					var viewContext = new ViewContext(
						ControllerContext,
						viewResult.View,
						ViewData,
						TempData,
						writer,
						new HtmlHelperOptions()
					);

					viewResult.View.RenderAsync(viewContext).Wait();
					return writer.GetStringBuilder().ToString();
				}
				else
				{
					throw new FileNotFoundException("View not found");
				}
			}
		}
   
        /// Delete item in cart
        [Route("/removecart/{productId:int}", Name = "removecart")]
		public IActionResult RemoveCart([FromRoute] int productId)
		{
			var cart = GetCartItems();
			var cartItem = cart.Find(p => p?.product?.Id == productId);

			if (cartItem != null)
			{
				cart.Remove(cartItem);
			}

			SaveCartSession(cart);

			decimal? priceTotal = 0;
			decimal? discountTotal = 0;
			decimal? taxTotal = 0;
			decimal? grandTotal = 0;

			foreach (var item in cart)
			{
				if (item?.product != null)
				{
					var itemPriceCol = item.quantity * item.product.Price;
					var itemDiscountCol = itemPriceCol * item.product.Discount;
					var itemTaxCol = (itemPriceCol - itemDiscountCol) * item.product.Tax;
					var itemTotalCol = itemPriceCol - itemDiscountCol + itemTaxCol;

					priceTotal += itemPriceCol;
					discountTotal += itemDiscountCol;
					taxTotal += itemTaxCol;
					grandTotal += itemTotalCol;

                }
			}

			// grandTotal = priceTotal - discountTotal + taxTotal;

			return Json(new { status= "OK", message = "Product removed from cart successfully.",
				data = new
				{
					priceTotal,
					discountTotal,
					taxTotal,
					grandTotal
				}
			});
		}

		// Update
		[Route("/updatecart", Name = "updatecart")]
		[HttpPost]
		public IActionResult UpdateCart([FromForm] int productId, [FromForm] int quantity)
		{
			var cart = GetCartItems();
			var cartItem = cart?.Find(p => p?.product?.Id == productId);
			if (cartItem != null)
			{
				cartItem.quantity = quantity;
			}

			SaveCartSession(cart);

			var priceCol = quantity * cartItem?.product?.Price;
			var discountCol = priceCol * cartItem?.product?.Discount;
			var taxCol = (priceCol - discountCol) * cartItem?.product?.Tax;
			var totalCol = priceCol - discountCol + taxCol;

			decimal? priceTotal = 0;
			decimal? discountTotal = 0;
			decimal? taxTotal = 0;
			decimal? grandTotal = 0;

			foreach (var item in cart)
			{
				if (item?.product != null)
				{
					var itemPriceCol = item.quantity * item.product.Price;
					var itemDiscountCol = itemPriceCol * item.product.Discount;
					var itemTaxCol = (itemPriceCol - itemDiscountCol) * item.product.Tax;
					var itemTotalCol = itemPriceCol - itemDiscountCol + itemTaxCol;

					priceTotal += itemPriceCol;
					discountTotal += itemDiscountCol;
					taxTotal += itemTaxCol;
					grandTotal += itemTotalCol;
				}
			}

			//return Ok();
			return Json(new
			{
				status = "OK",
				message = "Change quantity successfully.",
				data = new
				{
					cartItem,
					priceCol,
					discountCol,
					taxCol,
					totalCol,
					priceTotal,
					discountTotal,
					taxTotal,
					grandTotal
				}
			});
		}

		// Get cart info
		[Route("/cartinfo", Name = "cartinfo")]
		public IActionResult GetCartInfo()
		{
			var cart = GetCartItems();
			int cartCount = cart?.Count ?? 0;

            decimal? priceTotal = 0;
            decimal? discountTotal = 0;
            decimal? taxTotal = 0;
            decimal? grandTotal = 0;

            foreach (var item in cart)
            {
                if (item?.product != null)
                {
                    var itemPriceCol = item.quantity * item.product.Price;
                    var itemDiscountCol = itemPriceCol * item.product.Discount;
                    var itemTaxCol = (itemPriceCol - itemDiscountCol) * item.product.Tax;
                    var itemTotalCol = itemPriceCol - itemDiscountCol + itemTaxCol;

                    priceTotal += itemPriceCol;
                    discountTotal += itemDiscountCol;
                    taxTotal += itemTaxCol;
					grandTotal += itemTotalCol;
                }
            }

            return Json(new
			{
				status = "OK",
				cartCount = cartCount,
                grandTotal = grandTotal
            });
		}

		// Show Cart
		[Route("/cart", Name = "cart")]
		public IActionResult Cart()
		{
			return View(GetCartItems());
		}

		[Route("/checkout")]
		public IActionResult CheckOut()
		{
			return View();
		}
	}
}
