using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.UI
{
    public class MainContr
    {
        public FirmMngr Mngr { get; set; }
        public frmMain MainForm { get; set; }

        public MainContr()
        {
            // Заглушка: создаем тестовые данные
            //var firms = new List<Firm> { /* сюда добавить пару Firm из лабы 3 */ };
            //Mngr = new FirmMngr(firms);
            //MainForm = new frmMain(this);
        }

        public void ShowFilter()
        {
            var filterContr = new FilterContr(Mngr);
            // Добавляем правила на основе текущих полей отображения
            foreach (var field in Mngr.ViewSettings.Fields)
                filterContr.Rules.Add(field.CreateRule());

            var frm = new frmFilter(filterContr);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                Mngr = filterContr.ApplyFilter(); // Заменяем менеджер
                MainForm.RefreshList(); // Обновляем UI
            }
        }
    }
}
