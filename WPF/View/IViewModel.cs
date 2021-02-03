using WPF.Model;

namespace WPF.View
{
    public interface IViewModel
    {
        void OnModelUpdate(Project p);
    }
}
