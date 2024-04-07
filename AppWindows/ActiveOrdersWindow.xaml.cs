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
        int n = 0;


        private void DataSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            Status orderStatus = (Status)comboBox.SelectedItem;
            if(orderStatus == Status.Delivered)
            {
                StackPanel cbParent = comboBox.Parent as StackPanel;
                LB_ActiveOrders.Items.Remove(cbParent);
                // change the status in DB to delivered
            }
        }

        private enum Status
        {
            Prepering,
            Cooking,
            Done,
            Delivering,
            Delivered
        }
        private void BTN_Click(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = new TextBlock();
            StackPanel NewStackPanel = new StackPanel();
            ComboBox comboBox = new ComboBox();


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
            n++;
            textBlock.Text = $"Order ID {n}\n" +
                "----------------\n" +
                "Item 1\n" +
                "Item 2\n" +
                "Item 3\n" +
                "----------------\n" +
                "Total: 324 Lei";
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
    }
}
