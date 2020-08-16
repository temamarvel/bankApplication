using System.Data.Entity;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core;

namespace BankApplication {
     public enum Currency
    {
        RUB,
        EUR,
        USD,
    }


    //ГЛАВНОЕ ОКНО ПРОГРАММЫ
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
            //создаем и открываем окно добавления записи в бд
            AddWindow addWindow = new AddWindow() { Title = "Add new customer", Owner = this };
            var result = addWindow.ShowDialog(MessageBoxButton.OKCancel);

            //если запись успешно добавлена
            if (result == MessageBoxResult.OK)
            {
                //обновляем таблицу отображающую данные из бд
                viewSource.View.Refresh();
                //сохраняем изменения
                Context.SaveChanges();
                //выводим сообщения для пользователя о том что запись успешно добавлена
                ThemedMessageBox.Show(title: "Success", text: "New customer was added successfully!", icon: MessageBoxImage.Information, messageBoxButtons: MessageBoxButton.OK);
            }
            else
            {
                //если что-то пошло не так. говорим об этом пользователю
                ThemedMessageBox.Show(title: "Something went wrong!", text: "New customer wasn't added successfully!", icon: MessageBoxImage.Error, messageBoxButtons: MessageBoxButton.OK);
            }

            
        }

        void DeleteLoanButton_OnClick(object sender, RoutedEventArgs e)
        {
            //удаляем запись из бд
            Context.Customers.Remove(grid.SelectedItem as Customer);
            //сохраняем изменения в бд
            Context.SaveChanges();
            //обновляем таблицу отображающую данные из бд
            viewSource.View.Refresh();
        }


        //показываем окно с графиком платежей или по двойному клику на строчку в таблице
        //или по кнопке Add
        void Grid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowDetailsWindow();
        }

        void ShowDetailsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowDetailsWindow();
        }

        void ShowDetailsWindow()
        {
            //показываем окно с графиком платежей
            new InstalmentWindow((Customer)grid.SelectedItem).ShowDialog();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //на старте приложения забираем данные из бд
            viewSource = (CollectionViewSource)this.FindResource("customerViewSource");
            Context.Customers.Load();
            //обновляем передаем эти данные в таблицу для отображения
            viewSource.Source = Context.Customers.Local;
        }

        
    }
}