using System;
using System.Windows.Forms;
using CRM.Domain;

namespace CRM.UI
{
    public class frmAddEditFirm : Form
    {
        private Firm _firm;
        private bool _isEditMode;

        public TextBox TxtName { get; private set; }
        public TextBox TxtCountry { get; private set; }
        public TextBox TxtRegion { get; private set; }
        public TextBox TxtTown { get; private set; }
        public TextBox TxtStreet { get; private set; }
        public TextBox TxtPostInx { get; private set; }
        public TextBox TxtEmail { get; private set; }
        public TextBox TxtWeb { get; private set; }
        public DateTimePicker DtpDateIn { get; private set; }

        // Поля для добавления контакта
        private TextBox _txtContactDescr;
        private Button _btnAddContact;

        public frmAddEditFirm(Firm firm)
        {
            _firm = firm;
            _isEditMode = firm != null;

            this.Text = _isEditMode ? "Редактирование фирмы" : "Добавление фирмы";
            this.Size = new System.Drawing.Size(450, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int y = 10;

            // Функция для быстрого создания поля с подписью
            void AddField(string label, out TextBox textBox)
            {
                var lbl = new Label() { Text = label, Left = 20, Top = y + 3, Width = 100 };
                textBox = new TextBox() { Left = 130, Top = y, Width = 250 };
                this.Controls.Add(lbl);
                this.Controls.Add(textBox);
                y += 30;
            }

            AddField("Название:", out TextBox txtName);
            AddField("Страна:", out TextBox txtCountry);
            AddField("Регион:", out TextBox txtRegion);
            AddField("Город:", out TextBox txtTown);
            AddField("Улица:", out TextBox txtStreet);
            AddField("Индекс:", out TextBox txtPostInx);
            AddField("Email:", out TextBox txtEmail);
            AddField("Сайт (Web):", out TextBox txtWeb);

            TxtName = txtName; TxtCountry = txtCountry; TxtRegion = txtRegion;
            TxtTown = txtTown; TxtStreet = txtStreet; TxtPostInx = txtPostInx;
            TxtEmail = txtEmail; TxtWeb = txtWeb;

            // Дата ввода
            var lblDate = new Label() { Text = "Дата ввода:", Left = 20, Top = y + 3, Width = 100 };
            DtpDateIn = new DateTimePicker() { Left = 130, Top = y, Width = 250, Format = DateTimePickerFormat.Short };
            this.Controls.Add(lblDate);
            this.Controls.Add(DtpDateIn);
            y += 30;

            // --- Секция контактов (только для режима редактирования) ---
            if (_isEditMode)
            {
                var grpContacts = new GroupBox() { Text = "Добавить контакт", Left = 10, Top = y, Width = 400, Height = 100 };

                var lblDescr = new Label() { Text = "Описание:", Left = 10, Top = 25, Width = 60 };
                _txtContactDescr = new TextBox() { Left = 70, Top = 22, Width = 200, Text = "Звонок" };

                var lblDate1 = new Label() { Text = "Дата:", Left = 10, Top = 55, Width = 60 };
                var dtpContactDate = new DateTimePicker() { Left = 70, Top = 52, Width = 200, Format = DateTimePickerFormat.Short, Value = DateTime.Now };

                _btnAddContact = new Button() { Text = "Добавить", Left = 280, Top = 22, Width = 80 };

                _btnAddContact.Click += (s, e) =>
                {
                    var main = _firm.GetMain();
                    if (main != null)
                    {
                        // Используем дату из DateTimePicker!
                        var contact = new Contact(dtpContactDate.Value, dtpContactDate.Value, _txtContactDescr.Text, "Инфо", new ContType("Звонок", ""));
                        main.AddContact(contact);

                        MessageBox.Show("Контакт добавлен! Нажмите 'Сохранить', чтобы обновить таблицу.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _txtContactDescr.Text = "";
                    }
                };

                grpContacts.Controls.Add(lblDescr);
                grpContacts.Controls.Add(_txtContactDescr);
                grpContacts.Controls.Add(lblDate1);
                grpContacts.Controls.Add(dtpContactDate);
                grpContacts.Controls.Add(_btnAddContact);

                this.Controls.Add(grpContacts);
                y += 110;
            }

            // Кнопки Сохранить/Отмена
            var btnSave = new Button() { Text = "Сохранить", Left = 130, Top = y, Width = 100 };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button() { Text = "Отмена", Left = 240, Top = y, Width = 100 };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            // Заполнение полей, если это режим редактирования
            if (_isEditMode)
            {
                TxtName.Text = _firm.Name;
                TxtCountry.Text = _firm.Country;
                TxtRegion.Text = _firm.Region;
                TxtTown.Text = _firm.Town;
                TxtStreet.Text = _firm.Street;
                TxtPostInx.Text = _firm.PostInx;
                TxtEmail.Text = _firm.Email;
                TxtWeb.Text = _firm.Web;
                DtpDateIn.Value = _firm.DateIn;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Название фирмы не может быть пустым!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isEditMode)
            {
                // Редактируем существующую
                _firm.Name = TxtName.Text;
                _firm.Country = TxtCountry.Text;
                _firm.Region = TxtRegion.Text;
                _firm.Town = TxtTown.Text;
                _firm.Street = TxtStreet.Text;
                _firm.PostInx = TxtPostInx.Text;
                _firm.Email = TxtEmail.Text;
                _firm.Web = TxtWeb.Text;
                _firm.DateIn = DtpDateIn.Value;
            }
            else
            {
                // Создаем новую через фабрику (как требует лаба №3)
                var mainType = new SbFirmType("Главный офис", true);
                var newFirm = FirmFactory.Instance.Create(
                    TxtName.Text, TxtCountry.Text, TxtRegion.Text, TxtTown.Text,
                    TxtStreet.Text, TxtPostInx.Text, DtpDateIn.Value,
                    TxtEmail.Text, TxtWeb.Text, mainType
                );

                // Возвращаем созданную фирму через Tag, чтобы контроллер ее забрал
                this.Tag = newFirm;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}