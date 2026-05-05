using System;
using System.Windows.Forms;

namespace BeelineCipher
{
    /// <summary>
    /// Точка входа приложения «Шифр Билайна».
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Главный метод запуска приложения Windows Forms.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
