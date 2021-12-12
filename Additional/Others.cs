using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FreeZeHAX_Trainer
{
    internal class Others
    {
        #region Wait
        public void Wait(int milliseconds)
        {
            Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0)
            {
                return;
            }

            // Console.WriteLine("start wait timer");
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
        #endregion

        #region Extracting Files From The Solution
        public void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                w.Write(r.ReadBytes((int)s.Length));
            }
        }
        #endregion

        #region Random String
        public string GetRandomString()
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            int length = 15;

            char[] chars = new char[length];
            Random rd = new Random();

            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
        #endregion

        #region Cleaning Folders \\
        public void ClearFolder(string FolderName)
        {
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
        #endregion

        #region Disable | CMD
        public void DisableCommandPrompt()
        {
            //Key path :
            //HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows
            Registry.SetValue(
                @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\System",
                "DisableCMD",
                1,
                RegistryValueKind.DWord
                );
        }
        #endregion

        #region Disable | Task Manager
        public void DisableTaskManager()
        {
            //Path to registry key...
            //HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies
            Registry.SetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System",
                "DisableTaskMgr",
                1,
                RegistryValueKind.DWord
            );
        }
        #endregion

        #region DirClean
        public void DirClean(string FolderPath)
        {
            try
            {
                foreach (string dir in Directory.EnumerateDirectories(FolderPath))
                {
                    if (dir.Contains("Microsoft.Windows.StartMenuExperienceHost_"))
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
            catch (Exception) { }
        }
        #endregion

        #region Cheat Engine Names
        public string[] CE =
        {
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
        };
        #endregion
    }
}
