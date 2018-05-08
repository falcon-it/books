using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BooksClient
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SelectModeForm dlg = new SelectModeForm();
            dlg.ShowDialog();

            Form appForm = null;

            using (BookServiceClient bsc = BookServiceClient.instance)
            {
                bsc.connect();

                switch (dlg.AppMode)
                {
                    case SelectModeForm.mode.editor:
                        appForm = new EditorForm();
                        break;
                    case SelectModeForm.mode.buyer:
                        appForm = new BuyerForm();
                        break;
                    default:
                        return;
                }

                Application.Run(appForm);
            }
        }
    }
}
