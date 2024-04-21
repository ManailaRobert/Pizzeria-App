﻿using System;
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
        public DbSet<Customers> Customers { get; set; }
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
            //Customer Relationship 
            modelBuilder.Entity<Customers>().HasKey(a => a.CustomerID);
            modelBuilder.Entity<Customers>()
                    .HasMany(a => a.Adresses)
                    .WithOne(b => b.Customer)
                    .HasForeignKey(c => c.CustomerID);
            modelBuilder.Entity<Customers>()
                    .HasMany(a => a.CustomerOrders)
                    .WithOne(b => b.Customer)
                    .HasForeignKey(c => c.CustomerID);

            //Adress relationship 
            modelBuilder.Entity<Adress>().HasKey(a => a.AdressID);
            modelBuilder.Entity<Adress>()
                .HasOne(a => a.Customer)
                .WithMany(b=> b.Adresses)
                .HasForeignKey(c => c.CustomerID);
            modelBuilder.Entity<Adress>()
                .HasMany(a=> a.Orders)
                .WithOne(b => b.Adress)
                .HasForeignKey(c => c.AdressID);

            //OrderPizza relationship 
            modelBuilder.Entity<OrderPizza>().HasKey(a => a.OrderPizzaID);
            modelBuilder.Entity<OrderPizza>()
                .HasOne(a=>a.Pizza)
                .WithMany(b => b.OrderPizza)
                .HasForeignKey(c => c.PizzaID);
            modelBuilder.Entity<OrderPizza>()
                .HasOne(a => a.Order)
                .WithMany(b => b.OrderPizza)
                .HasForeignKey(c => c.OrderID);

            //pizza Relationship 
            modelBuilder.Entity<Pizza>().HasKey(a => a.PizzaID);
            modelBuilder.Entity<Pizza>()
                .HasOne(a => a.Size)
                .WithMany(b=> b.Pizzas)
                .HasForeignKey(c => c.SizeID);
            modelBuilder.Entity<Pizza>()
                .HasMany(a => a.OrderPizza)
                .WithOne(b => b.Pizza)
                .HasForeignKey(c => c.PizzaID);

            //Sizes relationship
            modelBuilder.Entity<Sizes>().HasKey(a => a.SizeID);
            modelBuilder.Entity<Sizes>()
                .HasMany(a => a.Pizzas)
                .WithOne(b => b.Size)
                .HasForeignKey(c => c.SizeID);

            //IngredientsGroup relationships
            modelBuilder.Entity<IngredientsGroup>().HasKey(a => a.IngredientsGroupID);
            modelBuilder.Entity<IngredientsGroup>()
                .HasOne(a => a.Pizza)
                .WithMany(b => b.IngredientsGroup)
                .HasForeignKey(c => c.PizzaID);
            modelBuilder.Entity<IngredientsGroup>()
                .HasOne(a => a.Ingredient)
                .WithMany()
                .HasForeignKey(c => c.IngredientID);

            //Ingredients Relationship
            modelBuilder.Entity<Ingredient>().HasKey(a => a.IngredientID);
            modelBuilder.Entity<Ingredient>()
                .HasMany(a => a.IngredientsGroup)
                .WithOne(b => b.Ingredient)
                .HasForeignKey(c => c.IngredientID);

            //OrderBeverage relationship 
            modelBuilder.Entity<OrderBeverage>().HasKey(a => a.OrderBeverageID);
            modelBuilder.Entity<OrderBeverage>()
                .HasOne(a => a.Order)
                .WithMany(b => b.OrderBeverages)
                .HasForeignKey(c => c.OrderID);
            modelBuilder.Entity<OrderBeverage>()
                .HasOne(a => a.Beverage)
                .WithMany(b => b.OrderBeverages)
                .HasForeignKey(c => c.BeverageID);

            //Beverage Relationship 
            modelBuilder.Entity<Beverage>().HasKey(a => a.BeverageID);
            modelBuilder.Entity<Beverage>()
                .HasMany(a => a.OrderBeverages)
                .WithOne(b => b.Beverage)
                .HasForeignKey(c => c.BeverageID);

            //Order Relationship 
            modelBuilder.Entity<Order>().HasKey(a => a.OrderID);
            modelBuilder.Entity<Order>()
                .HasOne(a => a.Customer)
                .WithMany(b => b.CustomerOrders)
                .HasForeignKey(c => c.CustomerID);
            modelBuilder.Entity<Order>()
                .HasOne(a => a.Adress)
                .WithMany(b => b.Orders)
                .HasForeignKey(c => c.AdressID);
            modelBuilder.Entity<Order>()
                 .HasMany(a => a.OrderPizza)
                 .WithOne(b => b.Order)
                 .HasForeignKey(c => c.PizzaID);

            base.OnModelCreating(modelBuilder);

        }
    }
}
