using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace FilesTidy
{
    public class ConfigSettings
    {
        public string RootFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["RootFolder"];
            }
        }

        public string ExcludedFolders
        {
            get
            {
                return ConfigurationManager.AppSettings["ExcludedFolders"];
            }
        }

        public double RemovalInterval
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["RemovalInterval"]);
            }
        }

        public double ServiceInterval
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["ServiceInterval"]);
            }
        }

        public string EmailTo
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailTo"];
            }
        }
    }
}
