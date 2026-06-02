using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class LaporanPenjualan : Form
    {
        private DataTable? dataLaporan;
        private DateTimePicker? dtpDari, dtpTill;
        private Button? btnTampilkan, btnExportPdf;
        private DataGridView? dgvLaporan;
        private Label? lblRingkasan;
        private Panel? pnlHeader, pnlFilter;
        private Label? lblTitle;

        public LaporanPenjualan()
        {
            InitializeComponent();
            LoadData(DateTime.Today.AddDays(-30), DateTime.Today);
        }

        private void InitializeComponent()
        {
            this.Text = "Laporan Penjualan - Crystal Reports Style";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);

            // Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(55, 71, 79)
            };
            lblTitle = new Label
            {
                Text = "LAPORAN PENJUALAN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // Filter panel
            pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 0)
            };
            Label lblDari = new Label { Text = "Dari Tanggal:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtpDari = new DateTimePicker { Location = new Point(130, 17), Size = new Size(180, 25), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            Label lblSampai = new Label { Text = "Sampai:", Location = new Point(330, 20), AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtpTill = new DateTimePicker { Location = new Point(390, 17), Size = new Size(180, 25), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            
            btnTampilkan = new Button
            {
                Text = "Tampilkan",
                Location = new Point(600, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTampilkan.FlatAppearance.BorderSize = 0;
            btnTampilkan.Click += BtnTampilkan_Click;

            btnExportPdf = new Button
            {
                Text = "Export PDF",
                Location = new Point(720, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.Click += BtnExportPdf_Click;

            pnlFilter.Controls.Add(lblDari);
            pnlFilter.Controls.Add(dtpDari);
            pnlFilter.Controls.Add(lblSampai);
            pnlFilter.Controls.Add(dtpTill);
            pnlFilter.Controls.Add(btnTampilkan);
            pnlFilter.Controls.Add(btnExportPdf);
            this.Controls.Add(pnlFilter);

            // Ringkasan panel
            lblRingkasan = new Label
            {
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(235, 245, 255),
                Padding = new Padding(20, 15, 0, 0),
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            this.Controls.Add(lblRingkasan);

            // DataGridView
            dgvLaporan = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                Font = new Font("Segoe UI", 9)
            };
            dgvLaporan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
            dgvLaporan.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLaporan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvLaporan.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvLaporan.EnableHeadersVisualStyles = false;
            dgvLaporan.AutoGenerateColumns = true;

            this.Controls.Add(dgvLaporan);
        }

        private void BtnTampilkan_Click(object? sender, EventArgs e)
        {
            if (dtpDari != null && dtpTill != null)
                LoadData(dtpDari.Value, dtpTill.Value);
        }

        private void LoadData(DateTime dari, DateTime sampai)
        {
            try
            {
                dataLaporan = DatabaseConnection.GetLaporanPenjualanLengkap(dari, sampai);
                var ringkasan = DatabaseConnection.GetRingkasanLaporan(dari, sampai);

                if (dgvLaporan != null)
                {
                    dgvLaporan.DataSource = dataLaporan;
                    FormatDataGridView();
                }

                if (lblRingkasan != null)
                {
                    lblRingkasan.Text = $"📊 RINGKASAN PENJUALAN  |  Periode: {dari:dd/MM/yyyy} - {sampai:dd/MM/yyyy}\n" +
                        $"Gross Sales: Rp {ringkasan.grossSales:N0}  |  Diskon: Rp {ringkasan.totalDiskon:N0}  |  Net Sales: Rp {ringkasan.netSales:N0}  |  " +
                        $"HPP: Rp {ringkasan.totalHpp:N0}  |  Laba Kotor: Rp {ringkasan.labaKotor:N0}  |  Transaksi: {ringkasan.totalTransaksi}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat laporan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvLaporan == null) return;
            if (dgvLaporan.Columns.Contains("Tanggal"))
                dgvLaporan.Columns["Tanggal"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
            if (dgvLaporan.Columns.Contains("Subtotal"))
                dgvLaporan.Columns["Subtotal"]!.DefaultCellStyle.Format = "N0";
            if (dgvLaporan.Columns.Contains("HargaJual"))
                dgvLaporan.Columns["HargaJual"]!.DefaultCellStyle.Format = "N0";
            if (dgvLaporan.Columns.Contains("Laba"))
                dgvLaporan.Columns["Laba"]!.DefaultCellStyle.Format = "N0";

            foreach (DataGridViewColumn col in dgvLaporan.Columns)
            {
                if (col.Name == "Qty" || col.Name == "DiskonPersen" || col.Name == "Subtotal" || col.Name == "HargaJual" || col.Name == "HargaBeli" || col.Name == "Laba")
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                else
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private void BtnExportPdf_Click(object? sender, EventArgs e)
        {
            if (dataLaporan == null || dataLaporan.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diekspor.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dtpDari == null || dtpTill == null) return;

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF file|*.pdf",
                Title = "Simpan Laporan Penjualan",
                FileName = $"LaporanPenjualan_{dtpDari.Value:yyyyMMdd}_{dtpTill.Value:yyyyMMdd}.pdf"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportToPdf(sfd.FileName);
            }
        }

        private void ExportToPdf(string filePath)
        {
            if (dtpDari == null || dtpTill == null || dataLaporan == null) return;
            var ringkasan = DatabaseConnection.GetRingkasanLaporan(dtpDari.Value, dtpTill.Value);
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                page.Orientation = PdfSharp.PageOrientation.Landscape;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont fontTitle = new XFont("Segoe UI", 16, XFontStyle.Bold);
                XFont fontHeader = new XFont("Segoe UI", 10, XFontStyle.Bold);
                XFont fontNormal = new XFont("Segoe UI", 8, XFontStyle.Regular);
                double y = 40;

                // Title and period
                gfx.DrawString("LAPORAN PENJUALAN - KASIRWEARIT", fontTitle, XBrushes.Black,
                    new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
                y += 25;
                gfx.DrawString($"Periode: {dtpDari.Value:dd/MM/yyyy} - {dtpTill.Value:dd/MM/yyyy}", fontNormal, XBrushes.Black,
                    new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
                y += 30;

                // Summary
                string summary = $"Gross Sales: Rp {ringkasan.grossSales:N0}   |   Diskon: Rp {ringkasan.totalDiskon:N0}   |   Net Sales: Rp {ringkasan.netSales:N0}   |   " +
                                 $"HPP: Rp {ringkasan.totalHpp:N0}   |   Laba: Rp {ringkasan.labaKotor:N0}   |   Transaksi: {ringkasan.totalTransaksi}";
                gfx.DrawString(summary, fontNormal, XBrushes.Black, new XRect(20, y, page.Width - 40, 40), XStringFormats.TopLeft);
                y += 35;

                // Table headers
                double[] cols = { 70, 50, 130, 45, 80, 100, 70, 80, 100 };
                string[] headers = { "Tanggal", "Jam", "Produk", "Qty", "Harga", "Subtotal", "Diskon%", "HPP", "Laba" };
                double x = 20;
                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawRectangle(XPens.Black, x, y, cols[i], 22);
                    gfx.DrawString(headers[i], fontHeader, XBrushes.Black, new XRect(x, y + 2, cols[i], 18), XStringFormats.TopCenter);
                    x += cols[i];
                }
                y += 22;

                // Data rows
                foreach (DataRow row in dataLaporan.Rows)
                {
                    if (y > page.Height - 40)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                        // Redraw header
                        x = 20;
                        for (int i = 0; i < headers.Length; i++)
                        {
                            gfx.DrawRectangle(XPens.Black, x, y, cols[i], 22);
                            gfx.DrawString(headers[i], fontHeader, XBrushes.Black, new XRect(x, y + 2, cols[i], 18), XStringFormats.TopCenter);
                            x += cols[i];
                        }
                        y += 22;
                    }
                    x = 20;
                    gfx.DrawString(Convert.ToDateTime(row["Tanggal"]).ToString("dd/MM/yyyy"), fontNormal, XBrushes.Black, new XRect(x, y, cols[0], 18), XStringFormats.TopLeft);
                    x += cols[0];
                    gfx.DrawString(row["Jam"]?.ToString() ?? "", fontNormal, XBrushes.Black, new XRect(x, y, cols[1], 18), XStringFormats.TopLeft);
                    x += cols[1];
                    gfx.DrawString(row["Produk"]?.ToString() ?? "", fontNormal, XBrushes.Black, new XRect(x, y, cols[2], 18), XStringFormats.TopLeft);
                    x += cols[2];
                    gfx.DrawString(row["Qty"]?.ToString(), fontNormal, XBrushes.Black, new XRect(x, y, cols[3], 18), XStringFormats.TopRight);
                    x += cols[3];
                    gfx.DrawString($"Rp {Convert.ToDecimal(row["HargaJual"]):N0}", fontNormal, XBrushes.Black, new XRect(x, y, cols[4], 18), XStringFormats.TopRight);
                    x += cols[4];
                    gfx.DrawString($"Rp {Convert.ToDecimal(row["Subtotal"]):N0}", fontNormal, XBrushes.Black, new XRect(x, y, cols[5], 18), XStringFormats.TopRight);
                    x += cols[5];
                    gfx.DrawString(row["DiskonPersen"]?.ToString() + "%", fontNormal, XBrushes.Black, new XRect(x, y, cols[6], 18), XStringFormats.TopRight);
                    x += cols[6];
                    gfx.DrawString($"Rp {Convert.ToDecimal(row["HargaBeli"]):N0}", fontNormal, XBrushes.Black, new XRect(x, y, cols[7], 18), XStringFormats.TopRight);
                    x += cols[7];
                    gfx.DrawString($"Rp {Convert.ToDecimal(row["Laba"]):N0}", fontNormal, XBrushes.Black, new XRect(x, y, cols[8], 18), XStringFormats.TopRight);
                    y += 18;
                }
                document.Save(filePath);
            }
            MessageBox.Show($"Laporan berhasil disimpan ke:\n{filePath}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}