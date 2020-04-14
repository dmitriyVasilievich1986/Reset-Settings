using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;

namespace ResetSettings
{
    public partial class Form1 : Form
    {
        private byte[] convert_to_byte(string text)
        {
            byte[] output = new byte[text.Split().Length];
            for (int a = 0; a < text.Split().Length; a++) output[a] = Convert.ToByte(text.Split()[a], 16);
            return output;
        }

        SerialPort PortUSB = new SerialPort();
        Dictionary<string, string> PSC24V10A_Set = new Dictionary<string, string>()
        {            
            { "нагрев1", "55 AA 09 0A 01 00 00 F0 41 45"},
            { "нагрев2", "55 AA 89 0A 01 00 00 00 00 94"},
            { "вкл1", "55 AA 09 08 03 00 00 14"},
            { "вкл2", "55 AA 89 08 03 00 00 94"},
            { "T1 вкл1", "55 AA 09 08 02 01 00 14"},
            { "T1 вкл2", "55 AA 89 08 02 00 00 93"},
            { "датчик t1", "55 AA 0B 07 01 AA BD"},
            { "датчик t2", "55 AA 8A 0E 01 00 00 00 00 00 00 00 00 99"}
        };
        Dictionary<string, string> PSC24V40A_Set = new Dictionary<string, string>()
        {
            { "ModBus1", "55 AA 01 07 08 01 11"},
            { "ModBus2", "55 AA 01 07 04 01 0D"},
            { "нагрев", "55 AA 0C 08 03 00 00 17"},
            { "T1 вкл", "55 AA 0C 08 02 01 00 17"},
            { "30C", "55 AA 0C 0A 01 00 00 F0 41 48"},
            { "датчик t1", "55 AA 0E 07 01 AA C0"},
            { "датчик t2", "55 AA 8D 0E 01 00 00 00 00 00 00 00 00 9C"}
        };
        Dictionary<string, string> RTU7_Set = new Dictionary<string, string>()
        {            
            { "Reset", "55 AA 13 07 01 00 1B"},
            { "ModBus1", "55 AA 01 07 08 01 11"},
            { "ModBus2", "55 AA 01 07 04 01 0D"},
            { "AC1", "55 AA 14 08 01 00 00 1D"},
            { "Set", "55 AA 13 07 02 00 1C"}
        };
        Dictionary<string, string> PM7E1DI8_Set = new Dictionary<string, string>()
        {            
            { "Din1 AC11", "55 AA 64 26 01 44 49 4E 31 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 6E"},
            { "Din1 AC12", "55 AA E4 26 01 44 49 4E 31 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 EE"},
            { "Din1 AC21", "55 AA 64 26 01 44 49 4E 32 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 6F"},
            { "Din1 AC22", "55 AA E4 26 01 44 49 4E 32 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 EF"},
            { "Din1 AC31", "55 AA 64 26 01 44 49 4E 33 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 70"},
            { "Din1 AC32", "55 AA E4 26 01 44 49 4E 33 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F0"},
            { "Din1 AC41", "55 AA 64 26 01 44 49 4E 34 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 71"},
            { "Din1 AC42", "55 AA E4 26 01 44 49 4E 34 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F1"},
            { "Din1 AC51", "55 AA 64 26 01 44 49 4E 35 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 72"},
            { "Din1 AC52", "55 AA E4 26 01 44 49 4E 35 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F2"},
            { "Din1 AC61", "55 AA 64 26 01 44 49 4E 36 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 73"},
            { "Din1 AC62", "55 AA E4 26 01 44 49 4E 36 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F3"},
            { "Din1 AC71", "55 AA 64 26 01 44 49 4E 37 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 74"},
            { "Din1 AC72", "55 AA E4 26 01 44 49 4E 37 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F4"},
            { "Din1 AC81", "55 AA 64 26 01 44 49 4E 38 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 75"},
            { "Din1 AC82", "55 AA E4 26 01 44 49 4E 38 5F 41 43 44 43 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F5"},
            { "Set Offset1", "55 AA B3 09 00 00 00 00 BC"},
            { "Set Offset2", "55 AA 10 07 02 00 19"}
        };
        Dictionary<string, string> PSC24V10A_Reset = new Dictionary<string, string>()
        {
            { "нагрев1", "55 AA 09 0A 01 00 00 A0 41 F5"},
            { "нагрев2", "55 AA 89 0A 01 00 00 00 00 94"}
        };
        Dictionary<string, string> MTU5_Set = new Dictionary<string, string>()
        {
            {"Reset", "55 AA 64 26 01 53 79 73 52 65 73 65 74 4B 66 4F 66 66 73 65 74 3D 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53" },
            {"modbus1.1", "55 AA 64 26 01 55 61 72 74 73 58 31 30 34 4F 70 4D 6F 64 65 3D 34 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3C" },
            {"modbus1.2", "55 AA E4 26 01 55 61 72 74 73 58 31 30 34 4F 70 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 B8" },
            {"modbus2.1", "55 AA 64 26 01 55 61 72 74 73 54 62 75 73 4F 70 4D 6F 64 65 3D 34 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ED" },
            {"modbus2.2", "55 AA E4 26 01 55 61 72 74 73 54 62 75 73 4F 70 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 69" },
            {"AC1.1", "55 AA 64 26 01 4B 66 41 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 6F" },
            {"AC1.2", "55 AA E4 26 01 4B 66 41 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 EF" },
            {"AC2.1", "55 AA 64 26 01 4B 66 42 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 70" },
            {"AC2.2", "55 AA E4 26 01 4B 66 42 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F0" },
            {"AC3.1", "55 AA 64 26 01 4B 66 43 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 71" },
            {"AC3.2", "55 AA E4 26 01 4B 66 43 4D 6F 64 65 3D 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 F1" },
            {"Set", "55 AA 64 26 01 53 79 73 53 65 74 4B 66 4F 66 66 73 65 74 3D 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 7C" }
        };

