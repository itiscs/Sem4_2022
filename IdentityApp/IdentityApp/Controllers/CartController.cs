using IdentityApp.Data;
using IdentityApp.Domain;
using IdentityApp.Models;
using IdentityApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IActionResult Index(string returnUrl)
        {
           
                return View(new CartIndexViewModel
                {
                    Cart = GetCart(),
                    ReturnUrl = returnUrl
                });
            
        }

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AddToCart(int productId, string returnUrl)
        {
            Product? prod = _context.Products.FirstOrDefault(g => g.ProductId == productId);

            if (prod != null)
            {
                var cart = GetCart();
                cart.AddItem(prod, 1);
                HttpContext.Session.Set<Cart>("Cart", cart);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public IActionResult RemoveFromCart(int productId, string returnUrl)
        {
            Product? prod = _context.Products.FirstOrDefault(g => g.ProductId == productId);

            if (prod != null)
            {
                GetCart().RemoveLine(prod);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public Cart GetCart()
        {
            Cart? cart =  HttpContext.Session.Get<Cart>("Cart");
            if (cart == null)
            {
                cart = new Cart();
                HttpContext.Session.Set<Cart>("Cart", cart);
            }
            return cart;
        }
    }
}
