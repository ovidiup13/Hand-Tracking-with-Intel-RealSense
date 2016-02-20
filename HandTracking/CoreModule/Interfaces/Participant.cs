namespace CoreModule.Interfaces
{
    /// <summary>
    ///     Class that represents the participant. Currently only contains the ID but other data can be filled in and output
    ///     to files for inspection.
    /// </summary>
    public class Participant
    {
        private readonly int _id;

        public Participant(int id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return "P" + _id;
        }
    }
}
