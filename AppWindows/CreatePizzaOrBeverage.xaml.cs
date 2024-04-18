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

namespace Pizzeria.AppWindows
{
    /// <summary>
    /// Interaction logic for CreatePizzaOrBeverage.xaml
    /// </summary>
    public partial class CreatePizzaOrBeverage : Page
    {
        public CreatePizzaOrBeverage()
        {
            InitializeComponent();
            RB_Pizza.IsChecked = true;
        }
        private List<Ingredients> IngredientsList;
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new PizzeriaDBContext())
            {
                try {
                    IngredientsList = db.Ingredients.ToList();
                    LB_Ingredients.ItemsSource = IngredientsList;
                }
                catch (Exception ex) {
                    MessageBox.Show($"Error Loading Ingredients");
                    File.WriteAllText("error.txt", ex.ToString());
                }

            }
        }
        private void RB_Pizza_Checked(object sender, RoutedEventArgs e)
        {
            BTN_Create.Content = "Create Pizza";
            TB_Price.IsEnabled = false;
            LB_Ingredients.IsEnabled = true;
            TB_Name.Text = "Pizza Name";
            CB_Sizes.IsEnabled = false;
        }

        private void RB_Beverage_Checked(object sender, RoutedEventArgs e)
        {
            BTN_Create.Content = "Create Beverage";
            TB_Price.IsEnabled = true;
            LB_Ingredients.IsEnabled = false;
            TB_Name.Text = "Beverage Name";
            CB_Sizes.IsEnabled = false;

        }
        private void RB_CustomPizza_Checked(object sender, RoutedEventArgs e)
        {
            BTN_Create.Content = "Create Custom Pizza";
            TB_Price.IsEnabled = false;
            LB_Ingredients.IsEnabled = true;
            TB_Name.Text = "Custom Pizza Name";
            CB_Sizes.IsEnabled = true;
        }
        private void BTN_Pizza_Click(object sender, RoutedEventArgs e)
        {
            BTN_Pizza.IsEnabled = false;
            BTN_Beverage.IsEnabled = true;
            //load pizza
        }

        private void BTN_Beverage_Click(object sender, RoutedEventArgs e)
        {
            BTN_Beverage.IsEnabled = false;
            BTN_Pizza.IsEnabled = true;
            //load beverages
        }


    }
}
