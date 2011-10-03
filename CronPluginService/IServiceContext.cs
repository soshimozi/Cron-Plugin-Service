namespace CronPluginService
{
    public interface IServiceContext
    {
        void Start(string [] args);
        void Stop();
        void Pause();
        void Continue();
    }
}
