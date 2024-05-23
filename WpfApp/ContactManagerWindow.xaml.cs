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
using System.Windows.Shapes;

namespace ContactManager
{
    /// <summary>
    /// Interaktionslogik für ContactManagerWindow.xaml
    /// </summary>
    public partial class ContactManagerWindow : Window
    {
        public ContactManagerWindow()
        {
            InitializeComponent();

            var lol = UserData.Username;
            // Load user-specific data based on _username
            LoadUserData();
        }
        private void LoadUserData()
        {
            
            var repository = new ContactRepository(/*UserData.Id*/1, 0);
            UserCategories.ItemsSource = repository.Categories;
            UserContacts.ItemsSource = repository.Contacts;

            BoxLol.Text = UserData.Username;

            // Use _username to load data from the database
            // For example:
            // string query = "SELECT * FROM Contacts WHERE UserId = (SELECT Id FROM Users WHERE Username = @Username)";
            // Load contacts and display them in the UI
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserCategories.SelectedItem != null)
            {
                
                int idCategories = (UserCategories.SelectedItem as Category).Id;
                var repository = new ContactRepository(/*UserData.Id*/1, idCategories);
                UserContacts.ItemsSource = repository.Contacts;

            }
        }
    }
}
