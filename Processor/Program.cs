using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

//TODO
// Stage3 takes >= 5 min only, currently, I take it all the time
// threading
// testing
// using File.ReadLines uses StreamReader internaly
// return Tupple<int, string, int>  (threadID, deviceID, counter)
namespace RC.CodingChallenge
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            EventCounter vc = new EventCounter();
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream fileStream = assembly.GetManifestResourceStream("Processor.HV1-2011-03-07.csv"))
            {
                var lineCount = File.ReadLines("/Users/dinob/Desktop/work/ReliableControls/Processor/Processor/HV1-2011-03-07.csv").Count();
                Console.WriteLine("Found {0} lines in file.", lineCount);

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    Parallel.For(0, 3, x => vc.ParseEvents("HV1", reader));
                    Console.WriteLine(vc.GetEventCount("HV1"));
                }
            }

            Console.ReadLine();
        }
    }
}
