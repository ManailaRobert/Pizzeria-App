using Microsoft.IdentityModel.Tokens;
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
using System.IO;
using System.Drawing;

namespace Pizzeria.AppWindows
{
    /// <summary>
    /// Interaction logic for AllOrdersWindow.xaml
    /// </summary>
    public partial class AllOrdersWindow : Page
    {
        private List<Order> ordersList;
        private int nrOfReports = 0;
        public AllOrdersWindow()
        {
            InitializeComponent();
            ordersList = AppUtilities.LoadLBOrders(0, LB_AllOrders, ordersList);
            nrOfReports = Convert.ToInt32(File.ReadAllText("reportsMade.txt"));      
        }

        private void LB_AllOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_AllOrders.SelectedIndex != -1)
            {
                string bar = "";
                Order order = LB_AllOrders.SelectedItem as Order;
                string items = "";
                int size = 0;


                LB_Details.Items.Clear();

                TB_OrderID.Text = $"Order ID: #{order.OrderID}";
                TB_TotalPrice.Text = $"Total price: {order.OrderPrice} RON";
                int first = 0;
                if (order.OrderPizza.Count > 0)
                    foreach (OrderPizza orderPizza in order.OrderPizza)
                    {

                    if (orderPizza.Pizza.ToString().Length > size)
                        size = orderPizza.Pizza.ToString().Length;
                        items += first == 0? $"{orderPizza.Pizza}": $"\n{orderPizza.Pizza}";
                        first++;
                    }
                if(order.OrderBeverages.Count>0)
                    foreach (OrderBeverage orderBeverage in order.OrderBeverages)
                    {
                        if (orderBeverage.Beverage.ToString().Length > size)
                            size = orderBeverage.Beverage.ToString().Length;
                        items += first == 0? $"{orderBeverage.Beverage}" : $"\n{orderBeverage.Beverage}";
                        first++;
                    }
                bar = AppUtilities.LineCreator(size+10);
                LB_Details.Items.Add(bar);
                LB_Details.Items.Add("Order Items:");
                LB_Details.Items.Add(items);
                LB_Details.Items.Add(bar);
                LB_Details.Items.Add("Details: ");
                LB_Details.Items.Add($"Name: {order.Customer}");
                LB_Details.Items.Add($"Adress: {order.Adress}");
                LB_Details.Items.Add($"Payment method: {order.PaymentMethod}");
                LB_Details.Items.Add($"PhoneNumber: {order.Customer.PhoneNumber}");
                LB_Details.Items.Add($"Date placed: {order.OrderDate}");
                LB_Details.Items.Add(bar);

            }
        }

        private void BTN_CreateReport_Click(object sender, RoutedEventArgs e)
        {
            if (ordersList.Count > 0)
            {
                string bar = AppUtilities.LineCreator(50);
                string report ="";
                report += $"Report made on:{DateTime.Now}\n";
                report += "\n" + bar + bar + "\n";
                report += "\n" + bar + bar + "\n";

                double TotalPriceOnOrders = 0;
                foreach (Order order in ordersList)
                {
                    string items = "";
                    int size = 0;
                    TotalPriceOnOrders += order.OrderPrice;

                    LB_Details.Items.Clear();

                    TB_OrderID.Text = $"Order ID: #{order.OrderID}";
                    TB_TotalPrice.Text = $"Total price: {order.OrderPrice} RON";

                    foreach (OrderPizza orderPizza in order.OrderPizza)
                        items += $"{orderPizza.Pizza}\n";
                    

                    foreach (OrderBeverage orderBeverage in order.OrderBeverages)
                        items += $"{orderBeverage.Beverage}\n";
                    
                    report += "\n" + bar + bar + "\n";
                    report += $"\nOrder ID: #{order.OrderID}";
                    report += "\n" + bar;
                    report += "\nOrder Items:";
                    report += "\n" + items;
                    report += "\n" + bar;
                    report += $"\nTotal price: {order.OrderPrice} RON";
                    report += "\n" + bar;
                    report += "\nDetails: ";
                    report += $"\nName: {order.Customer}";
                    report += $"\nAdress: {order.Adress}";
                    report += $"\nPayment method: {order.PaymentMethod}";
                    report += $"\nPhoneNumber: {order.Customer.PhoneNumber}";
                    report += $"\nDate placed: {order.OrderDate}";
                    report += "\n" + bar + bar + "\n";
                }

                report += "\n" + bar +bar + "\n";
                report += "\n" + bar + bar + "\n";
                report += "\n" + bar + bar + "\n";

                report += $"\nTotal Price: {TotalPriceOnOrders} RON";
                nrOfReports++;
                File.WriteAllText($"Report{nrOfReports}.txt", report);
                File.WriteAllText("reportsMade.txt", nrOfReports.ToString());
            }
            else MessageBox.Show("There are no orders available.");
            LB_AllOrders.SelectedIndex = -1;
            TB_OrderID.Text = "Order ID: --";
            TB_TotalPrice.Text = "Total price: -- RON";
        }
    }
}
