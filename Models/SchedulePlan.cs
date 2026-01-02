using System.Collections.ObjectModel;

namespace Decibel.Models
{
    public class SchedulePlan
    {
        public string Name { get; set; } = string.Empty;
        public ObservableCollection<VolumeSchedule> VolumeSchedules { get; set; } = [];
        public ObservableCollection<DayOfWeek> DayOfWeeks { get; set; } = [];

        public SchedulePlan() {}

        public SchedulePlan(string name, ObservableCollection<VolumeSchedule> volumeSchedules, ObservableCollection<DayOfWeek> dayOfWeeks)
        {
            Name = name;
            VolumeSchedules = volumeSchedules;
            DayOfWeeks = dayOfWeeks;
        }
    }
}
