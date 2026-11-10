using System;
using System.IO;
using System.Windows.Forms;
using CRM.UI;

namespace CRM.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                MainContr controller = new MainContr();
                Application.Run(controller.MainForm);
            }
            catch (Exception ex)
            {
                // Записываем ошибку в файл рядом с exe
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.WriteAllText(logPath, ex.ToString());

                MessageBox.Show($"Ошибка при запуске:\n{ex.Message}", "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}