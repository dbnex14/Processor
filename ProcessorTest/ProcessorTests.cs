using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace RC.CodingChallenge
{
    [TestFixture()]
    public class ProcessorTests
    {
        IEventCounter mCounter;
        Assembly mAssembly;

        [SetUp]
        public void Setup()
        {
            mCounter = new EventCounter();
            mAssembly = Assembly.GetExecutingAssembly();
        }

        [TearDown]
        public void Teardown()
        {
            mCounter = null;
        }

        #region INIT TESTS

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTests_ParseEvents_MisingArguments()
        {
            mCounter.ParseEvents("", null);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTests_ParseEvents_StreamReaderMissing()
        {
            mCounter.ParseEvents("ABC", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTests_ParseEvents_DeviceIdMissing()
        {
            Stream fileStream = mAssembly.GetManifestResourceStream("ProcessorTest.TestFiles.emptyfile.csv");

            StreamReader reader = new StreamReader(fileStream);
            mCounter.ParseEvents("", reader);
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ProcessorTests_ParseEvents_MissingFile()
        {
            StreamReader reader = new StreamReader("missingfile.csv");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTests_ParseEvents_FileEmpty()
        {
            Stream fileStream = mAssembly.GetManifestResourceStream("ProcessorTest.TestFiles.emptyfile.csv");

            StreamReader reader = new StreamReader(fileStream);
            mCounter.ParseEvents("ABC", reader);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTests_GetEventCount_DeviceIdEmptyString()
        {
            mCounter.GetEventCount("");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTests_GetEventCount_DeviceIdNull()
        {
            mCounter.GetEventCount(null);
        }
        #endregion


    }
}
