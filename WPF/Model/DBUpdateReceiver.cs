namespace SmartPert.Model
{
    public interface DBUpdateReceiver
    {
        void OnDBUpdate(Project p);
    }
}
