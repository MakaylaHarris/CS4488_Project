using Pert.Model;

namespace Pert.View
{
    public interface IViewModel
    {
        /// <summary>
        /// Receives updates from the model
        /// </summary>
        /// <param name="p">Current project</param>
        void OnModelUpdate(Project p);

    }
}
