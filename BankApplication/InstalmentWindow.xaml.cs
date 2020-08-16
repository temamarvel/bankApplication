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
        public string CustomerName { get; set; }
        public double LoanSum { get; set; }
        public string InterestRate { get; set; }
        public double TotalAnnuityPercentage { get; set; }
        public double TotalDifferentialPercentage { get; set; }

        public ObservableCollection<InstalmentRecord> AnnuityRecords { get; set; }
        public ObservableCollection<InstalmentRecord> DifferentialRecords { get; set; }
        public ObservableCollection<DataSeries> AnnuityData { get; set; }
        public ObservableCollection<DataSeries> DifferentialData { get; set; }

        public InstalmentViewModel(Customer customer)
        {
            CustomerName = $"{customer.Name} {customer.Surname}";
            LoanSum = (double)customer.Value;
            InterestRate = $"{customer.InterestRate.Value.ToString()}%";

            TotalAnnuityPercentage = CalculateAnnuityPayments(customer);
            TotalDifferentialPercentage = CalculateDifferentialPayments(customer);
        }

        double CalculateAnnuityPayments(Customer customer)
        {
            AnnuityRecords = new ObservableCollection<InstalmentRecord>();

            double totalPercentage = 0;

            var period = customer.EndDate - customer.StartDate;
            var months = period.Value.Days / 30;

            double interestRate = (double)customer.InterestRate / 12 / 100;
            double totalPayment = Math.Round((double)customer.Value * ((interestRate * Math.Pow(1 + interestRate, months)) / (Math.Pow(1 + interestRate, months) - 1)));
            double balance = (double)customer.Value;

            DateTime recordDate = customer.StartDate.Value;
            for (int i = 1; i <= months; i++)
            {
                recordDate = recordDate.AddDays(30);
                double percentage = Math.Round(balance * interestRate, 2);
                totalPercentage += percentage;
                double debtPayment = Math.Round(totalPayment - percentage, 2);
                balance -= debtPayment;
                if (i == months)
                {
                    totalPayment += balance;
                    balance = 0;
                }

                AnnuityRecords.Add(new InstalmentRecord() { Date = recordDate, TotalPayment = totalPayment, DebtPayment = debtPayment, Percentage = percentage, Balance = balance });
            }

            AnnuityData = new ObservableCollection<DataSeries>();

            ObservableCollection<DataPoint> valuesPayment = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in AnnuityRecords)
            {
                valuesPayment.Add(new DataPoint(record.Date, record.TotalPayment));
            }

            ObservableCollection<DataPoint> valuesDept = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in AnnuityRecords)
            {
                valuesDept.Add(new DataPoint(record.Date, record.DebtPayment));
            }

            ObservableCollection<DataPoint> valuesPercentage = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in AnnuityRecords)
            {
                valuesPercentage.Add(new DataPoint(record.Date, record.Percentage));
            }

            ObservableCollection<DataPoint> valuesBalance = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in AnnuityRecords)
            {
                valuesBalance.Add(new DataPoint(record.Date, record.Balance));
            }

            AnnuityData.Add(new DataSeries() { Name = "Payment", Values = valuesPayment });
            AnnuityData.Add(new DataSeries() { Name = "Dept", Values = valuesDept });
            AnnuityData.Add(new DataSeries() { Name = "Percentage", Values = valuesPercentage });
            AnnuityData.Add(new DataSeries() { Name = "Balance", Values = valuesBalance });

            return totalPercentage;
        }
        double CalculateDifferentialPayments(Customer customer)
        {
            DifferentialRecords = new ObservableCollection<InstalmentRecord>();

            double totalPercentage = 0;

            var period = customer.EndDate - customer.StartDate;
            var months = period.Value.Days / 30;

            double interestRate = (double)customer.InterestRate / 12 / 100;
            double debtPayment = Math.Round((double)customer.Value/months, 2);
            double balance = Math.Round((double)customer.Value, 2);

            DateTime recordDate = customer.StartDate.Value;
            for (int i = 1; i <= months; i++)
            {
                recordDate = recordDate.AddDays(30);
                double percentage = Math.Round(balance * interestRate, 2);
                totalPercentage += percentage;
                double totalPayment = Math.Round(debtPayment + percentage, 2);
                balance -= debtPayment;
                if (i == months)
                {
                    debtPayment += balance;
                    balance = 0;
                }

                DifferentialRecords.Add(new InstalmentRecord() { Date = recordDate, TotalPayment = totalPayment, DebtPayment = debtPayment, Percentage = percentage, Balance = balance });
            }

            DifferentialData = new ObservableCollection<DataSeries>();

            ObservableCollection<DataPoint> valuesPayment = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in DifferentialRecords)
            {
                valuesPayment.Add(new DataPoint(record.Date, record.TotalPayment));
            }

            ObservableCollection<DataPoint> valuesDept = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in DifferentialRecords)
            {
                valuesDept.Add(new DataPoint(record.Date, record.DebtPayment));
            }

            ObservableCollection<DataPoint> valuesPercentage = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in DifferentialRecords)
            {
                valuesPercentage.Add(new DataPoint(record.Date, record.Percentage));
            }

            ObservableCollection<DataPoint> valuesBalance = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in DifferentialRecords)
            {
                valuesBalance.Add(new DataPoint(record.Date, record.Balance));
            }

            DifferentialData.Add(new DataSeries() { Name = "TotalPayment", Values = valuesPayment });
            DifferentialData.Add(new DataSeries() { Name = "DebtPayment", Values = valuesDept });
            DifferentialData.Add(new DataSeries() { Name = "Percentage", Values = valuesPercentage });
            DifferentialData.Add(new DataSeries() { Name = "Balance", Values = valuesBalance });

            return totalPercentage;
        }
    }


    public class InstalmentRecord {
        public DateTime Date { get; set; }
        public double TotalPayment { get; set; }
        public double DebtPayment { get; set; }
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