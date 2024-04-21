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

namespace Pizzeria.AppWindows
{
    /// <summary>
    /// Interaction logic for AddClientsWindow.xaml
    /// </summary>
    public partial class AddClientsWindow : Page
    {
        public AddClientsWindow()
        {
            InitializeComponent();
            TB_AddAdress.Visibility = Visibility.Hidden;
            BTN_AddAdress.Visibility = Visibility.Hidden;
            BTN_Cancel.Visibility = Visibility.Hidden;

            AppUtilities.LoadLBCustomers(LB_Clients, CustomersList);
            
        }
        private void Reset()
        {
            TB_Name.Text = "Name";
            TB_Phone.Text = "Phone number";
            BTN_EditClient.Content = "Edit Client";
            BTN_EditAdress.Content = "Edit";

            TB_Name.IsEnabled = false;
            TB_Phone.IsEnabled = false;

            BTN_AddClient.IsEnabled = true;
            LB_Clients.IsEnabled = true;

            BTN_EditClient.IsEnabled = false;
            BTN_ShowAdressAdder.IsEnabled = false;
            BTN_EditAdress.IsEnabled = false;

            TB_AddAdress.Visibility = Visibility.Hidden;
            BTN_AddAdress.Visibility = Visibility.Hidden;
            BTN_ShowAdressAdder.Visibility = Visibility.Visible;
            CB_Adresses.Visibility = Visibility.Visible;
            BTN_Cancel.Visibility = Visibility.Hidden;

        }
        private List<Customers> CustomersList;
        private List<Adress> CustomerAdresses;
        private void LB_Clients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_Clients.SelectedItems.Count > 0)
            {
                Customers customer = LB_Clients.SelectedItem as Customers;
                TB_Name.Text = customer.Name;
                TB_Phone.Text = customer.PhoneNumber;
                CB_Adresses.ItemsSource = customer.Adresses;
                AppUtilities.LoadCBAdresses(CB_Adresses, CustomerAdresses, customer.CustomerID);
                BTN_ShowAdressAdder.IsEnabled = true;
                BTN_EditClient.IsEnabled = true;
                BTN_EditAdress.IsEnabled = true;
            }
            else
            {
                BTN_ShowAdressAdder.IsEnabled = false;
                BTN_EditClient.IsEnabled = false;
                BTN_EditAdress.IsEnabled = false;
                Reset();
            }

        }
        private void BTN_ShowAdressAdder_Click(object sender, RoutedEventArgs e)
        {
            if(TB_AddAdress.Visibility == Visibility.Hidden)
            {
                //show
                TB_AddAdress.Visibility = Visibility.Visible;
                BTN_AddAdress.Visibility = Visibility.Visible;
                BTN_Cancel.Visibility = Visibility.Visible;

                CB_Adresses.Visibility = Visibility.Hidden;
                BTN_ShowAdressAdder.Visibility = Visibility.Hidden;
                TB_AddAdress.Text = "Adress";
                BTN_EditClient.IsEnabled = false;  
                BTN_EditAdress.IsEnabled = false;
            }
            else
            {
                //hide
                TB_AddAdress.Visibility = Visibility.Hidden;
                BTN_AddAdress.Visibility = Visibility.Hidden;
                CB_Adresses.Visibility = Visibility.Visible;
                BTN_EditClient.IsEnabled = true;
                BTN_EditAdress.IsEnabled = true;
            }
        }
        private void BTN_AddClient_Click(object sender, RoutedEventArgs e)
        {
            if(TB_Name.IsEnabled == true)
            {

                string phoneNumber = TB_Phone.Text;
                string Name = TB_Name.Text;

                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        try
                        {
                            Customers newCustomer = new Customers(Name, phoneNumber);
                            db.Customers.Add(newCustomer);
                            MessageBox.Show("Customer created succesfully");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        db.SaveChanges();
                        AppUtilities.LoadLBCustomers(LB_Clients, CustomersList);
                    }
                    catch (Exception ex)
                    {
                        AppUtilities.ShowError(ex, "Error creating cutomer");
                    }
                }
                Reset();
            }
            else
            {
                TB_Name.Text = "Name";
                TB_Phone.Text = "Phone number";
                CB_Adresses.ItemsSource = new List<Adress>();

                Reset();

                TB_Name.IsEnabled = true;
                TB_Phone.IsEnabled = true;

                LB_Clients.IsEnabled = false;

                BTN_Cancel.Visibility = Visibility.Visible;

            }

        }

        private void BTN_EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(BTN_EditClient.Content, "Edit Client"))
            {
                BTN_EditClient.Content = "Save Client";
                TB_Name.IsEnabled = true;
                TB_Phone.IsEnabled = true;
                BTN_AddClient.IsEnabled = false;
                BTN_EditAdress.IsEnabled = false;
                BTN_ShowAdressAdder.IsEnabled = false;
                BTN_Cancel.Visibility= Visibility.Visible;
            }
            else
            {
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        string name = TB_Name.Text;
                        string phoneNumber = TB_Phone.Text;
                        Customers customer = db.Customers
                               .FirstOrDefault(customer => customer.CustomerID == ((Customers)LB_Clients.SelectedItem).CustomerID);
                        try
                        {
                            customer.Name = name;
                            customer.PhoneNumber = phoneNumber;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        db.SaveChanges();
                        MessageBox.Show("Successfully saved Client's details.");
                        AppUtilities.LoadLBCustomers(LB_Clients, CustomersList);
                    }
                    catch (Exception ex)
                    {
                        AppUtilities.ShowError(ex, "Error client details");
                    }

                }
                Reset();
            }
            
        }
       

        private void BTN_AddAdress_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Clients.SelectedItems.Count > 0)
            {
                Customers Customer = LB_Clients.SelectedItem as Customers;
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        string details = TB_AddAdress.Text;
                        Customers dbCustomer = db.Customers
                            .Include(customer => customer.Adresses)
                            .FirstOrDefault(customer => customer.CustomerID == Customer.CustomerID);
                        try{                       
                            Adress adress = new Adress(details);
                            dbCustomer.Adresses.Add(adress);
                            db.SaveChanges();
                            AppUtilities.LoadCBAdresses(CB_Adresses, CustomerAdresses, dbCustomer.CustomerID);
                            MessageBox.Show("Successfully added the adress");
                            AppUtilities.LoadCBAdresses(CB_Adresses,CustomerAdresses, dbCustomer.CustomerID);
                            CB_Adresses.SelectedIndex = CB_Adresses.Items.Count-1;
                            TB_AddAdress.Visibility = Visibility.Hidden;
                            BTN_AddAdress.Visibility = Visibility.Hidden;
                            CB_Adresses.Visibility = Visibility.Visible;
                            BTN_ShowAdressAdder.Visibility = Visibility.Visible;
                            BTN_EditClient.IsEnabled = true;
                            BTN_Cancel.Visibility = Visibility.Hidden;
                            BTN_EditAdress.IsEnabled = true;

                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        AppUtilities.ShowError(ex, "Failed to add a adress to this user");
                    }
                }
            }
            else MessageBox.Show("Select a client first.");
            
        }

        private void BTN_EditAdress_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(BTN_EditAdress.Content,"Edit") == true)
            {

                //on edit
                if (CB_Adresses.Items.Count > 0)
                {
                    BTN_EditAdress.Content = "Save";

                    TB_AddAdress.Visibility = Visibility.Visible;
                    BTN_Cancel.Visibility = Visibility.Visible;

                    CB_Adresses.Visibility = Visibility.Hidden;
                    BTN_ShowAdressAdder.Visibility = Visibility.Hidden;
                    BTN_EditClient.IsEnabled = false;
                    LB_Clients.IsEnabled = false;


                    Adress adress = CB_Adresses.SelectedItem as Adress;

                    TB_AddAdress.Text = adress.Details;
                }
            }
            else
            {

                //on save
                using (PizzeriaDBContext db = new PizzeriaDBContext())
                {
                    try
                    {
                        int selectedIndex = CB_Adresses.SelectedIndex;
                        string details = TB_AddAdress.Text;
                        Adress adress = db.Adresses
                               .FirstOrDefault(adress  => adress.AdressID == ((Adress)CB_Adresses.SelectedItem).AdressID);
                        try
                        {
                            adress.Details = details;
                            db.SaveChanges();
                            MessageBox.Show("Successfully saved the adress");
                            AppUtilities.LoadCBAdresses(CB_Adresses, CustomerAdresses, adress.CustomerID);
                            CB_Adresses.SelectedIndex = selectedIndex;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        AppUtilities.ShowError(ex, "Error saving the address");
                    }
                }
                Reset();
                LB_Clients.SelectedIndex = -1;
                CB_Adresses.ItemsSource = new List<Adress>();
            }
        }

        private void BTN_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            CB_Adresses.ItemsSource = null;
            LB_Clients.SelectedIndex = -1;

        }
    }
}
