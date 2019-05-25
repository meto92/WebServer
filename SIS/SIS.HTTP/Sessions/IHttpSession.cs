namespace SIS.HTTP.Sessions
{
    public interface IHttpSession
    {
        string Id { get; }

        object GetParameter(string name);

        T GetParameter<T>(string name);

        bool ContainsParameter(string name);

        void AddParameter(string name, object parameter);

        void ClearParameters();
    }
}