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
    class AdmeshAdapter
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
        public static void Rotate(String FileName,double AngleX, double AngleY, double AngleZ)
        {
            //PS C:\Users\Amit Sengupta\Documents\GitHub\Fracktory\Fracktory\Slic3r> .\admesh.exe --z-rotate=10 --x-rotate=50 'C:\Users\Amit Sengupta\Downloads\Extra_Printable_Companion_Cube.STL' -c --write-ascii-stl 'C:\Users\Amit Sengupta\Downloads\oct
            Process p = new Process();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = @"";
            p.StartInfo.FileName = AssemblyDirectory + @"\Slic3r\admesh.exe";
            p.StartInfo.Arguments = "--x-rotate " + AngleX + " --y-rotate " + AngleY + " --z-rotate " + AngleZ + " \"" + FileName + "\"  -c --write-ascii-stl \"" + FileName.Substring(0,FileName.IndexOf('.'))+"_rotated.stl\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
               
        }

    }
}
