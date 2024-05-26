using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ContactManager
{
    public partial class ContactManagerWindow : Window
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IContactRepository contactRepository;
        private int categoryId = 0;
        private readonly int userId = 0;
        private readonly string username = string.Empty;

        // TODO temporäre Lösung
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

            // TODO temporär
            this.userId = 1;
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

            // TODO categoryId stimmt nicht mehr, temporär wird Id auf 0 gesetzt. Alternative: nach Guid ergänzen
            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            categoryId = 0;
            lbCategories.SelectedIndex = categoryId;
        }

        private void Change_Contact(object sender, RoutedEventArgs e)
        {
            Contact contact = ((Button)sender).Tag as Contact;
            var changeContactWindow = new ChangeContact(contactRepository, contact, lbCategories.ItemsSource as ICollection<Category>);
            changeContactWindow.ShowDialog();

            lbContacts.ItemsSource = contactRepository.GetList(userId, categoryId);
        }

        private void Create_Category(object sender, RoutedEventArgs e)
        {
            var createCategoryWindow = new CreateCategory(categoryRepository, userId);
            createCategoryWindow.ShowDialog();

            // TODO categoryId stimmt nicht mehr, temporär wird Id auf 0 gesetzt. Alternative: nach Guid ergänzen
            lbCategories.ItemsSource = categoryRepository.GetList(userId);
            categoryId = 0;
            lbCategories.SelectedIndex = categoryId;
        }

        private void Create_Contact(object sender, RoutedEventArgs e)
        {
            var createContactWindow = new CreateContact(contactRepository, lbCategories.ItemsSource as ICollection<Category>, userId);
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
        }
    }
}