using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace FilesTidy
{
    public class FilesService
    {
        private Timer timer;
        private FileProcessor processor;
        private ConfigSettings config;
        private const string eventLogSource = "FilesTidy";
        private const string eventLog = "Application";

        public FilesService():this(new ConfigSettings())
        {

        }

        public FilesService(ConfigSettings configSettings)
        {
            this.config = configSettings;
        }

        public void StartService()
        {
            if (!EventLog.SourceExists(eventLogSource))
                EventLog.CreateEventSource(eventLogSource, eventLog);

            EventLog log = new EventLog();
            log.Source = eventLogSource;
            log.Log = eventLog;
            log.WriteEntry("Starting FileTidy service", EventLogEntryType.Information);
            this.processor = new FileProcessor(config, log);
            timer = new Timer(config.ServiceInterval);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
            timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.CheckFiles();
        }

        private void CheckFiles()
        {
            this.timer.Stop();
            this.processor.Process();
            this.timer.Start();
            this.timer.Enabled = true;
        }
    }
}
