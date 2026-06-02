using System;
using System.Drawing;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class Laporan
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
        private Panel pnlHeader = null!;
        private Label lblTitle = null!;
        
        // Filter Tanggal
        private DateTimePicker dtpDari = null!;
        private Label lblTo = null!;
        private DateTimePicker dtpSampai = null!;
        private Button btnTerapkan = null!;
        private Button btnPrint = null!;

        // Ringkasan Teks
        private TextBox txtTotalPenjualan = null!;
        private TextBox txtTotalDiskon = null!;
        private TextBox txtTotalBersih = null!;

        // Tabel Detail
        private DataGridView dgvLaporan = null!;

        private void InitializeComponentCustom()
        {
            this.Text = "Laporan Penjualan - WEAR IT FASHION";
            this.Size = new Size(850, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // ==========================================
            // HEADER PINK
            // ==========================================
            pnlHeader = new Panel() { Height = 60, Dock = DockStyle.Top, BackColor = Color.FromArgb(224, 158, 172), BorderStyle = BorderStyle.FixedSingle };
            lblTitle = new Label() { Text = "Ringkasan Penjualan", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // ==========================================
            // FILTER TANGGAL & TOMBOL PRINT
            // ==========================================
            dtpDari = new DateTimePicker() { Format = DateTimePickerFormat.Short, Location = new Point(40, 90), Width = 120, Font = new Font("Segoe UI", 10) };
            lblTo = new Label() { Text = "To", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(170, 93), AutoSize = true };
            dtpSampai = new DateTimePicker() { Format = DateTimePickerFormat.Short, Location = new Point(200, 90), Width = 120, Font = new Font("Segoe UI", 10) };
            
            btnTerapkan = new Button() { Text = "Terapkan", Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.FromArgb(224, 158, 172), ForeColor = Color.White, Location = new Point(340, 88), Size = new Size(90, 30), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnTerapkan.FlatAppearance.BorderSize = 0;
   

            btnPrint = new Button() { Text = "🖨️ Cetak / Ekspor PDF", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.White, Location = new Point(610, 85), Size = new Size(180, 35), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnPrint.Click += btnPrint_Click;

            this.Controls.Add(dtpDari); this.Controls.Add(lblTo); this.Controls.Add(dtpSampai);
            this.Controls.Add(btnTerapkan); this.Controls.Add(btnPrint);

            // ==========================================
            // FIELD RINGKASAN (Total Penjualan, Diskon, TOTAL)
            // ==========================================
            Label lblLabelPenjualan = new Label() { Text = "Total Penjualan", Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, Location = new Point(60, 160), AutoSize = true };
            txtTotalPenjualan = new TextBox() { ReadOnly = true, Font = new Font("Segoe UI", 11), TextAlign = HorizontalAlignment.Right, Location = new Point(540, 157), Width = 250, BackColor = Color.White };

            Label lblLabelDiskon = new Label() { Text = "Total Diskon", Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, Location = new Point(60, 195), AutoSize = true };
            txtTotalDiskon = new TextBox() { ReadOnly = true, Font = new Font("Segoe UI", 11), TextAlign = HorizontalAlignment.Right, Location = new Point(540, 192), Width = 250, BackColor = Color.White };

            Label lblLabelTotal = new Label() { Text = "TOTAL", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Black, Location = new Point(60, 245), AutoSize = true };
            txtTotalBersih = new TextBox() { ReadOnly = true, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = HorizontalAlignment.Right, Location = new Point(540, 242), Width = 250, BackColor = Color.White };

            this.Controls.Add(lblLabelPenjualan); this.Controls.Add(txtTotalPenjualan);
            this.Controls.Add(lblLabelDiskon); this.Controls.Add(txtTotalDiskon);
            this.Controls.Add(lblLabelTotal); this.Controls.Add(txtTotalBersih);

            // ==========================================
            // TABEL DETAIL BARANG TERJUAL (READ ONLY)
            // ==========================================
            Label lblDetailTitle = new Label() { Text = "Detail Barang Terjual", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(40, 310), AutoSize = true };
            this.Controls.Add(lblDetailTitle);

            dgvLaporan = new DataGridView();
            dgvLaporan.Location = new Point(40, 340);
            dgvLaporan.Size = new Size(750, 290);
            dgvLaporan.BackgroundColor = Color.White;
            dgvLaporan.ReadOnly = true;                 // READ ONLY
            dgvLaporan.AllowUserToAddRows = false;      // Tidak bisa tambah baris manual
            dgvLaporan.RowHeadersVisible = false;
            dgvLaporan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLaporan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLaporan.BorderStyle = BorderStyle.Fixed3D;

            // Inisialisasi Kolom
            dgvLaporan.ColumnCount = 5;
            dgvLaporan.Columns[0].Name = "Tanggal";
            dgvLaporan.Columns[1].Name = "Jam";
            dgvLaporan.Columns[2].Name = "Nama Barang";
            dgvLaporan.Columns[3].Name = "Qty";
            dgvLaporan.Columns[4].Name = "Subtotal";

            this.Controls.Add(dgvLaporan);
        }
    }
}