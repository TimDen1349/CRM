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
            this.Size = new System.Drawing.Size(600, 25); // Увеличили ширину
            this.Name = $"frmCompStr_{index}";

            var lbl = new Label() { Text = fieldName, Left = 5, Top = 3, Width = 100 };
            TxtValue = new TextBox() { Left = 110, Top = 0, Width = 150 };
            ChkEnable = new CheckBox() { Text = "нет", Left = 270, Top = 2, Width = 50 };

            // Сдвинули радиокнопки правее, чтобы не наезжали
            RbContains = new RadioButton() { Text = "Содержит", Left = 330, Top = 2, Checked = true, Width = 100 };
            RbNoContains = new RadioButton() { Text = "Не содержит", Left = 440, Top = 2, Width = 120 };

            ChkEnable.CheckedChanged += (s, e) => { TxtValue.Enabled = !ChkEnable.Checked; };

            this.Controls.AddRange(new Control[] { lbl, TxtValue, ChkEnable, RbContains, RbNoContains });
        }
    }
}
