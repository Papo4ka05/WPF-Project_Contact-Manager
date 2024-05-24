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

        public ContactManagerWindow()
        {
            InitializeComponent();

            categoryRepository = new CategoryRepository();
            contactRepository = new ContactRepository();

            // TODO temporaly
            UserData.Id = 1;

            lbCategories.ItemsSource = categoryRepository.GetList(UserData.Id);
            lbContacts.ItemsSource = contactRepository.GetList(UserData.Id, categoryId);

            BoxLol.Text = UserData.Username;
        }

        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbCategories.SelectedItem != null)
            {
                // temporäre Lösung
                categoryId = (lbCategories.SelectedItem as Category)?.Id ?? 0;
                lbContacts.ItemsSource = contactRepository.GetList(UserData.Id, categoryId);
            }
        }

        private void Change_Category(object sender, RoutedEventArgs e)
        {
            Category category = ((Button)sender).Tag as Category;
            var changeCategoryWindow = new ChangeCategory(categoryRepository, category);
            changeCategoryWindow.ShowDialog();

            lbCategories.ItemsSource = categoryRepository.GetList(UserData.Id);
        }

        private void Change_Contact(object sender, RoutedEventArgs e)
        {
            Contact contact = ((Button)sender).Tag as Contact;
            var changeContactWindow = new ChangeContact(contactRepository, contact, lbCategories.ItemsSource as ICollection<Category>);
            changeContactWindow.ShowDialog();

            lbContacts.ItemsSource = contactRepository.GetList(UserData.Id, categoryId);
        }

        private void Create_Category(object sender, RoutedEventArgs e)
        {
            var createCategoryWindow = new CreateCategory(categoryRepository);
            createCategoryWindow.ShowDialog();

            lbCategories.ItemsSource = categoryRepository.GetList(UserData.Id);
        }

        private void Create_Contact(object sender, RoutedEventArgs e)
        {
            var createContactWindow = new CreateContact(contactRepository, lbCategories.ItemsSource as ICollection<Category>);
            createContactWindow.ShowDialog();

            lbContacts.ItemsSource = contactRepository.GetList(UserData.Id, categoryId);
        }

        private void Delete_Category(object sender, RoutedEventArgs e)
        {
            var deletCategoryId = (((Button)sender).Tag as Category).Id;
            categoryRepository.Delete(deletCategoryId);

            categoryId = categoryId == deletCategoryId ? 0 : categoryId;

            lbCategories.ItemsSource = categoryRepository.GetList(UserData.Id);
        }

        private void Delete_Contact(object sender, RoutedEventArgs e)
        {
            var deleteContactId = (((Button)sender).Tag as Contact).Id;
            contactRepository.Delete(deleteContactId);

            lbContacts.ItemsSource = contactRepository.GetList(UserData.Id, categoryId);
        }
    }
}