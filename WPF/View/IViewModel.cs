using WPF.Model;

namespace WPF.View
{
    public interface IViewModel
    {
        void OnModelUpdate(Project p);

        bool SetConnectionString(string s);

        bool IsConnected();
    }
}
