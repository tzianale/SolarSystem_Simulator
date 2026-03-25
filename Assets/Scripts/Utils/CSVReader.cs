using System.IO;
using UnityEngine;
using System.Collections.Generic;


namespace Utils
{
    /// <summary>
    /// Provides utility methods for reading CSV files
    /// </summary>
    public class CsvReader : MonoBehaviour
    {
        /// <summary>
        /// Reads a CSV file from the specified path and returns the data as a list of lists of strings.
        /// Each inner list represents a row in the CSV file
        /// </summary>
        /// 
        /// <param name="path">The file path to the CSV file</param>
        /// 
        /// <returns>A list of lists of strings, where each inner list represents a row of data from the CSV file</returns>
        public static List<List<string>> ReadCsv(string path)
        {
            var basePath = Path.Combine(Application.streamingAssetsPath, path);
            var table = new List<List<string>>();
            using var reader = new StreamReader(basePath);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line != null)
                {
                    var values = line.Split(';');

                    table.Add(new List<string>(values));
                }

            }

            reader.Close();
            return table;
        }
    }
}