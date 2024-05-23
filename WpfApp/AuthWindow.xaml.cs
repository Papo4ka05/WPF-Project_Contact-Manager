using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    /// Interaktionslogik für AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();


        }

        private void Button_Auth_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text.Trim();
            string pass = PassBox.Password.Trim();

            if (login.Length < 5)
            {
                TextBoxLogin.ToolTip = "This field is not entered correctly!";
                TextBoxLogin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
                return;
            }
            if (pass.Length < 5)
            {
                PassBox.ToolTip = "This field is not entered correctly!";
                PassBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
                return;
            }
            int id = CheckUser(login, pass);
            // Check user credentials
            if (id != 0)
            {
                UserData.Username = login;
                UserData.Id = id;

                // Open ContactManagerWindow and pass user data
                ContactManagerWindow contactManagerWindow = new ContactManagerWindow();
                contactManagerWindow.Show();

                this.Close();
            }
            else
            {
                TextBoxLogin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
                PassBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
                MessageBox.Show("Invalid username or password!");
            }

        }

        private void Button_Registration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();

            Close();
        }
        private int CheckUser(string username, string password)
        {
            using (var connection = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                string query = "SELECT Id FROM Users WHERE Username = @Username AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                connection.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count;
            }
        }

        //public bool CheckUser(string username, string password)
        //{

        //    using (var connection = new SqlConnection(Properties.Settings.Default.ConnectionString))
        //    {
        //        if (connection == null)
        //        {
        //            throw new Exception("connection string is null");
        //        }

        //        var query = new SqlCommand($"Select case when exists (Select 1 from Users where Username = '{username}' AND Password = '{password}')" +
        //            $"THEN 'true' " +
        //            $"ELSE 'false' " +
        //            $"END", connection);

        //        connection.Open();

        //        var sqlDataAdapter = new SqlDataAdapter(query);
        //        var dataTable = new DataTable();
        //        sqlDataAdapter.Fill(dataTable);

        //        var gay = dataTable.Rows[0].ToString();

        //        var userExists = Convert.ToBoolean(query.ExecuteScalar());

        //        bool userExists = Convert.ToBoolean(dataTable.Rows[0].ToString());

        //        return userExists;
        //    }

        //    return userExists;
        //}
    }
}
