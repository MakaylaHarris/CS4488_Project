using WPF.Model;

namespace WPF.View
{
    public interface IViewModel
    {
        /// <summary>
        /// Receives updates from the model
        /// </summary>
        /// <param name="p">Current project</param>
        void OnModelUpdate(Project p);

        /// <summary>
        /// Attempts to open the new database connection
        /// </summary>
        /// <param name="s">connection string consisting of key-value pairs separated by semicolons</param>
        /// <returns></returns>
        bool SetConnectionString(string s);

        /// <summary>
        /// Tests if database is connected
        /// </summary>
        /// <returns>true if connected</returns>
        bool IsConnected();

        /// <summary>
        /// Refreshes the database
        /// </summary>
        void Refresh();
    }
}
