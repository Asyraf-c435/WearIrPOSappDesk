using System;
using System.Drawing;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class Master
    {
        private Label lblTitle;
        private TextBox txtKodeProduk, txtNamaProduk, txtHargaBeli, txtHargaJual, txtStokAwal;
        private Button btnSimpan, btnBatal;

        private void InitializeComponent()
        {
            this.Text = "Master Produk";
            this.Size = new Size(550, 480); // sedikit lebar untuk stok
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "Tambah / Edit Produk",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            int y = 80;
            int labelX = 30;
            int fieldX = 130;

            // Kode Produk
            AddLabelAndTextBox("Kode Produk:", ref y, labelX, fieldX, 200, out txtKodeProduk);
            // Nama Produk
            AddLabelAndTextBox("Nama Produk:", ref y, labelX, fieldX, 280, out txtNamaProduk);
            // Harga Beli
            AddLabelAndTextBox("Harga Beli:", ref y, labelX, fieldX, 200, out txtHargaBeli);
            // Harga Jual
            AddLabelAndTextBox("Harga Jual:", ref y, labelX, fieldX, 200, out txtHargaJual);
            // Stok Awal (Gudang) - khusus diatur lebih lebar
            Label lblStok = new Label { Text = "Stok Awal (Gudang):", Location = new Point(labelX, y), AutoSize = true };
            txtStokAwal = new TextBox
            {
                Location = new Point(fieldX, y - 3),
                Width = 300,
                MaxLength = 9,
                TextAlign = HorizontalAlignment.Right,
                Text = "0"
            };
            this.Controls.Add(lblStok);
            this.Controls.Add(txtStokAwal);
            y += 40;

            y += 40; // spasi sebelum tombol

            btnSimpan = new Button
            {
                Text = "Simpan",
                Location = new Point(180, y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(174, 225, 225),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSimpan.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSimpan);

            btnBatal = new Button
            {
                Text = "Batal",
                Location = new Point(300, y),
                Size = new Size(100, 35),
                BackColor = Color.Silver,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White
            };
            btnBatal.FlatAppearance.BorderSize = 0;
            btnBatal.Click += (s, e) => this.Close();
            this.Controls.Add(btnBatal);

            void AddLabelAndTextBox(string labelText, ref int yPos, int lx, int fx, int fw, out TextBox tb)
            {
                Label lbl = new Label { Text = labelText, Location = new Point(lx, yPos), AutoSize = true };
                tb = new TextBox { Location = new Point(fx, yPos - 3), Width = fw };
                this.Controls.Add(lbl);
                this.Controls.Add(tb);
                yPos += 40;
            }
        }
    }
}