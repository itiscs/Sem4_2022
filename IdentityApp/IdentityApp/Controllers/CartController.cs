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
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string returnUrl)
        {
            return View(new CartIndexViewModel
                {
                    Cart = GetCart(),
                    ReturnUrl = returnUrl==null ? "Products" : returnUrl
                });
            
        }

      

        public async Task<IActionResult> AddToCart(int productId, string returnUrl)
        {
            Product prod = await _unitOfWork.Products.GetById(productId);

            if (prod != null)
            {
                var cart = GetCart();
                cart.AddItem(prod, 1);
                HttpContext.Session.Set<Cart>("Cart", cart);
            }          
            return RedirectToAction("Index", new { returnUrl });
        }

        public async Task<IActionResult> RemoveFromCart(int productId, string returnUrl)
        {
            Product prod = await _unitOfWork.Products.GetById(productId);

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
            await _unitOfWork.Orders.Add(order);
            await _unitOfWork.CompleteAsync();
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
