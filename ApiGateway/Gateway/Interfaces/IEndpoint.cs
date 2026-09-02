namespace Gateway.Interfaces
{
    public interface IEndpoint
    {
        Task<string> HttpGetRequestAsync(string serverURL, string apiEndpoint);
    }
}
