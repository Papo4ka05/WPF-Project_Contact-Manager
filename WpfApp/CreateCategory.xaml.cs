using System;
using System.Windows;

namespace ContactManager
{
    public partial class CreateCategory : Window
    {
        private readonly ICategoryRepository categoryRepository;

        public CreateCategory(ICategoryRepository categoryRepository)
        {
            InitializeComponent();

            this.categoryRepository = categoryRepository;
        }

        private void Create_Category(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                throw new Exception("name is empty");
            }

            categoryRepository.Create(tbName.Text);

            Close();
        }
    }
}