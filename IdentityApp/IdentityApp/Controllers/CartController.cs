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
                    ReturnUrl = returnUrl==null ? "Products" : returnUrl
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
                var cart = GetCart();
                cart.RemoveLine(prod);
                HttpContext.Session.Set<Cart>("Cart", cart);                
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public IActionResult Checkout()
        {
            return View(GetCart());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Cart cart)
        {
            cart = GetCart();
            var order = new Order();
            order.OrderDate = DateTime.Now.ToUniversalTime();
            order.OrderStatus = OrderStatus.Created;
            order.UserName = User.Identity.Name;
            foreach (var line in cart.Lines)
            {
                order.Lines.Add(new OrderLine()
                {
                    ProductId = line.Product.ProductId,
                    Price = line.Product.Price,
                    Quantity = line.Quantity
                });
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Details","Orders",new { id = order.OrderID });
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
