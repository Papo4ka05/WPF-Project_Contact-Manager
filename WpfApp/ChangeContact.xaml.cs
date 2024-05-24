using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ContactManager
{
    public partial class ChangeContact : Window
    {
        public readonly Contact Contact;
        private readonly IContactRepository contactRepository;

        public ChangeContact(IContactRepository contactRepository, Contact contact, ICollection<Category> categories)
        {
            InitializeComponent();

            this.contactRepository = contactRepository;

            Contact = contact;

            cmbCategories.ItemsSource = categories;

            if(contact.CategoryId.HasValue)
            {
                cmbCategories.SelectedItem = categories.First(c => c.Id == contact.CategoryId);
            }

            // TODO date format
            tbDateOfBirth.Text = contact.DateOfBirth.ToString();
            tbEmail.Text = contact.Email;
            tbFirstName.Text = contact.FirstName;
            tbLastName.Text = contact.LastName;
            tbNote.Text = contact.Note;
            tbPhone.Text = contact.PhoneNumber;
        }

        private void Change(object sender, RoutedEventArgs e)
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

            var changeContact = new Contact
            {
                CategoryId = categoryId == 0 ? null : categoryId,
                DateOfBirth = dateOfBirth,
                Email = tbEmail.Text,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Id = Contact.Id,
                Note = tbNote.Text,
                PhoneNumber = tbPhone.Text,
                UserId = Contact.UserId,
            };

            contactRepository.Update(changeContact);

            Close();
        }
    }
}