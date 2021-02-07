namespace Pert.Model
{
    public interface DBUpdateReceiver
    {
        void OnDBUpdate(Project p);
    }
}
