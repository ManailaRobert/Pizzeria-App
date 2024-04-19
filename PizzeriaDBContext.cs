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
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Beverage> Beverages { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Ingredient> Ingredients { get; set;}
        public DbSet<IngredientsGroup> IngredientsGroup { get; set; }
        public DbSet<OrderBeverage> OrderBeverage { get; set; }

        public DbSet<OrderPizza> OrderPizza { get; set;}
        public DbSet<Order> Orders { get; set; }
        public DbSet<Sizes> Sizes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=RobertsComputer;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=true", sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());
            //optionsBuilder.UseSqlServer("Server=RobertsLaptop;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=true", sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());

            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizza>().HasKey(a => a.PizzaID);
            modelBuilder.Entity<Adress>().HasKey(a => a.AdressID);
            modelBuilder.Entity<Beverage>().HasKey(a => a.BeverageID);
            modelBuilder.Entity<Customer>().HasKey(a => a.CustomerID);
            modelBuilder.Entity<Ingredient>().HasKey(a => a.IngredientID);
            modelBuilder.Entity<IngredientsGroup>().HasKey(a => a.IngredientsGroupID);
            modelBuilder.Entity<OrderBeverage>().HasKey(a => a.OrderBeverageID);
            modelBuilder.Entity<OrderPizza>().HasKey(a => a.OrderPizzaID);
            modelBuilder.Entity<Order>().HasKey(a => a.OrderID);
            modelBuilder.Entity<Sizes>().HasKey(a => a.SizeID);

        }
    }
}
