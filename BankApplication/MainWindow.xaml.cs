using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;

namespace BankApplication {
     public enum Currency
    {
        RUB,
        EUR,
        USD,
    }

    public partial class MainWindow {

        public BankDBEntities Context { get; set; }
        CollectionViewSource viewSource;

        public MainWindow() {
            InitializeComponent();
            Context = new BankDBEntities();
            DataContext = this;
        }

        void AddLoanButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow() { Title = "Add new customer", Owner = this };

            var result = addWindow.ShowDialog(MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                viewSource.View.Refresh();
                Context.SaveChanges();
                ThemedMessageBox.Show(title: "Success", text: "New customer was added successfully!", icon: MessageBoxImage.Information, messageBoxButtons: MessageBoxButton.OK);
            }
            else
            {
                ThemedMessageBox.Show(title: "Something went wrong!", text: "New customer wasn't added successfully!", icon: MessageBoxImage.Error, messageBoxButtons: MessageBoxButton.OK);
            }

            
        }

        void DeleteLoanButton_OnClick(object sender, RoutedEventArgs e)
        {
            Context.Customers.Remove(grid.SelectedItem as Customer);
            Context.SaveChanges();
            viewSource.View.Refresh();
        }

        void Grid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowDetailsWindow();
        }

        void ShowDetailsWindow()
        {
            new InstalmentWindow((Customer)grid.SelectedItem).ShowDialog();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewSource = (CollectionViewSource)this.FindResource("customerViewSource");
            Context.Customers.Load();
            viewSource.Source = Context.Customers.Local;
        }

        private void ShowDetailsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowDetailsWindow();
        }
    }
}