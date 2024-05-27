using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
    /// Interaktionslogik für AddNewContactWindow.xaml
    /// </summary>
    public partial class AddNewContactWindow : Window
    {
        private readonly ContactRepository contactRepository;
        private readonly int userId = 0;
        public AddNewContactWindow(ContactRepository contactRepository, ICollection<Category> categories, int userId)
        {
            InitializeComponent();
            this.contactRepository = contactRepository;

            cmbCategories.ItemsSource = categories;
            this.userId = userId;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

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

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                byte[] AsBytes = File.ReadAllBytes(selectedImagePath);
                // String AsBase64String = Convert.ToBase64String(AsBytes);

            }
            else
            {
                MessageBox.Show("");
            }
        }


        private void Create(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("Phone is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            int? categoryId = (cmbCategories.SelectedItem as Category)?.Id;

            var contact = new Contact
            {
                CategoryId = categoryId == 0 ? null : categoryId,
                // TODO null wenn Text eingegeben wurde
                DateOfBirth = dpDateOfBirth.SelectedDate,
                Email = tbEmail.Text,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Note = tbNote.Text,
                PhoneNumber = tbPhone.Text,
                UserId = userId,
            };

            contactRepository.Create(contact);

            Close();
        }

    }
}
