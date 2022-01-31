using Glazki.Entities;
using Glazki.Utilities;
using Glazki.UI.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Glazki.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditAgent.xaml
    /// </summary>
    public partial class AddEditAgent : Page
    {
        #region Поля страницы AddEditAgent

        private Agent createAgent;
        private string pathProject = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"..\..\UI"));

        #endregion

        #region Конструктор страницы AddEditAgent

        public AddEditAgent(Agent transferAgent)
        {
            InitializeComponent();

            createAgent = transferAgent ?? new Agent();
            this.Title = transferAgent == null ? "Добавление агента" : "Редактирование агента";

            if (transferAgent != null)
            {
                if (!string.IsNullOrWhiteSpace(transferAgent.Logo))
                    ImageAgent.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(pathProject + transferAgent.Logo);

                DeleteAgentBtn.Visibility = Visibility.Visible;
                ProductSalesGrid.Visibility = Visibility.Visible;

                ProductSalesLV.ItemsSource = Transition.Context.ProductSale
                    .Where(p => p.AgentID == transferAgent.ID)
                    .ToList();
            }

            TypeCBox.ItemsSource = Transition.Context.AgentType.ToList();

            DataContext = createAgent;
        }

        #endregion

        #region Сохранение и удаление данных о агенте

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (string.IsNullOrWhiteSpace(createAgent.Title))
                error.AppendLine("Укажите наименование");
            if (createAgent.AgentType == null)
                error.AppendLine("Выберите тип агента");
            if (!uint.TryParse(createAgent.Priority.ToString(), out _))
                error.AppendLine("Некорректно указан приоритет");
            if (string.IsNullOrWhiteSpace(createAgent.INN))
                error.AppendLine("Укажите ИНН");
            if (string.IsNullOrWhiteSpace(createAgent.KPP))
                error.AppendLine("Укажите КПП");
            if (string.IsNullOrWhiteSpace(createAgent.DirectorName))
                error.AppendLine("Укажите директора");
            if (string.IsNullOrWhiteSpace(createAgent.Phone))
                error.AppendLine("Укажите телефон");
            if (string.IsNullOrWhiteSpace(createAgent.Email))
                error.AppendLine("Укажите email");
            if (string.IsNullOrWhiteSpace(createAgent.Address))
                error.AppendLine("Укажите адрес");

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}",
                    "Сохранение агента", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (createAgent.ID == 0)
                Transition.Context.Agent.Add(createAgent);

            try
            {
                Transition.Context.SaveChanges();
                MessageBox.Show($"Данные успешно сохранены",
                    "Сохранение агента", MessageBoxButton.OK, MessageBoxImage.Information);
                Transition.MainFrame.GoBack();
            }
            catch (ApplicationException er)
            {
                MessageBox.Show($"При сохранении агента возникла ошибка:\n{er.Message}",
                    "Сохранение агента", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteAgentBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (ProductSale prodSale in Transition.Context.ProductSale.ToList())
            {
                if (prodSale.AgentID == createAgent.ID)
                {
                    MessageBox.Show($"У агента присутствует история реализации продукции",
                        "Удаление агента", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (MessageBox.Show($"Вы хотите удалить агента \"{createAgent.Title}\"",
                    "Удаление агента", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Transition.Context.Agent.Remove(createAgent);
                    Transition.Context.SaveChanges();
                    MessageBox.Show($"Данные успешно удалены",
                        "Удаление агента", MessageBoxButton.OK, MessageBoxImage.Information);
                    Transition.MainFrame.GoBack();
                }
                catch (ApplicationException er)
                {
                    MessageBox.Show($"При удалении агента возникла ошибка:\n{er.Message}",
                        "Удаление агента", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Загрузка фотографии

        private void DownloadImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.png;*.jpg)|*.png;*jpg";
            ofd.InitialDirectory = pathProject + "\\agents";

            if (ofd.ShowDialog() == true)
            {
                if (!File.Exists(pathProject + "\\agents\\" + ofd.SafeFileName))
                    File.Copy(ofd.FileName, pathProject + "\\agents\\" + ofd.SafeFileName);
               
                createAgent.Logo = $@"\agents\{ofd.SafeFileName}";
                ImageAgent.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(pathProject + $@"\agents\{ofd.SafeFileName}");
            }
        }

        #endregion

        #region Добавление и удаление продукции, реализованной агентом

        private void AddProductSales_Click(object sender, RoutedEventArgs e)
        {
            AddProductSales addProdSale = new AddProductSales(createAgent.ID);

            if (addProdSale.ShowDialog() == true)
            {
                ProductSalesLV.ItemsSource = Transition.Context.ProductSale
                    .Where(p => p.AgentID == createAgent.ID)
                    .ToList();
            }    
        }

        private void DeleteProductSales_Click(object sender, RoutedEventArgs e)
        {
            var deleteDataProdSale = ProductSalesLV.SelectedItems.Cast<ProductSale>().ToList();

            if (deleteDataProdSale.Count != 0)
            {
                if (MessageBox.Show($"Вы хотите удалить {deleteDataProdSale.Count} элементов?",
                    "Удаление реализацию продукта", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Transition.Context.ProductSale.RemoveRange(deleteDataProdSale);
                        Transition.Context.SaveChanges();
                        Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                        ProductSalesLV.ItemsSource = Transition.Context.ProductSale
                            .Where(p => p.AgentID == createAgent.ID)
                            .ToList();
                        MessageBox.Show("Данные успешно удалены",
                            "Удаление реализацию продукта", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (ApplicationException er)
                    {
                        MessageBox.Show($"При удалении данных возникла ошибка:\n{er.Message}",
                            "Удаление реализацию продукта", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }    
            }
            else
                MessageBox.Show("Выберите хотя бы один элемент для удаления",
                        "Удаление реализацию продукта", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion
    }
}
