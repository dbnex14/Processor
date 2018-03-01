using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace RC.CodingChallenge
{
    [TestFixture()]
    public class ProcessorTests
    {
        const string _TEST_DEVICE_ID = "ABC";
        const string _EMPTY_FILE = "ProcessorTest.TestFiles.emptyfile.csv";
        const string _MISSING_FILE = "missingfile.csv";
        const string _NO_FAULTS_FILE = "ProcessorTest.TestFiles.nofaults.csv";
        const string _2_FAULTS_FILE = "ProcessorTest.TestFiles.hv1-2faults.csv";

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
            mAssembly = null;
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
            mCounter.ParseEvents(_TEST_DEVICE_ID, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTests_ParseEvents_DeviceIdMissing()
        {
            Stream fileStream = mAssembly.GetManifestResourceStream(_EMPTY_FILE);

            StreamReader reader = new StreamReader(fileStream);
            mCounter.ParseEvents("", reader);
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ProcessorTests_ParseEvents_MissingFile()
        {
            StreamReader reader = new StreamReader(_MISSING_FILE);
        }

        [Test]
        public void ProcessorTests_ParseEvents_FileEmpty()
        {
            // Arrange
            int expected = 0;
            Stream fileStream = mAssembly.GetManifestResourceStream(_EMPTY_FILE);
            StreamReader reader = new StreamReader(fileStream); 

            // Act
            mCounter.ParseEvents(_TEST_DEVICE_ID, reader);
            int actual = mCounter.GetEventCount(_TEST_DEVICE_ID);

            // Assert
            Assert.AreEqual(expected, actual, "not equal");
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

        [Test]
        public void ProcessorTests_GetEventCount_NoFaultsDevice()
        {
            // Arrange
            int expected = 0;

            // Act
            int actual = mCounter.GetEventCount("nofaultsdevice");

            // Assert
            Assert.AreEqual(expected, actual, "not equal");
        }
        #endregion


        #region PROCESSING

        [Test]
        public void ProcessorTests_ParseEvents_NoFaults()
        {
            // Arrange
            int expected = 0;
            Stream fileStream = mAssembly.GetManifestResourceStream(_NO_FAULTS_FILE);
            StreamReader reader = new StreamReader(fileStream);

            // Act
            mCounter.ParseEvents(_TEST_DEVICE_ID, reader);
            int actual = mCounter.GetEventCount(_TEST_DEVICE_ID);


            // Assert
            Assert.AreEqual(expected, actual, "not equal");
        }

        [Test]
        public void ProcessorTests_ParseEvents_2Faults()
        {
            // Arrange
            int expected = 2;
            Stream fileStream = mAssembly.GetManifestResourceStream(_2_FAULTS_FILE);
            StreamReader reader = new StreamReader(fileStream);

            // Act
            mCounter.ParseEvents(_TEST_DEVICE_ID, reader);
            int actual = mCounter.GetEventCount(_TEST_DEVICE_ID);

            // Assert
            Assert.AreEqual(expected, actual, "not equal");
        }

        #endregion
    }
}
