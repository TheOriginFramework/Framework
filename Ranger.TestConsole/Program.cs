using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TOF.Framework.Data;
using TOF.Framework.DependencyInjection;

namespace Ranger.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Initialize(builder => builder.RegisterTypesInAssembly(typeof(DbClient).Assembly));
            List<Task> dataReaderTasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                dataReaderTasks.Add(Task.Run(() =>
                    {
                        Console.WriteLine("Thread {0}: Data Load begins.", Task.CurrentId);
                        
                        var repo = new CandidateRepo();

                        try
                        {
                            var candidate = repo.List();
                            Console.WriteLine("Thread {0}: Data Load Completed.", Task.CurrentId);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Thread {0}: Report Error: {1}\r\n{2}", Task.CurrentId, e.Message, e.StackTrace);
                        }

                    }));
            }

            do
            {
                Thread.Sleep(1000);
            }
            while (dataReaderTasks.Where(t => t.Status == TaskStatus.RanToCompletion).Count() == dataReaderTasks.Count());

            Console.WriteLine("Task Completed.");
            Console.Read();
        }
    }
}
