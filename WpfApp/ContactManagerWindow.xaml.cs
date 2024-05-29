using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ContactManager
{
    public partial class ContactManagerWindow : Window
    {
        // Repositories for managing categories and contacts
        private readonly CategoryRepository categoryRepository;
        private readonly ContactRepository contactRepository;

        // Variables to store selected category and contact IDs
        private int categoryId = 0;
        private int contactId = 0;

        // User ID and username
        private readonly int userId = 0;
        private readonly string username = string.Empty;

        // Constructor for the main window (default user)
        public ContactManagerWindow()
        {
            InitializeComponent();

            // Initialize repositories
            categoryRepository = new CategoryRepository();
            contactRepository = new ContactRepository();

            // Default user information (temporary solution)
            userId = 1;
            username = "peter";

            // Populate category and contact lists
            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);

            // Display username
            BoxLol.Text = username;
        }

        // Constructor for the main window with specified user ID and username
        public ContactManagerWindow(int userId, string username)
        {
            InitializeComponent();

            // Initialize repositories
            categoryRepository = new CategoryRepository();
            contactRepository = new ContactRepository();

            // Set user information
            this.userId = userId;
            this.username = username;

            // Populate category and contact lists
            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);

            // Display username
            BoxLol.Text = this.username;
        }

        // Event handler for category selection change
        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbCategories.SelectedItem != null)
            {
                // Update selected category ID and refresh contact list
                categoryId = (lbCategories.SelectedItem as Category)?.Id ?? 0;
                lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);
            }
        }

        // Event handler for changing category
        private void Change_Category(object sender, RoutedEventArgs e)
        {
            // Retrieve the selected category and open the window to change it
            Category category = ((Button)sender).Tag as Category;
            var changeCategoryWindow = new ChangeCategory(categoryRepository, category);
            changeCategoryWindow.ShowDialog();

            // Refresh the category list and maintain the selection
            List<Category> categories = (categoryRepository.GetList(userId)).ToList();
            int indexCategory = categories.Select(item => item.Id).ToList().IndexOf(categoryId);
            lbCategories.ItemsSource = categories;
            lbCategories.SelectedIndex = indexCategory;
        }

        // Event handler for changing contact
        private void Change_Contact(object sender, RoutedEventArgs e)
        {
            // Retrieve the selected contact and open the window to change it
            Contact contact = ((Button)sender).Tag as Contact;
            var changeContactWindow = new ChangeContact(contactRepository, contact, lbCategories.ItemsSource as ICollection<Category>);
            changeContactWindow.ShowDialog();

            // Refresh the contact list and maintain the selection
            List<Contact> contacts = (contactRepository.GetList(userId, categoryId)).ToList();
            int indexContact = contacts.Select(item => item.Id).ToList().IndexOf(contactId);
            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);
            lbContacts.SelectedIndex = indexContact;

            // Update contact information fields if the selected contact is changed
            var newContact = contacts.ElementAt(indexContact);
            if (contactId == newContact.Id)
            {
                tb_FirstName.Text = newContact.FirstName;
                tb_LastName.Text = newContact.LastName;
                tb_Birthday.Text = newContact.DateOfBirth.HasValue ? newContact.DateOfBirth.Value.ToString("dd.MM.yyyy") : string.Empty;
                tb_PhoneNumber.Text = newContact.PhoneNumber;
                tb_Email.Text = newContact.Email;
                tb_Note.Text = newContact.Note;
            }
        }

        // Event handler for creating a new category
        private void Create_Category(object sender, RoutedEventArgs e)
        {
            // Open the window to create a new category
            var createCategoryWindow = new CreateCategory(categoryRepository, userId);
            createCategoryWindow.ShowDialog();

            // Refresh the category list and maintain the selection
            List<Category> categories = (categoryRepository.GetList(userId)).ToList();
            int indexCategory = categories.Select(item => item.Id).ToList().IndexOf(categoryId);
            lbCategories.ItemsSource = categories;
            lbCategories.SelectedIndex = indexCategory;
        }

        // Event handler for creating a new contact
        private void Create_Contact(object sender, RoutedEventArgs e)
        {
            // Open the window to create a new contact
            var createContactWindow = new AddNewContactWindow(contactRepository, lbCategories.ItemsSource as ICollection<Category>, userId);
            createContactWindow.ShowDialog();

            // Refresh the contact list
            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);
        }

        // Event handler for deleting a category
        private void Delete_Category(object sender, RoutedEventArgs e)
        {
            // Retrieve the ID of the category to be deleted
            var deletCategoryId = (((Button)sender).Tag as Category).Id;
            // Delete the category
            categoryRepository.Delete(deletCategoryId);

            // Reset category ID if the deleted category was selected
            categoryId = categoryId == deletCategoryId ? 0 : categoryId;

            // Refresh the category list and select the first category
            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            lbCategories.SelectedIndex = 0;
        }

        // Event handler for deleting a contact
        private void Delete_Contact(object sender, RoutedEventArgs e)
        {
            // Retrieve the ID of the contact to be deleted
            var deleteContactId = (((Button)sender).Tag as Contact).Id;
            // Delete the contact
            contactRepository.Delete(deleteContactId);

            // Refresh the contact list
            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);

            // Clear contact information fields if the deleted contact was selected
            if (contactId == deleteContactId)
            {
                tb_FirstName.Text = null;
                tb_LastName.Text = null;
                tb_Birthday.Text = null;
                tb_PhoneNumber.Text = null;
                tb_Email.Text = null;
                tb_Note.Text = null;
                tb_Category.Text = null;
            }
        }

        private void lbContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbContacts.SelectedItem != null)
            {
                // Update selected contact ID and display contact information
                Contact contact = (lbContacts.SelectedItem as Contact);
                contactId = contact?.Id ?? 0;

                if (contact != null)
                {
                    // Display category name if available
                    if (contact.CategoryId != null)
                    {
                        List<Category> categories = (categoryRepository.GetList(userId)).ToList();
                        Category category = categories.Where(item => item.Id == contact.CategoryId).First();
                        tb_Category.Text = category.Name;
                    }
                    else
                    {
                        tb_Category.Text = null;
                    }

                    // Display contact information
                    tb_FirstName.Text = contact.FirstName;
                    tb_LastName.Text = contact.LastName;
                    tb_Birthday.Text = contact.DateOfBirth.HasValue ? contact.DateOfBirth.Value.ToString("dd.MM.yyyy"): string.Empty;
                    tb_PhoneNumber.Text = contact.PhoneNumber;
                    tb_Email.Text = contact.Email;
                    tb_Note.Text = contact.Note;
                    iImage.Source = contact.Photo;
                }
            }

        }

        private void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("App Version: 1.0\nCompatible only with Windows users.");
        }
    }
}