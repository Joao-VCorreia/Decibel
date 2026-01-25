using Decibel.Models;
using Decibel.Models.Results;
using System.Collections.ObjectModel;

namespace Decibel.Interfaces.Services
{
    internal interface ISettingsService
    {
        ActionResult SaveSchedules(ObservableCollection<SchedulePlan> plans);
        ObservableCollection<SchedulePlan> LoadPlans();
    }
}
