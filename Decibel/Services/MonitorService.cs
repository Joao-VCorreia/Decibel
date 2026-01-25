using Decibel.Interfaces.Services;
using Decibel.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Decibel.Services
{
    class MonitorService : IDisposable
    {
        private readonly IAudioService _audioService;
        private readonly ISettingsService _settingsService;
        private readonly Timer _timer;
        private ObservableCollection<SchedulePlan> _plans;

        private bool _isMonitoring = false;
        public bool IsMonitoring => _isMonitoring;

        private DateTime _lastProcessdTime;

        public MonitorService(IAudioService audioService, ISettingsService settingsService)
        {
            _audioService = audioService;
            _settingsService = settingsService;

            _plans = _settingsService.LoadPlans();

            _timer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds); // Intervalo para checagem
            _timer.Elapsed += OnTimerElapsed;
        }

        public void StartMonitoring()
        {
            if (!_isMonitoring)
            {
                _timer.Start();
                _isMonitoring = true;
            }
        }

        public void StopMonitoring()
        {
            if (_isMonitoring)
            {
                _timer.Stop();
                _isMonitoring = false;
            }
        }

        // Garante que novos dados sejam carregados no monitor
        public void ReloadPlans()
        {
            _plans = _settingsService.LoadPlans();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        //Procura correspondencia da hora atual nos agendamentos
        private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;

            // Evita prisão de volume
            if (_lastProcessdTime.Date == now.Date &&
                _lastProcessdTime.Hour == now.Hour &&
                _lastProcessdTime.Minute == now.Minute)
            {
                return;
            }

            DayOfWeek currentDay = now.DayOfWeek;
            int currentHour = now.Hour;
            int currentMinute = now.Minute;

            var activePlans = _plans.Where(plan => plan.DayOfWeeks.Contains(currentDay));

            foreach (var plan in activePlans)
            {
                VolumeSchedule? targetSchedule = plan.VolumeSchedules
                    .FirstOrDefault(s => s.Hour == currentHour && s.Minute == currentMinute);

                if (targetSchedule != null)
                {
                    try
                    {
                        await _audioService.SetSystemVolume(targetSchedule.Volume);

                        _lastProcessdTime = now;

                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erro crítico ao ajustar volume: {ex.Message}");
                    }
                }
            }
        }
    }
}
