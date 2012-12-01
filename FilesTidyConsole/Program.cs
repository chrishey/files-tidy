using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
using FilesTidy;

namespace FilesTidyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            FilesService service = new FilesService();
            service.StartService();
        }
    }
}
