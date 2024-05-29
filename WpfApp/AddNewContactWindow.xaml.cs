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
        public string path = string.Empty;

        public AddNewContactWindow(ContactRepository contactRepository, ICollection<Category> categories, int userId)
        {
            InitializeComponent();
            this.contactRepository = contactRepository;

            cmbCategories.ItemsSource = categories;
            this.userId = userId;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            cmbCategories.SelectedIndex = 0;
            tbFirstName.Text = string.Empty;  
            tbLastName.Text = string.Empty;   
            dpDateOfBirth.SelectedDate = null;
            tbEmail.Text = string.Empty;      
            tbPhone.Text = string.Empty;      
            tbNote.Text = string.Empty;
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select an image
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // If an image is selected, set the path and update the image source
                path = openFileDialog.FileName;
                iImage.Source = new BitmapImage(new Uri(path));
            }
            else
            {
                // Display a message if no image is selected (message can be customized)
                MessageBox.Show("No image selected", "Image Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        // Event handler for the Create button click
        private void Create(object sender, RoutedEventArgs e)
        {
            // Validate that the phone number field is not empty
            if (string.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("Phone is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate that the first name field is not empty
            if (string.IsNullOrEmpty(tbFirstName.Text))
            {
                MessageBox.Show("First Name is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected category ID
            int? categoryId = (cmbCategories.SelectedItem as Category)?.Id;

            // Create a new contact object with the input data
            var contact = new Contact
            {
                CategoryId = categoryId == 0 ? null : categoryId,
                DateOfBirth = dpDateOfBirth.SelectedDate,
                Email = tbEmail.Text,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Note = tbNote.Text,
                PhoneNumber = tbPhone.Text,
                PhotoPath = string.IsNullOrEmpty(path) ? null : path, // Ensure path is set to null if empty
                UserId = userId,
            };

            // Add the new contact to the repository
            contactRepository.Create(contact);

            // Close the window
            Close();
        }


        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
