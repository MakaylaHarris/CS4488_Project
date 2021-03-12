using SmartPert.Model;

namespace SmartPert.View
{
    public interface IViewModel
    {
        /// <summary>
        /// Called when the database is disconnected
        /// </summary>
        void OnDisconnect();

        /// <summary>
        /// Receives updates from the model
        /// </summary>
        /// <param name="p">Current project</param>
        void OnModelUpdate(Project p);

    }
}
