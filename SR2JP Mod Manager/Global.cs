using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2JP_Mod_Manager
{
    internal class Global
    {
        public static string SR2Location = ""; // "JoeBloggs/Desktop/SaintsRow2FreeDownload" 
        public static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SR2JP_Mod_Manager");
    }
}
