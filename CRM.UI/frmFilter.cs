using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRM.Domain.Views;
using CRM.AppLogic;
using System.Windows.Forms;
using System.ComponentModel;

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
            this.Size = new System.Drawing.Size(550, 600);

            // Динамически создаем контролы для каждого правила
            int y = 10;
            int index = 0;
            foreach (var rule in contr.Rules)
            {
                if (rule is StrRule)
                {
                    var comp = new frmCompStr(rule.FieldName, index, y);
                    _fieldControls.Add(comp);
                    this.Controls.Add(comp);
                    y += 30;
                }
                // (Для дат аналогично создается frmCompVal)
                index++;
            }

            var btnApply = new Button() { Text = "Применить", Top = y + 10, Left = 10, Width = 100 };
            btnApply.Click += BtnApply_Click;
            this.Controls.Add(btnApply);
        }

        private void BtnApply_Click(object sender, System.EventArgs e)
        {
            int index = 0;
            foreach (var rule in Controller.Rules)
            {
                if (rule is StrRule strRule && _fieldControls[index] is frmCompStr uiComp)
                {
                    strRule.IsEnabled = !uiComp.ChkEnable.Checked;
                    strRule.FilterText = uiComp.TxtValue.Text;
                    // (Выбор операции Contains/NoContains)
                }
                index++;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
