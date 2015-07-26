using System;
using System.Reflection;
using System.Windows.Forms;
using Night_Filter.Properties;

namespace Night_Filter
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        #endregion

        private void AboutBox1_Load(object sender, EventArgs e)
        {
            string[] Lines = new string[10];
            Lines[0] = "Hotkeys:";
            Lines[1] = "Alt+Space \t Show/hide filter";
            Lines[2] = "Alt+F1    \t Toogle fullscreen";
            Lines[3] = "Alt+Plus  \t Increase filter";
            Lines[4] = "Alt+Minus \t Decrease filter";
            Lines[5] = "";
            Lines[6] = "Command line parameters:";
            Lines[7] = "XX    \t percent count (0-100)";
            Lines[8] = "/NOFS \t disable fullscreen";
            Lines[9] = "/HIDE \t do not apply filter";

            Text = string.Format("About {0}", AssemblyTitle);
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = string.Format("Version {0}", AssemblyVersion);
            labelCopyright.Text = AssemblyCopyright;
            textBoxDescription.Lines = Lines;
        }

        private void labelCompanyName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            labelWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start(Resources.HomePage);
        }
    }
}
