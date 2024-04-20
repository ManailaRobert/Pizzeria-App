using PizzeriaClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace Pizzeria
{
    public class AppUtilities
    {
       public static void LoadLBPizza (ListBox lb, List<Pizza> PizzaList)
        {
            PizzaList = AppUtilities.LoadPizzaList();
            lb.ItemsSource = PizzaList;
        }
        private static List<Pizza> LoadPizzaList()
        {
            List<Pizza> PizzaList = new List<Pizza>();  
            using(PizzeriaDBContext dB = new PizzeriaDBContext())
            {
                try
                {
                    PizzaList = dB.Pizza
                        .Include(pizza => pizza.Size)
                        .Include(pizza => pizza.IngredientsGroup)
                        .ThenInclude(ingredientsGroup => ingredientsGroup.Ingredient)
                        .ToList();
                    //MessageBox.Show("Pizza list loaded");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading Pizza");
                    File.WriteAllText("error.txt", ex.ToString());
                }  
            }
            return PizzaList;
        }

        public static void LoadLBBeverages(ListBox lb, List<Beverage> BeverageList)
        {
            BeverageList = AppUtilities.LoadBeverageList();
            lb.ItemsSource = BeverageList;
        }

        private static List<Beverage> LoadBeverageList()
        {
            List<Beverage> BeverageList = new List<Beverage>();
            using (PizzeriaDBContext dB = new PizzeriaDBContext())
            {
                try
                {
                    BeverageList = dB.Beverages.ToList();
                    //MessageBox.Show("Beverage list loaded");

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading Pizza");
                    File.WriteAllText("error.txt", ex.ToString());
                }
            }
            return BeverageList;
        }
    }
}
