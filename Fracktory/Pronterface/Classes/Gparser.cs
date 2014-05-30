using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Fracktory
{

        public class FivePoint
        {
            public double X = -1;
            public double Y = -1;
            public double Z = -1;
            public double E = -10;
            public double F = -1;
            public FivePoint(double X = -1, double Y = -1, double Z = -1, double E = -10, double F = -1)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
                this.E = E;
                this.F = F;
            }
        }
        public static class GParser
        {
            public static FivePoint getPoint(String GCode)
            {
                double X = -1;
                double Y = -1;
                double Z = -1;
                double E = -10;
                double F = -1;
                var TokenList = GCode.Split(' ').ToList<string>();
                var CommentFlag = TokenList.IndexOf(";");
                var ParanthesisFlag = TokenList.IndexOf("(");
                var ClosedParanthesisFlag = TokenList.IndexOf(")");
                int G1Flag = TokenList.IndexOf("G1");
                Dictionary<string, double> TokenDictionary = new Dictionary<string, double>();
                foreach (var Token in TokenList)
                {
                    if (Token.Count() > 0 && G1Flag >= 0 && (CommentFlag < 0 || TokenList.IndexOf(Token) < CommentFlag) && (ParanthesisFlag < 0 || TokenList.IndexOf(Token) < ParanthesisFlag))
                    {
                        if (Token[0] == 'X')
                        {
                            X = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                            TokenDictionary.Add("X", X);
                        }
                        else if (Token[0] == 'Y')
                        {
                            Y = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                            TokenDictionary.Add("Y", Y);
                        }
                        else if (Token[0] == 'Z')
                        {
                            Z = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                            TokenDictionary.Add("Z", Z);
                        }
                        else if (Token[0] == 'E')
                        {
                            E = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                            TokenDictionary.Add("E", E);
                        }
                        else if (Token[0] == 'F')
                        {
                            F = double.Parse(Token.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                            TokenDictionary.Add("F", F);
                        }

                    }
                }
                if (G1Flag >= 0)
                {
                    return new FivePoint(X, Y, Z, E, F);

                }

                return null;
            }
        }
    }

