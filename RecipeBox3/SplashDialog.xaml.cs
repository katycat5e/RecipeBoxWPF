﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecipeBox3
{
    public partial class SplashDialog : Window, INotifyPropertyChanged
    {
        private string _StatusText = "Initializing...";
        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                if (value != _StatusText)
                {
                    _StatusText = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SplashDialog()
        {
            InitializeComponent();
            DataContext = this;
            //VersionLabel.Text = Application.Current;
        }

        
    }
}
