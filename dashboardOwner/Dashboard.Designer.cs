using System;
using System.Data;               // <-- Tambahkan ini
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KasirWearIt
{
    public partial class OwnerDashboard : Form
    {
        private Panel pnlContent; // panel untuk konten dinamis

        public OwnerDashboard()
        {
            InitializeDashboard();
            AttachNavigationEvents();
        }

        private void InitializeDashboard()
        {
            // Window settings
            this.Text = "Owner Dashboard - KasirWearIt";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 245, 247);

            // ========== NAVIGASI ATAS ==========
            Panel pnlNav = new Panel()
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(224, 158, 172)
            };

            Button btnNavDashboard = CreateNavButton("📊 Dashboard", 15);
            Button btnNavTransaksi = CreateNavButton("💸 Transaksi", 175);
            Button btnNavMaster = CreateNavButton("📦 Master Data", 335);
            Button btnNavLaporan = CreateNavButton("📄 Laporan", 495); 

            pnlNav.Controls.Add(btnNavDashboard);
            pnlNav.Controls.Add(btnNavTransaksi);
            pnlNav.Controls.Add(btnNavMaster);
            pnlNav.Controls.Add(btnNavLaporan); 
            this.Controls.Add(pnlNav);
            

            // ========== PANEL KONTEN DINAMIS ==========
            pnlContent = new Panel()
            {
                Location = new Point(20, 140),
                Size = new Size(840, 540),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };
            this.Controls.Add(pnlContent);

            // Tampilkan halaman Dashboard awal
            ShowDashboardContent();
        }

       private void ShowDashboardContent()
{
    pnlContent.Controls.Clear();

    // 1. Ambil data dari database
    var todaySummary = Database.DatabaseConnection.GetSummaryToday();
    var peakLow = Database.DatabaseConnection.GetPeakLowDates(30);
    
    // Untuk chart, ambil data 7 hari terakhir
    DateTime startWeek = DateTime.Today.AddDays(-6);
    DateTime endWeek = DateTime.Today.AddDays(1);
    var dailySales = Database.DatabaseConnection.GetDailySales(startWeek, endWeek);

    int offset = 0;

    // Header
    Label lblDashboard = new Label()
    {
        Text = "Ringkasan Bisnis",
        Font = new Font("Segoe UI", 12, FontStyle.Bold),
        ForeColor = Color.HotPink,
        Location = new Point(10, offset + 10),
        AutoSize = true
    };
    Label lblWelcome = new Label()
    {
        Text = "Selamat Datang, Owner!",
        Font = new Font("Segoe UI", 16, FontStyle.Bold),
        ForeColor = Color.FromArgb(60, 60, 60),
        Location = new Point(10, offset + 35),
        AutoSize = true
    };
    pnlContent.Controls.Add(lblDashboard);
    pnlContent.Controls.Add(lblWelcome);

    // Row 1: Kartu summary (Gross Sales, Net Sales, Total Pax)
    FlowLayoutPanel flpTopCards = new FlowLayoutPanel()
    {
        Location = new Point(10, offset + 85),
        Width = 820,
        Height = 100,
        BackColor = Color.Transparent
    };
    flpTopCards.Controls.Add(CreateCard("Sales", "Gross Sales", $"Rp {todaySummary.grossSales:N0}"));
    flpTopCards.Controls.Add(CreateCard("Profit", "Net Sales", $"Rp {todaySummary.netSales:N0}"));
    flpTopCards.Controls.Add(CreateCard("Traffic", "Total Pax / Hari", $"{todaySummary.totalPax}"));
    pnlContent.Controls.Add(flpTopCards);

    // Label Statistik Penjualan
    Label lblPenjualan = new Label()
    {
        Text = "Statistik Penjualan (7 Hari Terakhir)",
        Font = new Font("Segoe UI", 12, FontStyle.Bold),
        ForeColor = Color.Gray,
        Location = new Point(10, offset + 195),
        AutoSize = true
    };
    pnlContent.Controls.Add(lblPenjualan);

    // Row 2: Kartu kecil (Peak Date, Top Income, Low Date, Lower Income)
    FlowLayoutPanel flpBottomCards = new FlowLayoutPanel()
    {
        Location = new Point(10, offset + 225),
        Width = 820,
        Height = 90,
        BackColor = Color.Transparent
    };
    flpBottomCards.Controls.Add(CreateSmallCard("Peak Date", peakLow.peakDate.ToString("dd/MM/yyyy")));
    flpBottomCards.Controls.Add(CreateSmallCard("Top Income", $"Rp {peakLow.peakIncome:N0}"));
    flpBottomCards.Controls.Add(CreateSmallCard("Low Date", peakLow.lowDate.ToString("dd/MM/yyyy")));
    flpBottomCards.Controls.Add(CreateSmallCard("Lower Income", $"Rp {peakLow.lowIncome:N0}"));
    pnlContent.Controls.Add(flpBottomCards);

    // Chart penjualan
    Chart chartPenjualan = new Chart()
    {
        Location = new Point(15, offset + 335),
        Size = new Size(810, 280),
        BackColor = Color.White,
        BorderlineColor = Color.FromArgb(224, 158, 172),
        BorderlineDashStyle = ChartDashStyle.Solid
    };
    ChartArea chartArea = new ChartArea("MainArea");
    chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
    chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
    chartPenjualan.ChartAreas.Add(chartArea);

    Series seriesSales = new Series("Penjualan Bersih")
    {
        ChartType = SeriesChartType.Column,
        Color = Color.FromArgb(224, 158, 172)
    };
    
    // Isi chart dengan data dari database
    if (dailySales.Rows.Count > 0)
    {
        foreach (DataRow row in dailySales.Rows)
        {
            string tgl = Convert.ToDateTime(row["Tanggal"]).ToString("dd/MM");
            decimal nilai = Convert.ToDecimal(row["TotalBersih"]);
            seriesSales.Points.AddXY(tgl, nilai);
        }
    }
    else
    {
        // Data dummy jika belum ada transaksi
        seriesSales.Points.AddXY("Tidak ada data", 0);
    }
    chartPenjualan.Series.Add(seriesSales);
    pnlContent.Controls.Add(chartPenjualan);
}
        // Event handler untuk tombol navigasi
    

        // ========== HELPER (sama seperti kode Anda) ==========
        private Button CreateNavButton(string text, int x)
        {
            Button btn = new Button()
            {
                Text = text,
                Location = new Point(x, 10),
                Size = new Size(145, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(174, 225, 225),
                ForeColor = Color.FromArgb(40, 60, 60),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => { btn.BackColor = Color.FromArgb(140, 210, 210); btn.ForeColor = Color.Black; };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.FromArgb(174, 225, 225); btn.ForeColor = Color.FromArgb(40, 60, 60); };
            return btn;
        }

        private Panel CreateCard(string labelTitle, string subtitle, string value)
        {
            Panel pnlCard = new Panel() { Size = new Size(250, 80), BackColor = Color.White, Margin = new Padding(0, 0, 20, 0) };
            pnlCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.FromArgb(224, 158, 172), ButtonBorderStyle.Solid);
            Label lblTitle = new Label() { Text = labelTitle, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.HotPink, Location = new Point(10, 10), AutoSize = true };
            Label lblSub = new Label() { Text = subtitle, Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(10, 30), AutoSize = true };
            Label lblVal = new Label() { Text = value, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(10, 45), AutoSize = true };
            pnlCard.Controls.Add(lblTitle); pnlCard.Controls.Add(lblSub); pnlCard.Controls.Add(lblVal);
            return pnlCard;
        }

        private Panel CreateSmallCard(string title, string value)
        {
            Panel pnlCard = new Panel() { Size = new Size(185, 70), BackColor = Color.FromArgb(235, 250, 250), Margin = new Padding(0, 0, 15, 0) };
            pnlCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.FromArgb(174, 225, 225), ButtonBorderStyle.Solid);
            Label lblTitle = new Label() { Text = title, Font = new Font("Segoe UI", 9), Location = new Point(10, 10), AutoSize = true, ForeColor = Color.DarkSlateGray };
            Label lblVal = new Label() { Text = value, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(10, 35), AutoSize = true, ForeColor = Color.CadetBlue };
            pnlCard.Controls.Add(lblTitle); pnlCard.Controls.Add(lblVal);
            return pnlCard;
        }
    }

    // Interface opsional untuk memberi tahu form bahwa ia di-embed
    public interface IEmbeddableForm
    {
        bool IsEmbedded { get; set; }
    }
}