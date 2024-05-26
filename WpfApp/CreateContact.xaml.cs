using System.Collections.Generic;
using System.Windows;

namespace ContactManager
{
    public partial class CreateContact : Window
    {
        private readonly ContactRepository contactRepository;
        private readonly int userId = 0;

        public CreateContact(ContactRepository contactRepository, ICollection<Category> categories, int userId)
        {
            InitializeComponent();
            this.contactRepository = contactRepository;

            cmbCategories.ItemsSource = categories;
            this.userId = userId;
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("phone is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

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