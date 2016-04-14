using System.Collections.Generic;
using System.IO;
using System.Text;
using AudioModule.Interfaces;
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
        public DataExporter(Participant participant)
        {
            _participant = participant;
        }

        /// <summary>
        /// </summary>
        /// <param name="audioDesign"></param>
        public void CreateConditionFile(string audioDesign)
        {
            //create directory if it does not exist already
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            //create trial data file and stream
            _conditionFile = Directory + "\\" + _participant + "_" + audioDesign + "_" + ".txt";

            _conditionDataStream = !File.Exists(_conditionFile) ? File.Create(_conditionFile) : new FileStream(_conditionFile, FileMode.Create);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioDesign"></param>
        /// <param name="speakerId"></param>
        public void CreateMovementTraceFile(string audioDesign, string speakerId)
        {
            //create directory if it does not exist already
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            //create trial data file and stream
            _traceFile = Directory + "\\" + _participant + "_" + audioDesign + "_" + speakerId + ".txt";

            _traceDataStream = !File.Exists(_traceFile) ? File.Create(_traceFile) : new FileStream(_traceFile, FileMode.Create);
        }

        /// <summary>
        ///     Method that sets the current target data for write to file.
        /// </summary>
        /// <param name="target">Marker ID of the speaker</param>
        /// <param name="closest">The closest speaker</param>
        /// <param name="distance"></param>
        /// <param name="time"></param>
        /// <param name="position"></param>
        public void AppendToConditionFile(string target, string closest, double distance, long time, PXCMPoint3DF32 position)
        {
            //create the line
            var line = target + ", " + closest + ", " + distance + ", " + time + ", " +
                       position.x + ", " + position.y + ", " + position.z + "\n";

            //write to file
            AddText(_conditionDataStream, line);
        }

        /// <summary>
        ///     TODO: append hand location to trace file
        /// </summary>
        /// <param name="time"></param>
        /// <param name="position">hand position in 3D</param>
        public void AppendToTraceFile(long time, PXCMPoint3DF32 position)
        {
            //create the line
            var line = time + ", " + position.x + ", " + position.y + ", " + position.z + "\n";

            //write to file
            AddText(_traceDataStream, line);
        }

        /// <summary>
        ///     Method that returns the current directory for the data files.
        /// </summary>
        /// <returns></returns>
        public static string GetDirectory() 
        {
            return Directory;
        }

        public void CloseConditionStream()
        {
            _conditionDataStream.Dispose();
        }

        public void CloseTraceStream()
        {
            _traceDataStream.Dispose();
        }

        public void CloseStreams()
        {
            _conditionDataStream?.Dispose();
            _traceDataStream?.Dispose();
        }

        /// <summary>
        /// Method that exports the speaker positions. 
        /// </summary>
        /// <param name="participant"></param>
        /// <param name="speakers"></param>
        public void ExportSpeakerPositions(Participant participant, List<Speaker> speakers)
        {
            //create directory if it does not exist already
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            var speakerFile = Directory + "\\" + _participant + "_Speaker_Positions.txt";
            var speakerFileStream = !File.Exists(speakerFile) ? File.Create(speakerFile) : new FileStream(speakerFile, FileMode.Create);

            foreach (var speaker in speakers)
            {
                var line = speaker.Marker.Id + ", " + speaker.Marker.Position3D.x + ", " + speaker.Marker.Position3D.y +
                           ", " + speaker.Marker.Position3D.z + "\n";
                AddText(speakerFileStream, line);
            }

            speakerFileStream.Close();
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
        private FileStream _conditionDataStream;
        private FileStream _traceDataStream;

        //files and directories
        private const string Directory = "data";
        private string _conditionFile, _traceFile;

        #endregion
    }
}