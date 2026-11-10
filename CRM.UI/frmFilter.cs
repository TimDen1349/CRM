using CRM.AppLogic;
using CRM.Domain.Filtering;
using CRM.Domain.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRM.Domain.Filtering;
using CRM.Domain.Views;
using CRM.AppLogic;

namespace CRM.UI
{
    public partial class frmFilter : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterContr Controller { get; set; }
        private List<UserControl> _fieldControls = new List<UserControl>();

        public frmFilter(FilterContr contr)
        {
            this.Controller = contr;
            this.Text = "Настройка фильтра";
            this.Size = new System.Drawing.Size(650, 600); // Сделали форму чуть шире

            int y = 10;
            int index = 0;

            foreach (var rule in contr.Rules)
            {
                UserControl comp = null;

                if (rule is StrRule)
                {
                    comp = new frmCompStr(rule.FieldName, index, y);
                }
                else if (rule is DateRule || rule is IntRule) // <--- ДОБАВИЛИ IntRule
                {
                    comp = new frmCompVal(rule.FieldName, index, y);
                }

                if (comp != null)
                {
                    _fieldControls.Add(comp);
                    this.Controls.Add(comp);
                    y += 30;
                }
                index++;
            }

            // Добавьте Name для кнопки применения
            var btnApply = new Button() { Name = "btnApply", Text = "Применить", Top = y + 10, Left = 10, Width = 100 };
            btnApply.Click += BtnApply_Click;
            this.Controls.Add(btnApply);

            // Настраиваем высоту формы динамически, чтобы все влезло
            this.Height = y + 100;
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            int index = 0;
            foreach (var rule in Controller.Rules)
            {
                if (_fieldControls[index] is frmCompStr strComp && rule is StrRule strRule)
                {
                    strRule.IsEnabled = !strComp.ChkEnable.Checked;
                    strRule.FilterText = strComp.TxtValue.Text;

                    if (strComp.RbContains.Checked) strRule.Operation = LogExpEnum.Contains;
                    else if (strComp.RbNoContains.Checked) strRule.Operation = LogExpEnum.NoContains;
                }
                else if (_fieldControls[index] is frmCompVal valComp)
                {
                    if (rule is DateRule dateRule)
                    {
                        dateRule.IsEnabled = !valComp.ChkEnable.Checked;
                        dateRule.FilterDate = valComp.DtpValue.Value;

                        if (valComp.RbGT.Checked) dateRule.Operation = LogExpEnum.GT;
                        else if (valComp.RbLT.Checked) dateRule.Operation = LogExpEnum.LT;
                        else dateRule.Operation = LogExpEnum.EQ;
                    }
                    else if (rule is IntRule intRule)
                    {
                        intRule.IsEnabled = !valComp.ChkEnable.Checked;
                        // Пытаемся распарсить значение из DateTimePicker (или замените на TextBox для чисел)
                        intRule.FilterValue = valComp.DtpValue.Value.Day;
                    }
                }
                index++;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
