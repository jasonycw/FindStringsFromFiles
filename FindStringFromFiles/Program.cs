using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindStringFromFiles
{
    // Modified from following reference.
    // Reference: http://stackoverflow.com/questions/13993530/better-search-for-a-string-in-all-files-using-c-sharp
    class Program
    {
        static void Main()
        {
            string sourceFolder = @".\searchFolder";
            string searchTexts = @".\searchTexts.txt";
            List<string> result = new List<string>();
            
            // Get all search strings from text file
            if (!File.Exists(searchTexts))
            {
                Console.WriteLine(searchTexts + " is needed to import the search string(s)!");
                Console.ReadKey();
                return;
            }
            var searchList = File.ReadLines(searchTexts);
            Console.WriteLine(searchList.Count() + " search string(s) are imported.");

            // Get all file names from search folder
            if (!Directory.Exists(sourceFolder))
            {
                Console.WriteLine(sourceFolder + " is needed for putting the files!");
                Console.ReadKey();
                return;
            }
            List<string> fileNameList = new List<string>();
            AddFileNamesToList(sourceFolder, fileNameList);
            Console.WriteLine(fileNameList.Count() + " file name(s) are found in " + sourceFolder);

            // Get all contents from search files
            Console.WriteLine("\nStart importing data...");
            List<string> contentList = new List<string>();
            foreach (string fileName in fileNameList)
            {
                Console.WriteLine("\t" + fileName + " is imported");
                string content = File.ReadAllText(fileName);
                contentList.Add(content.ToLower());
            }
            Console.WriteLine("Importing of " + contentList.Count() + " file(s) from searchFolder is done!");
            Console.WriteLine("\nStart searching...");

            // Search each string in all files' content and do stuffs
            foreach (string searchWord in searchList)
            {
                bool found = false;
                for (int i = 0; i < contentList.Count(); i++)
                {
                    if (contentList[i].Contains(searchWord.ToLower()))
                    {
                        found = true;

                        // Add the log to result string
                        result.Add(string.Format("{0,-40}", searchWord) + "\tFound: \t" + fileNameList[i]);

                        string resultsFolder = @".\Results\";
                        if (!Directory.Exists(resultsFolder))
                        {
                            Directory.CreateDirectory(resultsFolder);
                        }
                        File.Copy(fileNameList[i], Path.Combine(resultsFolder, searchWord + "_in_" + Path.GetFileName(fileNameList[i])), true);
                    }
                }
                if (!found)
                {
                    result.Add(String.Format("{0,-40}", searchWord) + "\tNot found.");
                }
            }

            Console.WriteLine("-----   DONE    -----");
            System.IO.File.WriteAllLines(@".\Results.txt", result);
            Process.Start(@".\Results.txt");
        }

        public static void AddFileNamesToList(string sourceDir, List<string> allFiles)
        {

            string[] fileEntries = Directory.GetFiles(sourceDir);
            foreach (string fileName in fileEntries)
            {
                allFiles.Add(fileName);
            }

            //Recursion    
            string[] subdirectoryEntries = Directory.GetDirectories(sourceDir);
            foreach (string item in subdirectoryEntries)
            {
                // Avoid "reparse points"
                if ((File.GetAttributes(item) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    AddFileNamesToList(item, allFiles);
                }
            }

        }
    }
}
