using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid.Native;

namespace BankApplication {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        void AddLoanButton_OnClick(object sender, RoutedEventArgs e) {
            (DataContext as ViewModel).Loans.Add(new Loan(){Name = nameEditor.Text, 
                                                               Surname = surnameEditor.Text, 
                                                               Value = valueEditor.Text.TryConvertToDouble(), 
                                                               Currency = currencyEditor.Text,
                                                               StartDate = startDateEditor.DateTime,
                                                               EndDate = endDateEditor.DateTime
                                                           });
            nameEditor.Clear();
            surnameEditor.Clear();
            valueEditor.Clear();
            currencyEditor.Clear();
        }

        void DeleteLoanButton_OnClick(object sender, RoutedEventArgs e) {
            var s = grid.SelectedItem;
            (DataContext as ViewModel).Loans.Remove((Loan)s);
        }

        void Grid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            new InstalmentWindow((Loan)grid.SelectedItem).ShowDialog();
        }
    }

    public class ViewModel {
        public ObservableCollection<Loan> Loans { get; set; }

        public ViewModel() {
            Loans = new ObservableCollection<Loan>(){new Loan() {Name = "Artem", Surname = "Denisov", Value = 400000, InterestRate = 18, Currency = "RUB", StartDate = new DateTime(2020, 3,10), EndDate = new DateTime(2020, 9,10)}};
        }
    }

    public class Loan {
        public string Name { get; set; }
        public string Surname { get; set; }
        public double Value { get; set; }
        
        public double InterestRate { get; set; }
        public string Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}