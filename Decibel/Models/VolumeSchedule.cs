using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Decibel.Models
{
    public class VolumeSchedule : INotifyPropertyChanged, IDataErrorInfo
    {
        private int _hour;
        private int _minute;
        private int _volume;

        // Construtor para serialização
        public VolumeSchedule()
        {
        }

        public VolumeSchedule(int hour, int minute, int volume)
        {
            Hour = hour;
            Minute = minute;
            Volume = volume;
        }

        public int Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                OnPropertyChanged(); // alerta
            }
        }

        public int Minute
        {
            get
            {
                return _minute;
            }
            set
            {
                _minute = value;
                OnPropertyChanged();
            }
        }

        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                OnPropertyChanged();
            }
        }

        // Configuracao INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        // [CallerMemberName]
        // O compilador automaticamente preenche o parâmetro 'name' com o NOME da propriedade
        // que chamou (ex: "Hour", "Minute", "Volume")
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            // O Invoke dispara o evento
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Configuração IDataErrorInfo
        [JsonIgnore]
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(Hour):
                        if (Hour < 0 || Hour > 23)
                        {
                            error = "A hora deve ser entre 0 e 23.";
                        }
                        break;

                    case nameof(Minute):
                        if (Minute < 0 || Minute > 59)
                        {
                            error = "O minuto deve ser entre 0 e 59.";
                        }
                        break;

                    case nameof(Volume):
                        if (Volume < 0 || Volume > 100)
                        {
                            error = "O volume deve ser entre 0 e 100.";
                        }
                        break;
                }

                return error;
            }

        }

        public bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(this[nameof(Hour)]))
                    return false;
                if (!string.IsNullOrEmpty(this[nameof(Minute)]))
                    return false;
                if (!string.IsNullOrEmpty(this[nameof(Volume)]))
                    return false;

                return true;
            }
        }

    }
}