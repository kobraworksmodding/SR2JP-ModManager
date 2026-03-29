using System;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace SR2JP_Mod_Manager
{
    class ArchiveExtractor
    {
        public static void Process(string archivePath)
        {
            string extractTo = $"{Global.SR2Location}\\mods";
            string[] targetExtensions = { ".XTBL", ".peg_pc", ".chunk_pc", ".lua", ".cts", ".le_strings", ".idx_map", ".xsb", ".xwb", ".anim_pc", ".fxo_pc", ".g_peg_pc" };

            // Folder name for root-level extraction
            string archiveName = Path.GetFileNameWithoutExtension(archivePath);

            using (var archive = ArchiveFactory.OpenArchive(archivePath))
            {
                bool found = false;

                foreach (var entry in archive.Entries)
                {
                    if (entry.IsDirectory)
                        continue;

                    string ext = Path.GetExtension(entry.Key);

                    foreach (var targetExt in targetExtensions)
                    {
                       
                        if (string.Equals(ext, targetExt, StringComparison.OrdinalIgnoreCase))
                        {
                            // Determine folder inside archive
                            string folderPathInArchive = Path.GetDirectoryName(entry.Key)?.Replace('/', Path.DirectorySeparatorChar);
                            bool isRootFile = string.IsNullOrEmpty(folderPathInArchive);

                            // Determine extraction folder
                            string extractPath = isRootFile
                                ? Path.Combine(extractTo, archiveName) 
                                : Path.Combine(extractTo, folderPathInArchive);

                            Directory.CreateDirectory(extractPath);

                            // Extract all files in the same folder (or root)
                            foreach (var e in archive.Entries)
                            {
                                if (e.IsDirectory)
                                    continue;

                                string entryFolder = Path.GetDirectoryName(e.Key)?.Replace('/', Path.DirectorySeparatorChar);
                                bool sameFolder = string.Equals(entryFolder, folderPathInArchive, StringComparison.OrdinalIgnoreCase);

                                if (sameFolder)
                                {
                                    // Build target file path (avoid double folders)
                                    string targetFilePath = Path.Combine(extractPath, Path.GetFileName(e.Key));

                                    using (var entryStream = e.OpenEntryStream())
                                    using (var fileStream = File.Create(targetFilePath))
                                    {
                                        entryStream.CopyTo(fileStream);
                                    }
                                }
                            }

                            Console.WriteLine("Extracted folder: " + (isRootFile ? archiveName : folderPathInArchive));
                            found = true;
                            break; // stop after first matching folder
                        }
                    }

                    if (found)
                        break;
                }

                if (!found)
                {
                    Console.WriteLine("No target files found in archive.");
                }
            }
        }
    }
}