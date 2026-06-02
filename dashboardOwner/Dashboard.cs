using System;
using System.Windows.Forms;

namespace KasirWearIt
{
    public partial class OwnerDashboard
    {
        // Method ini dipanggil dari konstruktor (sudah ada di Designer)
        private void AttachNavigationEvents()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel pnl && pnl.Dock == DockStyle.Top)
                {
                    foreach (Control child in pnl.Controls)
                    {
                        if (child is Button btn)
                        {
                            btn.Click += (s, e) => Navigate(btn.Text);
                        }
                    }
                }
            }
        }

        private void Navigate(string buttonText)
        {
            pnlContent.Controls.Clear();

            if (buttonText.Contains("Dashboard"))
            {
                // Tampilkan UI dashboard yang sudah dibuat di Designer
                ShowDashboardContent();
            }
            else if (buttonText.Contains("Transaksi"))
            {
                // Buka form PenyesuaianStok (bukan Transaksi)
                PenyesuaianStok formStok = new PenyesuaianStok();
                formStok.TopLevel = false;
                formStok.FormBorderStyle = FormBorderStyle.None;
                formStok.Dock = DockStyle.Fill;
                if (formStok is IEmbeddableForm embed) embed.IsEmbedded = true;
                pnlContent.Controls.Add(formStok);
                formStok.Show();
            }
            else if (buttonText.Contains("Master Data"))
            {
                MasterData formMaster = new MasterData();
                formMaster.TopLevel = false;
                formMaster.FormBorderStyle = FormBorderStyle.None;
                formMaster.Dock = DockStyle.Fill;
                if (formMaster is IEmbeddableForm embed) embed.IsEmbedded = true;
                pnlContent.Controls.Add(formMaster);
                formMaster.Show();
            }
            else if (buttonText.Contains("Laporan"))   // <-- TAMBAHAN UNTUK LAPORAN
            {
                // Buka form LaporanPenjualan sebagai dialog terpisah
                LaporanPenjualan formLaporan = new LaporanPenjualan();
                formLaporan.ShowDialog();
            }
        }
    }
}