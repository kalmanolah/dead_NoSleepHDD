using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NoSleepHDD
{
    public partial class Form1 : Form
    {
        RegistryKey rKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();
            checkBox1.Checked = rKey.GetValue("NoSleepHDD") != null;
            numericUpDown1.Value = Properties.Settings.Default.Interval;
            if (Properties.Settings.Default.Paths != null)
            {
                string[] paths = new string[Properties.Settings.Default.Paths.Count];
                Properties.Settings.Default.Paths.CopyTo(paths, 0);
                textBox1.Lines = paths;
            }
            timer.Tick += new EventHandler(timer_Tick);
            if (Environment.GetCommandLineArgs().Contains("/background"))
            {
                this.Hide();
                Start();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Write();
        }

        private void Write()
        {
            if (Properties.Settings.Default.Paths != null)
            {
                foreach (string str in Properties.Settings.Default.Paths)
                {
                    string p = Path.Combine(str, "NoSleepHDD.txt");
                    try
                    {
                        File.Delete(p);
                    }
                    catch (IOException)
                    {
                    }
                    try
                    {
                        using(StreamWriter sw = new StreamWriter(p))
                        {
                            sw.Write("NoSleepHDD - https://github.com/kalmanolah/NoSleepHDD");
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        private void Start()
        {
            label8.Text = "Started";
            timer.Interval = Properties.Settings.Default.Interval * 60 * 1000;
            timer.Start();
            Write();
        }

        private void Stop()
        {
            label8.Text = "Stopped";
            timer.Stop();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkLabel1.Text);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.gnu.org/licenses/gpl.txt");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                rKey.SetValue("NoSleepHDD", Application.ExecutablePath.ToString() + " /background");
            }
            else
            {
                rKey.DeleteValue("NoSleepHDD", false);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Interval = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            StringCollection coll = new StringCollection();
            coll.AddRange(textBox1.Lines);
            Properties.Settings.Default.Paths = coll;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (label8.Text == "Stopped")
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }
    }
}
