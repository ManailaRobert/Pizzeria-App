using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using PizzeriaClasses;
using System.IO;

namespace Pizzeria.AppWindows
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Page
    {
        private MainWindow MainWindow;
        private List<Pizza> PizzaList;
        private List<Beverage> BeverageList;
        private List<Sizes> SizesList;
        private List<Customers> CustomersList;
        private List<Adress> AdressesList;
        Cart cart;

        public OrderWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            MainWindow = mainWindow;
            AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList,1,1);//normal pizza
            AppUtilities.LoadCBSizes(CB_Sizes, SizesList);
            AppUtilities.LoadLBCustomers(LB_Clients, CustomersList);
            cart = new Cart();
        }

        private void BTN_Pizza_Click(object sender, RoutedEventArgs e)
        {
            int id = ((Sizes)CB_Sizes.SelectedItem).SizeID;
            AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList,1,id);
            BTN_Pizza.IsEnabled = false;
            BTN_Beverage.IsEnabled = true;
            BTN_Create.IsEnabled = true;
            CB_Sizes.IsEnabled = true;

        }

        private void BTN_Beverage_Click(object sender, RoutedEventArgs e)
        {
            AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);
            BTN_Pizza.IsEnabled = true;
            BTN_Beverage.IsEnabled = false;
            BTN_Create.IsEnabled = false;
            CB_Sizes.IsEnabled = false;
        }

        private void BTN_Create_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Main.Content = new CreatePizzaOrBeverage(cart,"da", MainWindow,this); 
        }

        private void LB_Clients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Customers customer = LB_Clients.SelectedItem as Customers;
            if(LB_Clients.SelectedIndex >= 0)
            AppUtilities.LoadCBAdresses(CB_Adresses, AdressesList, customer.CustomerID);
        }

        private void BTN_AddToCart_Click(object sender, RoutedEventArgs e)
        {
            cart.Add(LB_PizzaOrBeverage.SelectedItem as IBuyable);
            AppUtilities.LoadLBCart(LB_Cart, cart);
            TB_TotalPrice.Text = $"Total: {cart.cartPrice} RON";
        }

        private void BTN_ClearCart_Click(object sender, RoutedEventArgs e)
        {
            cart= new Cart();
            LB_Cart.ItemsSource = null;
            TB_TotalPrice.Text = "Total: 0 RON";
        }

        private void CB_Sizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = ((Sizes)CB_Sizes.SelectedItem).SizeID;
            AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList,1,id);

        }

        private void BTN_PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cart.Count > 0)
            {
                if(LB_Clients.SelectedItem != null)
                {
                    if(CB_Adresses.SelectedItem != null)
                    {
                        if(RB_Card.IsChecked == true || RB_Cash.IsChecked == true)
                        {
                            using(PizzeriaDBContext db = new PizzeriaDBContext())
                            {
                                try
                                { 
                                    Customers selectedCustomer = LB_Clients.SelectedItem as Customers;
                                    Adress selectedClientAdress = CB_Adresses.SelectedItem as Adress;

                                    Customers customer = db.Customers
                                        .FirstOrDefault(c => c.CustomerID == selectedCustomer.CustomerID);
                                    Adress adress = db.Adresses
                                        .FirstOrDefault(c => c.AdressID== selectedClientAdress.AdressID);

                                    string paymentMethod = RB_Card.IsChecked == true ? "card" : "numerar";

                                    Order order = new Order(customer.CustomerID,adress.AdressID,paymentMethod,cart.cartPrice,DateTime.Now,"active");
                                    db.Orders.Add(order);
                                    db.SaveChanges();
                                    foreach(Pizza pizza in cart.PizzasList)
                                    {
                                        OrderPizza pizzaOrder = new OrderPizza(order.OrderID,pizza.PizzaID);
                                        db.OrderPizza.Add(pizzaOrder);
                                    }
                                    db.SaveChanges();

                                    foreach (Beverage beverage in cart.BeveragesList)
                                    {
                                        OrderBeverage beverageOrder = new OrderBeverage(order.OrderID, beverage.BeverageID);
                                        db.OrderBeverage.Add(beverageOrder);
                                    }
                                    db.SaveChanges();
                                    MessageBox.Show("Order added Successfully");

                                    if(CB_Receipt.IsChecked == true)
                                    {
                                        string receipt = "";
                                        string border;
                                        string items = "";
                                        int size = 0;

                                        foreach(Pizza item in cart.PizzasList)
                                        {
                                            items += $"\n{item}";
                                            if(item.ToString().Length >size)
                                                size = item.ToString().Length;
                                        }
                                        foreach (Beverage item in cart.BeveragesList)
                                        {
                                            items += $"\n{item}";
                                            if (item.ToString().Length > size)
                                                size = item.ToString().Length;
                                        }
                                        border = AppUtilities.LineCreator(size);
                                        receipt += $"{border} " +
                                            $"\n#{order.OrderID} " +
                                            $"\n{border} " +
                                            $"{items} " +
                                            $"\n{border} " +
                                            $"\nTotal: {cart.cartPrice} RON" +
                                            $"\n{border}";
                                        File.WriteAllText($"receipt{order.OrderID}.txt",receipt);
                                    }
                                }
                                catch (Exception ex){
                                    AppUtilities.ShowError(ex,"Something went wrong when creating the order");
                                }
                            }
                        }
                        else MessageBox.Show("There is no selected payment method");
                    }
                    else MessageBox.Show("There are no adresses for this client");
                }
                else MessageBox.Show("No client selected");
            }
            else MessageBox.Show("Cart is empty");

            cart = new Cart();
            LB_Cart.ItemsSource = null;
            CB_Adresses.ItemsSource = null;

            TB_TotalPrice.Text = "Total: 0 RON";
            LB_PizzaOrBeverage.SelectedIndex = -1;
            LB_Clients.SelectedIndex = -1;
            RB_Card.IsChecked = false;
            RB_Cash.IsChecked = false; 
            CB_Receipt.IsChecked = false;
        }
    }
}
