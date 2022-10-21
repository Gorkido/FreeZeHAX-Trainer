using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace FreeZeHAX_Trainer
{
    internal class Others
    {
        #region Wait

        public void Wait(int milliseconds)
        { // Wait without stopping gui
            Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0)
            {
                return;
            }

            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

        #endregion Wait

        #region Cleaning Folder

        public void ClearFolder(string FolderName)
        { // Delete what's inside the folder
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception) { } // Ignore all exceptions
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                try
                {
                    di.Delete();
                }
                catch (Exception) { } // Ignore all exceptions
            }
        }

        #endregion Cleaning Folder

        #region Cheat Engine Names

        public string[] CE =
        { // Known Cheat Engines
            "cheatengine-x86_64",
            "draqxorengine-x86_64",
            "fixedengine-x86_64",
            "engine-x86_64",
            "-x86_64",
            "Fixed Engine",
            "FixedEngine",
            "Fixed-Engine",
            "fixed-engine",
            "Fixed-engine",
            "fixedengine",
            "fixed engine",
            "cheatengine",
            "cheat-engine",
            "Cheat-Engine",
            "Cheat-engine",
            "draqxorengine",
            "DraqxorEngine",
            "Draqxorengine",
            "Draqxor-Engine",
            "Draqxor-engine",
            "draqxor-engine",
            "Cheat Engine",
            "fixed engine",
            "cheat engine",
            "draqxor",
            "Cheat *",
            "Cheat*",
            "Fiddler.exe",
            "Fiddler",
            "Wireshark.exe",
            "Wireshark"
        };

        #endregion Cheat Engine Names

        #region Get Save.dat file location

        public string SaveDatPath()
        {
            string Result = "";
            try
            { // Get save.dat file's location from registry. If it doesn't exist, search for the default file location
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Growtopia"))
                {
                    if (key != null)
                    {
                        object o = key.GetValue("path");
                        Result = o != null ? o.ToString() : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Growtopia";
                    }
                    else
                    {
                        Result = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Growtopia";
                    }
                }
            }
            catch (Exception) { }
            return Result + "\\save.dat";
        }

        #endregion Get Save.dat file location
    }
}