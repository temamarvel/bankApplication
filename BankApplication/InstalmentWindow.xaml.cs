using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BankApplication {
    public partial class InstalmentWindow : Window {
        public InstalmentWindow() {
            InitializeComponent();
        }

        public InstalmentWindow(Customer customer) : this() {
            Customer = customer;
            DataContext = new InstalmentViewModel(Customer);
        }

        public Customer Customer { get; set; }
    }

    public class InstalmentViewModel {
        public ObservableCollection<InstalmentRecord> InstalmentRecords { get; set; }
        public ObservableCollection<DataSeries> Data { get; set; }

        public InstalmentViewModel(Customer customer) {
            InstalmentRecords = new ObservableCollection<InstalmentRecord>();


            var period = customer.EndDate - customer.StartDate;
            var months = period.Value.Days / 30;

            double interestRate = (double)customer.InterestRate / 12 / 100;
            double payment = Math.Round((double)customer.Value * ((interestRate * Math.Pow(1 + interestRate, months)) / (Math.Pow(1 + interestRate, months) - 1)));
            double balance = (double)customer.Value;

            DateTime recordDate = customer.StartDate.Value;
            for(int i = 1; i <= months; i++) {
                recordDate = recordDate.AddDays(30);
                double percentage = Math.Round(balance * interestRate, 2);
                double debt = Math.Round(payment - percentage, 2);
                balance -= debt;
                if(i == months) {
                    payment += balance;
                    balance = 0;
                }

                InstalmentRecords.Add(new InstalmentRecord() {Date = recordDate, Payment = payment, Debt = debt, Percentage = percentage, Balance = balance});
            }

            Data = new ObservableCollection<DataSeries>();

            ObservableCollection<DataPoint> valuesPayment = new ObservableCollection<DataPoint>();
            foreach(InstalmentRecord record in InstalmentRecords) {
                valuesPayment.Add(new DataPoint(record.Date, record.Payment));
            }

            ObservableCollection<DataPoint> valuesDept = new ObservableCollection<DataPoint>();
            foreach(InstalmentRecord record in InstalmentRecords) {
                valuesDept.Add(new DataPoint(record.Date, record.Debt));
            }

            ObservableCollection<DataPoint> valuesPercentage = new ObservableCollection<DataPoint>();
            foreach(InstalmentRecord record in InstalmentRecords) {
                valuesPercentage.Add(new DataPoint(record.Date, record.Percentage));
            }

            ObservableCollection<DataPoint> valuesBalance = new ObservableCollection<DataPoint>();
            foreach(InstalmentRecord record in InstalmentRecords) {
                valuesBalance.Add(new DataPoint(record.Date, record.Balance));
            }

            Data.Add(new DataSeries() {Name = "Payment", Values = valuesPayment});
            Data.Add(new DataSeries() {Name = "Dept", Values = valuesDept});
            Data.Add(new DataSeries() {Name = "Percentage", Values = valuesPercentage});
            Data.Add(new DataSeries() {Name = "Balance", Values = valuesBalance});
        }
    }


    public class InstalmentRecord {
        public DateTime Date { get; set; }
        public double Payment { get; set; }
        public double Debt { get; set; }
        public double Percentage { get; set; }
        public double Balance { get; set; }

    }

    public class DataSeries {
        public string Name { get; set; }
        public ObservableCollection<DataPoint> Values { get; set; }
    }

    public class DataPoint {
        public DateTime Date { get; set; }
        public double Value { get; set; }

        public DataPoint(DateTime date, double value) {
            Date = date;
            Value = value;
        }
    }
}