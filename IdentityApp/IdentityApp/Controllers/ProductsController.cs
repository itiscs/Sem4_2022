#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityApp.Infrastructure;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductService serv;
        private string loginToken;

        public ProductsController(IUnitOfWork unitOfWork, IConfiguration conf)
        {
            _unitOfWork = unitOfWork; 
            serv = new ProductService(conf);
        }

        private async Task LoginToApi()
        {
            if (HttpContext.Session.Keys.Contains("ApiToken"))
                loginToken = HttpContext.Session.GetString("ApiToken");
            else
            {
                loginToken = await serv.GetToken();
                HttpContext.Session.SetString("ApiToken", loginToken);
            }
            serv.AddToken(loginToken);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            await LoginToApi();
            //return View(await _unitOfWork.Products.GetAll());
            return View(await serv.GetProducts());

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            await LoginToApi();
            //var product = await _unitOfWork.Products.GetById(id.Value);
            var product = await serv.GetProductByID(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,ImageUrl,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                await LoginToApi();
                //await _unitOfWork.Products.Add(product);
                //await _unitOfWork.CompleteAsync();
                await serv.AddProduct(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var product = await _unitOfWork.Products.GetById(id.Value);
            await LoginToApi();
            var product = await serv.GetProductByID(id.Value);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,ImageUrl,Price")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //await _unitOfWork.Products.Update(product);
                    //await _unitOfWork.CompleteAsync();
                    await LoginToApi();
                    await serv.EditProduct(id, product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var product = await _unitOfWork.Products.GetById(id.Value);
            await LoginToApi();
            var product = await serv.GetProductByID(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //await _unitOfWork.Products.Delete(id);
            //await _unitOfWork.CompleteAsync();
            await LoginToApi();
            await serv.DeleteProduct(id);

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            // return _unitOfWork.Products.GetById(id) != null;
            return serv.GetProductByID(id) != null;

        }
    }
}
