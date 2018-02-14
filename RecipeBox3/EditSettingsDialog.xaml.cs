﻿using System;
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

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for EditSettingsDialog.xaml
    /// </summary>
    public partial class EditSettingsDialog : Window
    {
        /// <summary>Create a new instance of the class</summary>
        public EditSettingsDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
