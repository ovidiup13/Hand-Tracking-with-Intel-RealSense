using System.Windows;

namespace HandTracking
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /*
        App.xaml.cs extends the Application class, which is a central class in a WPF Windows application. 
        .NET will go to this class for starting instructions and then start the desired Window or Page from there. 
        This is also the place to subscribe to important application events, like application start, unhandled exceptions and so on.
        */


        //TODO: check if Intel RealSense Camera is connected to device
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            //create a new main menu window
            var menuWindow = new MenuWindow {Title = "Main Menu"};
            menuWindow.Show();
//            var drives = DriveInfo.GetDrives().Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);
        }
    }
}