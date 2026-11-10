using CRM.AppLogic;
using CRM.Domain.Views;
using System;
using System.Collections.Generic;
using System.Drawing; // Для Size
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRM.Domain;

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

            LvFirms = new ListView() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true };

            // ДОБАВЛЯЕМ ОБРАБОТЧИК ДВОЙНОГО КЛИКА ДЛЯ РЕДАКТИРОВАНИЯ
            LvFirms.DoubleClick += (s, e) =>
            {
                if (LvFirms.SelectedItems.Count > 0)
                {
                    // Достаем объект фирмы, который мы спрятали в Tag
                    var firm = LvFirms.SelectedItems[0].Tag as Firm;
                    if (firm != null) _contr.ShowEditFirm(firm);
                }
            };

            this.Controls.Add(LvFirms);

            var topPanel = new Panel() { Dock = DockStyle.Top, Height = 40 };

            var btnFilter = new Button() { Text = "Фильтр", Left = 10, Top = 8, Width = 100 };
            btnFilter.Click += (s, e) => _contr.ShowFilter();

            var btnColumns = new Button() { Text = "Столбцы", Left = 120, Top = 8, Width = 100 };
            btnColumns.Click += (s, e) => _contr.ShowColumnSettings();

            // НОВАЯ КНОПКА ДОБАВЛЕНИЯ
            var btnAdd = new Button() { Text = "Добавить фирму", Left = 230, Top = 8, Width = 120 };
            btnAdd.Click += (s, e) => _contr.ShowAddFirm();

            topPanel.Controls.Add(btnFilter);
            topPanel.Controls.Add(btnColumns);
            topPanel.Controls.Add(btnAdd);

            this.Controls.Add(topPanel);
            topPanel.SendToBack();

            RefreshList();
        }

        public void RefreshList()
        {
            LvFirms.BeginUpdate();
            try
            {
                LvFirms.Items.Clear();
                LvFirms.Columns.Clear();

                if (_contr.Mngr.ViewSettings.Fields.Count == 0) return;

                // Создаем колонки
                foreach (var field in _contr.Mngr.ViewSettings.Fields)
                    LvFirms.Columns.Add(field.Name);

                // Заполняем строками
                foreach (var firm in _contr.Mngr.Firms)
                {
                    var item = new ListViewItem();
                    item.Tag = firm;
                    for (int i = 0; i < _contr.Mngr.ViewSettings.Fields.Count; i++)
                    {
                        var val = _contr.Mngr.ViewSettings.Fields[i].GetValue(firm) ?? "";
                        if (i == 0) item.Text = val;
                        else item.SubItems.Add(val);
                    }
                    LvFirms.Items.Add(item);
                }

                // --- РУЧНАЯ НАСТРОЙКА ШИРИНЫ (Учитываем и заголовок, и данные) ---
                for (int i = 0; i < LvFirms.Columns.Count; i++)
                {
                    // 1. Замеряем ширину заголовка столбца
                    int maxWidth = TextRenderer.MeasureText(LvFirms.Columns[i].Text, LvFirms.Font).Width + 25; // +25 на отступы

                    // 2. Проходимся по всем ячейкам этого столбца и ищем самую длинную
                    foreach (ListViewItem item in LvFirms.Items)
                    {
                        // Защита от выхода за границы, если вдруг элементов меньше, чем колонок
                        if (i > 0 && i >= item.SubItems.Count) continue;

                        string cellText = (i == 0) ? item.Text : item.SubItems[i].Text;
                        int cellWidth = TextRenderer.MeasureText(cellText, LvFirms.Font).Width + 25;

                        if (cellWidth > maxWidth)
                            maxWidth = cellWidth;
                    }

                    // 3. Устанавливаем ширину (минимум 80px, максимум 400px)
                    LvFirms.Columns[i].Width = Math.Max(80, Math.Min(maxWidth, 400));
                }
            }
            finally
            {
                LvFirms.EndUpdate();
            }
        }
    }
}
