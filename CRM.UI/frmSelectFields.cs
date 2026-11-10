using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CRM.Domain.Views;

namespace CRM.UI
{
    public class frmSelectFields : Form
    {
        private CheckedListBox _clbFields;
        public List<Field> SelectedFields { get; private set; }

        public frmSelectFields(List<Field> allFields, List<Field> currentFields)
        {
            this.Text = "Настройка столбцов";
            this.Size = new System.Drawing.Size(300, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            _clbFields = new CheckedListBox()
            {
                Dock = DockStyle.Top,
                Height = 300,
                CheckOnClick = true
            };

            // Заполняем список всеми доступными полями
            foreach (var field in allFields)
            {
                // Если поле уже отображается - ставим галочку
                bool isChecked = currentFields.Any(f => f.GetType() == field.GetType());
                _clbFields.Items.Add(field, isChecked);
            }

            // Кнопка "Применить"
            var btnOk = new Button() { Text = "Применить", Dock = DockStyle.Bottom, Height = 40 };
            btnOk.Click += (s, e) =>
            {
                // Собираем выбранные поля
                SelectedFields = new List<Field>();
                foreach (var item in _clbFields.CheckedItems)
                {
                    SelectedFields.Add((Field)item);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(btnOk);
            this.Controls.Add(_clbFields);
        }
    }
}