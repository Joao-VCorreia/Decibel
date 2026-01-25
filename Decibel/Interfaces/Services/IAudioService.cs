namespace Decibel.Interfaces.Services
{
    internal interface IAudioService
    {
        Task SetSystemVolume(int targetVolumePercentage);
    }
}
