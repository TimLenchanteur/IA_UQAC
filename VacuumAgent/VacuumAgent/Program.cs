using System;
using System.Threading;

namespace VacuumAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Create two the two thread here but wait for start agent thread in environment thread
            Thread agent = new Thread(new ThreadStart(Vacuum.VaccumProc));
            Thread environment = new Thread(Environment.EnvironmentProc);

            environment.Start(agent);

            // Wait for both thread to join back main thread
            agent.Join();
            environment.Join();
        }
    }
}
