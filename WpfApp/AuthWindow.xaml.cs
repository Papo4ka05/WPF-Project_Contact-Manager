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
                // ToolTip показывается подсказку при наведение мышкой на сам объект
                TextBoxLogin.ToolTip = "Это поле введёно не корректно!";
                TextBoxLogin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));
            }
            if (pass.Length < 5)
            {
                PassBox.ToolTip = "Это поле введёно не корректно!";
                PassBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE3F44"));    //Brushes.DarkRed; (another example)
            }
            else
            {
                TextBoxLogin.ToolTip = "";
                TextBoxLogin.Background = Brushes.Transparent;
                PassBox.ToolTip = "";
                PassBox.Background = Brushes.Transparent;

                //User authUser = null;
                //using (ApplicationContext context db = new ApplicationContex())
                //{
                //    authUser = DBNull.Users.Where(b => b.Login == login && b.Pass == pass).FirstOrDefault();
                //}

                //if (authUser != null)
                //{
                //    MessageBox.Show("Всё хорошо!");
                //}
                //else
                //{
                //    MessageBox.Show("Incorrect input!");
                //}
                
            }

        }

        private void Button_Registration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            Hide();
        }


    }
}
