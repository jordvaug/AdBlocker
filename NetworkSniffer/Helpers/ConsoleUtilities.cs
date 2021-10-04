using NetworkSniffer.Enums;
using System;
using System.Collections.Generic;

namespace NetworkSniffer.Helpers
{
    /// <summary>
    /// Used to print formatted statements to the console.
    /// </summary>
    public class ConsoleUtilities
    {
        /// <summary>
        /// Resets color and returns cursor to directly below the header messages.
        /// </summary>
        public static void Reset()
        {
            Console.SetCursorPosition(0, 3); //accounts for header messages
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a single alarm message in red.
        /// </summary>
        /// <param name="info"></param>
        public static void WriteAlarm(string info)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            ClearLine();
            Console.WriteLine(info);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a message describing the most visited section.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="times"></param>
        /// <param name="spacing"></param>
        public static void WriteHits(string url, int times, int spacing)
        {
            Console.SetCursorPosition(0, spacing);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            ClearBlock();
            Console.SetCursorPosition(0, spacing+1);
            ClearLine();

            if (times != 0 && !String.IsNullOrEmpty(url))
            {
                Console.WriteLine(String.Format("{0, 5} was the most visited section at: {1,2} times", url, times));
            }
            else
            {
                Console.WriteLine("No sites were visited.");
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Writes a summary of all requests from the last interval.
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="uniqueHosts"></param>
        /// <param name="crud"></param>
        public static void WriteSummary(int requests, int uniqueHosts, List<int> crud)
        {
            ClearBlock();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,20}|{3,20}|{4,20}|{5,20}|", Headers.NumRequests,
                Headers.Hosts, Headers.Gets, Headers.Posts, Headers.Puts, Headers.Deletes));

            Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,20}|{3,20}|{4,20}|{5,20}|", requests, uniqueHosts,
                crud[0], crud[1], crud[2], crud[3]));

            Console.WriteLine("----------------------------------------------------------------------------------" +
                "---------------------------------------------");
            Console.ResetColor();
        }

        /// <summary>
        /// Clears the current line of text.
        /// </summary>
        public static void ClearLine()
        {
            var row = Console.CursorTop;
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, row);
        }

        /// <summary>
        /// Used to clear a block on the console without clearing the whole screen.
        /// </summary>
        public static void ClearBlock()
        {
            var row = Console.CursorTop;
            for (var i = 0; i < 4; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, row);
        }


    }
}