        Dictionary<string, byte[]> DIN32 = new Dictionary<string, byte[]>()
        {
            {"Reset1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Reset2", new byte[]{ 0x55, 0xAA, 0x09, 0x07, 0x01, 0x00, 0x11 } },            
            {"AC1.1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"AC1.2", new byte[]{ 0x55, 0xAA, 0x05, 0x08, 0x01, 0x00, 0x00, 0x0E } },
            {"AC1.3", new byte[]{ 0x55, 0xAA, 0x85, 0x08, 0x01, 0x00, 0x00, 0x8E } },
            {"AC2.1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"AC2.2", new byte[]{ 0x55, 0xAA, 0x05, 0x08, 0x02, 0x00, 0x00, 0x0F} },
            {"AC2.3", new byte[]{ 0x55, 0xAA, 0x85, 0x08, 0x02, 0x00, 0x00, 0x8F } },
            {"Set1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Set2", new byte[]{ 0x55, 0xAA, 0x09, 0x07, 0x02, 0x00, 0x12 } }
        };

        Dictionary<string, byte[]> MTU3_Set = new Dictionary<string, byte[]>()
        {
            {"Reset1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Reset2", new byte[]{ 0x55, 0xAA, 0x0D, 0x07, 0x01, 0x00, 0x15 } },
            {"AC1.1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"AC1.2", new byte[]{ 0x55, 0xAA, 0x08, 0x07, 0x01, 0x00, 0x10 } },
            {"AC1.3", new byte[]{ 0x55, 0xAA, 0x88, 0x07, 0x01, 0x00, 0x90 } },
            {"Set1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Set2", new byte[]{ 0x55, 0xAA, 0x0D, 0x07, 0x02, 0x00, 0x16 } }
        };

        Dictionary<string, byte[]> MTU3_Reset = new Dictionary<string, byte[]>()
        {
            {"Reset1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Reset2", new byte[]{ 0x55, 0xAA, 0x0D, 0x07, 0x01, 0x00, 0x15 } },
            {"AC1.1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"AC1.2", new byte[]{ 0x55, 0xAA, 0x08, 0x07, 0x01, 0x01, 0x11} },
            {"AC1.3", new byte[]{ 0x55, 0xAA, 0x88, 0x07, 0x01, 0x00, 0x90 } }
        };

        Dictionary<string, byte[]> RTU5_Set = new Dictionary<string, byte[]>()
        {
            {"Reset1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Reset2", new byte[]{ 0x55, 0xAA, 0x10, 0x07, 0x01, 0x00, 0x18 } },
            {"Reset3", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Reset4", new byte[]{ 0x55, 0xAA, 0x10, 0x07, 0x03, 0x00, 0x1A } },
            {"AC1.1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"AC1.2", new byte[]{ 0x55, 0xAA, 0x0B, 0x07, 0x01, 0x00, 0x13 } },
            {"AC1.3", new byte[]{ 0x55, 0xAA, 0x8B, 0x07, 0x01, 0x00, 0x93 } },
            {"ModBus1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"ModBus2", new byte[]{ 0x55, 0xAA, 0x01, 0x07, 0x04, 0x01, 0x0D } },
            {"ModBus3", new byte[]{ 0x55, 0xAA, 0x81, 0x07, 0x04, 0x00, 0x8C } },
            {"ModBus4", new byte[]{ 0x55, 0xAA, 0x01, 0x07, 0x08, 0x01, 0x11 } },
            {"ModBus5", new byte[]{ 0x55, 0xAA, 0x81, 0x07, 0x08, 0x00, 0x90 } },
            {"Set1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Set2", new byte[]{ 0x55, 0xAA, 0x10, 0x07, 0x02, 0x00, 0x19 } },
            {"Set3", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Set4", new byte[]{ 0x55, 0xAA, 0x10, 0x07, 0x04, 0x00, 0x1B } }
        };

        Dictionary<string, byte[]> RTU5_Reset = new Dictionary<string, byte[]>()
        {
            {"Reset1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Reset2", new byte[]{ 0x55, 0xAA, 0x10, 0x07, 0x01, 0x00, 0x18 } },
            {"Reset3", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"Reset4", new byte[]{ 0x55, 0xAA, 0x10, 0x07, 0x03, 0x00, 0x1A } },
            {"ModBus1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"ModBus2", new byte[]{ 0x55, 0xAA, 0x01, 0x07, 0x04, 0x02, 0x0E } },
            {"ModBus3", new byte[]{ 0x55, 0xAA, 0x81, 0x07, 0x04, 0x00, 0x8C } },
            {"ModBus4", new byte[]{ 0x55, 0xAA, 0x01, 0x07, 0x08, 0x01, 0x11 } },
            {"ModBus5", new byte[]{ 0x55, 0xAA, 0x81, 0x07, 0x08, 0x00, 0x90 } },
            {"AC1.1", new byte[]{ 0x55, 0xAA, 0xB3, 0x09, 0x00, 0x00, 0x00, 0x00, 0xBC } },
            {"AC1.2", new byte[]{ 0x55, 0xAA, 0x0B, 0x07, 0x01, 0x01, 0x14 } },
            {"AC1.3", new byte[]{ 0x55, 0xAA, 0x8B, 0x07, 0x01, 0x00, 0x93 } }
        };

        public Form1()
        {
            InitializeComponent();

            PortUSB.DataReceived += (s, e) => { PortUSB.DiscardInBuffer(); };
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.T) Test(null, null); };
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!PortUSB.IsOpen) return;
            button1.Enabled = false;
            button2.Enabled = false;
            switch (comboBox1.Text)
            {
                case "MTU5":
                    foreach (string a in MTU5_Set.Values)
                        { PortUSB.Write(convert_to_byte(a), 0, convert_to_byte(a).Length); Thread.Sleep(200); }
                    break;
                case "RTU7":
                    foreach (string a in RTU7_Set.Values)
                        { PortUSB.Write(convert_to_byte(a), 0, convert_to_byte(a).Length); Thread.Sleep(200); }
                    break;
                case "DIN32":
                    foreach (byte[] a in DIN32.Values)
                        { PortUSB.Write(a, 0, a.Length); Thread.Sleep(200); }
                    break;
                case "MTU3":
                    foreach (byte[] a in MTU3_Set.Values)
                        { PortUSB.Write(a, 0, a.Length); Thread.Sleep(200); }
                    break;
                case "RTU5":
                    foreach (byte[] a in RTU5_Set.Values)
                    { PortUSB.Write(a, 0, a.Length); Thread.Sleep(200); }
                    break;
                case "PSC24V10A":
                    foreach (string a in PSC24V10A_Set.Values)
                        { PortUSB.Write(convert_to_byte(a), 0, convert_to_byte(a).Length); Thread.Sleep(200); }
                    break;
                case "PSC24V40A":
                    foreach (string a in PSC24V40A_Set.Values)
                        { PortUSB.Write(convert_to_byte(a), 0, convert_to_byte(a).Length); Thread.Sleep(200); }
                    break;
                case "PM7 E1 DI8":
                    foreach (string a in PM7E1DI8_Set.Values)
                        { PortUSB.Write(convert_to_byte(a), 0, convert_to_byte(a).Length); Thread.Sleep(200); }
                    break;


            }
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!PortUSB.IsOpen)
            {
                try
                {
                    PortUSB.PortName = "COM7";
                    PortUSB.BaudRate = 115200;
                    PortUSB.Open();
                }
                catch (Exception err)
                { }
            }
            if (!PortUSB.IsOpen)
            {
                try
                {
                    PortUSB.PortName = "COM16";
                    PortUSB.BaudRate = 115200;
                    PortUSB.Open();
                }
                catch (Exception err) { }
            }

            if (PortUSB.IsOpen) { button1.Enabled = true; button2.Enabled = true; }
            else { button1.Enabled = false; button2.Enabled = false; }
        }

        private void Test(object sender, EventArgs e)
        {
            try
                {
                    PortUSB.PortName = "COM7";
                    PortUSB.BaudRate = 115200;
                    PortUSB.Open();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!PortUSB.IsOpen) return;
            button1.Enabled = false;
            button2.Enabled = false;
            switch (comboBox1.Text)
            {
                case "MTU3":                    
                    foreach (byte[] a in MTU3_Reset.Values)
                        { PortUSB.Write(a, 0, a.Length); Thread.Sleep(200); }                    
                    break;
                case "RTU5":
                    foreach (byte[] a in RTU5_Reset.Values)
                    { PortUSB.Write(a, 0, a.Length); Thread.Sleep(200); }
                    break;
                case "PSC24V10A":
                    foreach (string a in PSC24V10A_Reset.Values)
                    { PortUSB.Write(convert_to_byte(a), 0, convert_to_byte(a).Length); Thread.Sleep(200); }
                    break;
            }
            button1.Enabled = true;
            button2.Enabled = true;
        }
    }
}
