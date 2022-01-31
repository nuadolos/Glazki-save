using Glazki.Utilities;
using Glazki.Entities;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Glazki.UI.Windows;

namespace Glazki.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для AgentView.xaml
    /// </summary>
    public partial class AgentView : Page
    {
        #region Поля страницы AgentView

        Pagination pagination;

        #endregion

        #region Конструктор страницы AgentView

        public AgentView()
        {
            InitializeComponent();

            pagination = new Pagination(Transition.Context.Agent.ToList().Count);

            var allType = Transition.Context.AgentType.ToList();
            allType.Insert(0, new AgentType { Title = "Все типы" });
            AgentTypesCBox.ItemsSource = allType;
            AgentTypesCBox.SelectedIndex = 0;
            SortCBox.SelectedIndex = 0;
        }

        #endregion

        #region Сортировка данных в AgentLView

        private void SortListView()
        {
            var itemsUpdate = Transition.Context.Agent.ToList();

            if (AgentTypesCBox.SelectedIndex > 0)
                itemsUpdate = itemsUpdate
                    .Where(p => p.AgentTypeID == (AgentTypesCBox.SelectedItem as AgentType).ID)
                    .ToList();

            if (SearchTBox.Text != "Введите для поиска" && !string.IsNullOrWhiteSpace(SearchTBox.Text))
                itemsUpdate = itemsUpdate
                    .Where(p => p.Title.ToLower().Contains(SearchTBox.Text.ToLower())
                    || p.Phone.Contains(SearchTBox.Text)
                    || p.Email.ToLower().Contains(SearchTBox.Text.ToLower()))
                    .ToList();

            switch(SortCBox.SelectedIndex)
            {
                case 1:
                    {
                        if (!(bool)DescendingCheck.IsChecked)
                            itemsUpdate = itemsUpdate.OrderBy(p => p.Title).ToList();
                        else
                            itemsUpdate = itemsUpdate.OrderByDescending(p => p.Title).ToList();
                        break;
                    }
                case 2:
                    {
                        if (!(bool)DescendingCheck.IsChecked)
                            itemsUpdate = itemsUpdate.OrderBy(p => p.Discount).ToList();
                        else
                            itemsUpdate = itemsUpdate.OrderByDescending(p => p.Discount).ToList();
                        break;
                    }
                case 3:
                    {
                        if (!(bool)DescendingCheck.IsChecked)
                            itemsUpdate = itemsUpdate.OrderBy(p => p.Priority).ToList();
                        else
                            itemsUpdate = itemsUpdate.OrderByDescending(p => p.Priority).ToList();
                        break;
                    }
            }

            pagination.IsSorted(itemsUpdate.Count);
            pagination.Definition(FirstNumList, SecondNumList, ThirdNumList);

            if (!pagination.HasPreviousPage && !pagination.HasNextPage)
            {
                BackList.Visibility = Visibility.Hidden;
                ForwardList.Visibility = Visibility.Hidden;
            }
            else if (!pagination.HasPreviousPage)
            {
                BackList.Visibility = Visibility.Hidden;
                ForwardList.Visibility = Visibility.Visible;
            }
            else if (!pagination.HasNextPage)
            {
                BackList.Visibility = Visibility.Visible;
                ForwardList.Visibility = Visibility.Hidden;
            }
            else
            {
                BackList.Visibility = Visibility.Visible;
                ForwardList.Visibility = Visibility.Visible;
            }

            AgentLView.ItemsSource = pagination.LimitedList<Agent>(itemsUpdate);
        }

        private void SearchTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTBox.Text != "Введите для поиска" && !string.IsNullOrWhiteSpace(SearchTBox.Text))
            {
                pagination.Zeroing();
                SortListView();
            }
        }

        private void SearchTBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTBox.Text = null;
        }

        private void SearchTBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTBox.Text))
                SearchTBox.Text = "Введите для поиска";
        }

        private void SortCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pagination.Zeroing();
            SortListView();
        }

        private void AgentTypesCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pagination.Zeroing();
            SortListView();
        }

        private void DescendingCheck_Checked(object sender, RoutedEventArgs e)
        {
            pagination.Zeroing();
            SortListView();
        }

        private void DescendingCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            pagination.Zeroing();
            SortListView();
        }

        #endregion

        #region Пагинация (Навигация по страницам)

        private void BackList_Click(object sender, RoutedEventArgs e)
        {
            pagination.GoBack();
            SortListView();
        }

        private void FirstNumList_Click(object sender, RoutedEventArgs e)
        {
            pagination.NumPage = int.Parse((sender as Button).Content.ToString()) - 1;
            pagination.GetIndex();
            SortListView();
        }

        private void SecondNumList_Click(object sender, RoutedEventArgs e)
        {
            pagination.NumPage = int.Parse((sender as Button).Content.ToString()) - 1;
            pagination.GetIndex();
            SortListView();
        }

        private void ThirdNumList_Click(object sender, RoutedEventArgs e)
        {
            pagination.NumPage = int.Parse((sender as Button).Content.ToString()) - 1;
            pagination.GetIndex();
            SortListView();
        }

        private void ForwardList_Click(object sender, RoutedEventArgs e)
        {
            pagination.GoForward();
            SortListView();
        }

        #endregion

        #region Переход на страницу AddEditAgent

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Transition.MainFrame.Navigate(new AddEditAgent(null));
        }

        private void AgentLView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AgentLView.SelectedItem is Agent tempAg)
                Transition.MainFrame.Navigate(new AddEditAgent(tempAg));
        }

        #endregion

        #region Открытие модального окна для изменения приоритета выделенных элементов

        private void UpdatePriorityBtn_Click(object sender, RoutedEventArgs e)
        {
            List<Agent> lAgents = AgentLView.SelectedItems.Cast<Agent>().ToList();

            ChangePriority changePrior = new ChangePriority(lAgents);

            if (changePrior.ShowDialog() == true)
            {
                SortListView();
            }
        }

        #endregion

        #region Обновление AgentLView после добавления/редактирования элементов

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                SortListView();
            }
        }

        #endregion
    }
}
