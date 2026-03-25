using System.Collections.Generic;
using Models;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    /// <summary>
    /// Test fixture for testing the PlanetDatabase class.
    /// </summary>
    public class PlanetDatabaseTests
    {
        private List<string> logOutput;

        /// <summary>
        /// Sets up the test environment by initializing the log output list and subscribing to the log message event.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Initialize the log output list and subscribe to the log message event
            logOutput = new List<string>();
            Application.logMessageReceived += LogMessageReceived;
        }

        /// <summary>
        /// Cleans up the test environment by unsubscribing from the log message event.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Unsubscribe from the log message event to clean up
            Application.logMessageReceived -= LogMessageReceived;
            logOutput = null;
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            logOutput.Add(condition);
        }

        /// <summary>
        /// Tests that the PrintKeys method outputs all the planet keys to the log.
        /// </summary>
        [Test]
        public void TestPrintKeys()
        {
            // Call the PrintKeys method
            PlanetDatabase.PrintKeys();

            // Check the log output contains all the planet keys
            foreach (var key in PlanetDatabase.Planets.Keys)
            {
                Assert.IsTrue(logOutput.Contains($"Planet Key: {key}"), $"Log output does not contain expected key: {key}");
            }
        }
    }
}