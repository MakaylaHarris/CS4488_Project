namespace SmartPert.Model
{
    /// <summary>
    /// Receives updates about the database
    /// </summary>
    public interface DBUpdateReceiver
    {
        bool SaveSettings { get; set; }

        /// <summary>
        /// When the database disconnects
        /// </summary>
        void OnDBDisconnect();

        /// <summary>
        /// When the database is updated
        /// </summary>
        /// <param name="p"></param>
        void OnDBUpdate(Project p);
    }
}
