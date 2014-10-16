using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace Student
{
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           bool isRuned=true;
           Mutex mutex = new Mutex(true, "StudentControl", out isRuned);
            if (isRuned)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new OperaForm());
                //Application.Run(new frmLogin("4"));
                mutex.ReleaseMutex();
                //退出
                Environment.Exit(0);
                Application.Exit();
            }
            else
            {
                MessageBox.Show("程序已启动,请关闭后，再重新打开!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
        }
    }
}
