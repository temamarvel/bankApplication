using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using System;
using System.Windows;

namespace BankApplication
{
    public partial class AddWindow : ThemedWindow
    {
        public AddWindow()
        {
            InitializeComponent();
        }

        private void ThemedWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogButtonResult == MessageBoxResult.OK)
            {
                (Owner as MainWindow).Context.Customers.Add(new Customer()
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
            }

            nameEditor.Clear();
            surnameEditor.Clear();
            valueEditor.Clear();
            currencyEditor.Clear();
        }
    }
}
