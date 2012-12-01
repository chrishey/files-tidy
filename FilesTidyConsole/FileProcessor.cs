using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FilesTidy
{
    public class FileProcessor
    {
        private ConfigSettings Config;
        private EventLog Log;
        private List<string> Errors;
        private List<string> ExcludedFolders;

        public FileProcessor(ConfigSettings config, EventLog log)
        {
            this.Config = config;
            this.Log = log;
            this.Errors = new List<string>();
            this.ExcludedFolders = new List<string>();
        }

        public void Process()
        {
            if (!Directory.Exists(Config.RootFolder))
            {
                this.Log.WriteEntry(string.Format("{0} is not found", Config.RootFolder), EventLogEntryType.Warning);
                return;
            }

            // check files under root folder
            IEnumerable<string> RootFiles = Directory.EnumerateFiles(Config.RootFolder);

            this.CheckFiles(RootFiles, Config.RootFolder);

            string[] splitter = new string[1];
            splitter[0] = ",";
            this.ExcludedFolders = Config.ExcludedFolders.Split(splitter, StringSplitOptions.RemoveEmptyEntries).ToList();

            // check all directories under root folder
            IEnumerable<string> RootDirectories = Directory.EnumerateDirectories(Config.RootFolder);
            foreach (string directory in RootDirectories)
            {
                this.CheckSubDirectories(Config.RootFolder + "\\" + directory);
            }
        }

        public void CheckFiles(IEnumerable<string> files, string directoryPath)
        {
            foreach (string filePath in files)
            {
                string path = directoryPath + "\\" + filePath;
                if (!File.Exists(path))
                {
                    Log.WriteEntry(string.Format("{0} is not found", path), EventLogEntryType.Warning);
                    return;
                }

                DateTime creationTime = File.GetCreationTime(path);
                if (creationTime <= DateTime.Now.AddDays(Config.RemovalInterval))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch
                    {
                        Log.WriteEntry(string.Format("Failed to delete {0}", path), EventLogEntryType.Error);
                        this.Errors.Add(path);
                    }
                    finally
                    {
                        if (this.Errors.Any())
                        {
                            StringBuilder mailBody = new StringBuilder();
                            mailBody.AppendLine("The service failed to delete the following files:");
                            int i = 0;
                            foreach(string error in this.Errors)
                            {
                                if (i == 0)
                                    mailBody.AppendLine(error);
                                else
                                    mailBody.AppendLine("," + error);
                                i++;
                            }

                            EmailHelper.SendEmail("FilesTidy Deletion Failures", mailBody.ToString(), Config.EmailTo, Encoding.Default);
                        }
                    }
                }
            }
        }

        public void CheckSubDirectories(string path)
        {
            int workingDirectoryPosition = path.LastIndexOf("\\");
            if (this.ExcludedFolders.Exists(f => f.ToString() == path.Substring(workingDirectoryPosition, path.Length - workingDirectoryPosition)))
                return;

            IEnumerable<string> files = Directory.EnumerateFiles(path);
            this.CheckFiles(files, path);

            IEnumerable<string> directories = Directory.EnumerateDirectories(path);
            foreach (string directory in directories)
            {
                CheckSubDirectories(path + "\\" + directory);
            }
        }
    }
}
