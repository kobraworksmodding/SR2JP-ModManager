using System;
using System.IO;

namespace SR2JP_Mod_Manager
{
    internal class Global
    {
        public static string SR2Location = ""; // "JoeBloggs/Desktop/SaintsRow2FreeDownload" 
        public static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SR2JP_Mod_Manager");
        public static bool bAddToList = false;
        public static int numOfDupes = 0;

        public static string NormalizePathFmt(string path)
        {
            return path
                .Replace('\\', '/')
                .Trim()
                .ToLower();
        }
    }
}
