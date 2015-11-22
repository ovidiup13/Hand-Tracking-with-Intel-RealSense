using HandTracking.Interfaces.Settings;

namespace HandTracking.Interfaces.Module
{
    public abstract class IModule
    {
        protected ITracking Tracking;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public abstract ITracking GetInstance(ISettings settings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract ITracking GetInstace();
    }
}