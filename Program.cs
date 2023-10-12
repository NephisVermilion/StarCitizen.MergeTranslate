using System;
using System.Collections.Generic;
using System.IO;

namespace StarCitizen.MergeTranslate
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentTranslationFilePath = null;
            string newTranslationFilePath = null;
            string destinationFilePath = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-a":
                        currentTranslationFilePath = args[++i];
                        break;
                    case "-n":
                        newTranslationFilePath = args[++i];
                        break;
                    case "-d":
                        destinationFilePath = args[++i];
                        break;
                    default:
                        Console.WriteLine($"Unrecognized option: {args[i]}");
                        PrintUsage();
                        return;
                }
            }

            if (currentTranslationFilePath == null || newTranslationFilePath == null || destinationFilePath == null)
            {
                PrintUsage();
                return;
            }

            var currentTranslations = ParseTranslationFile(currentTranslationFilePath);
            var newTranslations = ParseTranslationFile(newTranslationFilePath);

            var cpt = 0;

            // Merge translations
            foreach (var entry in newTranslations)
            {
                if (currentTranslations.ContainsKey(entry.Key))
                {
                    newTranslations[entry.Key] = entry.Value;
                    cpt++;
                }

            }
            Console.WriteLine($"Number of changes: {cpt}");


            // Save merged translations to the destination file
            SaveMergedTranslations(destinationFilePath, newTranslations);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: TranslationMerger -a <current translation file> -n <new translation file> -d <destination file>");
        }

        private static Dictionary<string, string> ParseTranslationFile(string filePath)
        {
            var translations = new Dictionary<string, string>();

            foreach (var line in File.ReadLines(filePath))
            {
                if (!line.Contains("=") || line.TrimStart().StartsWith(";"))
                {
                    continue;
                }

                var split = line.Split('=', 2);
                var key = split[0].Trim();
                var value = split.Length > 1 ? split[1].Trim() : string.Empty;

                translations[key] = value;
            }

            return translations;
        }

        private static void SaveMergedTranslations(string destinationPath, Dictionary<string, string> translations)
        {
            using var writer = new StreamWriter(destinationPath);

            foreach (var entry in translations)
            {
                writer.WriteLine($"{entry.Key}={entry.Value}");
            }
        }
    }
}
