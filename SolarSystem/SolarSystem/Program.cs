using SolarSystem.Backend;

namespace SolarSystem
{
    public class Program
    {
        
        
        public static void Main(string[] args)
        {
            // Picking Path
            // "/Users/Casper/Library/Projects/Uni/P5/wetransfer-f8286e/Picking 02-10-2017.csv"
            Runner runner = new Runner("/Users/kasper/Downloads/wetransfer-f8286e/Picking 02-10-2017.csv", 5, 0.2);
            

            //var runner = new Runner();
            //runner.StartSendingOrders();

             /*var order = OrderHandler.ConstructOrder();
             var handler = new Handler();
             handler.OnOrderBoxFinished += box => Console.WriteLine($"BOX FINISHED: {box}");
             
             TimeKeeper.Tick += () => Console.WriteLine(TimeKeeper.CurrentDateTime);
             
             Console.WriteLine("Start ticking");
             var t = new Thread(() => TimeKeeper.StartTicking(10, DateTime.Now));
             t.Start();
             Console.WriteLine("Listen for completed orders");
             handler.ReceiveOrder(order);*/

        }

        /*static void Main(string[] args) {
            //var t = new Thread(() => CallToChildThread(1000));
            Console.WriteLine("In Main: Creating the Child thread");
            t.Start();
            Console.ReadKey();
            
        }*/
    }
}