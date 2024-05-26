using System.Windows;

namespace ContactManager
{
    public partial class ChangeCategory : Window
    {
        private readonly Category category;
        private readonly CategoryRepository categoryRepository;

        public ChangeCategory(CategoryRepository categoryRepository, Category category)
        {
            InitializeComponent();

            this.categoryRepository = categoryRepository;
            this.category = category;

            tbName.Text = category.Name;
        }

        private void Change(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("name is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            categoryRepository.Update(category.Id, tbName.Text);

            Close();
        }
    }
}