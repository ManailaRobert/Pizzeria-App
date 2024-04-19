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
        private List<Ingredient> IngredientsLoadList;
        private List<Ingredient> IngredientsList;
        private double pizzaPrice ;
        private List<Sizes> SizesList;


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            AppUtilities.LoadPizzaList(LB_PizzaOrBeverage);

            using (var db = new PizzeriaDBContext())
            {
                try
                {
                    IngredientsLoadList = db.Ingredients.ToList();
                    LB_Ingredients.ItemsSource = IngredientsLoadList;
                    LB_Ingredients.SelectedItem = LB_Ingredients.Items[0];
                    //MessageBox.Show("Loaded Ingredients");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Loading Ingredients");
                    File.WriteAllText("error.txt", ex.ToString());
                }

                try
                {
                    SizesList = db.Sizes.ToList();
                    CB_Sizes.ItemsSource = SizesList;
                    CB_Sizes.SelectedItem = CB_Sizes.Items[0];
                   
                    //MessageBox.Show("Loaded Ingredients");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Loading Sizes");
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
            //CB_Sizes.IsEnabled = false;
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
        private void CalculatePrice()
        {

            IngredientsList = new List<Ingredient>();
            pizzaPrice = 0;
            foreach (var item in LB_Ingredients.SelectedItems)
            {
                Ingredient ingredient = (Ingredient)item;
                IngredientsList.Add(ingredient);
                pizzaPrice += ingredient.Price;
            }
            if(CB_Sizes.SelectedItem != null)
            pizzaPrice += ((Sizes)CB_Sizes.SelectedItem).Price;
            TB_Price.Text = $"{pizzaPrice.ToString()} RON";
        }
        private void LB_Ingredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void CB_Sizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void BTN_Create_Click(object sender, RoutedEventArgs e)
        {
            if (RB_Pizza.IsChecked == true || RB_CustomPizza.IsChecked == true)
            {
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                { 
                    try
                    {
                        string PizzaName = TB_Name.Text;
                        int sizeID = ((Sizes)CB_Sizes.SelectedItem).SizeID;
                        string custom = RB_CustomPizza.IsChecked == true ? "da" : "nu";

                        Pizza pizza = new Pizza(PizzaName, pizzaPrice, sizeID, custom);
                        //verify if already exists
                        db.Pizza.Add(pizza);
                        db.SaveChanges();

                        foreach (Ingredient ingredient in IngredientsList)
                        {
                            IngredientsGroup group = new IngredientsGroup(ingredient.IngredientID, pizza.PizzaID);
                            db.IngredientsGroup.Add(group);
                        }
                        db.SaveChanges();
                        MessageBox.Show("Added Pizza");
                    }
                    catch (Exception ex)
                    {
                        File.WriteAllText("error.txt", ex.ToString());
                    }
                }
            }
            else
            {
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        string beverageName = TB_Name.Text;

                        double beveragePrice = Convert.ToDouble(TB_Price.Text);
                        
                        Beverage beverage = new Beverage(beverageName, beveragePrice);
                        //verify if already exists
                        db.Beverages.Add(beverage);

                        db.SaveChanges();
                        MessageBox.Show("Added Beverage");
                    }
                    catch (Exception ex)
                    {
                        File.WriteAllText("error.txt", ex.ToString());
                    }
                }
                
            }
            
        }
    }
}
