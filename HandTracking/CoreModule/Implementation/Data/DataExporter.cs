using System.IO;
using System.Text;
using CoreModule.Interfaces;

namespace CoreModule.Implementation.Data
{
    /// <summary>
    ///     Class that exports trial data to appropriate text files to be used for analysis.
    /// </summary>
    internal class DataExporter
    {
        /// <summary>
        ///     Constructor that creates a new data exporter.
        /// </summary>
        public DataExporter(Participant participant, string audioDesign)
        {
            _participant = participant;
            CreateFiles(audioDesign);
        }

        /// <summary>
        /// </summary>
        /// <param name="audioDesign"></param>
        private void CreateFiles(string audioDesign)
        {
            //create directory if it does not exist already
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            //create trial data file and stream
            _trialData = _directory + "\\" + _participant + "_" + audioDesign + "_" + ".txt";

            _trialDataStream = !File.Exists(_trialData) ? File.Create(_trialData) : new FileStream(_trialData, FileMode.Create);

            //TODO: create trace file
        }

        /// <summary>
        ///     Method that sets the current target data for write to file.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="closest"></param>
        /// <param name="distance"></param>
        /// <param name="time"></param>
        /// <param name="position"></param>
        public void SetTrialData(string target, string closest, double distance, long time, PXCMPoint3DF32 position)
        {
            //create the line
            var line = target + ", " + closest + ", " + distance + ", " + time + ", " +
                       position.x + ", " + position.y + ", " + position.z + "\n";

            //write to file
            AddText(_trialDataStream, line);
        }

        /// <summary>
        ///     TODO: append hand location to trace file
        /// </summary>
        /// <param name="speaker">id of the speaker</param>
        /// <param name="position">hand position in 3D</param>
        public void UpdateLocation(int speaker, PXCMPoint3DF32 position)
        {
            //append position to trace file
        }

        /// <summary>
        ///     Method that sets the current directory for the data files.
        /// </summary>
        /// <param name="path"></param>
        public static void SetDirectory(string path)
        {
            _directory = path;
        }

        /// <summary>
        ///     Method that returns the current directory for the data files.
        /// </summary>
        /// <returns></returns>
        public static string GetDirectory()
        {
            return _directory;
        }

        public void CloseStream()
        {
            _trialDataStream?.Dispose();
            _traceDataStream?.Dispose();
        }

        /// <summary>
        ///     Method that adds text to a specific file stream.
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="value"></param>
        private static void AddText(FileStream fs, string value)
        {
            var info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        #region vars

        private readonly Participant _participant;

        //data streams
        private FileStream _trialDataStream;
        private FileStream _traceDataStream;

        //files and directories
        private static string _directory = "data";
        private string _trialData, _traceData;

        #endregion
    }
}