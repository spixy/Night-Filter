using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Night_Filter
{
    public partial class Form1 : Form
    {
        private int initialStyle;
        private DateTime lastEvent;
        private Point mouse;

        private const string HomePage = "https://github.com/spixy/Night-Filter";
        private const string UpdateFile = "https://raw.githubusercontent.com/spixy/Night-Filter/master/lastversion";
        private readonly string ConfigFile = Environment.CurrentDirectory + "\\NightFilter.ini";

        private bool IsFullscreen
        {
            get { return WindowState == FormWindowState.Maximized; }
        }

        public Form1()
        {
            InitializeComponent();

            MouseWheel += Form1_MouseWheel;
        }

        // General

        private void LoadArgs()
        {
            string[] Args = Environment.GetCommandLineArgs();

            for (byte k = 1; k < Args.Length; k++)
            {
                if (Args[k].ToUpper() == "/HIDE")
                {
                    Hide();
                    showToolStripMenuItem.Text = "Show";
                }
                else if (Args[k].ToUpper() == "/NOFS")
                {
                    ToggleFullScreen(false);
                    fullscreenToolStripMenuItem.Checked = false;
                }
                else
                {
                    try
                    {
                        double n = Convert.ToInt32(Args[k]) / 100d;
                        SetOpacity(n);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        private void LoadConfig()
        {
            if (!File.Exists(ConfigFile))
                return;

            try
            {
                string[] lines = File.ReadAllLines(ConfigFile);

                BackColor = Color.FromArgb(Convert.ToInt32(lines[0]));

                if (lines[1] == "0")
                    fullscreenToolStripMenuItem.PerformClick();

                if (lines[2] == "0")
                    clickThroughToolStripMenuItem.PerformClick();

                switch (lines[3])
                {
                    case "0":
                        forcedOnTopToolStripMenuItem.PerformClick();
                        break;
                    case "1.5":
                        forcedmediumToolStripMenuItem.PerformClick();
                        break;
                    case "2":
                        forcedfastToolStripMenuItem.PerformClick();
                        break;
                }

                double n = Convert.ToDouble(lines[4]);
                SetOpacity(n);
            }
            catch
            {
                // ignored
            }
        }

        private void SaveConfig()
        {
            using (StreamWriter sw = new StreamWriter(ConfigFile, false))
            {
                sw.WriteLine(BackColor.ToArgb());
                sw.WriteLine(fullscreenToolStripMenuItem.Checked ? "1" : "0");
                sw.WriteLine(clickThroughToolStripMenuItem.Checked ? "1" : "0");

                if (forcedOnTopToolStripMenuItem.Checked) sw.WriteLine("1");
                else if (forcedmediumToolStripMenuItem.Checked) sw.WriteLine("1.5");
                else if (forcedfastToolStripMenuItem.Checked) sw.WriteLine("2");
                else sw.WriteLine("0");

                sw.WriteLine(Opacity);
            }
        }

        private void ToggleVisibility()
        {
            if (Visible)
            {
                Hide();
                timer1.Enabled = false;
                showToolStripMenuItem.Text = "Show";
            }
            else
            {
                Show();
                timer1.Enabled = true;
                showToolStripMenuItem.Text = "Hide";
            }
        }

        private void ToggleFullScreen(bool value)
        {
            if (value)
            {
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.None;

                if (clickThroughToolStripMenuItem.Checked)
                    Utility.SetWindowLong(Handle, -20, initialStyle);

                clickThroughToolStripMenuItem.Checked = false;
                clickThroughToolStripMenuItem.PerformClick();
            }
            else
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.Sizable;

                if (clickThroughToolStripMenuItem.Checked)
                    Utility.SetWindowLong(Handle, -20, initialStyle);
            }
        }

        private void SetOpacity(double opacity)
        {
            int opacity_int = (int) (opacity * 100);

            ToolStripMenuItem item = FindMenuItem(opacity_int);

            if (item == null)
            {
                toolStripTextBox1.Text = opacity_int.ToString();
            }
            else
            {
                Opacity = opacity;
                Utility.Uncheck(item, brightnessToolStripMenuItem.DropDownItems);
            }
        }

        private ToolStripMenuItem FindMenuItem(int opacity)
        {
            foreach (ToolStripMenuItem item in brightnessToolStripMenuItem.DropDownItems)
            {
                if (item.Text == opacity + " %")
                    return item;
            }

            return null;
        }

        // Form

        private void Form1_Load(object sender, EventArgs e)
        {
            initialStyle = Utility.GetWindowLong(Handle, -20) | 0x80000 | 0x20;
            lastEvent = DateTime.Now;
            Utility.SetWindowLong(Handle, -20, initialStyle);

            LoadConfig();

            LoadArgs();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
            Application.Exit();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button != MouseButtons.Left) || clickThroughToolStripMenuItem.Checked)
                return;

            Cursor = Cursors.SizeAll;
            mouse = new Point(e.X, e.Y);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) || clickThroughToolStripMenuItem.Checked)
                Cursor = Cursors.Default;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Cursor == Cursors.SizeAll) && (!clickThroughToolStripMenuItem.Checked))
                Location = new Point(e.X - mouse.X + Location.X, e.Y - mouse.Y + Location.Y);
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_MENU))
                return;

            if (e.Delta > 0)
            {
                SetOpacity(Opacity + 0.02);
            }
            else if (e.Delta < 0)
            {
                SetOpacity(Opacity - 0.02);
            }
        }

        // Notify icon

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            ToggleVisibility();
        }

        // Context menu

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (forcedOnTopToolStripMenuItem.Checked)
                timer1.Enabled = false;

            runAtStartupToolStripMenuItem.Checked = Utility.IsAtStartup("Night Filter");
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (forcedOnTopToolStripMenuItem.Checked)
                timer1.Enabled = true;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleVisibility();
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (forcedOnTopToolStripMenuItem.Checked)
                timer1.Enabled = false;

            colorDialog1.Color = BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
                BackColor = colorDialog1.Color;

            if (forcedOnTopToolStripMenuItem.Checked)
                timer1.Enabled = true;
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                Opacity = Convert.ToInt32(item.Text.Replace(" %", "")) / 100d;
                Utility.Uncheck(item, brightnessToolStripMenuItem.DropDownItems);
            }
            catch
            {
                // ignored
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text.Length != 0)
            {
                if (toolStripTextBox1.Text.Contains("%"))
                    toolStripTextBox1.Text = toolStripTextBox1.Text.Replace("%", "");

                try
                {
                    Opacity = Convert.ToDouble(toolStripTextBox1.Text) / 100;
                    Utility.Uncheck(otherToolStripMenuItem, brightnessToolStripMenuItem.DropDownItems);
                }
                catch
                {
                    // ignored
                }
            }

            toolStripTextBox1.Text = ((int)(Opacity * 100)).ToString();
        }

        private void fullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleFullScreen(fullscreenToolStripMenuItem.Checked);
        }

        private void onTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopMost = onTopToolStripMenuItem.Checked;
            forcedOnTopToolStripMenuItem.Enabled = onTopToolStripMenuItem.Checked;
            forcedfastToolStripMenuItem.Enabled = onTopToolStripMenuItem.Checked;
        }

        private void forcedOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forcedmediumToolStripMenuItem.Checked = false;
            forcedfastToolStripMenuItem.Checked = false;
            timer1.Enabled = forcedOnTopToolStripMenuItem.Checked;
            timer1.Interval = 250;
        }

        private void forcedmediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forcedOnTopToolStripMenuItem.Checked = false;
            forcedfastToolStripMenuItem.Checked = false;
            timer1.Enabled = forcedmediumToolStripMenuItem.Checked;
            timer1.Interval = 50;
        }

        private void forcedfastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forcedOnTopToolStripMenuItem.Checked = false;
            forcedmediumToolStripMenuItem.Checked = false;
            timer1.Enabled = forcedfastToolStripMenuItem.Checked;
            timer1.Interval = 16;
        }

        private void clickThroughToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickThroughToolStripMenuItem.Checked)
            {
                Utility.SetWindowLong(Handle, -20, initialStyle);
            }
            else
            {
                FormBorderStyle = (fullscreenToolStripMenuItem.Checked) ? FormBorderStyle.None : FormBorderStyle.Sizable;
            }
        }

        private void runAtStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Utility.SetAsStartup("Night Filter", runAtStartupToolStripMenuItem.Checked))
            {
                MessageBox.Show("Cannot change startup settings", "Error");
                runAtStartupToolStripMenuItem.Checked = Utility.IsAtStartup("Night Filter");
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updater updater = new Updater(UpdateFile);

            if (updater.IsUpdateAvailable() &&
                MessageBox.Show("Update Available. Download new version?", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start(HomePage);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
            timer1.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Timers

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Visible)
                TopMost = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_MENU))
                return;

            if (Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_F1))
            {
                if ((DateTime.Now - lastEvent).Milliseconds > 200)
                {
                    ToggleFullScreen(!IsFullscreen);
                    lastEvent = DateTime.Now;
                }
            }
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_SPACE))
            {
                if ((DateTime.Now - lastEvent).Milliseconds > 200)
                {
                    ToggleVisibility();
                    lastEvent = DateTime.Now;
                }
            }
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D0) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD0)) SetOpacity(0);
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D1) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD1)) toolStripMenuItem10.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D2) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD2)) toolStripMenuItem9.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D3) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD3)) toolStripMenuItem8.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D4) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD4)) toolStripMenuItem7.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D5) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD5)) toolStripMenuItem6.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D6) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD6)) toolStripMenuItem5.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D7) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD7)) toolStripMenuItem4.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D8) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD8)) toolStripMenuItem3.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.D9) || Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_NUMPAD9)) toolStripMenuItem2.PerformClick();
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_PLUS)) SetOpacity(Opacity + 0.02);
            else if (Utility.IsKeyPressed(Utility.VirtualKeyStates.VK_MINUS)) SetOpacity(Opacity - 0.02);
        }
    }
}