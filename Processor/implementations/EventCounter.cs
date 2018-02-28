using System;
using System.IO;
//using RC.CodingChallenge;

namespace RC.CodingChallenge
{
    public class EventCounter : IEventCounter
    {
        public EventCounter()
        {
        }

        public int GetEventCount(string deviceID)
        {
            if (string.IsNullOrEmpty(deviceID))
            {
                throw new ArgumentException();
            }

            return 0;
        }

        public void ParseEvents(string deviceID, StreamReader eventLog)
        {
            if (string.IsNullOrEmpty(deviceID) 
                || eventLog == null
                || eventLog.EndOfStream)
            {
                throw new ArgumentException();
            }


        }
    }
}
