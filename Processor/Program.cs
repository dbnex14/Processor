using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

//TODO
// Stage3 takes >= 5 min only, currently, I take it all the time
// IF TIME: mock thread testing
// IF TIME: return Tupple<int, string, int>  (threadID, deviceID, counter)
namespace RC.CodingChallenge
{
    class MainClass
    {
        const int _THREADS = 100;
        const string _DEVICE_ID = "HV1";
        const string _FILE_PATH = "/Users/dinob/Desktop/work/ReliableControls/Processor/Processor/HV1-2011-03-07.csv";

        public static void Main(string[] args)
        {
            Run();

            Console.ReadLine();
        }

        public static void Run()
        {
            EventCounter vc = new EventCounter();
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (FileStream fileStream = new FileStream(_FILE_PATH
                                                          , FileMode.Open
                                                          , FileAccess.Read))
            {
                using (TextReader reader = new StreamReader(fileStream))
                {
                    vc.ParseEvents(_DEVICE_ID, (System.IO.StreamReader)reader);
                    Thread[] threads = new Thread[_THREADS];
                    for (int i = 0; i < _THREADS; i++)
                    {
                        int temp = i;
                        threads[temp] = new Thread(() =>
                        {
                            vc.ParseEvents(_DEVICE_ID, (System.IO.StreamReader)reader);
                        });
                        threads[temp].Start();
                    }

                    Console.WriteLine(vc.GetEventCount(_DEVICE_ID));
                }
            }
        }

    }
}
