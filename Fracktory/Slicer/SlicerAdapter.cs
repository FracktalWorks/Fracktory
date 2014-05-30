using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fracktory
{
    public class SlicerAdapter
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
        public Double ScaleFactor
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
        public PrintConfiguration Config;
        public Boolean HasFinished
        {
            get;
            set;
        }
        public int ProgressValue
        {
            get;
            set;
        }
        public string ProgressOutput
        {
            get;
            set;
        }
        public SlicerAdapter(String FileName)
        {
            if (FileName == "")
            {
                return;
            }
            ProgressOutput = "";
            ProgressValue = 0;
            HasFinished = false;
            ScaleFactor = 1;
            this.FileName= FileName;
            Process p = new Process();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = @"";
            p.StartInfo.FileName = AssemblyDirectory + @"\Slic3r\slic3r-console.exe";
            p.StartInfo.Arguments = "--info \"" + FileName + "\"";
            p.StartInfo.UseShellExecute = false;
            
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            while (output == "" || output.IndexOf("parts won't fit in your print") != -1)
            {
                if (ScaleFactor < 0.01)
                {
                    ScaleFactor -= 0.001;

                }
                else if (ScaleFactor<0.1)
                {
                    ScaleFactor -= 0.01;

                }
                else
                {
                    ScaleFactor -= 0.1;
                }
                p.StartInfo.Arguments = "--scale "+ ScaleFactor+" --info \"" + FileName + "\"";
                p.Start();
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            parseOutput(output);
        }


        void parseOutput (string OutputText)
        {
            int indexS = 0;
            int indexE = 0;

            #region findXYZ
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
            #endregion

            #region findRest
            OutputText = OutputText.Substring(indexE);
            indexS = indexE = 0;
            indexS = OutputText.IndexOfAny(new char[]{'0','1','2','3','4','5','6','7','8','9'});
            indexE = OutputText.IndexOf('\n');
            NumberOfFacets = int.Parse(OutputText.Substring(indexS, indexE - indexS));

            OutputText = OutputText.Substring(indexE+1);
            indexS = indexE = 0;
            indexS = OutputText.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            indexE = OutputText.IndexOf('\n');
            NumberOfShells = int.Parse(OutputText.Substring(indexS, indexE - indexS));

            OutputText = OutputText.Substring(indexE+1);
            indexS = indexE = 0;
            indexS = OutputText.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            indexE = OutputText.IndexOf('\n');
            Volume = double.Parse(OutputText.Substring(indexS, indexE - indexS));

            OutputText = OutputText.Substring(indexE);
            if (OutputText.IndexOf("no") > 0)
            {
                NeededRepair = false;
            }
            else
            {
                NeededRepair = true;
            }


            #endregion
        }

        public void GenerateGcode()
        {
            if (FileName == "" || Config == null)
            {
                return;
            }
            else
            {
                Process p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WorkingDirectory = @"";
                p.StartInfo.FileName = AssemblyDirectory + @"\Slic3r\slic3r-console.exe";
                p.StartInfo.Arguments = "--load \"" + AssemblyDirectory+@"\Configuration\default.ini" + "\"" + " " +Config.generateExtraConfigurationString() + " \""+FileName +"\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                //string output = p.StandardOutput.ReadToEnd();
                //Console.WriteLine(output);
                string outputOld ="";
                string outputNew ="";
                String allOutput="";

              
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    
                    while (p.HasExited == false)
                    {
                        Thread.Sleep(300);
                        if (p.StandardOutput.ReadLine() == null)
                        {
                            continue;
                        }
                        outputNew = p.StandardOutput.ReadLine();
                        if (outputNew != outputOld)
                        {
                            Console.WriteLine(outputNew);
                            outputOld = outputNew;
                            allOutput += outputOld;
                            if (outputOld.IndexOf("Exporting G-code")>-1)
                            {
                                ProgressOutput = "Exporing G-Code";
                            }
                            else if (outputOld.IndexOf("=>") > -1)
                            {
                                ProgressOutput = outputOld.Substring(3);
                            }
                            else
                            {
                                ProgressOutput = outputOld;
                            }

                            if (allOutput.IndexOf("Filament required") != -1)
                            {
                                ProgressValue = 100;
                            }
                            else if (allOutput.IndexOf("Exporting G-code") != -1)
                            {
                                ProgressValue = 60;
                            }
                            else if (allOutput.IndexOf("Generating horizontal shells") != -1)
                            {
                                ProgressValue = 40;
                            }
                            else if (allOutput.IndexOf(" Preparing infill surfaces") != -1)
                            {
                                ProgressValue = 30;
                            }
                            else if (allOutput.IndexOf(" Detecting solid surfaces") != -1)
                            {
                                ProgressValue = 30;
                            }
                            else if (allOutput.IndexOf("Generating perimeters") != -1)
                            {
                                ProgressValue = 20;
                            }
                            else if (allOutput.IndexOf("Processing triangulated mesh") != -1)
                            {
                                ProgressValue = 20;
                            }
                            
                        }
                    }
                });
                
                p.WaitForExit();
                HasFinished = true;
            }
        }

    }
}
