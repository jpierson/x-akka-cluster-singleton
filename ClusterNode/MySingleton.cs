using Akka.Actor;
using System;

namespace ClusterNode
{
    public class MySingleton : ReceiveActor
    {

        public MySingleton()
        {
            Console.WriteLine("Singleton started");

            ReceiveAny(e =>
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Message received \"{e} \"");
                Console.ForegroundColor = originalColor;
            });
        }
    }
}