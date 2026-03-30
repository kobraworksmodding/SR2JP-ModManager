using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace  SR2JP_Mod_Manager {
    public static class FileChecker
    {
        public static string GetDuplicateFilesSubfolders(string rootFolder)
        {
            if (!Directory.Exists(rootFolder))
                return $"Folder does not exist: {rootFolder}\r\n";

            Dictionary<string, HashSet<string>> fileSubfolders = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (string file in Directory.EnumerateFiles(rootFolder, "*", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(file);
                string relativePath = GetRelativePath(rootFolder, file);
                string[] parts = relativePath.Split(Path.DirectorySeparatorChar);

                string firstLevelFolder = parts.Length > 1 ? parts[0] : "(mods root)";

                if (!fileSubfolders.ContainsKey(fileName))
                    fileSubfolders[fileName] = new HashSet<string>();

                fileSubfolders[fileName].Add(firstLevelFolder);
            }

            StringBuilder sb = new StringBuilder();
            bool anyDuplicates = false;

            foreach (var kvp in fileSubfolders)
            {
                if (kvp.Value.Count > 1)
                {
                    Global.numOfDupes++;
                    anyDuplicates = true;
                    if (!(kvp.Key == "desktop.ini"))
                    {
                        sb.AppendLine("↓ " + kvp.Key + " ↓");
                        sb.AppendLine("----------");
                        sb.AppendLine(string.Join("\n", kvp.Value));
                        sb.AppendLine("----------");
                        sb.AppendLine();
                    }
                }
            }

            if (!anyDuplicates)
                sb.AppendLine("No duplicate files found.");

            return sb.ToString();
        }

        // Helper to get relative path
        private static string GetRelativePath(string root, string fullPath)
        {
            if (!root.EndsWith(Path.DirectorySeparatorChar.ToString()))
                root += Path.DirectorySeparatorChar;

            Uri rootUri = new Uri(root);
            Uri fileUri = new Uri(fullPath);
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}

