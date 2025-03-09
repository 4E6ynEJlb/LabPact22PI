using Refit;
using WeatherServer;

namespace GrpcClient.APIs
{
    internal interface IRefitRestApi
    {
        [Get("/Current")]
        internal Task<WeatherResponse> GetCurrentWeather([Query] string city);
        [Get("/Multiple")]
        internal Task<List<WeatherResponse>> GetMultipleWeather([Query] string city);
    }
}
