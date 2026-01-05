using Decibel.Helpers;
using Decibel.Models;
using Decibel.Models.Results;
using Decibel.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace Decibel.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        // Servicos
        private readonly SettingsService _settingsService = new();
        private readonly AudioService _audioService = new();
        private readonly MonitorService _monitorService;

        // Propriedade do comando
        public ICommand ToggleDayCommand { get; }

        // Propriedades dos dados
        private ObservableCollection<SchedulePlan> _availablePlans;
        private SchedulePlan? _selectedPlan;

        public ObservableCollection<SchedulePlan> AvailablePlans
        {
            get => _availablePlans;
            set { _availablePlans = value; OnPropertyChanged(); }
        }

        public SchedulePlan? SelectedPlan
        {
            get => _selectedPlan;
            set { _selectedPlan = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            ToggleDayCommand = new RelayCommand(ExecuteToggleDay);

            AvailablePlans = _settingsService.LoadPlans();
            SelectedPlan = AvailablePlans.FirstOrDefault();

            _monitorService = new(_audioService, _settingsService);
            _monitorService.StartMonitoring();
        }

        private void ExecuteToggleDay(object parameter)
        {
            if (_selectedPlan == null) return;

            if (parameter is DayOfWeek day)
            {
                if (SelectedPlan.DayOfWeeks.Contains(day))
                {
                    SelectedPlan.DayOfWeeks.Remove(day); //Unckeck
                }
                else
                {
                    SelectedPlan.DayOfWeeks.Add(day); //Check
                }
            }
        }

        // Implementacao INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public void AddNewPlan()
        {
            int cont = 1;

            while (AvailablePlans.Any(plan => plan.Name == $"PLANO {cont}"))
            {
                cont++;
            }

            string planName = $"PLANO {cont}";

            var newPlan = new SchedulePlan(planName, [], []);
            AvailablePlans.Add(newPlan);

            SelectedPlan = newPlan;
        }

        public ActionResult RemovePlan(SchedulePlan planToRemove)
        {
            bool wasRemoved = AvailablePlans.Remove(planToRemove);

            if (wasRemoved)
            {
                if (SelectedPlan == planToRemove)
                {
                    SelectedPlan = AvailablePlans.FirstOrDefault();
                }

                return ActionResult.Success("Plano removido com sucesso.");
            }
            else
            {
                return ActionResult.Failure("O plano não foi encontrado na lista.");
            }
        }


        public ActionResult SaveData()
        {
            var result = _settingsService.SaveSchedules(AvailablePlans);

            if (result.IsSuccess)
            {
                _monitorService.ReloadPlans();
            }

            return result;

        }

        public ActionResult DiscardChanges()
        {
            try
            {
                AvailablePlans = _settingsService.LoadPlans();
                SelectedPlan = AvailablePlans.FirstOrDefault();

                return ActionResult.Success("Alterações descartadas com sucesso.");
            }
            catch (Exception ex)
            {
                return ActionResult.Failure($"Erro ao descartar alterações: {ex.Message}");
            }
        }

        public bool CheckUnsavedChanges()
        {
            var savedPlans = _settingsService.LoadPlans();

            string currentJson = JsonSerializer.Serialize(AvailablePlans);
            string savedJson = JsonSerializer.Serialize(savedPlans);

            return currentJson != savedJson;
        }

        public bool HasValidationErrors()
        {
            if (SelectedPlan == null) return false;

            return SelectedPlan.VolumeSchedules.Any(s => !s.IsValid);
        }

        public void EndMonitoring()
        {
            _monitorService.StopMonitoring();
            _monitorService.Dispose();
        }

        public ActionResult AddSchedule()
        {
            if (SelectedPlan == null)
            {
                return ActionResult.Failure("Nenhum plano selecionado.");
            }

            //Limitação de agendamentos por plano
            if (SelectedPlan.VolumeSchedules.Count < 12)
            {
                SelectedPlan.VolumeSchedules.Add(new VolumeSchedule(12, 0, 50));
                return ActionResult.Success("Agendamento adicionado com sucesso.");
            }
            else
            {
                return ActionResult.Failure("Limite de agendamentos excedido");
            }
        }

        public ActionResult RemoveSchedule(VolumeSchedule scheduleToRemove)
        {
            if (SelectedPlan == null)
            {
                return ActionResult.Failure("Nenhum plano selecionado.");
            }

            if (scheduleToRemove == null)
            {
                return ActionResult.Failure("Horário inválido");
            }

            if (SelectedPlan.VolumeSchedules.Contains(scheduleToRemove))
            {
                SelectedPlan.VolumeSchedules.Remove(scheduleToRemove);
                return ActionResult.Success("Horário removido com sucesso.");
            }
            else
            {
                return ActionResult.Failure("Não foi possível encontrar esse horário no plano selecionado.");
            }

        }


    }
}
