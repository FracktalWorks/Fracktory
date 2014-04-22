using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fracktory
{
    class SlicerAdapter
    {

        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public String FileName
        {
            get;
            set;
        }

        public Double X{get;set;}
        public Double Y
        {
            get;
            set;
        }
        public Double Z
        {
            get;
            set;
        }
        public Double Volume
        {
            get;
            set;
        }
        public int NumberOfFacets
        {
            get;
            set;
        }
        public int NumberOfShells
        {
            get;
            set;
        }
        public Boolean NeededRepair
        {
            get;
            set;
        }
        public SlicerAdapter(String FileName)
        {
            this.FileName= FileName;
            Process p = new Process();
            p.StartInfo.WorkingDirectory = @"";
            p.StartInfo.FileName = AssemblyDirectory + @"\Slic3r\slic3r-console.exe";
            p.StartInfo.Arguments = "--info \"" + FileName + "\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            parseOutput(output);
            Console.WriteLine("Output:");
            Console.WriteLine(output);   
        }

        //size:              x=92.951 y=93.426 z=26.120
        //number of facets:  5546
        //number of shells:  1
        //volume:            22252.453
        //needed repair:     no
        void parseOutput (string OutputText)
        {
            int indexS = 0;
            int indexE = 0;

            while (OutputText[indexS] != '=')
            {
                indexS++;
            }
            indexS++;
            indexE = indexS;
            while (OutputText[indexE] != ' ')
            {
                indexE++;
            }
            X = Double.Parse(OutputText.Substring(indexS, indexE - indexS +1));
            indexS++;
            while (OutputText[indexS] != '=')
            {
                indexS++;
            }
            indexS++;
            indexE = indexS;
            while (OutputText[indexE] != ' ')
            {
                indexE++;
            }
            Y = Double.Parse(OutputText.Substring(indexS, indexE - indexS + 1));
            indexS++;
            while (OutputText[indexS] != '=')
            {
                indexS++;
            }
            indexS++;
            indexE = indexS;
            while (OutputText[indexE] != ' ')
            {
                indexE++;
            }
            Z = Double.Parse(OutputText.Substring(indexS, indexE - indexS + 1));

        }
        //check for errors
 
        //repair


        //slice

    }

public class RedirectingProcessOutput
{
    //public static void Main()
    //{
    //    Process p = new Process();
    //    p.StartInfo.FileName = "cmd.exe";
    //    p.StartInfo.Arguments = "/c dir *.cs";
    //    p.StartInfo.UseShellExecute = false;
    //    p.StartInfo.RedirectStandardOutput = true;
    //    p.Start();

    //    string output = p.StandardOutput.ReadToEnd();
    //    p.WaitForExit();

    //    Console.WriteLine("Output:");
    //    Console.WriteLine(output);    
    //}
}
}
