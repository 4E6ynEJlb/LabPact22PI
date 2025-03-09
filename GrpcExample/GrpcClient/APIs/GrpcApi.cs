using WeatherServer;
using Grpc.Net.Client;
using Grpc.Core;

namespace GrpcClient.APIs
{
    internal class GrpcApi
    {
        private GrpcChannel _channel;
        private WeatherService.WeatherServiceClient _client;
        private WeatherRequest _request;

        internal GrpcApi(string city)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _channel = GrpcChannel.ForAddress("https://localhost:7276", new GrpcChannelOptions { HttpHandler = httpHandler});
            _client = new WeatherService.WeatherServiceClient(_channel); 
            _request = new WeatherRequest { City = city };
        }

        internal async Task<WeatherResponse> GetCurrentWeather(string city)
        {
            return await _client.GetCurrentWeatherAsync(_request);
        }

        internal async Task MonitorWeather(string city)
        {
            using var call = _client.MonitorWeather(_request, deadline: DateTime.UtcNow + TimeSpan.FromSeconds(10));
            try
            {
                await foreach (var weather in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"Погода в городе {weather.City}: {weather.Temperature}°C, {weather.Description} ({weather.Timestamp})");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Stream завершен");
            }
        }

        internal async Task GetMultipleWeather(string city)
        {
            var multiple =  await _client.GetMultipleWeatherAsync(_request);
            foreach (var weather in multiple.Weather)
            {
                Console.WriteLine($"Погода в городе {weather.City}: {weather.Temperature}°C, {weather.Description} ({weather.Timestamp})");
            }
        }

        ~GrpcApi()
        {
            _channel.Dispose();
        }
    }
}
