using System.IO;

namespace SolarSystem.Backend.Classes.Simulation
{
    public static class Outputter
    {
        private static readonly StreamWriter File = new StreamWriter(@"C:\output.csv");
        public static void WriteLineToFile(string s)
        {
                File.WriteLine(s);
        }
    }
}
