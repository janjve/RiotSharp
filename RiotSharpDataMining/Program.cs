using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharpDataMining.Constants;
using RiotSharpDataMining.DataCollection;

namespace RiotSharpDataMining
{
    public class Program
    {
        public static void Main()
        {
            StartCheck();
            DataRetreiver.Run2();

            Console.WriteLine("Press enter to close.");
            Console.ReadLine();
        }

        private static void StartCheck()
        {
            // Check to avoid unintented starts.
            string command;
            do
            {
                Console.WriteLine("Type \"go\" to begin collecting data from Riots servers.");
                command = Console.ReadLine();
            } while (!"go".Equals(command));
        }
    }
}
