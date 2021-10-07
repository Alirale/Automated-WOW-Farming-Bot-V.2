using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolydayRadar
{
    public class RadarEngine
    {
        private static Task RadarTask;
        private static double Radious = 0;
        public static bool Continue=true;
        //DisplayFusionFunction dsp = new DisplayFusionFunction();

        public static void RadousCalculator() 
        {
            Calcs calculator = new Calcs();
            Radious = calculator.CalcDisstance(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2, Form1.LootRadious.X, Form1.LootRadious.Y);
        }
        
        public static void LunchRadar() 
        {
            RadarTask = new Task(()=> 
            {
                while (Form1.Continue)
                {
                    DisplayFusionFunction.Run(Radious);
                    Task.Delay(40).Wait();
                }
            });
            RadarTask.Start();
        }


        public static void Test() 
        {
            Bitmap bmp = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\samples\" + "Loot2.png");
            Color pixelColor = bmp.GetPixel(2, 2);
            MessageBox.Show(GetHTMLColor(pixelColor)) ;
        }
        private static string GetHTMLColor(Color pixelColor)
        {
            return ColorTranslator.ToHtml(pixelColor);
        }

    }
}
