using System;
using System.IO;
using System.Windows.Forms;

namespace SR2JP_Mod_Manager
{
    internal class Startup
    {
        public static bool bIsJuicedInstalled = true;
        public static void CheckForJuicedPatch()
        {
            string[] CommonFiles = { "juiced.ini", "loose.txt" };

            foreach (var fileName in CommonFiles)
            {
                string fullPath = Path.Combine(Global.SR2Location, fileName);

                if (File.Exists(fullPath))
                {
                    //MessageBox.Show($"{fileName} exists.");
                }
                else
                {
                    if (bIsJuicedInstalled == true)
                    {
                        MessageBox.Show("It seems that Juiced Patch isn't installed, or hasn't been installed correctly." +
                            "\n\nThis is okay for now but be warned that none of the mods you manage currently" +
                            "\nwill likely be loaded into your game." +
                            "\n\nPlease install the latest version of Juiced Patch if you want to use your mods.", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bIsJuicedInstalled = false;
                    }
                }
            }
        }
    }
}
