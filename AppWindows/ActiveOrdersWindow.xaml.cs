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

namespace Pizzeria
{
    /// <summary>
    /// Interaction logic for Active_Orders_Window.xaml
    /// </summary>
    public partial class Active_Orders_Window : Page
    {
        public Active_Orders_Window()
        {
            InitializeComponent();
        }

        private void BTN_Click(object sender, RoutedEventArgs e)
        {
            //TextBlock textBlock = new TextBlock();
            //textBlock.Text = "Order ID 1\n" +
            //    "Item 1\n" +
            //    "Item 2\n" +
            //    "Item 3\n" +
            //    "----------------\n" +
            //    "Total: 324 Lei";
            //LB_ActiveOrders.Items.Add(textBlock);
        }
    }
}
