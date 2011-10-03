namespace CronPluginService.Framework.Plugin
{
    public interface IPluginJob
    {
        void Execute(PluginContext context);
    }
}
