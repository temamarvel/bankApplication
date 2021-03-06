﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BankApplication {

    //ОКНО ГРАФИКА ПЛАТЕЖЕЙ
    public partial class InstalmentWindow {
        public InstalmentWindow() {
            InitializeComponent();
        }

        public InstalmentWindow(Customer customer) : this() {
            Customer = customer;
            //при открытии окна создаем экземпляр моедли данных
            //которая сгенерирует график платежей 
            DataContext = new InstalmentViewModel(Customer);
        }

        public Customer Customer { get; set; }
    }


    //КЛАСС МОДЕЛИ ДАННЫХ
    //ОТ ОТВЕЧАЕТ ЗА РАСЧЕТ ГРАФИКА ПЛАТЕЖЕЙ ДЛЯ КОНКРЕТНОЙ ЗАПИСИ В БД
    public class InstalmentViewModel {

        public string CustomerName { get; set; }
        public string LoanSum { get; set; }
        public string InterestRate { get; set; }
        public string TotalAnnuityPercentage { get; set; }
        public string TotalDifferentialPercentage { get; set; }

        public ObservableCollection<InstalmentRecord> AnnuityRecords { get; set; }
        public ObservableCollection<InstalmentRecord> DifferentialRecords { get; set; }
        public ObservableCollection<DataSeries> AnnuityData { get; set; }
        public ObservableCollection<DataSeries> DifferentialData { get; set; }

        public InstalmentViewModel(Customer customer)
        {
            //при создании модели, заполняем все поля данными
            CustomerName = $"{customer.Name} {customer.Surname}";
            LoanSum = $"{(double)customer.Value} {customer.Currency}";
            InterestRate = $"{customer.InterestRate.Value}%";

            TotalAnnuityPercentage = $"{CalculateAnnuityPayments(customer)} {customer.Currency}";
            TotalDifferentialPercentage = $"{CalculateDifferentialPayments(customer)} {customer.Currency}";
        }

        //расчет аннуитетных платежей
        double CalculateAnnuityPayments(Customer customer)
        {
            AnnuityRecords = new ObservableCollection<InstalmentRecord>();

            //просто расчет согласно формуле
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
                //на каждом шаге добавляем запись в таблицу
                AnnuityRecords.Add(new InstalmentRecord() { Date = recordDate, TotalPayment = Math.Round(totalPayment, 2), DebtPayment = Math.Round(debtPayment, 2), Percentage = percentage, Balance = Math.Round(balance, 2) });
            }

            //создаем данные для графиков
            AnnuityData = CreateGraphData(AnnuityRecords);

            return Math.Round(totalPercentage, 2);
        }


        //расчет дифференцированных платежей
        double CalculateDifferentialPayments(Customer customer)
        {
            DifferentialRecords = new ObservableCollection<InstalmentRecord>();

            //просто расчет согласно формуле
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

                //на каждом шаге добавляем запись в таблицу
                DifferentialRecords.Add(new InstalmentRecord() { Date = recordDate, TotalPayment = Math.Round(totalPayment, 2), DebtPayment = Math.Round(debtPayment, 2), Percentage = percentage, Balance = Math.Round(balance, 2) });
            }

            //создаем данные для графиков
            DifferentialData = CreateGraphData(DifferentialRecords);

            return Math.Round(totalPercentage, 2);
        }

        ObservableCollection<DataSeries> CreateGraphData(ObservableCollection<InstalmentRecord> records)
        {
            ObservableCollection<DataSeries> data = new ObservableCollection<DataSeries>();

            ObservableCollection<DataPoint> valuesPayment = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in records)
            {
                valuesPayment.Add(new DataPoint(record.Date, record.TotalPayment));
            }

            ObservableCollection<DataPoint> valuesDept = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in records)
            {
                valuesDept.Add(new DataPoint(record.Date, record.DebtPayment));
            }

            ObservableCollection<DataPoint> valuesPercentage = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in records)
            {
                valuesPercentage.Add(new DataPoint(record.Date, record.Percentage));
            }

            ObservableCollection<DataPoint> valuesBalance = new ObservableCollection<DataPoint>();
            foreach (InstalmentRecord record in records)
            {
                valuesBalance.Add(new DataPoint(record.Date, record.Balance));
            }

            data.Add(new DataSeries() { Name = "TotalPayment", Values = valuesPayment });
            data.Add(new DataSeries() { Name = "DebtPayment", Values = valuesDept });
            data.Add(new DataSeries() { Name = "Percentage", Values = valuesPercentage });
            data.Add(new DataSeries() { Name = "Balance", Values = valuesBalance });

            return data;
        }
    }

    //cлужебные классы для отображения данных и графиков
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