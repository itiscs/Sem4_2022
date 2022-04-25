#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiApp.Models;

namespace WebApiApp.Data
{
    public class WebApiContext : DbContext
    {
        public WebApiContext (DbContextOptions<WebApiContext> options)
            : base(options)
        {
        }

        public DbSet<WebApiApp.Models.Product> Product { get; set; }
    }
}
