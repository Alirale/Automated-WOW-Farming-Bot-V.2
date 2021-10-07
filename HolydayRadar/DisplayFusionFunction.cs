using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace HolydayRadar
{
    public class DisplayFusionFunction
    {
		public static bool SpinAlowed = true;
		static InputSimulator sim = new InputSimulator();
		static MouseClickStuffs mvsc = new MouseClickStuffs();
		
		static Bitmap bmp2 = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Loot.bmp");
		static Bitmap bmp;
		static Form1 frm = new Form1();
		static int counter = 0;

		


		public static void Run(double Radius)//180
		{
			int startX = Screen.PrimaryScreen.Bounds.Width / 2;
			int startY = Screen.PrimaryScreen.Bounds.Height / 2;

			for (double i = 0.0d; i < Math.PI * 2.0d; i += 0.5d)
			{
				if (SpinAlowed && Form1.Continue)
				{
					int x = startX + (int)(Radius * Math.Cos(i));
					int y = startY + (int)(Radius * Math.Sin(i));

					takeCursor();
					Cursor.Position = new Point(x, y);
					Task.Delay(40).Wait();
				}
			}
		}

		public static void takeCursor()
		{
			Point REFPOINT = Point.Empty;
			//int cursorX = 0;
			//int cursorY = 0;
			REFPOINT.X = 0;
			REFPOINT.Y = 0;

			bmp = CaptureScreen.CaptureImageCursor(ref REFPOINT, 1);

			if (bmp != null)
			{
				//Console.WriteLine("Picture");
				//counter++;
				//bmp.Save(AppDomain.CurrentDomain.BaseDirectory+ @"\New folder\"+ "Pic"+counter+".png",ImageFormat.Png);
				//try
				//{
					Color pixelColor = bmp.GetPixel(1, 1);
				if (GetHTMLColor(pixelColor) == "#E9CB91") //GetHTMLColor(pixelColor) == "#D8AA60" || 
				{
					//Console.WriteLine("YES!");
					Task.Delay(60).Wait();
					SendKeys.SendWait("{F2}");

					Task.Delay(600).Wait();
					for (int i = 0; i < 5; i++)
					{
						Task.Delay(60).Wait();
						mvsc.DoLeftMouseClickdown();

						Task.Delay(60).Wait();

						mvsc.DoLeftMouseClickup();
						Task.Delay(60).Wait();

					}
					Task.Delay(600).Wait();

					Task.Delay(60).Wait();
					SendKeys.SendWait("{F3}");

				}
			}
		}

		private static string GetHTMLColor(Color pixelColor)
		{
			return ColorTranslator.ToHtml(pixelColor);
		}

	}
}
