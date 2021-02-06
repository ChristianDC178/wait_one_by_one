using System;
using System.Threading.Tasks;

namespace WaitOneByOne.DotNetCoreExample
{
    class Program
    {

        static void Main(string[] args)
        {

            int PROCESSOR_COUNT = Environment.ProcessorCount;

            Task[] tasks = new Task[PROCESSOR_COUNT];
            int wantToExecute = 30, index = 0, executed = 0;
            object lockObject = new object();
            int finished = 0;

            Action action = () =>
            {
                var number = (new Random()).Next(2000, 9000);
                Console.WriteLine($" Thread Id:  {System.Threading.Thread.CurrentThread.ManagedThreadId} -- Milliseconds {number} ");
                System.Threading.Thread.Sleep(number);
                lock (lockObject)
                {
                    finished++;
                    Console.WriteLine($" Thread Id - {System.Threading.Thread.CurrentThread.ManagedThreadId} - Finished {finished} ");

                    if (finished == executed)
                    {
                        Console.WriteLine("Process done !");
                    }
                }
            };

            do
            {
                tasks[index] = Task.Factory.StartNew(action);
                executed++;
                index++;
            } while (index < PROCESSOR_COUNT);

            bool continueLoop = true;
            while (continueLoop)
            {
                int indexFinished = Task.WaitAny(tasks);
                if (indexFinished > -1)
                {
                    tasks[indexFinished] = Task.Factory.StartNew(action);
                    executed++;
                }
                continueLoop = executed < wantToExecute;
            }

            Console.ReadKey();

        }
    }
}
