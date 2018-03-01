using System;
using System.Collections.Generic;
using System.IO;

namespace RC.CodingChallenge
{
    enum FaultConditions 
    { 
        Stage3 = '3', //start validating fault
        Stage2 = '2', //validating...
                      //fall through Stage2 or Stage3 -> validating...
        Stage0 = '0'  //end -> increment counter (Fault confirmed!)
    };

    public class EventCounter : IEventCounter
    {
        const int _MINUTES = 5;

        /**
         * Making assumption based on provided example of CSV file that one file 
         * provides readings from one and only one device.  
         * However, if ever multiple devices' readings are stored in a CSV file, 
         * we could store faults in a dictionary for each device.  So leaving 
         * dictionary but a simple integer counter would suffice for the 1:1 
         * relationship. 
        */
        private  Dictionary<string, int> mFaults; //deviceId-counter 

        //private int mCounter;
        private  bool found3;
        private  bool found2;
        private  bool found0;

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
                || eventLog == null)
            {
                throw new ArgumentException();
            }

            if (eventLog.EndOfStream)
            {
                return;
            }

            foreach (int cnt in ProcessLog(deviceID, eventLog))
            {
                UpdateFaults(deviceID, cnt);
            }
        }

        /**
         * Needs refactoring, to long, difficult to expand on.
         */
        private IEnumerable<int> ProcessLog(string deviceID, StreamReader eventLog)
        {
            int counter = 0;
            string line3 = ""; // for interval check

            while(!eventLog.EndOfStream)
            {
                lock(this)
                {
                    if (!found3)  // check Stage3
                    {
                        var line = eventLog.ReadLine();
                        if (string.IsNullOrEmpty(line)) yield break;
                        if (line[line.Length - 1] == (char)FaultConditions.Stage3)
                        {
                            found3 = true;
                            line3 = line;
                        }
                        continue;
                    }

                    if (!found2)  // check Stage2
                    {
                        var line2 = eventLog.ReadLine();
                        if (string.IsNullOrEmpty(line2)) yield break;
                        if (line2[line2.Length - 1] == (char)FaultConditions.Stage2)
                        {
                            found2 = true;
                        }
                        else if (line2[line2.Length - 1] == (char)FaultConditions.Stage3)
                        {
                            found3 = true;
                            line3 = line2;
                        }
                        else
                        {
                            found3 = false;
                            line3 = "";
                            found2 = false;
                        }
                        continue;
                    }

                    if (!found0)  // check Stage0
                    {
                        var line0 = eventLog.ReadLine();
                        if (string.IsNullOrEmpty(line0)) yield break;

                        if (line0[line0.Length - 1] == (char)FaultConditions.Stage0)
                        {
                            found0 = true;
                            counter++;
                            yield return counter;

                            ClearAllFlags();
                        }
                        else if (line0[line0.Length - 1] == (char)FaultConditions.Stage2)
                        {
                            found2 = true;
                        }
                        else if (line0[line0.Length - 1] == (char)FaultConditions.Stage3)
                        {
                            found3 = true;
                            line3 = line0;
                        }
                        else
                        {
                            ClearAllFlags();
                        }
                        continue;
                    }
                } // end lock
            }
        }

        private bool Stage3OccuredForFiveMinutes(string stage3line, string stage2line)
        {
            char[] delimiter = new char[] { '\t' };
            DateTime time3 = DateTime.Parse(stage3line.Split(delimiter)[0]);
            DateTime time2 = DateTime.Parse(stage2line.Split(delimiter)[0]);
            time3 = time3.AddMinutes(_MINUTES);
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
        }

    }
}
