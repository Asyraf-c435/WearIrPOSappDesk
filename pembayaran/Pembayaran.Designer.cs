using System;
using System.Drawing;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class Pembayaran
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Deklarasi Komponen UI
        private DataGridView dgvPesanan = null!;
        private Label lblSubTotalValue = null!;
        private TextBox txtDiskonPersen = null!;
        private Label lblGrandTotalValue = null!;
        private TextBox txtUangDibayar = null!;
        private Label lblKembalianValue = null!;
        private Button btnCash = null!;
        private Button btnQris = null!;
        private Button btnBayar = null!;
        private Button btnBatal = null!;

        private void InitializeComponentCustom()
        {
            this.Text = "Sistem Pembayaran POS";
            this.Size = new Size(1000, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White; // Dominan Putih Bersih sesuai tema POS

            // ==========================================
            // HEADER PANEL (Pink Kalem sejalan dengan Header POS Anda)
            // ==========================================
            Panel pnlHeader = new Panel() { Height = 55, Dock = DockStyle.Top, BackColor = Color.FromArgb(224, 158, 172) };
            Label lblTitle = new Label() { Text = "💳 PROSES PEMBAYARAN", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60), AutoSize = true, Location = new Point(20, 16) };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // MAIN CONTAINER (Padding agar rapi)
            Panel pnlMain = new Panel() { Dock = DockStyle.Fill, Padding = new Padding(25), BackColor = Color.White };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            // ==========================================
            // BAGIAN ATAS: TABEL PESANAN
            // ==========================================
            GroupBox gbTabel = new GroupBox() { Text = " Rincian Belanja Transaksi ", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(120, 60, 80), Dock = DockStyle.Top, Height = 250 };
            
            dgvPesanan = new DataGridView();
            dgvPesanan.Dock = DockStyle.Fill;
            dgvPesanan.BackgroundColor = Color.White;
            dgvPesanan.BorderStyle = BorderStyle.Fixed3D;
            dgvPesanan.AllowUserToAddRows = false;
            dgvPesanan.AllowUserToDeleteRows = false;
            dgvPesanan.RowHeadersVisible = false;
            dgvPesanan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPesanan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPesanan.RowTemplate.Height = 35;
            dgvPesanan.ColumnHeadersHeight = 35;
            dgvPesanan.EnableHeadersVisualStyles = false;
            
            // Mengunci agar tidak masuk ke mode pengetikan teks saat sel diklik (tetap normal)
            dgvPesanan.EditMode = DataGridViewEditMode.EditProgrammatically;

            // Desain Header Tabel: Menyesuaikan warna header abu/pink kalem
            dgvPesanan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245); 
            dgvPesanan.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgvPesanan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPesanan.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 245, 245);

            // Definisi Kolom Tabel
            dgvPesanan.ColumnCount = 5;
            dgvPesanan.Columns[0].Name = "Kode Barang";
            dgvPesanan.Columns[1].Name = "Nama Barang";
            dgvPesanan.Columns[2].Name = "Harga";
            dgvPesanan.Columns[3].Name = "Qty";
            dgvPesanan.Columns[4].Name = "Subtotal";

            // Kolom Tombol Aksi: Edit
            DataGridViewButtonColumn btnColEdit = new DataGridViewButtonColumn();
            btnColEdit.Name = "AksiEdit";
            btnColEdit.HeaderText = "Edit";
            btnColEdit.Text = "✏️ Edit";
            btnColEdit.UseColumnTextForButtonValue = true;
            btnColEdit.FlatStyle = FlatStyle.Flat;
            btnColEdit.DefaultCellStyle.BackColor = Color.White;
            btnColEdit.DefaultCellStyle.ForeColor = Color.FromArgb(120, 60, 80);
            dgvPesanan.Columns.Add(btnColEdit);

            // Kolom Tombol Aksi: Hapus
            DataGridViewButtonColumn btnColHapus = new DataGridViewButtonColumn();
            btnColHapus.Name = "AksiHapus";
            btnColHapus.HeaderText = "Hapus";
            btnColHapus.Text = "❌ Hapus";
            btnColHapus.UseColumnTextForButtonValue = true;
            btnColHapus.FlatStyle = FlatStyle.Flat;
            btnColHapus.DefaultCellStyle.BackColor = Color.White;
            btnColHapus.DefaultCellStyle.ForeColor = Color.IndianRed;
            dgvPesanan.Columns.Add(btnColHapus);

            // Pengaturan Lebar Kolom Proposional
            dgvPesanan.Columns[0].Width = 100;
            dgvPesanan.Columns[1].Width = 220;
            dgvPesanan.Columns[2].Width = 110;
            dgvPesanan.Columns[3].Width = 60;
            dgvPesanan.Columns[4].Width = 120;
            dgvPesanan.Columns["AksiEdit"].Width = 80;
            dgvPesanan.Columns["AksiHapus"].Width = 80;

            gbTabel.Controls.Add(dgvPesanan);
            pnlMain.Controls.Add(gbTabel);

            // ==========================================
            // BAGIAN BAWAH: DATA KALKULATOR & KONTROL
            // ==========================================
            Panel pnlKalkulator = new Panel() { Dock = DockStyle.Fill, Location = new Point(25, 275), Height = 320, BackColor = Color.White };
            pnlMain.Controls.Add(pnlKalkulator);
            pnlKalkulator.BringToFront();

            int lblX = 10, valX = 180, rowY = 25;

            // 1. Tampilan Subtotal
            Label lblSubTotal = new Label() { Text = "Subtotal :", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(lblX, rowY), AutoSize = true };
            lblSubTotalValue = new Label() { Text = "Rp 180.000", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(50, 50, 50), Location = new Point(valX, rowY), AutoSize = true };
            pnlKalkulator.Controls.Add(lblSubTotal); pnlKalkulator.Controls.Add(lblSubTotalValue);
            rowY += 40;

            // 2. Field Diskon
            Label lblDiskon = new Label() { Text = "Diskon (%) :", Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, Location = new Point(lblX, rowY), AutoSize = true };
            txtDiskonPersen = new TextBox() { Font = new Font("Segoe UI", 11), Location = new Point(valX, rowY - 3), Width = 80, Text = "0", TextAlign = HorizontalAlignment.Right };
            pnlKalkulator.Controls.Add(lblDiskon); pnlKalkulator.Controls.Add(txtDiskonPersen);
            rowY += 40;

            // 3. Hasil Harga setelah Diskon (Total Akhir)
            Label lblSetelahDiskon = new Label() { Text = "Total Akhir :", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60), Location = new Point(lblX, rowY), AutoSize = true };
            lblGrandTotalValue = new Label() { Text = "Rp 180.000", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(120, 60, 80), Location = new Point(valX, rowY - 5), AutoSize = true };
            pnlKalkulator.Controls.Add(lblSetelahDiskon); pnlKalkulator.Controls.Add(lblGrandTotalValue);
            rowY += 50;

            // 4. Field Uang Dibayar
            Label lblUangDibayar = new Label() { Text = "Uang Dibayar :", Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, Location = new Point(lblX, rowY), AutoSize = true };
            txtUangDibayar = new TextBox() { Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(valX, rowY - 3), Width = 180, Text = "0", TextAlign = HorizontalAlignment.Right };
            pnlKalkulator.Controls.Add(lblUangDibayar); pnlKalkulator.Controls.Add(txtUangDibayar);
            rowY += 45;

            // 5. Kembalian
            Label lblKembalian = new Label() { Text = "Kembalian :", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(lblX, rowY), AutoSize = true };
            lblKembalianValue = new Label() { Text = "Rp 0", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.ForestGreen, Location = new Point(valX, rowY - 2), AutoSize = true };
            pnlKalkulator.Controls.Add(lblKembalian); pnlKalkulator.Controls.Add(lblKembalianValue);

            // Sisi Kanan: Pilihan Metode Pembayaran (Metode Choose / Toggle Logic)
            int kananX = 520;
            Label lblMetodeTitle = new Label() { Text = "Pilih Metode Pembayaran:", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(kananX, 25), AutoSize = true };
            pnlKalkulator.Controls.Add(lblMetodeTitle);

            // Tombol Metode Cash
            btnCash = new Button() { Text = "💵 CASH", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(kananX, 60), Size = new Size(180, 55), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCash.FlatAppearance.BorderSize = 1;
            pnlKalkulator.Controls.Add(btnCash);

            // Tombol Metode QRIS
            btnQris = new Button() { Text = "📱 QRIS", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(kananX + 190, 60), Size = new Size(180, 55), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnQris.FlatAppearance.BorderSize = 1;
            pnlKalkulator.Controls.Add(btnQris);

            // TOMBOL PROSES UTAMA (Warna Cyan Pastel khas panel ORDER POS anda)
            btnBayar = new Button() { Text = "PROSES BAYAR", Font = new Font("Segoe UI", 11, FontStyle.Bold), BackColor = Color.FromArgb(174, 225, 225), ForeColor = Color.FromArgb(50, 50, 50), Location = new Point(kananX, 150), Size = new Size(370, 48), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnBayar.FlatAppearance.BorderSize = 0;
            pnlKalkulator.Controls.Add(btnBayar);

            // TOMBOL BATAL (Abu-abu netral bawaan sistem)
            btnBatal = new Button() { Text = "BATAL TRANSAKSI", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.FromArgb(230, 230, 230), ForeColor = Color.DimGray, Location = new Point(kananX, 210), Size = new Size(370, 40), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnBatal.FlatAppearance.BorderSize = 0;
            pnlKalkulator.Controls.Add(btnBatal);

            // Integrasi ke logic penanganan event
            txtDiskonPersen.TextChanged += InputKalkulasi_Changed;
            txtUangDibayar.TextChanged += InputKalkulasi_Changed;
            btnCash.Click += BtnCash_Click;
            btnQris.Click += BtnQris_Click;
            btnBayar.Click += BtnBayar_Click;
            btnBatal.Click += (s, e) => this.Close();
            dgvPesanan.CellContentClick += dgvPesanan_CellContentClick;
        }
    }
}