using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Fracktory
{
    public static class GHelper
    {
        public static string SetUnitsToMM()
        {
            return "G21";
        }
        public static string SetUnitsToInch()
        {
            return "G20";
        }
        public static string SetToAbsolutePositioning()
        {
            return "G90";
        }
        public static string SetToRelativePositioning()
        {
            return "G91";
        }
        public static string SetPosition(double X = -1, double Y = -1, double Z = -1, double E = -1)
        {
            string returnValue = "";
            if (X != -1)
            {
                returnValue += " X" + X.ToString();
            }
            if (Y != -1)
            {
                returnValue += " Y" + Y.ToString();
            }
            if (Z != -1)
            {
                returnValue += " Z" + Z.ToString();
            }
            if (E != -1)
            {
                returnValue += " E" + E.ToString();
            }
            return "G92" + returnValue;
        }
        public static string Move(double X = -1, double Y = -1, double Z = -1, double E = -1, double F = -1)
        {
            string returnValue = "";
            if (X != -1)
            {
                returnValue += " X" + X.ToString();
            }
            if (Y != -1)
            {
                returnValue += " Y" + Y.ToString();
            }
            if (Z != -1)
            {
                returnValue += " Z" + Z.ToString();
            }
            if (E != -1)
            {
                returnValue += " E" + E.ToString();
            }
            if (F != -1)
            {
                returnValue += " F" + F.ToString();
            }

            return "G1" + returnValue;
        }
        public static string MoveToOrigin(double X = 0, double Y = 0)
        {
            return "G28 X" + X.ToString() + " Y" + Y.ToString();
        }
        public static string FanOff()
        {
            return "M106 S0";
        }
        public static string FanOn(int speed)
        {
            return "M106 S" + speed.ToString();
        }
        public static string SetBedTemparature(double Temparature)
        {
            return "M140 S" + Temparature.ToString();
        }
        public static string SetBedTemparatureAndWait(double Temparature)
        {
            return "M190 S" + Temparature.ToString();
        }
        public static string SetExtruderTemparature(double Temparature)
        {
            return "M104 S" + Temparature.ToString();
        }
        public static string SetExtruderTemparatureAndWait(double Temparature)
        {
            return "M109 S" + Temparature.ToString();
        }
        public static string DisableMotors()
        {
            return "M84";
        }
        public static string EmergencyStop()
        {
            return "M112";
        }
        public static string GetCurrentPosition()
        {
            return "M114";
        }
        public static string GetTemparature()
        {
            return "M105";
        }

        #region SD_Support

        //sd support
        //M20: List SD card
        //Example: M20
        //All files in the root folder of the SD card are listed to the serial port. This results in a line like:
        //ok Files: {SQUARE.G,SQCOM.G,}
        //The trailing comma is optional. Note that file names are returned in upper case, but - when sent to the M23 command (below) they must be in lower case. This seems to be a function of the SD software. Go figure...
        public static string SD_ListSDCard()
        {
            return "M20";
        }

        //M21: Initialize SD card

        //Example: M21

        //The SD card is initialized. If an SD card is loaded when the machine is switched on, this will happen by default. SD card must be initialized for the other SD functions to work.

        public static string SD_InitializeSDCard()
        {
            return "M21";
        }
        //M22: Release SD card

        //Example: M22

        //SD card is released and can be physically removed.
        public static string SD_ReleaseSDCard()
        {
            return "M22";
        }
        //M23: Select SD file

        //Example: M23 filename.gco

        //The file specified as filename.gco (8.3 naming convention is supported) is selected ready for printing.
        public static string SD_SelectSDFile(string FileName)
        {
            return "M23 " + FileName.ToLower();
        }

        //M24: Start/resume SD print

        //Example: M24

        //The machine prints from the file selected with the M23 command.

        public static string SD_StartSDPrint()
        {
            return "M24";
        }
        //M25: Pause SD print

        //Example: M25

        //The machine pause printing at the current position within the file selected with the M23 command.

        public static string SD_PauseSDPrint()
        {
            return "M25";
        }


        //M26: Set SD position

        //Example: M26

        //Set SD position in bytes (M26 S12345).

        public static string SD_SetSDPosition(int bytes)
        {
            return "M26 S" + bytes.ToString();
        }

        //M27: Report SD print status

        //Example: M27
        //Report SD print status.
        public static string SD_ReportSDPrintStatus()
        {
            return "M27";
        }


        //M28: Begin write to SD card

        //Example: M28 filename.gco

        //File specified by filename.gco is created (or overwritten if it exists) on the SD card and all subsequent commands sent to the machine are written to that file.

        public static string SD_WriteToSD(string FileName)
        {
            return "M28 " + FileName.ToString();
        }
        //M29: Stop writing to SD card

        //Example: M29 filename.gco

        //File opened by M28 command is closed, and all subsequent commands sent to the machine are executed as normal.

        public static string SD_StopWritingToSD(string FileName)
        {
            return "M29 " + FileName.ToString();
        }
        //M30: Delete a file on the SD card

        //Example: M30 filename.gco

        //filename.gco is deleted. 
        public static string SD_DdeleteFileFromSD(string FileName)
        {
            return "M30 " + FileName.ToString();
        }

        #endregion
    }
    //public static class GParser
    //{
    //    public static Point3D? getPoint(String GCode)
    //    {
    //        double X=-1;
    //        double Y=-1;
    //        double Z=-1;
    //        double E=-1;
    //        var TokenList = GCode.Split(' ').ToList<string>();
    //        var CommentFlag = TokenList.IndexOf(";");
    //        var ParanthesisFlag = TokenList.IndexOf("(");
    //        var ClosedParanthesisFlag = TokenList.IndexOf(")");
    //        int G1Flag = TokenList.IndexOf("G1");
    //        Dictionary<string, double> TokenDictionary = new Dictionary<string, double>();
    //        foreach (var Token in TokenList)
    //        {
    //            if (Token.Count() > 0 && G1Flag >=0 && (CommentFlag < 0 || TokenList.IndexOf(Token) < CommentFlag) && (ParanthesisFlag < 0 || TokenList.IndexOf(Token) < ParanthesisFlag))
    //            {
    //                if (Token[0] == 'X')
    //                {
    //                    X = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
    //                    TokenDictionary.Add("X", X);
    //                }
    //                else if (Token[0] == 'Y')
    //                {
    //                    Y = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
    //                    TokenDictionary.Add("Y", Y);
    //                }
    //                else if (Token[0] == 'Z')
    //                {
    //                    Z = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
    //                    TokenDictionary.Add("Z", Z);
    //                }
    //                else if (Token[0] == 'E')
    //                {
    //                    E = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
    //                    TokenDictionary.Add("E", E);
    //                }
    //            }
    //        }
    //        if (G1Flag >= 0)
    //        {
    //            if (X != -1)
    //                X = X - 100;
    //            if (Y != -1)
    //                Y = Y - 100;
    //            return new Point3D(X, Y, Z);
    //        }
            
    //        return null;
    //        }

        

    //}

}
