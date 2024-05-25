using System.Windows;

namespace ContactManager
{
    public partial class CreateCategory : Window
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly int userId;

        public CreateCategory(ICategoryRepository categoryRepository, int userId)
        {
            InitializeComponent();

            this.categoryRepository = categoryRepository;
            this.userId = userId;
        }

        private void Create_Category(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("name is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            categoryRepository.Create(userId, tbName.Text);

            Close();
        }
    }
}