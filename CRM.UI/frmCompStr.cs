using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRM.Domain.Views;
using CRM.AppLogic;

namespace CRM.UI
{
    public partial class frmCompStr : UserControl
    {
        public TextBox TxtValue { get; private set; }
        public RadioButton RbContains { get; private set; }
        public RadioButton RbNoContains { get; private set; }
        public CheckBox ChkEnable { get; private set; }

        public frmCompStr(string fieldName, int index, int top)
        {
            this.Location = new System.Drawing.Point(10, top);
            this.Size = new System.Drawing.Size(500, 25);
            this.Name = $"frmCompStr_{index}"; // Уникальное имя

            var lbl = new Label() { Text = fieldName, Left = 5, Top = 3, Width = 100 };
            TxtValue = new TextBox() { Name = "txtField_" + index, Left = 110, Top = 0, Width = 150 }; ChkEnable = new CheckBox() { Text = "нет", Left = 270, Top = 2 };
            RbContains = new RadioButton() { Text = "Содержит", Left = 320, Top = 2, Checked = true };
            RbNoContains = new RadioButton() { Text = "Не содержит", Left = 410, Top = 2 };

            // Логика инверсии "нет" (если галочка стоит, фильтр выключен)
            ChkEnable.CheckedChanged += (s, e) => { TxtValue.Enabled = !ChkEnable.Checked; };

            this.Controls.AddRange(new Control[] { lbl, TxtValue, ChkEnable, RbContains, RbNoContains });
        }
    }
}
