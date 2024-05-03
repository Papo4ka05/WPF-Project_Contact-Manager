using System.Windows;

namespace ContactManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // TODO set global user id
            var userId = 1;

            // open `All` category by default
            var repository = new ContactRepository(userId, 0);

            dgContacts.ItemsSource = repository.Contacts;
            lvCategories.ItemsSource = repository.Categories;
        }
    }
}
