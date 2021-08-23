namespace AsanaNet.Interfaces
{
    public interface IAsanaOptions
    {
        string ApiKeyOrBearerToken { get; }
        AuthenticationType AuthType { get; }

        void ErrorCallback(string arg1, string arg2, string arg3, object response);
    }
}