using MadMilkman.Ini;
using System;
using System.IO;

namespace SR2JP_Mod_Manager
{
    internal class INIManager
    {
        private static readonly string IniPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Global.appDataPath}\\options.ini");

        private static void EnsureIniExists()
        {
            if (!File.Exists(IniPath))
            {
                IniFile ini = new IniFile();
                ini.Save(IniPath);
            }
        }

        public static void SetValue(string section, string key, string keyValue)
        {
            EnsureIniExists();

            IniFile ini = new IniFile();
            ini.Load(IniPath);

            IniSection iniSection;

            if (ini.Sections.Contains(section))
                iniSection = ini.Sections[section];
            else
                iniSection = ini.Sections.Add(section);

            if (iniSection.Keys.Contains(key))
                iniSection.Keys[key].Value = keyValue;
            else
                iniSection.Keys.Add(key, keyValue);

            ini.Save(IniPath);
        }

        public static string GetValue(string section, string key, string keyValue = "")
        {
            EnsureIniExists();

            IniFile ini = new IniFile();
            ini.Load(IniPath);

            if (!ini.Sections.Contains(section))
                return keyValue;

            IniSection iniSection = ini.Sections[section];

            if (!iniSection.Keys.Contains(key))
                return keyValue;

            return iniSection.Keys[key].Value;
        }
        public static bool ValueExists(string section, string key)
        {
            EnsureIniExists();

            IniFile ini = new IniFile();
            ini.Load(IniPath);

            if (!ini.Sections.Contains(section))
                return false;

            IniSection iniSection = ini.Sections[section];

            return iniSection.Keys.Contains(key);
        }
    }
}
