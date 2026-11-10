using System;
using System.Drawing;
using System.Windows.Forms;

namespace CRM.UI
{
    public class frmCompVal : UserControl
    {
        public DateTimePicker DtpValue { get; private set; }
        public CheckBox ChkEnable { get; private set; }
        public RadioButton RbGT { get; private set; }
        public RadioButton RbLT { get; private set; }
        public RadioButton RbEQ { get; private set; }

        public frmCompVal(string fieldName, int index, int top)
        {
            this.Location = new Point(10, top);
            this.Size = new Size(600, 25);
            this.Name = $"frmCompVal_{index}";

            var lbl = new Label() { Text = fieldName, Left = 5, Top = 3, Width = 100 };
            DtpValue = new DateTimePicker() { Left = 110, Top = 0, Width = 150, Format = DateTimePickerFormat.Short };
            ChkEnable = new CheckBox() { Text = "нет", Left = 270, Top = 2, Width = 50 };

            // Для дат используем операции: =, >, <
            RbEQ = new RadioButton() { Text = "=", Left = 330, Top = 2, Checked = true, Width = 40 };
            RbGT = new RadioButton() { Text = ">", Left = 380, Top = 2, Width = 40 };
            RbLT = new RadioButton() { Text = "<", Left = 430, Top = 2, Width = 40 };

            ChkEnable.CheckedChanged += (s, e) => { DtpValue.Enabled = !ChkEnable.Checked; };

            this.Controls.AddRange(new Control[] { lbl, DtpValue, ChkEnable, RbEQ, RbGT, RbLT });
        }
    }
}