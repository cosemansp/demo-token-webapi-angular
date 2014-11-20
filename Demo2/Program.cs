using System;
using Microsoft.Owin.Hosting;

namespace Demo2
{
    public class Config
    {
        public static string SecretkeyJwt { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Config.SecretkeyJwt = "g7jI^U2+;0R89$V";

            using (WebApp.Start<Startup>("http://localhost:9000"))
            {
                Console.WriteLine("Listening on port: 9000");
                Console.WriteLine("Press Enter to quit");
                Console.ReadLine();
            }
        }
    }
}
