using Decibel.Models;
using Decibel.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Decibel
{
    public partial class MainWindow : Window
    {
        //Garantir que app deve ser encerrado e não minimizado
        private bool _isAppClosing = false;

        private MainViewModel VM => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }

        //Encerramento implicito para execução em segundo plano
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isAppClosing && VM.CheckUnsavedChanges())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Existem alterações não salvas. Deseja salvar antes de sair?",
                    "Alterações Pendentes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MenuItemSaveChanges_Click(this, new RoutedEventArgs());

                    if (HasValidationErrors())
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (result == MessageBoxResult.No) { }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (!_isAppClosing)
            {
                e.Cancel = true;

                MessageBoxResult result = MessageBox.Show(
                    "O aplicativo continuará executando em segundo plano para monitorar seus agendamentos.",
                    "Execução em Segundo Plano",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information
                    );

                if (result == MessageBoxResult.OK)
                {
                    this.Hide();
                }
            }
        }

        private void MenuItemSaveChanges_Click(object sender, RoutedEventArgs e)
        {

            if (HasValidationErrors())
            {
                MessageBox.Show(
                    "Não foi possível salvar, corrija os agendamentos destacados em vermelho.",
                    "Erro de Validação",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = VM.SaveData();

            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result.Message, "Erro ao Salvar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItemDiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            var result = VM.DiscardChanges();

            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message,
                                "Sucesso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result.Message,
                                "Erro",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void MenuItemMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItemCloseApp_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Tem certeza que deseja encerrar o monitoramento? O ajuste de volume será interrompido.",
                "Confirmação de Encerramento",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _isAppClosing = true;

                VM.EndMonitoring();

                //Encerramento explícito
                Application.Current.Shutdown();
            }
        }


        private void ButtonScheduleAdd_Click(object sender, RoutedEventArgs e)
        {
            var result = VM.AddSchedule();

            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonScheduleRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is VolumeSchedule scheduleToRemove)
            {
                var result = VM.RemoveSchedule(scheduleToRemove);

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Não foi possível identificar o horário.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonPlanAdd_Click(object sender, RoutedEventArgs e)
        {
            VM.AddNewPlan();
        }


        private void ButtonRemovePlan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button bnt && bnt.DataContext is SchedulePlan planToRemove)
            {
                MessageBoxResult resultMessageBox = MessageBox.Show(
                    "Tem certeza que deseja excluir o plano de agendamentos?",
                    "Confirmação de exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultMessageBox == MessageBoxResult.Yes)
                {
                    var result = VM.RemovePlan(planToRemove);

                    if (!result.IsSuccess)
                    {
                        MessageBox.Show(result.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Não foi possível identificar o plano.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool HasValidationErrors()
        {
            if (VM.HasValidationErrors())
            {
                return true;
            }

            // Erros de Formatação
            foreach (var item in ListaHorarios.Items)
            {
                var element = ListaHorarios.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

                if (element != null && Validation.GetHasError(element))
                {
                    return true;
                }
            }

            return false;
        }

    }
}