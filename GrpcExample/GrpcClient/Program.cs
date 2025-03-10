using GrpcClient.APIs;
using Refit;
using System.Diagnostics;
using WeatherServer;

Stopwatch stopwatch = new Stopwatch();
stopwatch.Stop();
stopwatch.Reset();
string city = "Москва";
GrpcApi grpcApi = new GrpcApi(city);
long ticksSum = 0;

Console.WriteLine("Запрос с использованием stream:");
await grpcApi.MonitorWeather(city);
Console.WriteLine("Для замера скорости нажмите Enter");
Console.ReadLine();

for (int counter = 0; counter < 10005; counter++)
{
    stopwatch.Start();
    WeatherResponse weatherResponse = await grpcApi.GetCurrentWeather(city);
    stopwatch.Stop();
    if(counter > 4)//первые 5 запросов используются для разогрева
        ticksSum += stopwatch.ElapsedTicks;
    stopwatch.Reset();
    Console.WriteLine($"Замер скорости для grpc: {counter}/10005");
}
float grpcSum = ticksSum / 10000.0f;

IRefitRestApi refitRestApi = RestService.For<IRefitRestApi>("https://localhost:7276/api/Weather");
ticksSum = 0;
for (int counter = 0; counter < 10005; counter++)
{
    stopwatch.Start();
    WeatherResponse weatherResponse = await refitRestApi.GetCurrentWeather(city);
    stopwatch.Stop();
    if (counter > 4)//первые 5 запросов используются для разогрева
        ticksSum += stopwatch.ElapsedTicks;
    stopwatch.Reset();
    Console.WriteLine($"Замер скорости для REST: {counter}/10005");
}

Console.WriteLine($"Общее время выполнения запроса:\n\tREST: {ticksSum / 10000.0f} мс\n\tgrpc: {grpcSum} мс");

stopwatch.Start();
List<WeatherResponse> weatherResponseGrpc = await grpcApi.GetMultipleWeather(city);
stopwatch.Stop();
Console.WriteLine($"Замер скорости получения массива для grpc: {stopwatch.ElapsedTicks / 10000.0f} мс");
stopwatch.Reset();

stopwatch.Start();
List<WeatherResponse> weatherResponseRest = await refitRestApi.GetMultipleWeather(city);
stopwatch.Stop();
Console.WriteLine($"Замер скорости получения массива для REST: {stopwatch.ElapsedTicks / 10000.0f} мс");
stopwatch.Reset();

Console.ReadLine();