namespace SignalR_Mvc_BusDemo.Utilities
{
    public static class ConsoleColors
    {
        public static void WriteLine(ConsoleColor color, string message)
        {
            var previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = previous;
        }
    }
}