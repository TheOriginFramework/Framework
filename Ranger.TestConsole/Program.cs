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
            int count = 60000;

            //for (int i = 0; i < count; i++)
            //{
            //    dataReaderTasks.Add(new Task(() =>
            //        {
            //            Console.WriteLine("Thread {0}: Data Load begins.", Task.CurrentId);
                        
            //            var repo = new CandidateRepo();

            //            try
            //            {
            //                var candidate = repo.List();
            //                Console.WriteLine("Thread {0}: Data Load Completed.", Task.CurrentId);
            //            }
            //            catch (Exception e)
            //            {
            //                Console.WriteLine("Thread {0}: Report Error: {1}\r\n{2}", Task.CurrentId, e.Message, e.StackTrace);
            //            }

            //        }));
            //}

            // fire tasks.
            var result = Parallel.For(1, count, i =>
                {
                    Console.WriteLine("Thread {0}: Data Load begins.", i);

                    var repo = new CandidateRepo();

                    try
                    {
                        var candidate = repo.List();
                        Console.WriteLine("Thread {0}: Data Load Completed.", i);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Thread {0}: Report Error: {1}\r\n{2}", i, e.Message, e.StackTrace);
                    }
                });
            
            Console.WriteLine("Task Completed.");
            Console.Read();
        }
    }
}
