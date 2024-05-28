using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ContactManager
{
    public partial class ContactManagerWindow : Window
    {
        private readonly CategoryRepository categoryRepository;
        private readonly ContactRepository contactRepository;
        private int categoryId = 0;
        private int contactId = 0;
        private readonly int userId = 0;
        private readonly string username = string.Empty;

        // TODO temporäre Lösung        //          //          ////        //      //
        public ContactManagerWindow()
        {
            InitializeComponent();

            categoryRepository = new CategoryRepository();
            contactRepository = new ContactRepository();

            // TODO temporär
            userId = 1;
            username = "peter";

            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);

            BoxLol.Text = username;
        }

        public ContactManagerWindow(int userId, string username)
        {
            InitializeComponent();

            categoryRepository = new CategoryRepository();
            contactRepository = new ContactRepository();

            this.userId = userId;
            this.username = username;

            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);

            BoxLol.Text = this.username;
        }

        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbCategories.SelectedItem != null)
            {
                categoryId = (lbCategories.SelectedItem as Category)?.Id ?? 0;
                lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);
            }
        }

        private void Change_Category(object sender, RoutedEventArgs e)
        {
            Category category = ((Button)sender).Tag as Category;
            var changeCategoryWindow = new ChangeCategory(categoryRepository, category);
            changeCategoryWindow.ShowDialog();

            List<Category> categories = (categoryRepository.GetList(userId)).ToList();
            int indexCategory = categories.Select(item => item.Id).ToList().IndexOf(categoryId);
            //foreach (var item in categories)
            //{
            //    if (item.Id == categoryId)
            //    {
            //        return item.Id;
            //    }
            //}
            //int oldIndex = lbCategories.SelectedIndex;
            lbCategories.ItemsSource = categories;

            lbCategories.SelectedIndex = indexCategory;
            
        }

        private void Change_Contact(object sender, RoutedEventArgs e)
        {
            Contact contact = ((Button)sender).Tag as Contact;
            var changeContactWindow = new ChangeContact(contactRepository, contact, lbCategories.ItemsSource as ICollection<Category>);
            changeContactWindow.ShowDialog();

            List<Contact> contacts = (contactRepository.GetList(userId, categoryId)).ToList();
            int indexContact = contacts.Select(item => item.Id).ToList().IndexOf(contactId);

            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);

            lbContacts.SelectedIndex = indexContact;

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

        private void Create_Category(object sender, RoutedEventArgs e)
        {
            var createCategoryWindow = new CreateCategory(categoryRepository, userId);
            createCategoryWindow.ShowDialog();

            List<Category> categories = (categoryRepository.GetList(userId)).ToList();
            int indexCategory = categories.Select(item => item.Id).ToList().IndexOf(categoryId);
            lbCategories.ItemsSource = categories;

            lbCategories.SelectedIndex = indexCategory;
        }

        private void Create_Contact(object sender, RoutedEventArgs e)
        {
            var createContactWindow = new AddNewContactWindow(contactRepository, lbCategories.ItemsSource as ICollection<Category>, userId);
            createContactWindow.ShowDialog();

            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);
        }

        private void Delete_Category(object sender, RoutedEventArgs e)
        {
            var deletCategoryId = (((Button)sender).Tag as Category).Id;
            categoryRepository.Delete(deletCategoryId);

            categoryId = categoryId == deletCategoryId ? 0 : categoryId;

            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            lbCategories.SelectedIndex = 0;
        }

        private void Delete_Contact(object sender, RoutedEventArgs e)
        {
            var deleteContactId = (((Button)sender).Tag as Contact).Id;
            contactRepository.Delete(deleteContactId);

            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);

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
                Contact contact = (lbContacts.SelectedItem as Contact);

                contactId = contact?.Id ?? 0 ; 

                if (contact != null)
                {
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