namespace DiscordAutoliker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите токен");
            string token = Console.ReadLine();
            Console.WriteLine("Введите id канала");
            string channelId = Console.ReadLine();
            Console.WriteLine("Введите id пользователя");
            string userId = Console.ReadLine();
            Console.WriteLine("Введите задержку в миллисекундах");
            int intervalMs = int.Parse(Console.ReadLine());
            Autoliker autoliker = new Autoliker(token ?? throw new ArgumentNullException("token"), channelId ?? throw new ArgumentNullException("channelId"), userId ?? throw new ArgumentNullException("userId"), intervalMs);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            autoliker.RunAsync(cancellationTokenSource.Token);
            Console.WriteLine("Для выхода нажмите \"Enter\"");
            Console.ReadLine();
            cancellationTokenSource.Cancel();
        }
    }
}
