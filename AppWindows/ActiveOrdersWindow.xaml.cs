using PizzeriaClasses;
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

namespace Pizzeria.AppWindows
{
    /// <summary>
    /// Interaction logic for Active_Orders_Window.xaml
    /// </summary>
    public partial class Active_Orders_Window : Page
    {

        class CustomComboBox:ComboBox
        {
            public int id;
        }

        private enum Status
        {
            Prepering,
            Cooking,
            Done,
            Delivering,
            Delivered
        }
        private List<Order> ActiveOrdersList = new List<Order>();

        public Active_Orders_Window()
        {
            InitializeComponent();


            ActiveOrdersList = AppUtilities.LoadLBOrders(1);// all orders

            foreach(Order order in ActiveOrdersList)
            {
                TextBlock textBlock = new TextBlock();
                StackPanel NewStackPanel = new StackPanel();
                CustomComboBox comboBox = new CustomComboBox();
                comboBox.id = order.OrderID;
                string pizzaList = "";
                string beverageList = "";
                string line = "";
                int size = 0;


                //ComboBox Proprieties
                comboBox.ItemsSource = Enum.GetValues(typeof(Status));
                comboBox.SelectedItem = Status.Prepering;
                comboBox.Width = 150;
                comboBox.Height = 20;
                comboBox.SelectionChanged += DataSelected;
                comboBox.FontSize = 12;

                //StackPanel Proprieties
                NewStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                NewStackPanel.Orientation = Orientation.Horizontal;

                //TexttBlock Proprieties
                foreach (OrderPizza orderPizza in order.OrderPizza)
                {
                    if (orderPizza.Pizza.ToString().Length > size)
                        size = orderPizza.Pizza.ToString().Length;

                    pizzaList += $"{orderPizza.Pizza}\n";
                }
                foreach (OrderBeverage orderBeverage in order.OrderBeverages)
                {
                    if (orderBeverage.Beverage.ToString().Length > size)
                        size = orderBeverage.Beverage.ToString().Length;

                    beverageList += $"{orderBeverage.Beverage}\n";
                }
                line += AppUtilities.LineCreator(size);
                line += "\n";
                textBlock.Text = line + 
                    $"Order ID: {order.OrderID}\n" +
                    line+
                    pizzaList+
                    beverageList+
                    line;


                textBlock.FontSize = 14;
                textBlock.FontSize = 14;


                /*Adding the children
                 * hierarchy: 
                 * 0: LB_ActiveOrders
                 * 1: NewStackPanel
                 * 2: textBlock + comboBox  
                */
                NewStackPanel.Children.Add(textBlock);
                NewStackPanel.Children.Add(comboBox);
                LB_ActiveOrders.Items.Add(NewStackPanel);
            }
            //MessageBox.Show("Orders loaded");
        }
        private void DataSelected(object sender, SelectionChangedEventArgs e)
        {
            CustomComboBox comboBox = sender as CustomComboBox;
            Status orderStatus = (Status)comboBox.SelectedItem;
            if(orderStatus == Status.Delivered)
            {
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        StackPanel cbParent = comboBox.Parent as StackPanel;
                        LB_ActiveOrders.Items.Remove(cbParent);
                        // change the status in DB to delivered

                        Order order = db.Orders.FirstOrDefault(o => o.OrderID == comboBox.id);
                        order.OrderStatus = "completed";
                        db.SaveChanges();
                        MessageBox.Show($"Order {order.OrderID} finished.");
                    }
                    catch (Exception ex)
                    {
                        AppUtilities.ShowError(ex, "Something went wrong when changing order status. ");
                    }
                }

            }
        }
    }
}
