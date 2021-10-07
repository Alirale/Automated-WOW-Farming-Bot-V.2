using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolydayRadar
{
    public class Calcs
    {
        private double DegreeTurn(double Rad)
        {
            return Rad * (180 / Math.PI);
        }


        public Double MouseTurn(double Degree)
        {
            MouseClickStuffs mvsc = new MouseClickStuffs();
            double Bound = Screen.PrimaryScreen.Bounds.Width;
            return ((Degree) * ((Bound * 800) / (1440 * 180)));
        }

        public List<string> RadDegreeTurn(Double CurrentX, Double CurrentY, Double CurrentFacing, Double DestinationX, Double DestinationY)
        {
            double teta, Degreeteta, LastAnswer;
            double DeltaX = DestinationX - CurrentX;
            double DeltaY = DestinationY - CurrentY;
            double FinalAnswer = 0;
            List<string> OutPuts = new List<string>();

            if ((CurrentX ) == 0 && (CurrentY) == 0)
            {
                return null;
            }

            if (DestinationY < CurrentY)
            {
                teta = Math.Atan(DeltaX/DeltaY); 
                Degreeteta = DegreeTurn(teta);

                FinalAnswer= - (CurrentFacing - Degreeteta);

                
            }
            else if (DestinationY == CurrentY)
            {
                teta = Math.Atan(DeltaX / DeltaY);
                Degreeteta = DegreeTurn(teta);

                FinalAnswer = 180 - (CurrentFacing - Degreeteta);


            }
            else if (DestinationY > CurrentY)
            {

                teta = Math.Atan(DeltaX / DeltaY);
                Degreeteta = DegreeTurn(teta);

                FinalAnswer= 180 - (CurrentFacing - Degreeteta);
                
            }

            if (FinalAnswer >= 0 && FinalAnswer <= 180) // 1 cc
            {
                LastAnswer = FinalAnswer;
                if (LastAnswer >= 360)
                {
                    LastAnswer = LastAnswer - 360;
                }
                if (LastAnswer < -360)
                {
                    LastAnswer = LastAnswer + 360;
                }

                LastAnswer = Math.Abs(LastAnswer);

                OutPuts.Add((LastAnswer).ToString());
                OutPuts.Add("cc");
                if (OutPuts.Count == 2)
                {
                    return OutPuts;
                }
            }

            else if(FinalAnswer > 180 )// 2 cw
            {
                LastAnswer = 360 - FinalAnswer ;
                if (LastAnswer >= 360)
                {
                    LastAnswer = LastAnswer - 360;
                }
                if (LastAnswer < -360)
                {
                    LastAnswer = LastAnswer + 360;
                }

                LastAnswer = Math.Abs(LastAnswer);

                OutPuts.Add((LastAnswer).ToString());
                OutPuts.Add("cw");
                if (OutPuts.Count==2)
                {
                    return OutPuts;
                }
            }
            
            else if (FinalAnswer < 0 && FinalAnswer >= -180)// 3 cw
            {
                LastAnswer = -FinalAnswer;
                if (LastAnswer >= 360)
                {
                    LastAnswer = LastAnswer - 360;
                }
                if (LastAnswer < -360)
                {
                    LastAnswer = LastAnswer + 360;
                }

                LastAnswer = Math.Abs(LastAnswer);

                OutPuts.Add((LastAnswer).ToString());
                OutPuts.Add("cw");

                if (OutPuts.Count == 2)
                {
                    return OutPuts;
                }
            }
            else if (FinalAnswer < -180)// 4 cc
            {
                LastAnswer = 360 + FinalAnswer;
                if (LastAnswer >= 360)
                {
                    LastAnswer = LastAnswer - 360;
                }
                if (LastAnswer < -360)
                {
                    LastAnswer = LastAnswer + 360;
                }

                LastAnswer = Math.Abs(LastAnswer);

                OutPuts.Add((LastAnswer).ToString());
                OutPuts.Add("cc");
                if (OutPuts.Count == 2)
                {
                    return OutPuts;
                }
            }
            return null;
        }


        public List<String> DataExtractor(String FileName)
        {
            string line = "";
            List<string> LineDatas = new List<string>();
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + FileName))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        LineDatas.Add(line);
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            return LineDatas;
        }


        public double CalcDisstance(Double x1, Double y1, Double x2, Double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public Double To_180(double From360) 
        {
            if (From360 > 180)
            {
                return (From360 - 360);
            }
            else
            {
                return From360;
            }
        }

        public double StandingKeyTurnTime(double Degree)
        {
            return (double)((Degree * 1000) / (180));
        }
    }
}
