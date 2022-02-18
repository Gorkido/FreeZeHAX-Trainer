using System;
using System.Windows.Forms;

namespace FreeZeHAX_Trainer
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Trainer());
        }
    }
}