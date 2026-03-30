using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SharpCompress.Archives;

namespace SR2JP_Mod_Manager
{
    public struct ExtractedFolder
    {
        public string FolderPath;   // Path relative to "mods" folder (normalized)
        public string ExtractPath;  // Full path on disk

        public ExtractedFolder(string folderPath, string extractPath)
        {
            FolderPath = folderPath;
            ExtractPath = extractPath;
        }

        public override string ToString() => FolderPath;
    }

    class ArchiveExtractor
    {
        private static string GetRelativeToMods(string fullPath, string modsRoot)
        {
            fullPath = Path.GetFullPath(fullPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            modsRoot = Path.GetFullPath(modsRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (!fullPath.StartsWith(modsRoot, StringComparison.OrdinalIgnoreCase))
            {
                // fallback: just return folder name prefixed with "mods"
                return Global.NormalizePathFmt(Path.Combine("mods", Path.GetFileName(fullPath)));
            }

            string relative = fullPath.Substring(modsRoot.Length);
            if (relative.StartsWith(Path.DirectorySeparatorChar.ToString()) || relative.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
                relative = relative.Substring(1);

            // Normalize slashes
            relative = Global.NormalizePathFmt(relative);

            return Global.NormalizePathFmt(Path.Combine("mods", relative));
        }

        public static List<ExtractedFolder> Process(string archivePath)
        {
            string modsRoot = Path.Combine(Global.SR2Location, "mods");
            string[] targetExtensions = {
                ".XTBL", ".peg_pc", ".chunk_pc", ".lua", ".cts",
                ".le_strings", ".idx_map", ".xsb", ".xwb",
                ".anim_pc", ".fxo_pc", ".g_peg_pc", ".smesh_pc", ".cmesh_pc"
            };

            var extractedFolders = new List<ExtractedFolder>();

            using (var archive = ArchiveFactory.OpenArchive(archivePath))
            {
                // Find all folders containing target files
                var foldersWithTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var entry in archive.Entries)
                {
                    if (entry.IsDirectory) continue;

                    string ext = Path.GetExtension(entry.Key);
                    if (!targetExtensions.Any(t => string.Equals(t, ext, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    string folderInArchive = Path.GetDirectoryName(entry.Key) ?? "";
                    folderInArchive = Global.NormalizePathFmt(folderInArchive);
                    foldersWithTargets.Add(folderInArchive);
                }

                if (foldersWithTargets.Count == 0)
                {
                    Console.WriteLine("No target files found in archive.");
                    return extractedFolders;
                }

                foreach (var folder in foldersWithTargets)
                {
                    // Compute full extraction path inside mods
                    string fullExtractPath = Path.Combine(modsRoot, folder.Replace('/', Path.DirectorySeparatorChar));
                    Directory.CreateDirectory(fullExtractPath);

                    foreach (var entry in archive.Entries)
                    {
                        if (entry.IsDirectory) continue;

                        string entryFolder = Path.GetDirectoryName(entry.Key) ?? "";
                        entryFolder = Global.NormalizePathFmt(entryFolder);

                        if (!string.Equals(entryFolder, folder, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string targetFilePath = Path.Combine(fullExtractPath, Path.GetFileName(entry.Key));

                        using (var entryStream = entry.OpenEntryStream())
                        using (var fileStream = File.Create(targetFilePath))
                        {
                            entryStream.CopyTo(fileStream);
                        }
                    }

                    string relativeFolder = GetRelativeToMods(fullExtractPath, modsRoot);
                    extractedFolders.Add(new ExtractedFolder(relativeFolder, fullExtractPath));
                }
            }

            Console.WriteLine("Folders containing target files:");
            foreach (var f in extractedFolders)
            {
                Console.WriteLine(f);
            }

            return extractedFolders;
        }
    }
}