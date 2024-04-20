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
        public static void ShowError(Exception ex, string message)
        {
            MessageBox.Show(message);
            File.WriteAllText("error.txt", ex.ToString());
        }
        public static void LoadLBPizza (ListBox lb, List<Pizza> PizzaList)
        {
            PizzaList = AppUtilities.LoadPizzaList();
            lb.ItemsSource = PizzaList;
        }

        public static void LoadLBBeverages(ListBox lb, List<Beverage> BeverageList)
        {
            BeverageList = AppUtilities.LoadBeverageList();
            lb.ItemsSource = BeverageList;
        }
       
        public static void LoadLBCustomers(ListBox lb, List<Customers> CustomersList)
        {
            CustomersList = AppUtilities.LoadCustomersList();
            lb.ItemsSource = CustomersList;
        }
        public static void LoadCBAdresses(ComboBox cb, List<Adress> AdressList, int id)
        {
            AdressList = LoadAdressList(id);
            cb.ItemsSource = AdressList;
            cb.SelectedIndex = 0;
        }
        
        private static List<Pizza> LoadPizzaList()
        {
            List<Pizza> PizzaList = new List<Pizza>();
            using (PizzeriaDBContext dB = new PizzeriaDBContext())
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
                    ShowError(ex, "Error loading Pizza");
                }
            }
            return PizzaList;
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
                    ShowError(ex,"Error loading Beverages");
                }
            }
            return BeverageList;
        }
        private static List<Customers> LoadCustomersList()
        {
            List<Customers> CustomersList = new List<Customers>();
            using (PizzeriaDBContext dB = new PizzeriaDBContext())
            {
                try
                {
                    CustomersList = dB.Customers
                        .Include(customer => customer.Adresses)
                        .ToList();
                    //MessageBox.Show("Customer list loaded");

                }
                catch (Exception ex)
                {
                    ShowError(ex, "Error loading customers");
                }
            }
            return CustomersList;
        }
        private static List<Adress> LoadAdressList(int id)
        {
            List<Adress> AdressList = new List<Adress>();
            using (PizzeriaDBContext dB = new PizzeriaDBContext())
            {
                try
                {
                    AdressList = dB.Adresses
                        .Where(adress => adress.CustomerID == id)
                        .ToList();
                    //MessageBox.Show("Adress list loaded");

                }
                catch (Exception ex)
                {
                    ShowError(ex, "Error loading ");
                }
                return AdressList;
            }
        }
    }
}
