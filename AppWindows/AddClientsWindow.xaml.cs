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
    /// Interaction logic for AddClientsWindow.xaml
    /// </summary>
    public partial class AddClientsWindow : Page
    {
        public AddClientsWindow()
        {
            InitializeComponent();
            BTN_SaveEdit.Content = "Edit";
            //TB_AddAdress.Visibility = Visibility.Hidden;
            //BTN_AddAdress.Visibility = Visibility.Hidden;
        }

        private void BTN_ShowAdressAdder(object sender, RoutedEventArgs e)
        {
            if(TB_AddAdress.Visibility == Visibility.Hidden)
            {
                TB_AddAdress.Visibility = Visibility.Visible;
                BTN_AddAdress.Visibility = Visibility.Visible;
                CB_Adresses.Visibility = Visibility.Hidden;
            }
            else
            {
                TB_AddAdress.Visibility = Visibility.Hidden;
                BTN_AddAdress.Visibility = Visibility.Hidden;
                CB_Adresses.Visibility = Visibility.Visible;
            }
        }

        private int id;
        private void BTN_SaveEdit_Click(object sender, RoutedEventArgs e)
        {
             // id edited entry
            if (BTN_SaveEdit.Content == "Edit")
            {
                TB_AddAdress.Visibility = Visibility.Visible;
                BTN_AddAdress.Visibility = Visibility.Visible;
                BTN_SaveEdit.Content = "Save";
                CB_Adresses.Visibility = Visibility.Hidden;

                //load adress in TB_AddAdress
                // id  = entry.id;
                //TB_AddAdress.Text = Text;
            }
            else
            {
                string adress = TB_AddAdress.Text;

                if (string.IsNullOrEmpty(adress)!= true && adress != "New Adress")
                {


                    // edit data at that entry.id = id

                }
                TB_AddAdress.Visibility = Visibility.Hidden;
                BTN_AddAdress.Visibility = Visibility.Hidden;
                CB_Adresses.Visibility = Visibility.Visible;
                BTN_SaveEdit.Content = "Edit";
                id = 0;
            }
        }
    }
}
