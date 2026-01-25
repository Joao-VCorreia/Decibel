using Decibel.Interfaces.Services;
using Decibel.Models;
using Decibel.Models.Results;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Decibel.Services
{
    class SettingsService : ISettingsService
    {
        public ActionResult SaveSchedules(ObservableCollection<SchedulePlan> plans)
        {
            EnsureResult ensureResult = EnsureEnvironment();

            if (ensureResult.IsSuccess)
            {
                string filePath = Path.Combine(ensureResult.Path!, "plans.json");

                try
                {
                    string? jsonString = JsonSerializer.Serialize(plans, new JsonSerializerOptions { WriteIndented = true });

                    File.WriteAllText(filePath, jsonString);

                    return ActionResult.Success();
                }
                catch (Exception ex)
                {
                    return ActionResult.Failure($"Erro ao salvar agendamento: {ex.Message}");
                }
            }
            else
            {
                return ActionResult.Failure(ensureResult.Message);
            }
        }


        public ObservableCollection<SchedulePlan> LoadPlans()
        {
            EnsureResult ensureResult = EnsureEnvironment();

            if (!ensureResult.IsSuccess)
            {
                throw new InvalidOperationException(ensureResult.Message);
            }

            string filePath = Path.Combine(ensureResult.Path!, "plans.json");

            if (!File.Exists(filePath))
            {
                return new ObservableCollection<SchedulePlan>();
            }

            try
            {
                string plansString = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(plansString))
                {
                    return new ObservableCollection<SchedulePlan>();
                }

                return JsonSerializer.Deserialize<ObservableCollection<SchedulePlan>>(plansString) ?? new ObservableCollection<SchedulePlan>();
            }
            catch (JsonException)
            {
                // TODO: Implementar recuperação de dados a partir do último backup consistente.

                return new ObservableCollection<SchedulePlan>();
            }


        }

        private EnsureResult EnsureEnvironment()
        {
            try
            {
                string userAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string directoryPath = Path.Combine(userAppDataPath, "Decibel");

                Debug.WriteLine(directoryPath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return EnsureResult.Success(directoryPath);
            }
            catch (Exception ex)
            {
                return EnsureResult.Failure($"Erro ao garantir ambiente: {ex.Message}");
            }
        }
    }
}
