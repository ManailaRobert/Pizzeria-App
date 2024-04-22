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
using System.ComponentModel.Design;

namespace Pizzeria
{
    public class AppUtilities
    {
        public static void ShowError(Exception ex, string message)
        {
            MessageBox.Show(message);
            File.WriteAllText("error.txt", $"{ex.ToString()}");
        }
        public static string  LineCreator(int size)
        {
            string str = "";

            for (int i = 0; i <= size + 4; i++)
                str += "-";
            return str;
        }
        public static void LoadLBPizza (ListBox lb, List<Pizza> PizzaList,int type = 0,int sizeID = -1)
        {
            // type = 0 all pizzas
            PizzaList = LoadPizzaList(type,sizeID);
            lb.ItemsSource = PizzaList;
        }

        public static void LoadLBBeverages(ListBox lb, List<Beverage> BeverageList)
        {
            BeverageList = AppUtilities.LoadBeverageList();
            lb.ItemsSource = BeverageList;
        }
       
        public static void LoadLBCustomers(ListBox lb, List<Customers> CustomersList)
        {
            CustomersList = LoadCustomersList();
            lb.ItemsSource = CustomersList;
        }
        public static void LoadCBAdresses(ComboBox cb, List<Adress> AdressList, int id)
        {
            AdressList = LoadAdressList(id);
            cb.ItemsSource = AdressList;
            cb.SelectedIndex = 0;
        }
        
        public static void LoadCBSizes(ComboBox cb, List<Sizes> SizeList)
        {
            SizeList = LoadSizesList();
            cb.ItemsSource = SizeList;
            cb.SelectedIndex = 0;
        }

        public static void LoadLBCart(ListBox lb, Cart CartList)
        {
            lb.ItemsSource= CartList.ToList();
        }

        public static List<Order> LoadLBOrders (int type , ListBox lb = null, List<Order> OrderList = null,string customerName = "")
        {
            if(type == 0)
            {
                //all orders 
                List<Order> orderList = LoadOrderList(type);
                lb.ItemsSource = orderList;
                lb.SelectedItem = -1;
                return orderList;


            }
            else if(type == 1)
            {
                //active orders
                List<Order> orderList = LoadOrderList(type);
                return orderList;
            }
            else
            {
                //all orders by client name
                List<Order> orderList = LoadOrderList(type);
                lb.ItemsSource = orderList;
                lb.SelectedItem = -1;
                return orderList;
            }
        }
        private static List<Order> LoadOrderList(int type,string customerName = "")
        {
            List<Order> list = new List<Order>();
            if(type == 0)
            {
                //all orders
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        list = db.Orders
                            .Include(ob => ob.OrderBeverages).ThenInclude(b => b.Beverage)
                            .Include(op => op.OrderPizza).ThenInclude(p => p.Pizza).ThenInclude(s => s.Size)
                            .Include(c => c.Customer)
                            .Include(a => a.Adress)
                            .ToList();
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "Something went wrong wen loading order list");
                    }
                }
            }
            else if(type == 1)
            {
                // active orders
                using(PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        list = db.Orders
                            .Include(ob => ob.OrderBeverages)
                                .ThenInclude(b => b.Beverage)
                            .Include(op => op.OrderPizza)
                                .ThenInclude(p => p.Pizza)
                                .ThenInclude(s => s.Size)
                            .Where(s => string.Equals(s.OrderStatus, "active") == true)
                            .ToList();
                    }
                    catch(Exception ex)
                    {
                        ShowError(ex, "Something went wrong wen loading order list");
                    }
                }
            }
            return list;
        }
        private static List<Sizes> LoadSizesList()
        {
            List<Sizes> sizes = new List<Sizes>();
            using (PizzeriaDBContext db = new PizzeriaDBContext())
            {
                try
                {
                   sizes = db.Sizes.ToList();
                }
                catch(Exception ex)
                {
                    ShowError(ex,"Error Loading Sizes");

                }
            }
            return sizes;
        }
        private static List<Pizza> LoadPizzaList(int type,int sizeId)
        {
            List<Pizza> PizzaList = new List<Pizza>();
            using (PizzeriaDBContext dB = new PizzeriaDBContext())
            {
                try
                {
                    if(type == 0)
                    {
                        //custom and normal pizza
                        PizzaList = dB.Pizza
                            .Include(pizza => pizza.Size)
                            .Include(pizza => pizza.IngredientsGroup)
                            .ThenInclude(ingredientsGroup => ingredientsGroup.Ingredient)
                            .ToList();

                    }
                    else if(type == 1)
                    {
                        //normal pizza
                        if (sizeId == -1)
                            PizzaList = dB.Pizza
                                .Include(pizza => pizza.Size)
                                .Include(pizza => pizza.IngredientsGroup)
                                .ThenInclude(ingredientsGroup => ingredientsGroup.Ingredient)
                                .Where(pizza => string.Equals(pizza.Custom, "nu"))
                                .ToList();
                        else PizzaList = dB.Pizza //normal pizza by sizes
                            .Include(pizza => pizza.Size)
                            .Include(pizza => pizza.IngredientsGroup)
                            .ThenInclude(ingredientsGroup => ingredientsGroup.Ingredient)
                            .Where(pizza => pizza.SizeID == sizeId)
                            .Where(pizza => string.Equals(pizza.Custom, "nu"))
                            .ToList();
                    }
                    else
                    {
                        //custom pizza
                        PizzaList = dB.Pizza
                            .Include(pizza => pizza.Size)
                            .Include(pizza => pizza.IngredientsGroup)
                            .ThenInclude(ingredientsGroup => ingredientsGroup.Ingredient)
                            .Where(pizza => string.Equals(pizza.Custom, "da"))
                            .ToList();
                    }

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
