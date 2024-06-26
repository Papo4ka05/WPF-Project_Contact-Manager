﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace ContactManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            // Trim delete useless spaces
            string login = TextBoxLogin.Text.Trim();
            string pass = PassBox.Password.Trim();
            string passRepeat = PassBoxRepeat.Password.Trim();
            string email = TextBoxEmail.Text.Trim().ToLower();

            bool inputError = false;

            if (login.Length < 5 || login.Length > 50)
            {
                // ToolTip показывается подсказку при наведение мышкой на сам объект
                TextBoxLogin.ToolTip = "This field is not entered correctly!";
                TextBoxLogin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
                inputError = true;
            }
            if (pass.Length < 5 || pass.Length > 50)
            {
                PassBox.ToolTip = "This field is not entered correctly!";
                PassBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));    //Brushes.DarkRed; (another example)
                inputError = true;
            }
            if (pass != passRepeat)
            {
                PassBoxRepeat.ToolTip = "This field is not entered correctly!";
                PassBoxRepeat.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
                inputError = true;
            }
            if (email.Length < 5 || !email.Contains("@") || !email.Contains("."))
            {
                TextBoxEmail.ToolTip = "This field is not entered correctly!";
                TextBoxEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
                inputError = true;
            }
            if (CheckUser(login, email))
            {
                MessageBox.Show("This user already exists!");
                inputError = true;
            }
            if(!inputError)
            {
                TextBoxLogin.ToolTip = "";
                TextBoxLogin.Background = Brushes.Transparent;
                PassBox.ToolTip = "";
                PassBox.Background = Brushes.Transparent;
                PassBoxRepeat.ToolTip = "";
                PassBoxRepeat.Background = Brushes.Transparent;
                TextBoxEmail.ToolTip = "";
                TextBoxEmail.Background = Brushes.Transparent;

                int id = CreateUser(login, pass, email);

                // TODO userId fehlt
                ContactManagerWindow contactManagerWindow = new ContactManagerWindow(id, login);
                contactManagerWindow.Show();

                Close();
            }
        }

        private void Button_Window_Auth_Click(object sender, RoutedEventArgs e)
        {
            

            // необходимо создать объект на основе того окна на которую нужно перейти
            AuthWindow authWindow = new AuthWindow();
            // открывает новое окно
            authWindow.Show();

            Close();
        }

        public int CreateUser(string username, string password, string email)
        {
            using (var connection = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (connection == null)
                {
                    MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return -1;
                }

                try
                {
                    connection.Open();

                    var query = new SqlCommand(
                        "Insert into Users (Username, Password, Email) values (@Username, @Password, @Email); " +
                        "Select CAST(scope_identity() AS int);",
                        connection);

                    query.Parameters.AddWithValue("@Username", username);
                    query.Parameters.AddWithValue("@Password", password);
                    query.Parameters.AddWithValue("@Email", email);

                    // Выполняем запрос и получаем ID нового пользователя
                    int newUserId = (int)query.ExecuteScalar();
                    return newUserId;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return -1;
                }
            }
        }


        public bool CheckUser(string username, string email) 
        {
            
            using (var connection = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (connection == null)
                {
                    MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var query = new SqlCommand($"Select case when exists (Select 1 from Users where Username = '{username}' or Email = '{email}')" +
                    $"THEN 'true' " +
                    $"ELSE 'false' " +
                    $"END", connection);

                connection.Open();

                var sqlDataAdapter = new SqlDataAdapter(query);
                var dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                var userExists = Convert.ToBoolean(query.ExecuteScalar());

                // bool userExists = Convert.ToBoolean(dataTable.Rows[0].ToString());

                return userExists;
            }

            //return userExists;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string originalText = textBox.Tag as string;
                if (textBox.Text == originalText)
                {
                    textBox.Text = "";
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string originalText = textBox.Tag as string;
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = originalText;
                }
            }
        }
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                string originalText = passwordBox.Tag as string;
                if (passwordBox.Password == originalText)
                {
                    passwordBox.Password = "";
                }
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                string originalText = passwordBox.Tag as string;
                if (string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    passwordBox.Password = originalText;
                }
            }
        }

    }
}
