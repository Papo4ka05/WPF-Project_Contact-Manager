using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ContactManager
{
    /// <summary>
    /// Interaktionslogik für AddNewContactWindow.xaml
    /// </summary>
    public partial class AddNewContactWindow : Window
    {
        public AddNewContactWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_Drop(object sender, DragEventArgs e)
        {

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string originalText = textBox.Tag as string;
                if (textBox.Text == originalText)
                {
                    textBox.Text = "";
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string originalText = textBox.Tag as string;
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = originalText;
                }
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
            }

        }

    }
}
