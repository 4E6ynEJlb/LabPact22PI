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
Console.WriteLine("Запрос с использованием repeated");
await grpcApi.GetMultipleWeather(city);
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
float grpcAvg = ticksSum / 10000 / 10000.0f;

IRefitRestApi refitRestApi = RestService.For<IRefitRestApi>("https://localhost:7276/api/Weather");
ticksSum = 0;
stopwatch = new Stopwatch();
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

Console.WriteLine($"Среднее время выполнения запроса:\n\tREST: {ticksSum / 10000 / 10000.0f} мс\n\tgrpc: {grpcAvg} мс");
Console.ReadLine();