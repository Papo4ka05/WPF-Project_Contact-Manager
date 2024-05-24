﻿using System;
using System.Windows;

namespace ContactManager
{
    public partial class ChangeCategory : Window
    {
        private readonly Category category;
        private readonly ICategoryRepository categoryRepository;

        public ChangeCategory(ICategoryRepository categoryRepository, Category category)
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
                throw new Exception("name is empty");
            }

            categoryRepository.Update(category.Id, tbName.Text);

            Close();
        }
    }
}