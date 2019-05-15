using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WSY.Experiments.SubwayExperiment
{
    class Program
    {
        private const int maxTransfer = 4;
        private static Dictionary<string, List<string>> stationList = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> lineList = new Dictionary<string, List<string>>();
        private static Dictionary<string, int[]> allResults = new Dictionary<string, int[]>();

        static void Main(string[] args)
        {
            Console.Write("Input source filename: ");
            string srcFileName = Console.ReadLine();
            Console.Write("Input destination filename: ");
            string dstFileName = Console.ReadLine();
            initialize(srcFileName);
            calculate();
            writeResult(dstFileName);
        }

        private static void writeResult(string dstFileName)
        {
            int stationCount = stationList.Count;
            using (StreamWriter writer = new StreamWriter(dstFileName, false, Encoding.UTF8))
            {
                writer.Write("StationName");
                for (int i = 0; i < maxTransfer; i++)
                {
                    writer.Write(',');
                    writer.Write(i);
                    writer.Write("Transfer");
                }
                writer.WriteLine();
                foreach (KeyValuePair<string, int[]> result in allResults)
                {
                    writer.Write(result.Key);
                    foreach (int count in result.Value)
                    {
                        writer.Write(',');
                        writer.Write(count);
                    }
                    writer.WriteLine();
                }
            }
        }

        private static void calculate()
        {
            foreach (string stationName in stationList.Keys)
            {
                IEnumerable<string> data = new List<string>() { stationName };
                for (int i = 0; i < maxTransfer; i++)
                {
                    data = calculateStation(data).ToHashSet();
                    allResults[stationName][i] = data.Count();
                }
                Console.WriteLine("{0}\tcalculated!", stationName);
            }
            Console.WriteLine("Calculation finished! {0} stations calculated!", stationList.Count);
        }

        private static IEnumerable<string> calculateStation(IEnumerable<string> stationNameList)
        {
            List<string> result = new List<string>();
            foreach (string stationName in stationNameList)
            {
                result.AddRange(calculateOneStation(stationName));
            }
            return result;
        }

        private static IEnumerable<string> calculateOneStation(string stationName)
        {
            List<string> result = new List<string>();
            stationList[stationName].ForEach(lineName => result.AddRange(lineList[lineName]));
            return result;
        }

        private static void initialize(string filename)
        {
            foreach (string lineString in File.ReadLines(filename))
            {
                string[] lineData = lineString.Split('\t');
                string lineName = lineData[0];
                if (!lineList.ContainsKey(lineName))
                {
                    lineList.Add(lineName, new List<string>());
                }
                foreach (string stationName in lineData[1].Split(','))
                {
                    if (!stationList.ContainsKey(stationName))
                    {
                        stationList.Add(stationName, new List<string>());
                    }
                    stationList[stationName].Add(lineName);
                    lineList[lineName].Add(stationName);
                    if (!allResults.ContainsKey(stationName))
                    {
                        allResults[stationName] = new int[maxTransfer];
                    }
                }
            }
            Console.WriteLine("Initialized!");
        }
    }
}

