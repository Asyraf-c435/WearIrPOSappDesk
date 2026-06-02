using System;
using System.Windows.Forms;

namespace KasirWearIt
{
    public partial class FormPenyesuaianStokPopup : Form
    {
        private int stokGudangAwal, stokSistemAwal;
        private int stokGudangSekarang, stokSistemSekarang;
        private string namaProduk;
        private int idOutlet;
        private string kodeProduk;

        private Label lblGudang, lblSistem;
        private NumericUpDown nudGudang, nudSistem;
        private Button btnApply, btnCancel;

        public FormPenyesuaianStokPopup(int idOutlet, string kodeProduk, string namaProduk, int stokGudang, int stokSistem)
        {
            this.idOutlet = idOutlet;
            this.kodeProduk = kodeProduk;
            this.namaProduk = namaProduk;
            this.stokGudangAwal = stokGudang;
            this.stokSistemAwal = stokSistem;
            this.stokGudangSekarang = stokGudang;
            this.stokSistemSekarang = stokSistem;

            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = $"Penyesuaian Stok - {namaProduk}";
            this.Size = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblInfo = new Label()
            {
                Text = "Gunakan tombol +/- untuk menambah/mengurangi stok.\nPerubahan otomatis mempengaruhi stok yang lain (transfer).",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Italic),
                ForeColor = System.Drawing.Color.Gray
            };
            this.Controls.Add(lblInfo);

            // Stok Gudang
            lblGudang = new Label()
            {
                Text = "Stok Gudang:",
                Location = new System.Drawing.Point(20, 70),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(lblGudang);

            nudGudang = new NumericUpDown()
            {
                Location = new System.Drawing.Point(150, 67),
                Width = 80,
                Minimum = 0,
                Maximum = 999999,
                Increment = 1,
                ReadOnly = false,
                Font = new System.Drawing.Font("Segoe UI", 10)
            };
            nudGudang.ValueChanged += NudGudang_ValueChanged;
            this.Controls.Add(nudGudang);

            // Stok Sistem
            lblSistem = new Label()
            {
                Text = "Stok Sistem:",
                Location = new System.Drawing.Point(20, 110),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(lblSistem);

            nudSistem = new NumericUpDown()
            {
                Location = new System.Drawing.Point(150, 107),
                Width = 80,
                Minimum = 0,
                Maximum = 999999,
                Increment = 1,
                ReadOnly = false,
                Font = new System.Drawing.Font("Segoe UI", 10)
            };
            nudSistem.ValueChanged += NudSistem_ValueChanged;
            this.Controls.Add(nudSistem);

            // Tombol
            btnApply = new Button()
            {
                Text = "Apply",
                Location = new System.Drawing.Point(80, 160),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += BtnApply_Click;
            this.Controls.Add(btnApply);

            btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(200, 160),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.Silver,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            nudGudang.Value = stokGudangSekarang;
            nudSistem.Value = stokSistemSekarang;
        }

        private void NudGudang_ValueChanged(object sender, EventArgs e)
        {
            int perubahan = (int)nudGudang.Value - stokGudangSekarang;
            if (perubahan != 0)
            {
                // Tambah stok gudang -> kurangi stok sistem (transfer dari sistem ke gudang)
                // Atau sebaliknya: kurangi stok gudang -> tambah stok sistem
                int newSistem = stokSistemSekarang - perubahan;
                if (newSistem < 0) newSistem = 0;
                nudSistem.Value = newSistem;
                stokGudangSekarang = (int)nudGudang.Value;
                stokSistemSekarang = (int)nudSistem.Value;
            }
            else
            {
                stokGudangSekarang = (int)nudGudang.Value;
            }
        }

        private void NudSistem_ValueChanged(object sender, EventArgs e)
        {
            int perubahan = (int)nudSistem.Value - stokSistemSekarang;
            if (perubahan != 0)
            {
                // Tambah stok sistem -> kurangi stok gudang (transfer dari gudang ke sistem)
                int newGudang = stokGudangSekarang - perubahan;
                if (newGudang < 0) newGudang = 0;
                nudGudang.Value = newGudang;
                stokSistemSekarang = (int)nudSistem.Value;
                stokGudangSekarang = (int)nudGudang.Value;
            }
            else
            {
                stokSistemSekarang = (int)nudSistem.Value;
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            bool berhasil = Database.DatabaseConnection.UpdateStockBoth(idOutlet, kodeProduk, stokSistemSekarang, stokGudangSekarang);
            if (berhasil)
            {
                MessageBox.Show("Stok berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Gagal menyimpan perubahan stok.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}