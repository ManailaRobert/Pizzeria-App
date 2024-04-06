using Pizzeria.AppWindows;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        object previousPage;
        private void BTN_Order_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new OrderWindow();
            Main.NavigationService.RemoveBackEntry();
        }

        private void BTN_ActiveOrders_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Active_Orders_Window();
            Main.NavigationService.RemoveBackEntry();
        }

        private void BTN_NewClient_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new AddClientsWindow();
            Main.NavigationService.RemoveBackEntry();
        }

        private void BTN_NewPizzaBeverage(object sender, RoutedEventArgs e)
        {
            Main.Content = new CreatePizzaOrBeverage();
            Main.NavigationService.RemoveBackEntry();
        }

        private void BTN_AllOrders_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new AllOrdersWindow();
            Main.NavigationService.RemoveBackEntry();
        }
    }
}