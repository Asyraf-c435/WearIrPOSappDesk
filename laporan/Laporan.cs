using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class Laporan : Form
    {
       public Laporan()
        {
            InitializeComponentCustom();

            // =========================================================
            // 1. MENGHIDUPKAN TOMBOL TERAPKAN (FILTER TANGGAL)
            // =========================================================
            btnTerapkan.Click += (s, e) => MuatDataLaporan();

            // =========================================================
            // 2. MEMBUAT TOMBOL REFRESH (RESET KE SEMUA DATA)
            // =========================================================
            Button btnRefresh = new Button()
            {
                Text = "🔄 Refresh (Semua)",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.LightSkyBlue,
                ForeColor = Color.Black,
                Location = new Point(btnTerapkan.Right + 10, btnTerapkan.Top), 
                Size = new Size(150, btnTerapkan.Height),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            
            btnRefresh.Click += (s, e) => 
            {
                dtpDari.Value = new DateTime(2000, 1, 1); 
                dtpSampai.Value = DateTime.Now;           
                MuatDataLaporan();                        
            };

            if (btnTerapkan.Parent != null)
                btnTerapkan.Parent.Controls.Add(btnRefresh);
            else
                this.Controls.Add(btnRefresh);

            // Render awal saat form pertama kali dibuka
            MuatDataLaporan();
        }

      private void MuatDataLaporan()
        {
            try
            {
                DateTime dari = dtpDari.Value.Date;
                DateTime sampai = dtpSampai.Value.Date.AddDays(1).AddSeconds(-1);

                var dt = DatabaseConnection.LaporanHarian(dari, sampai);

                dgvLaporan.Rows.Clear();
                int totalPenjualanKotor = 0;
                int totalDiskon = 0;

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    // 1. Ekstrak Tanggal Saja (Tahun, Bulan, Hari)
                    string tgl = Convert.ToDateTime(row["Tanggal"]).ToString("dd/MM/yyyy");

                    // 2. Ekstrak Jam Saja (Jam dan Menit), di-trim dari atribut yang ada
                    string jam = "";
                    if (dt.Columns.Contains("Jam") && row["Jam"] != DBNull.Value)
                    {
                        // Jika pakai MySQL TIME(), format bawaannya adalah TimeSpan
                        if (row["Jam"] is TimeSpan ts) {
                            jam = ts.ToString(@"hh\:mm");
                        } else {
                            string rawJam = row["Jam"].ToString() ?? "";
                            jam = rawJam.Length >= 5 ? rawJam.Substring(0, 5) : rawJam;
                        }
                    }
                    else
                    {
                        // Jika ngambil langsung dari datetime transaksi jual, trim ambil jamnya saja
                        jam = Convert.ToDateTime(row["Tanggal"]).ToString("HH:mm");
                    }

                    string nama  = row["Produk"].ToString() ?? "";
                    string qty   = row["Qty"].ToString() ?? "0";
                    string sub   = Convert.ToInt32(row["Subtotal"]).ToString("N0");

                    // Masukkan ke baris tabel (DataGrid)
                    dgvLaporan.Rows.Add(tgl, jam, nama, qty, sub);

                    // Kalkulasi Total
                    totalPenjualanKotor += Convert.ToInt32(row["Subtotal"]);
                    int sub1 = Convert.ToInt32(row["Subtotal"]);
                    int dPct = Convert.ToInt32(row["DiskonPct"]);
                    totalDiskon += (sub1 * dPct / 100);
                }

                // Tampilkan ke textbox bawah
                txtTotalPenjualan.Text = totalPenjualanKotor.ToString("N0");
                txtTotalDiskon.Text = totalDiskon.ToString("N0");
                txtTotalBersih.Text = (totalPenjualanKotor - totalDiskon).ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTerapkan_Click(object? sender, EventArgs e)
        {
            MuatDataLaporan();
        }

        private void btnPrint_Click(object? sender, EventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == DialogResult.OK)
            {
                PrintDocument1.Print();
            }
        }

        private void PrintDocument1_PrintPage(object? sender, PrintPageEventArgs e)
        {
            if (e.Graphics == null) return;

            Font fontJudul = new Font("Courier New", 14, FontStyle.Bold);
            Font fontStandar = new Font("Courier New", 10, FontStyle.Regular);
            Font fontTebal = new Font("Courier New", 10, FontStyle.Bold);
            Brush brush = Brushes.Black;

            int startX = 50;
            int startY = 50;
            int offset = 0;

            string garisPemisah = "------------------------------------------------";

            e.Graphics.DrawString("WEAR IT FASHION", fontJudul, brush, startX + 80, startY + offset); offset += 25;
            e.Graphics.DrawString("Ringkasan Laporan Penjualan", fontStandar, brush, startX + 55, startY + offset); offset += 20;
            e.Graphics.DrawString($"Periode: {dtpDari.Value.ToString("dd/MM/yyyy")} - {dtpSampai.Value.ToString("dd/MM/yyyy")}", fontStandar, brush, startX + 15, startY + offset); offset += 25;
            e.Graphics.DrawString(garisPemisah, fontStandar, brush, startX, startY + offset); offset += 20;

            e.Graphics.DrawString("ITEM                 QTY   SUBTOTAL", fontTebal, brush, startX, startY + offset); offset += 20;
            e.Graphics.DrawString(garisPemisah, fontStandar, brush, startX, startY + offset); offset += 20;

            foreach (DataGridViewRow row in dgvLaporan.Rows)
            {
                if (row.Cells[2].Value != null)
                {
                    string namaBarang = row.Cells[2].Value?.ToString() ?? "";

                    if (namaBarang.Length > 18) namaBarang = namaBarang.Substring(0, 18);
                    else namaBarang = namaBarang.PadRight(18);

                    string qty = row.Cells[3].Value?.ToString()?.PadRight(4) ?? "0   ";
                    string subtotal = row.Cells[4].Value?.ToString() ?? "0";

                    e.Graphics.DrawString($"{namaBarang} {qty}  Rp {int.Parse(subtotal):N0}", fontStandar, brush, startX, startY + offset);

                    offset += 20;

                    string tgl = row.Cells[0].Value?.ToString() ?? "";
                    string jam = row.Cells[1].Value?.ToString() ?? "";
                    e.Graphics.DrawString($"  ({tgl} {jam})", new Font("Courier New", 8, FontStyle.Italic), Brushes.Gray, startX, startY + offset);
                    offset += 20;
                }
            }

            e.Graphics.DrawString(garisPemisah, fontStandar, brush, startX, startY + offset); offset += 20;
            e.Graphics.DrawString($"Total Penjualan : {txtTotalPenjualan.Text.PadLeft(18)}", fontStandar, brush, startX, startY + offset); offset += 20;
            e.Graphics.DrawString($"Total Diskon    : {txtTotalDiskon.Text.PadLeft(18)}", fontStandar, brush, startX, startY + offset); offset += 20;
            e.Graphics.DrawString(garisPemisah, fontStandar, brush, startX, startY + offset); offset += 20;
            e.Graphics.DrawString($"TOTAL BERSIH    : {txtTotalBersih.Text.PadLeft(18)}", fontTebal, brush, startX, startY + offset); offset += 40;

            e.Graphics.DrawString("*** END OF REPORT ***", fontStandar, brush, startX + 80, startY + offset);
        }

        private System.Drawing.Printing.PrintDocument PrintDocument1 = new System.Drawing.Printing.PrintDocument();
    }
}