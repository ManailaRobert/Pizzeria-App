using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzeriaClasses;


namespace Pizzeria
{
    class PizzeriaDBContext:DbContext
    {
        public DbSet<Pizza> Pizza { get; set; }
        public DbSet<Adresses> Adresses { get; set; }
        public DbSet<Beverages> Beverages { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Ingredients> Ingredients { get; set;}
        public DbSet<IngredientsGroup> IngredientsGroup { get; set; }
        public DbSet<OrderBeverage> OrderBeverage { get; set; }

        public DbSet<OrderPizza> OrderPizza { get; set;}
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Sizes> Sizes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=RobertsComputer;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=true", sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());
            optionsBuilder.UseSqlServer("Server=RobertsLaptop;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=true", sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizza>().HasKey(a => a.PizzaID);
            modelBuilder.Entity<Adresses>().HasKey(a => a.AdressID);
            modelBuilder.Entity<Beverages>().HasKey(a => a.BeverageID);
            modelBuilder.Entity<Customers>().HasKey(a => a.CustomerID);
            modelBuilder.Entity<Ingredients>().HasKey(a => a.IngredientID);
            modelBuilder.Entity<IngredientsGroup>().HasKey(a => a.IngredientsGroupID);
            modelBuilder.Entity<OrderBeverage>().HasKey(a => a.OrderBeverageID);
            modelBuilder.Entity<OrderPizza>().HasKey(a => a.OrderPizzaID);
            modelBuilder.Entity<Orders>().HasKey(a => a.OrderID);
            modelBuilder.Entity<Sizes>().HasKey(a => a.SizeID);

        }
    }
}
