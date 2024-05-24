using System;
using System.Collections.Generic;
using System.Windows;

namespace ContactManager
{
    public partial class CreateContact : Window
    {
        private readonly IContactRepository contactRepository;

        public CreateContact(IContactRepository contactRepository, ICollection<Category> categories)
        {
            InitializeComponent();
            this.contactRepository = contactRepository;

            cmbCategories.ItemsSource = categories;
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            DateTime? dateOfBirth = null;

            if (string.IsNullOrEmpty(tbPhone.Text))
            {
                throw new Exception("phone is empty");
            }

            if (!string.IsNullOrEmpty(tbDateOfBirth.Text))
            {
                var dateParsed = DateTime.TryParse(tbDateOfBirth.Text, out DateTime date);
                if (!dateParsed)
                {
                    throw new Exception("date not parsed");
                }
            }

            int? categoryId = (cmbCategories.SelectedItem as Category)?.Id;

            var contact = new Contact
            {
                CategoryId = categoryId == 0 ? null : categoryId,
                DateOfBirth = dateOfBirth,
                Email = tbEmail.Text,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Note = tbNote.Text,
                PhoneNumber = tbPhone.Text,
                UserId = UserData.Id,
            };

            contactRepository.Create(contact);

            Close();
        }
    }
}