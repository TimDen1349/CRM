using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Domain.Views;
using CRM.AppLogic;
using System.Windows.Forms;
using System.Drawing; // Для Size

namespace CRM.UI
{
    public partial class frmMain : Form
    {
        private MainContr _contr;
        public ListView LvFirms { get; private set; }

        public frmMain(MainContr contr)
        {
            _contr = contr;
            this.Text = "CRM Система";
            this.Size = new Size(800, 600);

            // Настраиваем ListView
            LvFirms = new ListView() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true };
            this.Controls.Add(LvFirms);

            // Кнопка фильтра
            var btnFilter = new Button() { Text = "Фильтр", Dock = DockStyle.Top };
            btnFilter.Click += (s, e) => _contr.ShowFilter();
            this.Controls.Add(btnFilter);

            RefreshList();
        }

        public void RefreshList()
        {
            LvFirms.Items.Clear();
            LvFirms.Columns.Clear();

            // Создаем колонки на основе FirmVw
            foreach (var field in _contr.Mngr.ViewSettings.Fields)
                LvFirms.Columns.Add(field.Name);

            // Заполняем строками
            foreach (var firm in _contr.Mngr.Firms)
            {
                var item = new ListViewItem();
                for (int i = 0; i < _contr.Mngr.ViewSettings.Fields.Count; i++)
                {
                    var val = _contr.Mngr.ViewSettings.Fields[i].GetValue(firm) ?? "";
                    if (i == 0) item.Text = val;
                    else item.SubItems.Add(val);
                }
                LvFirms.Items.Add(item);
            }

            // --- АВТОМАТИЧЕСКАЯ НАСТРОЙКА ШИРИНЫ СТОЛБЦОВ ---
            if (LvFirms.Columns.Count == 0) return;

            for (int i = 0; i < LvFirms.Columns.Count; i++)
            {
                // 1. Замеряем ширину заголовка столбца
                int maxWidth = TextRenderer.MeasureText(LvFirms.Columns[i].Text, LvFirms.Font).Width + 20;

                // 2. Проходимся по всем ячейкам этого столбца и ищем самую длинную
                foreach (ListViewItem item in LvFirms.Items)
                {
                    string cellText = (i == 0) ? item.Text : item.SubItems[i].Text;
                    int cellWidth = TextRenderer.MeasureText(cellText, LvFirms.Font).Width + 20;

                    if (cellWidth > maxWidth)
                        maxWidth = cellWidth;
                }

                // 3. Устанавливаем ширину (ограничиваем минимум 80px, максимум 300px)
                LvFirms.Columns[i].Width = Math.Max(80, Math.Min(maxWidth, 300));
            }
        }
    }
}
