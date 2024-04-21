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
using System.Linq.Expressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Pizzeria.AppWindows
{
    /// <summary>
    /// Interaction logic for CreatePizzaOrBeverage.xaml
    /// </summary>
    public partial class CreatePizzaOrBeverage : Page
    {

        private MainWindow mainWindow;
        private OrderWindow OrderWindow;
        private List<Ingredient> IngredientsLoadList;
        private List<Ingredient> IngredientsList;
        private double Price;
        private List<Sizes> SizesList;
        private List<Pizza> PizzaList;
        private List<Beverage> BeverageList;
        private Cart Cart;
        private string pizzaType;

        public CreatePizzaOrBeverage(Cart cart = null,string PizzaType = "nu", MainWindow main = null, OrderWindow orderWindow = null)
        {
            InitializeComponent();
            RB_Pizza.IsChecked = true;
            pizzaType = PizzaType;
            if(string.Equals(pizzaType, "da")) {// on creat pizza from orders
                mainWindow = main;
                OrderWindow = orderWindow;
                Cart = cart;
                BTN_Back.Visibility = Visibility.Visible;
                BTN_Pizza.IsEnabled = false;
                BTN_Edit.IsEnabled = false;
                RB_CustomPizza.IsChecked = true;
                RB_Beverage.IsEnabled = false;
                RB_Pizza.IsEnabled = false;
                BTN_Beverage.IsEnabled = false;
                LB_PizzaOrBeverage.IsEnabled = false;
            }
            else BTN_Back.Visibility = Visibility.Hidden;


            AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);
            AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList);
            AppUtilities.LoadCBSizes(CB_Sizes,SizesList);
            using (var db = new PizzeriaDBContext())
            {
                try
                {
                    IngredientsLoadList = db.Ingredients.ToList();
                    LB_Ingredients.ItemsSource = IngredientsLoadList;
                    //MessageBox.Show("Loaded Ingredients");
                }
                catch (Exception ex)
                {
                    AppUtilities.ShowError(ex,"Error loading Ingredients");
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
            if(RB_Beverage.IsChecked == false)
            {
                TB_Price.IsEnabled = false;
                LB_Ingredients.IsEnabled = true;
            }
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

            if(RB_Beverage.IsChecked == true)
            {
                TB_Price.IsEnabled = true; 
                LB_Ingredients.IsEnabled = false;
            }


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

        private void CreatePizza(out Pizza CreatedPizza)
        {
            CreatedPizza = null;
            using (PizzeriaDBContext db = new PizzeriaDBContext())
            {
                try
                {
                    if (IngredientsList.Count > 0)
                    {
                        Pizza pizza;
                        try
                        {
                            string PizzaName = TB_Name.Text;
                            int sizeID = ((Sizes)CB_Sizes.SelectedItem).SizeID;
                            string custom = RB_CustomPizza.IsChecked == true ? "da" : "nu";
                            pizza = new Pizza(PizzaName, Price, sizeID, custom);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return ;
                        }
                        db.Pizza.Add(pizza);
                        db.SaveChanges();

                        //get last pizza inserted
                        Pizza newPizza = db.Pizza
                            .Include(pizza => pizza.IngredientsGroup)
                            .FirstOrDefault(pizzaFromDB => pizzaFromDB.PizzaID == pizza.PizzaID);

                        // add ingredients to last pizza inserted
                        foreach (Ingredient ingredient in IngredientsList)
                            pizza.IngredientsGroup.Add(new IngredientsGroup(ingredient.IngredientID, pizza.PizzaID));

                        db.SaveChanges();

                        if (BTN_Pizza.IsEnabled == false)
                            AppUtilities.LoadLBPizza(LB_PizzaOrBeverage, PizzaList);

                        if (string.Equals(BTN_Edit.Content, "Save Pizza"))
                        {
                            BTN_Undo.IsEnabled = false;
                            BTN_Edit.Content = "Edit Pizza";
                            LB_PizzaOrBeverage.IsEnabled = true;
                            BTN_Beverage.IsEnabled = true;
                            RB_Beverage.IsEnabled = true;
                            RB_CustomPizza.IsEnabled = true;
                            RB_Pizza.IsEnabled = true;
                        }
                        MessageBox.Show("Pizza created");

                        if (string.Equals(pizzaType, "da"))
                        {
                            try{
                                pizza = db.Pizza
                                    .Include(s => s.Size)
                                    .FirstOrDefault(p => p.PizzaID == pizza.PizzaID);
                                Cart.Add(pizza);
                                AppUtilities.LoadLBCart(OrderWindow.LB_Cart, Cart);
                                OrderWindow.TB_TotalPrice.Text = $"{Cart.cartPrice} RON";
                            }catch (Exception ex)
                            {
                                AppUtilities.ShowError(ex, "Something went wrong when adding custom pizza to cart.");
                            }
                        }

                    }
                    else MessageBox.Show("Insuficient ingredients");

                }
                catch (Exception ex)
                {
                    AppUtilities.ShowError(ex, "Error adding pizza");
                }
            }
        }
        private void BTN_Create_Click(object sender, RoutedEventArgs e)
        {
            if (RB_Pizza.IsChecked == true || RB_CustomPizza.IsChecked == true)
            {
                //pizza create
                Pizza CreatedPizza;
                CreatePizza(out CreatedPizza);
            }
            else
            {
                //beverage create
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {

                        if (double.TryParse(TB_Price.Text.Split("")[0],out double result))
                        {
                            string beverageName = TB_Name.Text;

                            double beveragePrice = result;
                            Beverage beverage;
                            try{
                                beverage = new Beverage(beverageName, beveragePrice);

                                MessageBox.Show("Beverage Created");
                            }
                            catch (Exception ex)
                                {
                                MessageBox.Show(ex.Message);
                                return;
                                }
                            db.Beverages.Add(beverage);
                            db.SaveChanges();


                            if (BTN_Beverage.IsEnabled == false)
                                AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);

                        }
                        else MessageBox.Show("Invalid Price");

                    }
                    catch (Exception ex)
                    {
                        AppUtilities.ShowError(ex, "Error adding beverage");
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
                        //BTN_Create.IsEnabled = false;
                        LB_PizzaOrBeverage.IsEnabled = false;
                        BTN_Beverage.IsEnabled = false;
                        TB_Price.IsEnabled = false;
                        

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
                                AppUtilities.ShowError(ex, "Error retriving pizza details");
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
                                try
                                {
                                    pizza.Name = TB_Name.Text;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    return;
                                }
                                
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
                                       // MessageBox.Show($"Ingredient to add {Lb_Ingredient} to pizza {pizza}");
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
                                           // MessageBox.Show($"Ingredient to remove {toRemoveIngredient.Ingredient} from pizza {pizza} with ingredientID {toRemoveIngredient.IngredientID} and pizzaID {toRemoveIngredient.PizzaID}");
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
                            AppUtilities.ShowError(ex, "Error saving pizza details");
                        }
                    }
                    LB_Ingredients.SelectedItems.Clear();
                    BTN_Edit.Content = "Edit Pizza";
                    BTN_Undo.IsEnabled = false;
                    BTN_Create.IsEnabled = true;
                    BTN_Create.Content = "Create Pizza";
                    if (RB_Beverage.IsChecked == true)
                        TB_Price.IsEnabled = true;
                    else
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
                                AppUtilities.ShowError(ex, "Error retriving pizza details");
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

                        try{
                            beverage.Name = TB_Name.Text;
                        }
                        catch (Exception ex) { 
                            MessageBox.Show(ex.Message); 
                            return; 
                        }

                        if (double.TryParse(TB_Price.Text.Split()[0], out double result))
                        {
                            beverage.Price = result;
                            db.SaveChanges();
                            AppUtilities.LoadLBBeverages(LB_PizzaOrBeverage, BeverageList);
                            MessageBox.Show("Beverage saved succesfuly.");
                        }
                        else MessageBox.Show("Invalid Price");
                     
                    }
                    catch (Exception ex)
                    {
                        AppUtilities.ShowError(ex, "Error saving beverage details");
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
                        AppUtilities.ShowError(ex, "Error retriving pizza details");
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
                        AppUtilities.ShowError(ex, "Error retriving pizza details");
                    }
                }
            }
        }

        private void BTN_Back_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Main.GoBack();  
        }

        private void BTN_ClearIngredients_Click(object sender, RoutedEventArgs e)
        {
            LB_Ingredients.SelectedItems.Clear();
        }
    }
}
