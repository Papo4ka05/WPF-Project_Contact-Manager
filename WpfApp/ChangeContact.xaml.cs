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

            dpDateOfBirth.SelectedDate = contact.DateOfBirth;
            tbEmail.Text = contact.Email;
            tbFirstName.Text = contact.FirstName;
            tbLastName.Text = contact.LastName;
            tbNote.Text = contact.Note;
            tbPhone.Text = contact.PhoneNumber;
        }

        private void Change(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("phone is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            int? categoryId = (cmbCategories.SelectedItem as Category)?.Id;

            var changeContact = new Contact
            {
                CategoryId = categoryId == 0 ? null : categoryId,
                // TODO null wenn Text eingegeben wurde
                DateOfBirth = dpDateOfBirth.SelectedDate,
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