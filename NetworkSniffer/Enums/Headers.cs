namespace NetworkSniffer.Enums
{
    /// <summary>
    /// Used to format print statements to the console.
    /// </summary>
    class Headers
    {
        public static string NumRequests { get { return "Number of Requests"; } }
        public static string Hosts { get { return "Unique Hosts"; } }
        public static string Gets { get { return "Get Requests"; } }
        public static string Posts { get { return "Post Requests"; } }
        public static string Puts { get { return "Put Requests"; } }
        public static string Deletes { get { return "Delete Requests"; } }
    }
}
