using HandTracking.Interfaces.Settings;

namespace HandTracking.Interfaces.Module
{
    public abstract class IModule
    {
        protected Tracking Tracking;

        /// <summary>
        /// Method that returns a tracking instance.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public abstract Tracking GetInstance(ISettings settings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Tracking GetInstance();
    }
}