using NAudio.CoreAudioApi;

namespace Decibel.Services
{
    class AudioService
    {
        //Encontrando os dispositivos de audio ativos
        private readonly MMDeviceEnumerator _deviceEnumerator;

        public AudioService()
        {
            _deviceEnumerator = new();
        }

        public async Task SetSystemVolume(int targetVolumePercentage)
        {
            //Garantindo range
            if (targetVolumePercentage < 0 || targetVolumePercentage > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(targetVolumePercentage), "O volume deve ser um valor entra 0 e 100");
            }

            try
            {
                //Ajuste de volume gradual em disposito padrão configurado no sistema
                MMDevice defaultDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                AudioEndpointVolume volumeController = defaultDevice.AudioEndpointVolume;

                int currentVolume = (int)Math.Round(volumeController.MasterVolumeLevelScalar * 100);

                int step = targetVolumePercentage > currentVolume ? 1 : -1;

                while (currentVolume != targetVolumePercentage)
                {
                    currentVolume += step;

                    if ((step == 1 && currentVolume > targetVolumePercentage) ||
                        (step == -1 && currentVolume < targetVolumePercentage))
                    {
                        currentVolume = targetVolumePercentage;
                    }

                    float targetVolumeFloat = (float)currentVolume / 100.0f;
                    volumeController.MasterVolumeLevelScalar = targetVolumeFloat;

                    // Desmuta, caso estivesse mudo
                    volumeController.Mute = false;

                    await Task.Delay(25);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ajustar o volume: {ex.Message}");
                throw;
            }
        }
    }
}
