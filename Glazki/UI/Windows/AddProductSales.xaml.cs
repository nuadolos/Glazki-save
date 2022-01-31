using Glazki.Entities;
using Glazki.Utilities;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Glazki.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddProductSales.xaml
    /// </summary>
    public partial class AddProductSales : Window
    {
        #region Поля окна AddProductSales

        private ProductSale newProdSale;

        #endregion

        #region Конструктор окна AddProductSales

        public AddProductSales(int idAgent)
        {
            InitializeComponent();

            ProductsCBox.ItemsSource = Transition.Context.Product.ToList();

            newProdSale = new ProductSale() { AgentID = idAgent};
            DataContext = newProdSale;
        }

        #endregion

        #region Сохранение реализации продукта

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (newProdSale.Product == null)
                error.AppendLine("Выберите продукт");
            if (!DateTime.TryParse(newProdSale.SaleDate.ToString(), out _))
                error.AppendLine("Корректно укажите дату");
            if (!uint.TryParse(newProdSale.ProductCount.ToString(), out _))
                error.AppendLine("Корректно укажите количество");

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соответствуют следующим критериям:\n{error}", 
                    "Реализация продукта", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Transition.Context.ProductSale.Add(newProdSale);

            try
            {
                Transition.Context.SaveChanges();
                Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                MessageBox.Show("Данные успешно сохранены",
                    "Реализация продукта", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                this.Close();
            }
            catch (ApplicationException er)
            {
                MessageBox.Show($"При сохранении данных возникла ошибка:\n{er.Message}",
                    "Реализация продукта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Отмена добавления реализации продукта

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
