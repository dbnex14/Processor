using System;
using System.Collections.Generic;
using System.IO;
//using RC.CodingChallenge;

namespace RC.CodingChallenge
{
    enum FaultConditions 
    { 
        Stage3 = '3', //start validating fault
        Stage2 = '2', //validating...
                      //any number of Stage2 or Stage3 -> validating...
        Stage0 = '0'  //end -> increment counter (Fault confirmed!)
    };

    public class EventCounter : IEventCounter
    {
        /* making assumption based on provided example of CSV file that
         * one file provided readings from one device only.  However, if
         * ever multiple devices' readings are stored in a CSV file, we 
         * could store faults in dictionary for each device.  So leaving 
         * dictionary but a simple integer counter would suffice for the
         * 1:1 relationship. */
        private volatile Dictionary<string, int> mFaults; //deviceId-counter //TODO make volatile
        //private int mCounter;
        private bool found3;
        private bool found2;
        private bool found0;

        public EventCounter()
        {
            mFaults = new Dictionary<string, int>();
        }

        public int GetEventCount(string deviceID)
        {
            if (string.IsNullOrEmpty(deviceID))
            {
                throw new ArgumentException();
            }

            if (mFaults.Count == 0 
                || !mFaults.ContainsKey(deviceID))
            {
                return 0;
            }

            return mFaults[deviceID];
        }

        public void ParseEvents(string deviceID, StreamReader eventLog)
        {
            if (string.IsNullOrEmpty(deviceID) 
                || eventLog == null
                || eventLog.Peek() == -1)
            {
                throw new ArgumentException();
            }

            foreach (int cnt in ProcessLog(deviceID, eventLog))
            {
                //Console.WriteLine(cnt);
                UpdateFaults(deviceID, cnt);
            }
        }

        private IEnumerable<int> ProcessLog(string deviceID, StreamReader eventLog)
        {
            int counter = 0;
            string line3 = ""; // rememeber it so we can parse out date/time
            TextReader reader = TextReader.Synchronized(eventLog);
            //TextReader rd = eventLog;
            //rd.ReadLine();
            //eventLog = (System.IO.StreamReader)StreamReader.Synchronized(eventLog);
            while(!eventLog.EndOfStream)
            {
                lock(this)
                {
                    if (!found3)  // check Stage3
                    {
                        var line = eventLog.ReadLine();
                        if (line[line.Length - 1] == (char)FaultConditions.Stage3)
                        {
                            found3 = true;
                            line3 = line;
                            continue;
                        }
                        else { continue; }
                    }

                    if (!found2)  // check Stage2
                    {
                        var line2 = eventLog.ReadLine();
                        if (line2[line2.Length - 1] == (char)FaultConditions.Stage2)
                        {
                            //if (Stage3OccuredForFiveMinutes(line3, line2))
                            //{
                            //    found2 = true;
                            //    continue;
                            //}
                            //else 
                            //{
                            //    found3 = false;
                            //    line3 = "";
                            //    found2 = false;
                            //    continue;
                            //}
                            found2 = true;
                            continue;
                        }
                        else if (line2[line2.Length - 1] == (char)FaultConditions.Stage3)
                        {
                            found3 = true;
                            line3 = line2;
                            continue;
                        }
                        else
                        {
                            found3 = false;
                            line3 = "";
                            found2 = false;
                            continue;
                        }
                    }

                    if (!found0)  // check Stage0
                    {
                        var line0 = eventLog.ReadLine();
                        if (line0[line0.Length - 1] == (char)FaultConditions.Stage0)
                        {
                            found0 = true;
                            counter++;
                            yield return counter;

                            ClearAllFlags();
                        }
                        else if (line0[line0.Length - 1] == (char)FaultConditions.Stage2)
                        {
                            //if (Stage3OccuredForFiveMinutes(line3, line0))
                            //{
                            //    found2 = true;
                            //    continue;
                            //}
                            //else
                            //{
                            //    found3 = false;
                            //    line3 = "";
                            //    found2 = false;
                            //    continue;
                            //}
                            found2 = true;
                            continue;
                        }
                        else if (line0[line0.Length - 1] == (char)FaultConditions.Stage3)
                        {
                            found3 = true;
                            line3 = line0;
                            continue;
                        }
                        else
                        {
                            ClearAllFlags();
                            continue;
                        }
                    }
                } // end lock
            }
        }

        private bool Stage3OccuredForFiveMinutes(string stage3line, string stage2line)
        {
            char[] delimiter = new char[] { '\t' };
            DateTime time3 = DateTime.Parse(stage3line.Split(delimiter)[0]);
            DateTime time2 = DateTime.Parse(stage2line.Split(delimiter)[0]);
            time3 = time3.AddMinutes(5);
            return time3 <= time2;
        }

        private void UpdateFaults(string deviceID, int counter)
        {
            lock(this)
            {
                if (mFaults.ContainsKey(deviceID))
                {
                    mFaults[deviceID] = counter;
                }
                else
                {
                    mFaults.Add(deviceID, counter);
                }
            }

        }

        private void ClearAllFlags()
        {
            found3 = false;
            found2 = false;
            found0 = false;
            //line3 = ""; //TODO do I need to clear this?
        }

    }
}
