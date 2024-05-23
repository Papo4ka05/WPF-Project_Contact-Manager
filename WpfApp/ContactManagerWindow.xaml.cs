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
        public User User { get; set; }

        public ContactManagerWindow()
        {
            InitializeComponent();

            _username = username;

            // Load user-specific data based on _username
            LoadUserData();
        }
        private void LoadUserData()
        {
            // Use _username to load data from the database
            // For example:
            // string query = "SELECT * FROM Contacts WHERE UserId = (SELECT Id FROM Users WHERE Username = @Username)";
            // Load contacts and display them in the UI
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
