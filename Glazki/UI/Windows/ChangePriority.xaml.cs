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
    /// Логика взаимодействия для ChangePriority.xaml
    /// </summary>
    public partial class ChangePriority : Window
    {
        #region Поля окна ChangePriority

        AgentPriorityHistory agentPrH;
        List<Agent> lAgents;
        int tempPrior = 0;

        #endregion

        #region Конструктор окна ChangePriority

        public ChangePriority(List<Agent> transferListAg)
        {
            InitializeComponent();

            foreach (Agent ag in transferListAg)
            {
                if (tempPrior < ag.Priority)
                    tempPrior = ag.Priority;
            }
            PriorityTBox.Text = tempPrior.ToString();

            lAgents = transferListAg;
        }

        #endregion

        #region Изменение и добавление записи в историю приоритета

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PriorityTBox.Text, out tempPrior))
            {
                foreach (Agent ag in lAgents)
                {
                    ag.Priority = tempPrior;

                    agentPrH = new AgentPriorityHistory()
                    {
                        AgentID = ag.ID,
                        ChangeDate = DateTime.Now.Date,
                        PriorityValue = tempPrior
                    };
                    Transition.Context.AgentPriorityHistory.Add(agentPrH);
                }

                try
                {
                    Transition.Context.SaveChanges();
                    Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                    MessageBox.Show($"Приотет успешно изменен на {tempPrior}",
                        "Изменение приоритета", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    this.Close();
                }
                catch (ApplicationException er)
                {
                    MessageBox.Show($"При изменении приоритета возникла ошибка:\n{er.Message}",
                        "Изменение приоритета", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("В поле для ввода должно быть число", 
                    "Изменение приоритета", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        #region Отмена изменения приоритета

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
