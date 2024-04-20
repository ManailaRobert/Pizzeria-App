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
using Microsoft.EntityFrameworkCore;

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
        private double Price ;
        private List<Sizes> SizesList;
        private List<Pizza> PizzaList;
        private List<Beverage> BeverageList;

        private void ReLoadLB()
        {
            
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);
            AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList);
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
                    MessageBox.Show($"Error loading Ingredients");
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
                    MessageBox.Show($"Error loading Sizes");
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
            LB_Ingredients.SelectedItems.Clear();
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
            LB_Ingredients.SelectedItems.Clear();
        }
        private void BTN_Pizza_Click(object sender, RoutedEventArgs e)
        {
            BTN_Pizza.IsEnabled = false;
            BTN_Beverage.IsEnabled = true;
            TB_Price.IsEnabled = false;
            CB_Sizes.IsEnabled = true;

            BTN_Edit.Content = "Edit Pizza";
            AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList);
            
        }

        private void BTN_Beverage_Click(object sender, RoutedEventArgs e)
        {
            BTN_Beverage.IsEnabled = false;
            BTN_Pizza.IsEnabled = true;
            BTN_Edit.Content = "Edit Beverage";
            CB_Sizes.IsEnabled = false;
            TB_Price.IsEnabled = true;
            LB_Ingredients.IsEnabled = false;


            AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);
        }
        private void CalculatePrice()
        {

            IngredientsList = new List<Ingredient>();
            Price = 0;
            foreach (var item in LB_Ingredients.SelectedItems)
            {
                Ingredient ingredient = (Ingredient)item;
                IngredientsList.Add(ingredient);
                Price += ingredient.Price;
            }
            if(CB_Sizes.SelectedItem != null)
            Price += ((Sizes)CB_Sizes.SelectedItem).Price;
            TB_Price.Text = $"{Price.ToString()} RON";
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
                //pizza create
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                { 
                    try
                    {
                        if(IngredientsList.Count > 0)
                        {
                            if(String.IsNullOrEmpty(TB_Name.Text) == false)
                            {
                                string PizzaName = TB_Name.Text;
                                int sizeID = ((Sizes)CB_Sizes.SelectedItem).SizeID;
                                string custom = RB_CustomPizza.IsChecked == true ? "da" : "nu";

                                Pizza pizza = new Pizza(PizzaName, Price, sizeID, custom);
                                //verify if already exists
                                db.Pizza.Add(pizza);
                                db.SaveChanges();

                                foreach (Ingredient ingredient in IngredientsList)
                                {
                                    IngredientsGroup group = new IngredientsGroup(ingredient.IngredientID, pizza.PizzaID);
                                    db.IngredientsGroup.Add(group);
                                }
                                db.SaveChanges();

                                if (BTN_Pizza.IsEnabled == false)
                                    AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList);
                                IngredientsList.Clear();
                                MessageBox.Show("Pizza created");
                            }else MessageBox.Show("Invalid Name for the pizza");

                        }
                        else MessageBox.Show("Insuficient ingredients");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding pizza");
                        File.WriteAllText("error.txt", ex.ToString());
                    }
                }
            }
            else
            {
                //beverage create
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        if (String.IsNullOrEmpty(TB_Name.Text) == false)
                        {
                            if (double.TryParse(TB_Price.Text.Split("")[0],out double result))
                            {
                                string beverageName = TB_Name.Text;

                                double beveragePrice = result;

                                Beverage beverage = new Beverage(beverageName, beveragePrice);
                                //verify if already exists
                                db.Beverages.Add(beverage);

                                db.SaveChanges();


                                if (BTN_Beverage.IsEnabled == false)
                                    AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);

                                MessageBox.Show("Beverage Created");
                            }
                            else MessageBox.Show("Invalid Price");

                        }
                        else MessageBox.Show("Invalid name for the beverage");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding beverage");
                        File.WriteAllText("error.txt", ex.ToString());
                    }
                }
                
            }
            
        }
        private void BTN_Edit_Click(object sender, RoutedEventArgs e)
        {

                Pizza pizza;
                if (String.Equals(BTN_Edit.Content, "Edit Pizza"))
                {
                  
                    if (LB_PizzaOrBeverage.SelectedItem != null)
                    {
                        RB_Pizza.IsEnabled = false;
                        RB_CustomPizza.IsEnabled = false;
                        RB_Beverage.IsEnabled = false;
                        BTN_Edit.Content = "Save Pizza"; // on edit
                        BTN_Create.IsEnabled = false;
                        LB_PizzaOrBeverage.IsEnabled = false;
                        BTN_Beverage.IsEnabled = false;
                        

                        LB_Ingredients.IsEnabled = true;
                        BTN_Undo.IsEnabled = true;
                        CB_Sizes.IsEnabled = true;
                        LB_Ingredients.SelectedItems.Clear();
                        using (PizzeriaDBContext db = new PizzeriaDBContext())
                        {
                            try
                            {
                                pizza = LB_PizzaOrBeverage.SelectedItem as Pizza;
                                TB_Name.Text = pizza.Name;
                                CB_Sizes.SelectedIndex = pizza.SizeID - 1;
                                Price = pizza.Price;
                                TB_Price.Text = $"{Price} RON";

                                foreach (Ingredient i in LB_Ingredients.Items)
                                    foreach (IngredientsGroup ingredientsGroup in pizza.IngredientsGroup)
                                        if (i.IngredientID == ingredientsGroup.IngredientID)
                                            LB_Ingredients.SelectedItems.Add(i);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error retriving pizza details");
                                File.WriteAllText("error.txt", ex.ToString());
                            }
                        }


                    }

                }
                else if(String.Equals(BTN_Edit.Content, "Save Pizza"))
                {
                    //on save
                    using (PizzeriaDBContext db = new PizzeriaDBContext())
                    {
                        try
                        {
                            if(string.IsNullOrEmpty(TB_Name.Text) == false)
                            {
                                pizza = db.Pizza
                                    .Include(pizza => pizza.Size)
                                    .Include(pizza => pizza.IngredientsGroup)
                                    .ThenInclude(ingredientsGroup => ingredientsGroup.Ingredient)
                                    .FirstOrDefault(pizza => pizza.PizzaID == ((Pizza)LB_PizzaOrBeverage.SelectedItem).PizzaID);
                                
                                //changing details
                                pizza.Name = TB_Name.Text;
                                pizza.Price = Price;
                                pizza.SizeID = ((Sizes)CB_Sizes.SelectedItem).SizeID;
                                //changing ingredients
                                foreach (Ingredient Lb_Ingredient in LB_Ingredients.SelectedItems)
                                {
                                    bool contains = false;
                                    foreach (IngredientsGroup PizzaIngredientsGroup in pizza.IngredientsGroup)
                                    {
                                        if (PizzaIngredientsGroup.IngredientID == Lb_Ingredient.IngredientID)
                                        {
                                            contains = true;
                                            break;
                                        }
                                    }
                                    if (contains == false)
                                    {
                                        pizza.IngredientsGroup.Add(new IngredientsGroup(Lb_Ingredient.IngredientID, pizza.PizzaID));
                                        pizza.Price = Price;
                                        MessageBox.Show($"Ingredient to add {Lb_Ingredient} to pizza {pizza}");
                                    }

                                }
                                db.SaveChanges();

                                foreach (IngredientsGroup PizzaIngredientsGroup in pizza.IngredientsGroup)
                                {
                                    bool contains = false;
                                    if (LB_Ingredients.SelectedItems.Count > 0)
                                    {
                                        foreach (Ingredient Lb_Ingredient in LB_Ingredients.SelectedItems)
                                        {

                                            if (PizzaIngredientsGroup.IngredientID == Lb_Ingredient.IngredientID)
                                            {
                                                contains = true;
                                                break;
                                            }
                                        }
                                        if (contains == false)
                                        {
                                            IngredientsGroup toRemoveIngredient = db.IngredientsGroup
                                                .Include(ingredientGroup => ingredientGroup.Ingredient)
                                                .FirstOrDefault(ingredientsGroup => ingredientsGroup.PizzaID == pizza.PizzaID && ingredientsGroup.Ingredient.IngredientID == PizzaIngredientsGroup.IngredientID);
                                            MessageBox.Show($"Ingredient to remove {toRemoveIngredient.Ingredient} from pizza {pizza} with ingredientID {toRemoveIngredient.IngredientID} and pizzaID {toRemoveIngredient.PizzaID}");
                                            pizza.IngredientsGroup.Remove(toRemoveIngredient);
                                        }
                                    }
                                }

                                db.SaveChanges();
                                AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList);
                                MessageBox.Show("Pizza saved succesfully");
                            }
                            else MessageBox.Show($"Invalid Name");


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error saving pizza details");
                            File.WriteAllText("error.txt", ex.ToString());

                        }
                    }
                    LB_Ingredients.SelectedItems.Clear();
                    BTN_Edit.Content = "Edit Pizza";
                    BTN_Undo.IsEnabled = false;
                    BTN_Create.IsEnabled = true;
                    BTN_Create.Content = "Create Pizza";
                    TB_Price.IsEnabled = false;
                    LB_Ingredients.IsEnabled = true;
                    TB_Name.Text = "Pizza Name";
                    LB_PizzaOrBeverage.IsEnabled = true;
                    RB_Pizza.IsEnabled = true;
                    RB_CustomPizza.IsEnabled = true;
                    RB_Beverage.IsEnabled = true;
                    BTN_Beverage.IsEnabled = true;

                }

                Beverage beverage;
                if (String.Equals(BTN_Edit.Content, "Edit Beverage"))
                {
                    if (LB_PizzaOrBeverage.SelectedItem != null)
                    {
                        BTN_Edit.Content = "Save Beverage"; // on edit
                        BTN_Undo.IsEnabled = true;
                        TB_Price.IsEnabled = true;

                        RB_Pizza.IsEnabled = false;
                        RB_CustomPizza.IsEnabled = false;
                        RB_Beverage.IsEnabled = false;
                        BTN_Create.IsEnabled = false;
                        LB_PizzaOrBeverage.IsEnabled = false;
                        LB_Ingredients.IsEnabled=false;
                        CB_Sizes.IsEnabled = false;
                        BTN_Pizza.IsEnabled = false;
                        

                        LB_Ingredients.SelectedItems.Clear();
                        using (PizzeriaDBContext db = new PizzeriaDBContext())
                        {
                            try
                            {
                                beverage = LB_PizzaOrBeverage.SelectedItem as Beverage;
                                TB_Name.Text = beverage.Name;
                                Price = beverage.Price;
                                TB_Price.Text = $"{Price}";

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error retriving pizza details");
                                File.WriteAllText("error.txt", ex.ToString());
                            }
                        }


                    }

                }
                else if(String.Equals(BTN_Edit.Content, "Save Beverage"))
                {
                    //on save
                    using (PizzeriaDBContext db = new PizzeriaDBContext())
                    {
                        try
                        {
                            beverage = db.Beverages
                            .FirstOrDefault(beverage => beverage.BeverageID == ((Beverage)LB_PizzaOrBeverage.SelectedItem).BeverageID);
                            ;

                            if(String.IsNullOrEmpty(TB_Name.Text) == false)
                            {
                                if (double.TryParse(TB_Price.Text.Split()[0], out double result))
                                {
                                    beverage.Name = TB_Name.Text;
                                    beverage.Price = result;
                                    db.SaveChanges();
                                    AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);
                                    MessageBox.Show("Beverage saved succesfuly.");
                                } else MessageBox.Show("Invalid Price");

                            }
                            else MessageBox.Show($"Invalid name");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error saving beverage details");
                            File.WriteAllText("error.txt", ex.ToString());

                        }
                    }
                    BTN_Edit.Content = "Edit Beverage";
                    BTN_Undo.IsEnabled = false;
                    BTN_Create.IsEnabled = true;
                    BTN_Create.Content = "Create Beverage";
                    TB_Price.IsEnabled = false;
                    TB_Name.Text = "Beverage Name";
                    LB_PizzaOrBeverage.IsEnabled = true;
                    RB_Pizza.IsEnabled = true;
                    RB_CustomPizza.IsEnabled = true;
                    RB_Beverage.IsEnabled = true;
                    BTN_Pizza.IsEnabled = true;

            }

        }

        private void BTN_Undo_Click(object sender, RoutedEventArgs e)
        {
            if(BTN_Pizza.IsEnabled == false)
            {
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        Pizza pizza = LB_PizzaOrBeverage.SelectedItem as Pizza;
                        TB_Name.Text = pizza.Name;
                        CB_Sizes.SelectedIndex = pizza.SizeID - 1;
                        Price = pizza.Price;
                        TB_Price.Text = $"{Price} RON";

                        foreach (Ingredient i in LB_Ingredients.Items)
                            foreach (IngredientsGroup ingredientsGroup in pizza.IngredientsGroup)
                                if (i.IngredientID == ingredientsGroup.IngredientID)
                                    LB_Ingredients.SelectedItems.Add(i);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error retriving pizza details");
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
                        Beverage beverage = LB_PizzaOrBeverage.SelectedItem as Beverage;
                        TB_Name.Text = beverage.Name;
                        Price = beverage.Price;
                        TB_Price.Text = $"{Price}";

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error retriving pizza details");
                        File.WriteAllText("error.txt", ex.ToString());
                    }
                }
            }
        }
    }
}
