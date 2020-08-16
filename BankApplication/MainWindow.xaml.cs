using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;

namespace BankApplication {
     public enum Currency
    {
        RUB,
        EUR,
        USD,
    }

    public partial class MainWindow {

        BankDBEntities context = new BankDBEntities();
        CollectionViewSource viewSource;

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
        }

        void AddLoanButton_OnClick(object sender, RoutedEventArgs e)
        {
            context.Customers.Add(new Customer()
            {
                Id = Guid.NewGuid(),
                Name = nameEditor.Text,
                Surname = surnameEditor.Text,
                Value = (float)valueEditor.Text.TryConvertToDouble(),
                Currency = currencyEditor.Text,
                StartDate = startDateEditor.DateTime,
                EndDate = endDateEditor.DateTime,
                InterestRate = (float)interestRateEditor.Text.TryConvertToDouble()
            });

            nameEditor.Clear();
            surnameEditor.Clear();
            valueEditor.Clear();
            currencyEditor.Clear();

            viewSource.View.Refresh();
            context.SaveChanges();
        }

        void DeleteLoanButton_OnClick(object sender, RoutedEventArgs e)
        {
            context.Customers.Remove(grid.SelectedItem as Customer);
            context.SaveChanges();
            viewSource.View.Refresh();
        }

        void Grid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new InstalmentWindow((Customer)grid.SelectedItem).ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewSource = (CollectionViewSource)this.FindResource("customerViewSource");
            context.Customers.Load();
            viewSource.Source = context.Customers.Local;
        }
    }


    public class Loan
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public double Value { get; set; }

        public double InterestRate { get; set; }
        public string Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}