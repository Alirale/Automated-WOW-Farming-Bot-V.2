using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WindowsInput;
using GlobalHotKey;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace HolydayRadar
{
    public partial class Form1 : Form
    {
        public static HotKeyManager hotKeyManager = new HotKeyManager();
        public Point VendorPosition;
        public static Point LootRadious;
        public static Boolean Continue = true;
        public static Bitmap Looticon;
        InputSimulator sim = new InputSimulator();
        Stopwatch TimeOut = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                var PauseEngine = hotKeyManager.Register(Key.F2, System.Windows.Input.ModifierKeys.Shift);
                var ResumeEngine = hotKeyManager.Register(Key.F3, System.Windows.Input.ModifierKeys.Shift);
                var GetCircularLootRadious = hotKeyManager.Register(Key.F5, System.Windows.Input.ModifierKeys.None);
                hotKeyManager.KeyPressed += HotKeyManager_KeyPressed;
                Thread.Sleep(3000);

            Thread T = new Thread(() =>
            {
                sim.Mouse.MoveMouseTo(10, 10);
                sim.Mouse.LeftButtonDoubleClick();
                Thread.Sleep(1000);
                sim.Mouse.LeftButtonDoubleClick();
                SetCircularLootR();
                RadarEngine.RadousCalculator();
                RadarEngine.LunchRadar();
                TimeOutCanceller();
            });
            T.Start();
            TimeOut.Start();

        }

        private void HotKeyManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.F2 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                Pause();
            }
            if (e.HotKey.Key == Key.F3 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                Resume();
            }
            if (e.HotKey.Key == Key.F5 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                GetCircularLootR();
            }

        }

        private void TimeOutCanceller()
        {
            while (TimeOut.Elapsed.TotalMinutes < 5)
            {
                Thread.Sleep(10000);
            }

            TimeOut.Stop();
            TimeOut.Reset();

            Form1.hotKeyManager.Unregister(Key.F1, System.Windows.Input.ModifierKeys.Shift);
            Form1.hotKeyManager.Unregister(Key.F2, System.Windows.Input.ModifierKeys.Shift);
            Form1.hotKeyManager.Unregister(Key.F5, System.Windows.Input.ModifierKeys.None);

            this.Invoke(new Action(delegate
            {
                Form1.hotKeyManager.Dispose();
            }));

            Application.Restart();
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        public void Resume()
        {
            Continue = true;
            Task.Delay(40).Wait();
            RadarEngine.LunchRadar();
        }

        public void Pause()
        {
            Continue = false;
        }

        public void HotkeyDisposer()
        {
            this.Invoke(new Action(delegate
            {
                Form1.hotKeyManager.Dispose();
            }));
        }

        private void GetCircularLootR()
        {
            MouseClickStuffs msvc = new MouseClickStuffs();
            LootRadious = msvc.GetCursorPosition();
            string ToSave="";

            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Circle.txt"))
            {
                ToSave = LootRadious.X.ToString() + "," + LootRadious.Y.ToString();
                sw.WriteLine(ToSave);
            }

            String ToReads = "R :";
            ToReads += "\n";
            string line = "";
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Circle.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    ToReads += line;
                    ToReads += "\n";
                }
            }
            MessageBox.Show(ToReads);
        }


        private void SetCircularLootR() 
        {
            String ToReads = "R :";
            ToReads += "\n";
            string line = "";
            List<string> parts = new List<string>(2);
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Circle.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        parts = line.Split(',').ToList();
                        LootRadious.X = Convert.ToInt32(parts[0]);
                        LootRadious.Y = Convert.ToInt32(parts[1]);
                        ToReads += LootRadious.X.ToString() + "," + LootRadious.Y.ToString();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RadarEngine.Test();
            //3,3:
            //Finger --> #253A3D
            //Fight -->  #53656C
            //Loot -->   #D8AA60
            //UnLoot-->  #707070

            //1,1:
            //#E9CB91 --> Loot2
            //#E8CA90 --> Loot

            //2,2
            //#E0B975 --> Loot2
            //#DFB975 --> Loot
        }
    }
}
