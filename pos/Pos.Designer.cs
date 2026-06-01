using System;
using System.Drawing;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class Pos
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

        // Deklarasi Elemen Struktur UI
        private Panel pnlSidebar = null!;
        private Panel pnlMain = null!;
        private Panel pnlHeader = null!;
        private Panel pnlContent = null!;
        private Panel pnlRightOrder = null!;
        private Panel pnlCenter = null!;
        
        // MENGGUNAKAN FLOWLAYOUTPANEL UNTUK LIST ORDER (BUKAN TABEL LAGI)
        private FlowLayoutPanel flpOrderList = null!;
        private FlowLayoutPanel flpProducts = null!;
        private TextBox txtSearch = null!;

        private Label lblTotalBarang = null!;
        private Label lblTotalBayar = null!;
        private Panel pnlFooter = null!;
        private Label lblFooterSalam = null!;
        private Label lblFooterNama = null!;

        private void InitializeComponentCustom()
        {
            // 1. PENGATURAN JENDELA UTAMA
            this.Text = "Kasir Wear It Fashion";
            this.Size = new Size(1100, 650); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // =========================================================
            // 2. SIDEBAR (Kiri - Abu-abu)
            // =========================================================
            pnlSidebar = new Panel() { Width = 65, Dock = DockStyle.Left, BackColor = Color.FromArgb(210, 210, 210), BorderStyle = BorderStyle.FixedSingle };

            // Tombol Laporan (Dulu btnTrash)
            Button btnLaporan = new Button() { Text = "📊", Font = new Font("Segoe UI", 16), FlatStyle = FlatStyle.Flat, Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
            btnLaporan.FlatAppearance.BorderSize = 0;
            btnLaporan.Click += BtnLaporan_Click;

            Button btnPos = new Button() { Text = "📠", Font = new Font("Segoe UI", 16), FlatStyle = FlatStyle.Flat, Dock = DockStyle.Top, Height = 65, BackColor = Color.FromArgb(190, 190, 190) };
            btnPos.FlatAppearance.BorderSize = 0;

            Button btnLock = new Button() { Text = "🔒", Font = new Font("Segoe UI", 16), FlatStyle = FlatStyle.Flat, Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
            btnLock.FlatAppearance.BorderSize = 0;
            btnLock.Click += BtnLock_Click;

            pnlSidebar.Controls.Add(btnLaporan);
            pnlSidebar.Controls.Add(btnPos);
            pnlSidebar.Controls.Add(btnLock);

            // =========================================================
            // 3. MAIN AREA (Kanan dari Sidebar)
            // =========================================================
            pnlMain = new Panel() { Dock = DockStyle.Fill };
            
            // Header Pink (Atas)
            pnlHeader = new Panel() { Height = 50, Dock = DockStyle.Top, BackColor = Color.FromArgb(224, 158, 172), BorderStyle = BorderStyle.FixedSingle };
            Label lblTitle = new Label() { Text = "📠 Kasir Wear It Fashion", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60), AutoSize = true, Location = new Point(10, 14) };
            pnlHeader.Controls.Add(lblTitle);

            // Area Konten
            pnlContent = new Panel() { Dock = DockStyle.Fill };

            // =========================================================
            // 4. PANEL ORDER & KONTROL (Kanan)
            // =========================================================
            pnlRightOrder = new Panel() { Width = 350, Dock = DockStyle.Right, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblOrderHeader = new Label() { Text = "ORDER", Dock = DockStyle.Top, Height = 45, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 11, FontStyle.Bold), BackColor = Color.FromArgb(174, 225, 225) };

            // Panel Ringkasan Bawah (Diperkecil karena Input Qty pindah ke dalam Card)
            Panel pnlOrderBottom = new Panel() { Dock = DockStyle.Bottom, Height = 130, BackColor = Color.FromArgb(250, 250, 250) };
            pnlOrderBottom.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlOrderBottom.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            lblTotalBarang = new Label() { Text = "Total Produk: 0 item", Font = new Font("Segoe UI", 10, FontStyle.Regular), Location = new Point(15, 15), AutoSize = true };
            lblTotalBayar = new Label() { Text = "Total: Rp 0", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(232, 65, 24), Location = new Point(15, 40), AutoSize = true };

            Button btnBayar = new Button() { Text = "Bayar", Dock = DockStyle.Bottom, Height = 50, Font = new Font("Segoe UI", 12, FontStyle.Bold), BackColor = Color.FromArgb(174, 225, 225), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnBayar.FlatAppearance.BorderSize = 0;
            btnBayar.Click += BtnBayar_Click;

            pnlOrderBottom.Controls.Add(lblTotalBarang);
            pnlOrderBottom.Controls.Add(lblTotalBayar);
            pnlOrderBottom.Controls.Add(btnBayar);

            // TAMPILAN BARU: Tempat list order dalam bentuk Card Kotak
            flpOrderList = new FlowLayoutPanel() { Dock = DockStyle.Fill, BackColor = Color.White, AutoScroll = true, Padding = new Padding(5) };

            pnlRightOrder.Controls.Add(flpOrderList);
            pnlRightOrder.Controls.Add(pnlOrderBottom);
            pnlRightOrder.Controls.Add(lblOrderHeader);

         // =========================================================
            // 5. PANEL PRODUK & PENCARIAN (Tengah)
            // =========================================================
            pnlCenter = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(230, 235, 235) };

            Panel pnlSearch = new Panel() { Dock = DockStyle.Top, Height = 35, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
        
            
            txtSearch = new TextBox() { 
                Font = new Font("Segoe UI", 10, FontStyle.Italic), 
                BorderStyle = BorderStyle.None, 
                Location = new Point(10, 8),
                Width = 350,
                Text = "Cari nama barang di sini...", 
                ForeColor = Color.Gray 
            };

            txtSearch.Enter += (s, e) => {
                if (txtSearch.Text == "Cari nama barang di sini...") {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                    txtSearch.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                }
            };

            txtSearch.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text)) {
                    txtSearch.Text = "Cari nama barang di sini...";
                    txtSearch.ForeColor = Color.Gray;
                    txtSearch.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                }
            };

            Label lblSearchIcon = new Label() { Text = "🔍", Font = new Font("Segoe UI", 12), Dock = DockStyle.Right, Width = 35, TextAlign = ContentAlignment.MiddleCenter };

            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(lblSearchIcon);

            // 👇 INI BAGIAN YANG KEMARIN TERHAPUS & BIKIN ERROR 👇
            flpProducts = new FlowLayoutPanel() { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10), BackColor = Color.FromArgb(240, 245, 245) };
            
            pnlCenter.Controls.Add(flpProducts);
            pnlCenter.Controls.Add(pnlSearch);
            // 👆 ================================================= 👆

            // =========================================================
            // 6. MERAKIT SEMUA PANEL
            // =========================================================
            pnlContent.Controls.Add(pnlCenter);
            pnlContent.Controls.Add(pnlRightOrder);
            
            pnlMain.Controls.Add(pnlContent);
            pnlMain.Controls.Add(pnlHeader);

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);

            // =========================================================
            // 7. FOOTER BAWAH (Sapaan Waktu + Nama Kasir)
            // =========================================================
            pnlFooter = new Panel() { Dock = DockStyle.Bottom, Height = 40, BackColor = Color.FromArgb(44, 62, 80) };
            pnlFooter.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlFooter.ClientRectangle, Color.FromArgb(34, 52, 70), ButtonBorderStyle.Solid); };

            lblFooterSalam = new Label()
            {
                Text = "Selamat Pagi",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblFooterNama = new Label()
            {
                Text = "👤 -",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(200, 200, 200),
                Dock = DockStyle.Right,
                Width = 220,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 15, 0)
            };

            pnlFooter.Controls.Add(lblFooterNama);
            pnlFooter.Controls.Add(lblFooterSalam);
            this.Controls.Add(pnlFooter);
        }
    }
}