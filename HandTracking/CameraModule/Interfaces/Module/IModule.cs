using CameraModule.Interfaces.Settings;

namespace CameraModule.Interfaces.Module
{
    public interface IModule
    {
        /// <summary>
        /// Method that returns a tracking instance.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Tracking GetInstance(ISettings settings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Tracking GetInstance();
    }
}