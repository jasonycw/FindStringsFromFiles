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
            // Put all the files that needed to be searched here
            string search_files_folder = @".\searchFolder";
            // Put all the string that needed to be find here, 1 line == 1 string
            string search_texts_file = @".\searchTexts.txt";

            // Founded files will be copied here
            string resultsFolder = @".\Results\";
            // Result will be logged here
            string resultsTxt = @".\Results.txt";
            List<string> result = new List<string>();
            
            // Get all search strings from the text file
            if (!File.Exists(search_texts_file))
            {
                Console.WriteLine(search_texts_file + " is needed to import the search string(s)!");
                Console.ReadKey();
                return;
            }
            var searchList = File.ReadLines(search_texts_file);
            Console.WriteLine(searchList.Count() + " search string(s) are imported.");

            // Get all file names from the search folder
            if (!Directory.Exists(search_files_folder))
            {
                Console.WriteLine(search_files_folder + " is needed for putting the files!");
                Console.ReadKey();
                return;
            }
            List<string> fileNameList = new List<string>();
            AddFileNamesToList(search_files_folder, fileNameList);
            Console.WriteLine(fileNameList.Count() + " file name(s) are found in " + search_files_folder);

            // Get all contents from the search files
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

            // Search each string in all files' content and do some stuffs
            if (!Directory.Exists(resultsFolder))
            {
                Directory.CreateDirectory(resultsFolder);
            }
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

                        // Copy the file to result folder
                        File.Copy(fileNameList[i], Path.Combine(resultsFolder, searchWord + "_in_" + Path.GetFileName(fileNameList[i])), true);
                    }
                }
                if (!found)
                {
                    result.Add(String.Format("{0,-40}", searchWord) + "\tNot found.");
                }
            }

            // Save the result file and open it
            System.IO.File.WriteAllLines(resultsTxt, result);
            Process.Start(resultsTxt);
            Console.WriteLine("-----   DONE    -----");
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
