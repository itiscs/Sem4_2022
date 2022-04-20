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

namespace IdentityApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.Orders.GetAll();
            if (User.IsInRole("Admin"))
                return View(orders.OrderBy(o=>o.OrderDate));
            else
                return View(orders.Where(o=>o.UserName==User.Identity.Name).OrderBy(o=>o.OrderDate).ToList());
        }

        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _unitOfWork.Orders.GetById(id.Value);

            if (order == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("Admin") && order.UserName != User.Identity.Name)
            {
                return BadRequest();
            }

            
            //return View(order.Include(o => o.Lines).ThenInclude(l => l.Product));
            return View(order);
        }



        // GET: Orders/Edit/5

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _unitOfWork.Orders.GetById(id.Value);
            if (order == null)
            {
                return NotFound();
            }
          
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,UserName,OrderDate,OrderStatus")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.OrderDate = order.OrderDate.ToUniversalTime();
                    await _unitOfWork.Orders.Update(order);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _unitOfWork.Orders.GetById(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Orders.Delete(id);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _unitOfWork.Orders.GetById(id) != null;
        }
    }
}
