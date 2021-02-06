using Pert.Model;

namespace Pert.View
{
    public interface IViewModel
    {
        void OnModelUpdate(Project p);

        bool SetConnectionString(string s);

        bool IsConnected();
    }
}
