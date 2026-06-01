using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KasirWearIt
{
    public partial class Pembayaran : Form
    {
        private string metodeTerpilih = "CASH";
        private double subtotalAwal = 0;
        private Action? onPaymentSuccess;

        public Pembayaran()
        {
            InitializeComponentCustom();
        }

        // Constructor yang dipanggil dari Pos.cs (3 argumen)
        public Pembayaran(int totalBelanja, List<string[]> daftarBarang, Action onSuccessCallback)
        {
            this.subtotalAwal = totalBelanja;
            this.onPaymentSuccess = onSuccessCallback;
            InitializeComponentCustom();

            // Isi DataGridView dari keranjang POS
            foreach (var item in daftarBarang)
            {
                // item[0]=Nama, item[1]=Harga, item[2]=Qty, item[3]=Subtotal
                dgvPesanan.Rows.Add(item);
            }
            dgvPesanan.ClearSelection();
            HitungTotal();
        }

        private void Pembayaran_Load(object sender, EventArgs e)
        {
            // Menampilkan Data Dummy Sesuai dengan Contoh Kasus Anda
            dgvPesanan.Rows.Add("B02", "CARDIGAN PINK", 180000, 1, 180000);
            
            // Bersihkan seleksi biru awal agar tampilan grid bersih normal
            dgvPesanan.ClearSelection();

            SetMetodePembayaran("CASH");
            HitungTotal();
        }

        // Logika Choose / Switch Status Metode Pembayaran
        private void SetMetodePembayaran(string metode)
        {
            metodeTerpilih = metode;

            if (metode == "CASH")
            {
                // Tombol Cash Aktif: Mengikuti warna tema Cyan Soft panel kanan POS Anda
                btnCash.BackColor = Color.FromArgb(174, 225, 225);
                btnCash.ForeColor = Color.FromArgb(40, 40, 40);
                btnCash.FlatAppearance.BorderColor = Color.FromArgb(140, 200, 200);

                // Tombol QRIS Tidak Aktif: Putih Bersih border Abu-abu
                btnQris.BackColor = Color.White;
                btnQris.ForeColor = Color.DarkGray;
                btnQris.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);

                // Fitur Uang & Perhitungan Kembalian Diaktifkan
                txtUangDibayar.Enabled = true;
                lblKembalianValue.Text = "Rp 0";
            }
            else if (metode == "QRIS")
            {
                // Tombol QRIS Aktif: Mengikuti warna tema Cyan Soft panel kanan POS Anda
                btnQris.BackColor = Color.FromArgb(174, 225, 225);
                btnQris.ForeColor = Color.FromArgb(40, 40, 40);
                btnQris.FlatAppearance.BorderColor = Color.FromArgb(140, 200, 200);

                // Tombol Cash Tidak Aktif: Putih Bersih border Abu-abu
                btnCash.BackColor = Color.White;
                btnCash.ForeColor = Color.DarkGray;
                btnCash.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);

                // Fitur Uang & Kembalian Dimatikan Otomatis
                txtUangDibayar.Text = "0";
                txtUangDibayar.Enabled = false;
                lblKembalianValue.Text = "DIKUNCI (QRIS)";
            }
        }

        private void BtnCash_Click(object sender, EventArgs e) => SetMetodePembayaran("CASH");
        private void BtnQris_Click(object sender, EventArgs e) => SetMetodePembayaran("QRIS");

        private void InputKalkulasi_Changed(object sender, EventArgs e)
        {
            HitungTotal();
        }

        private void HitungTotal()
        {
            double.TryParse(txtDiskonPersen.Text, out double diskonPersen);
            double nilaiDiskon = subtotalAwal * (diskonPersen / 100);
            double grandTotal = subtotalAwal - nilaiDiskon;

            lblSubTotalValue.Text = string.Format("Rp {0:N0}", subtotalAwal);
            lblGrandTotalValue.Text = string.Format("Rp {0:N0}", grandTotal);

            if (metodeTerpilih == "CASH")
            {
                double.TryParse(txtUangDibayar.Text, out double uangBayar);
                double kembalian = uangBayar - grandTotal;
                
                if (kembalian >= 0)
                    lblKembalianValue.Text = string.Format("Rp {0:N0}", kembalian);
                else
                    lblKembalianValue.Text = "Uang Kurang";
            }
        }

        // Manajemen Klik Tombol Aksi di Kolom Grid
        private void dgvPesanan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // FITUR HAPUS BARANG
            if (dgvPesanan.Columns[e.ColumnIndex].Name == "AksiHapus")
            {
                DialogResult res = MessageBox.Show("Hapus item ini dari keranjang belanja?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    dgvPesanan.Rows.RemoveAt(e.RowIndex);
                    subtotalAwal = 0; 
                    HitungTotal();
                }
            }

            // FITUR EDIT BARANG (Perbaikan Frame Panjang & Lebar Agar Tidak Terpotong)
            if (dgvPesanan.Columns[e.ColumnIndex].Name == "AksiEdit")
            {
                string currentQty = dgvPesanan.Rows[e.RowIndex].Cells["Qty"].Value?.ToString() ?? "";
                
                // Mengatur dimensi aman (Lebar 390, Tinggi 180) agar frame pop-up longgar dan bagus
                Form popUp = new Form() { 
                    Width = 390, 
                    Height = 180, 
                    Text = "Ubah Jumlah Barang", 
                    StartPosition = FormStartPosition.CenterParent, 
                    FormBorderStyle = FormBorderStyle.FixedToolWindow,
                    BackColor = Color.White
                };
                
                Label lblInfo = new Label() { Left = 25, Top = 22, Text = "Masukkan Kuantitas Baru:", Font = new Font("Segoe UI", 10), AutoSize = true };
                TextBox txtInput = new TextBox() { Left = 25, Top = 52, Width = 320, Font = new Font("Segoe UI", 11), Text = currentQty };
                
                Button btnSimpan = new Button() { 
                    Text = "Simpan", 
                    Left = 245, 
                    Top = 92, 
                    Width = 100, 
                    Height = 32, 
                    DialogResult = DialogResult.OK, 
                    BackColor = Color.FromArgb(174, 225, 225), // Warna serasi tema POS
                    FlatStyle = FlatStyle.Flat 
                };
                btnSimpan.FlatAppearance.BorderSize = 0;
                
                popUp.Controls.AddRange(new Control[] { lblInfo, txtInput, btnSimpan });
                popUp.AcceptButton = btnSimpan;

                if (popUp.ShowDialog() == DialogResult.OK)
                {
                    if (int.TryParse(txtInput.Text, out int qtyBaru) && qtyBaru > 0)
                    {
                        double harga = Convert.ToDouble(dgvPesanan.Rows[e.RowIndex].Cells["Harga"].Value);
                        double subtotalBaru = harga * qtyBaru;

                        dgvPesanan.Rows[e.RowIndex].Cells["Qty"].Value = qtyBaru;
                        dgvPesanan.Rows[e.RowIndex].Cells["Subtotal"].Value = subtotalBaru;

                        subtotalAwal = subtotalBaru; 
                        HitungTotal();
                    }
                    else
                    {
                        MessageBox.Show("Jumlah item tidak valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                dgvPesanan.ClearSelection(); // Mengembalikan fokus grid ke normal netral
            }
        }

        // Penanganan Akhir Tombol "PROSES BAYAR"
        private void BtnBayar_Click(object sender, EventArgs e)
        {
            double.TryParse(txtDiskonPersen.Text, out double diskonPersen);
            double grandTotal = subtotalAwal - (subtotalAwal * (diskonPersen / 100));

            if (metodeTerpilih == "QRIS")
            {
                // Menampilkan Pop-up Barcode & Sisa Waktu hitung mundur
                Form qrForm = new Form() { Width = 350, Height = 450, Text = "Scan QRIS Payment", StartPosition = FormStartPosition.CenterParent, BackColor = Color.White };
                Label lblTotalQris = new Label() { Text = "TOTAL TAGIHAN:\n" + string.Format("Rp {0:N0}", grandTotal), Font = new Font("Segoe UI", 13, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 60, ForeColor = Color.FromArgb(120, 60, 80) };
                
                PictureBox pbQr = new PictureBox() { Size = new Size(200, 200), Location = new Point(75, 80), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 248, 248), SizeMode = PictureBoxSizeMode.CenterImage };
                Label lblMockQr = new Label() { Text = "[ Kotak Barcode QRIS ]", AutoSize = true, Location = new Point(40, 90), ForeColor = Color.DarkGray };
                pbQr.Controls.Add(lblMockQr);

                Label lblTimer = new Label() { Text = "Sisa Waktu Pembayaran: 05:00", Font = new Font("Segoe UI", 10, FontStyle.Italic), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(20, 300), Width = 300, ForeColor = Color.Gray };
                Button btnSelesaiQr = new Button() { Text = "Simulasi Sukses Terbayar", Location = new Point(50, 340), Size = new Size(250, 40), BackColor = Color.FromArgb(174, 225, 225), FlatStyle = FlatStyle.Flat };
                btnSelesaiQr.FlatAppearance.BorderSize = 0;

                btnSelesaiQr.Click += (s, ev) => {
                    qrForm.Close();
                    MessageBox.Show("Pembayaran menggunakan QRIS Sukses dikonfirmasi!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    onPaymentSuccess?.Invoke();
                    this.Close();
                };

                qrForm.Controls.AddRange(new Control[] { lblTotalQris, pbQr, lblTimer, btnSelesaiQr });
                qrForm.ShowDialog();
            }
            else // METODE CASH / TUNAI
            {
                double.TryParse(txtUangDibayar.Text, out double uangBayar);
                if (uangBayar < grandTotal)
                {
                    MessageBox.Show("Jumlah uang pembayaran kurang dari total tagihan akhir!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string namaBarang = dgvPesanan.Rows[0].Cells["Nama Barang"].Value?.ToString() ?? "";
                string qtyBarang = dgvPesanan.Rows[0].Cells["Qty"].Value?.ToString() ?? "";
                string hargaBarang = string.Format("Rp {0:N0}", dgvPesanan.Rows[0].Cells["Harga"].Value);
                string kembalianStr = lblKembalianValue.Text;

                // Detail Data Cetak Nota Struk Penjualan POS
                string strukNota = $"=== STRUK NOTA WEAR IT ===\n\n" +
                                   $"Barang : {namaBarang}\n" +
                                   $"Qty    : {qtyBarang} x {hargaBarang}\n" +
                                   $"---------------------------\n" +
                                   $"Total  : {lblGrandTotalValue.Text}\n" +
                                   $"Bayar  : {string.Format("Rp {0:N0}", uangBayar)}\n" +
                                   $"Kembali: {kembalianStr}\n\n" +
                                   $"===========================\n" +
                                   $"Terima kasih telah berbelanja!";

                MessageBox.Show(strukNota, "Printer Struk (Simulasi)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                onPaymentSuccess?.Invoke();
                this.Close(); // Kembali ke sistem POS Utama
            }
        }
    }
}