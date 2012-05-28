﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IISExpressManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileIO _fileIO = new FileIO();
        private IList<WebSite> _webSites;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _webSites = WebSite.GetAllWebsites(_fileIO);
                lstSites.ItemsSource = _webSites;
                cboProtocols.ItemsSource = Enum.GetValues(typeof(WebSite.BindingProtocol));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }

        private void lstSites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentSite = lstSites.SelectedItem as WebSite;
            grpSite.DataContext = currentSite;
            btnSave.DataContext = currentSite;
            grpApplication.DataContext = currentSite;
            grpBinding.DataContext = currentSite;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var currentSite = lstSites.SelectedItem as WebSite;
            currentSite.IsDirty = false;
        }
    }
}
