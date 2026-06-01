using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KasirWearIt.Database;
using PdfSharp.Pdf;
using PdfSharpDrawing = PdfSharp.Drawing;

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

        public Pembayaran(int totalBelanja, List<string[]> daftarBarang, Action onSuccessCallback)
        {
            this.subtotalAwal = totalBelanja;
            this.onPaymentSuccess = onSuccessCallback;
            InitializeComponentCustom();

            foreach (var item in daftarBarang)
            {
                if (item.Length == 5)
                {
                    dgvPesanan.Rows.Add(item[0], item[1], item[2], item[3], item[4]);
                }
                else if (item.Length == 4)
                {
                    dgvPesanan.Rows.Add("-", item[0], item[1], item[2], item[3]);
                }
            }
            dgvPesanan.ClearSelection();
            HitungTotal();
        }

        private void SetMetodePembayaran(string metode)
        {
            metodeTerpilih = metode;
            if (metode == "CASH")
            {
                btnCash.BackColor = Color.FromArgb(174, 225, 225);
                btnCash.ForeColor = Color.FromArgb(40, 40, 40);
                btnCash.FlatAppearance.BorderColor = Color.FromArgb(140, 200, 200);
                btnQris.BackColor = Color.White;
                btnQris.ForeColor = Color.DarkGray;
                btnQris.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                txtUangDibayar.Enabled = true;
                lblKembalianValue.Text = "Rp 0";
            }
            else if (metode == "QRIS")
            {
                btnQris.BackColor = Color.FromArgb(174, 225, 225);
                btnQris.ForeColor = Color.FromArgb(40, 40, 40);
                btnQris.FlatAppearance.BorderColor = Color.FromArgb(140, 200, 200);
                btnCash.BackColor = Color.White;
                btnCash.ForeColor = Color.DarkGray;
                btnCash.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                txtUangDibayar.Text = "0";
                txtUangDibayar.Enabled = false;
                lblKembalianValue.Text = "DIKUNCI (QRIS)";
            }
        }

        private void BtnCash_Click(object sender, EventArgs e) => SetMetodePembayaran("CASH");
        private void BtnQris_Click(object sender, EventArgs e) => SetMetodePembayaran("QRIS");
        private void InputKalkulasi_Changed(object sender, EventArgs e) => HitungTotal();

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

        private void dgvPesanan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

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

            if (dgvPesanan.Columns[e.ColumnIndex].Name == "AksiEdit")
            {
                string currentQty = dgvPesanan.Rows[e.RowIndex].Cells["Qty"].Value?.ToString() ?? "";

                Form popUp = new Form()
                {
                    Width = 390,
                    Height = 180,
                    Text = "Ubah Jumlah Barang",
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedToolWindow,
                    BackColor = Color.White
                };

                Label lblInfo = new Label() { Left = 25, Top = 22, Text = "Masukkan Kuantitas Baru:", Font = new Font("Segoe UI", 10), AutoSize = true };
                TextBox txtInput = new TextBox() { Left = 25, Top = 52, Width = 320, Font = new Font("Segoe UI", 11), Text = currentQty };
                Button btnSimpan = new Button()
                {
                    Text = "Simpan",
                    Left = 245,
                    Top = 92,
                    Width = 100,
                    Height = 32,
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(174, 225, 225),
                    FlatStyle = FlatStyle.Flat
                };
                btnSimpan.FlatAppearance.BorderSize = 0;
                popUp.Controls.AddRange(new Control[] { lblInfo, txtInput, btnSimpan });
                popUp.AcceptButton = btnSimpan;

                if (popUp.ShowDialog() == DialogResult.OK)
                {
                    if (int.TryParse(txtInput.Text, out int qtyBaru) && qtyBaru > 0)
                    {
                        string hargaStr = dgvPesanan.Rows[e.RowIndex].Cells["Harga"].Value?.ToString() ?? "0";
                        hargaStr = hargaStr.Replace("Rp ", "").Replace(",", "");
                        if (double.TryParse(hargaStr, out double harga))
                        {
                            double subtotalBaru = harga * qtyBaru;
                            dgvPesanan.Rows[e.RowIndex].Cells["Qty"].Value = qtyBaru;
                            dgvPesanan.Rows[e.RowIndex].Cells["Subtotal"].Value = $"Rp {subtotalBaru:N0}";

                            subtotalAwal = 0;
                            foreach (DataGridViewRow row in dgvPesanan.Rows)
                            {
                                if (row.Cells["Subtotal"].Value != null)
                                {
                                  string subStr = row.Cells["Subtotal"].Value?.ToString()?.Replace("Rp ", "").Replace(",", "") ?? "0";
                                    if (double.TryParse(subStr, out double sub))
                                        subtotalAwal += sub;
                                }
                            }
                            HitungTotal();
                        }
                        else
                        {
                            MessageBox.Show("Gagal membaca harga barang!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Jumlah item tidak valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                dgvPesanan.ClearSelection();
            }
        }

        private void BtnBayar_Click(object sender, EventArgs e)
        {
            double.TryParse(txtDiskonPersen.Text, out double diskonPersen);
            double grandTotal = subtotalAwal - (subtotalAwal * (diskonPersen / 100));

            var items = new List<(string kode, decimal harga, int qty, decimal sub)>();
            foreach (DataGridViewRow row in dgvPesanan.Rows)
            {
                if (row.IsNewRow) continue;
                string kode = row.Cells["Kode Barang"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(kode) || kode == "-") continue;

                string hargaStr = row.Cells["Harga"].Value?.ToString() ?? "0";
                hargaStr = hargaStr.Replace("Rp ", "").Replace(",", "");
                if (!decimal.TryParse(hargaStr, out decimal hargaSatuan)) continue;

                if (!int.TryParse(row.Cells["Qty"].Value?.ToString(), out int qty)) continue;

                string subStr = row.Cells["Subtotal"].Value?.ToString() ?? "0";
                subStr = subStr.Replace("Rp ", "").Replace(",", "");
                if (!decimal.TryParse(subStr, out decimal subtotalItem)) continue;

                items.Add((kode, hargaSatuan, qty, subtotalItem));
            }

            if (items.Count == 0)
            {
                MessageBox.Show("Tidak ada item yang valid untuk disimpan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idUser = Session.IdUser;
            if (idUser <= 0)
            {
                MessageBox.Show("Data user tidak ditemukan. Silakan login ulang.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal subtotalDec = (decimal)subtotalAwal;
            decimal diskonPct = (decimal)diskonPersen;
            decimal totalAkhir = (decimal)grandTotal;
            string metodeBayar = metodeTerpilih;
            decimal uangBayar = 0;
            decimal kembalian = 0;

            // QRIS
            if (metodeBayar == "QRIS")
            {
                Form qrForm = new Form()
                {
                    Width = 400,
                    Height = 520,
                    Text = "QRIS Payment",
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White
                };

                Label lblTotal = new Label()
                {
                    Text = $"Total: Rp {grandTotal:N0}",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Height = 50,
                    ForeColor = Color.FromArgb(120, 60, 80)
                };

                PictureBox pbQr = new PictureBox()
                {
                    Size = new Size(250, 250),
                    Location = new Point(75, 70),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.WhiteSmoke
                };
string qrPath = "";
string[] possiblePaths = {
    System.IO.Path.Combine(Application.StartupPath, "resources", "qris.png"),
    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "resources", "qris.png"),
};

foreach (var path in possiblePaths)
{
    string fullPath = System.IO.Path.GetFullPath(path);
    if (System.IO.File.Exists(fullPath))
    {
        qrPath = fullPath;
        break;
    }
}

// Setelah loop, cek apakah qrPath ditemukan
if (string.IsNullOrEmpty(qrPath))
{
    MessageBox.Show("File QRIS tidak ditemukan.");
    pbQr.BackColor = Color.LightGray;
}
else
{
    try { pbQr.Image = Image.FromFile(qrPath); }
    catch { pbQr.BackColor = Color.LightGray; }
}

                Label lblTimer = new Label()
                {
                    Text = "Sisa waktu: 01:00",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(50, 340),
                    Width = 300,
                    Height = 40
                };

                Button btnConfirm = new Button()
                {
                    Text = "Bayar Sekarang",
                    Location = new Point(100, 400),
                    Size = new Size(200, 45),
                    BackColor = Color.FromArgb(174, 225, 225),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold)
                };
                btnConfirm.FlatAppearance.BorderSize = 0;

                qrForm.Controls.Add(lblTotal);
                qrForm.Controls.Add(pbQr);
                qrForm.Controls.Add(lblTimer);
                qrForm.Controls.Add(btnConfirm);

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                int timeLeft = 60;
                timer.Interval = 1000;
                bool isPaid = false;

                timer.Tick += (ts, te) =>
                {
                    timeLeft--;
                    if (timeLeft < 0)
                    {
                        timer.Stop();
                        qrForm.Close();
                        MessageBox.Show("Waktu pembayaran QRIS telah habis! Transaksi dibatalkan.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    lblTimer.Text = $"Sisa waktu: {timeLeft / 60:D2}:{timeLeft % 60:D2}";
                };
                timer.Start();

                btnConfirm.Click += (s, ev) =>
                {
                    timer.Stop();
                    isPaid = true;
                    qrForm.Close();
                };

                qrForm.FormClosing += (s, ev) =>
                {
                    if (!isPaid) timer.Stop();
                };

                qrForm.ShowDialog();

                if (!isPaid) return;

                uangBayar = totalAkhir;
                kembalian = 0;
            }
            // CASH
            else if (metodeBayar == "CASH")
            {
                if (!decimal.TryParse(txtUangDibayar.Text, out uangBayar))
                {
                    MessageBox.Show("Masukkan angka yang valid untuk uang dibayar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (uangBayar < totalAkhir)
                {
                    MessageBox.Show("Uang yang dibayar kurang dari total tagihan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                kembalian = uangBayar - totalAkhir;
            }

            // Simpan transaksi
            try
            {
                int idTransaksi = DatabaseConnection.SimpanTransaksi(
                    idUser,
                    subtotalDec,
                    diskonPct,
                    totalAkhir,
                    metodeBayar,
                    uangBayar,
                    kembalian,
                    items.ToArray()
                );

                if (idTransaksi > 0)
                {
                    GenerateStrukPDF(idTransaksi, dgvPesanan, subtotalDec, diskonPct, totalAkhir, metodeBayar, uangBayar, kembalian);
                    onPaymentSuccess?.Invoke();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal menyimpan transaksi. Silakan cek kembali data atau hubungi administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan saat menyimpan transaksi:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ======================= METHOD GENERATE STRUK PDF =======================
 private void GenerateStrukPDF(int idTransaksi, DataGridView dgv, decimal subtotal, decimal diskonPersen, decimal grandTotal, string metodeBayar, decimal uangBayar, decimal kembalian)
{
    const double pageWidth = 280;       // lebar halaman dalam point
    const double leftMargin = 15;
    const double rightMargin = 15;
    double contentWidth = pageWidth - leftMargin - rightMargin;

    // Lebar kolom (proporsional)
    double colNama = contentWidth * 0.50;      // 50% untuk nama barang
    double colQty = contentWidth * 0.10;       // 10% untuk qty
    double colHarga = contentWidth * 0.20;     // 20% untuk harga
    double colSubtotal = contentWidth * 0.20;  // 20% untuk subtotal

    // Posisi X masing-masing kolom
    double xNama = leftMargin;
    double xQty = xNama + colNama;
    double xHarga = xQty + colQty;
    double xSubtotal = xHarga + colHarga;

    // Format alignment
    var leftAlign = PdfSharpDrawing.XStringFormats.TopLeft;
    var rightAlign = new PdfSharpDrawing.XStringFormat()
    {
        Alignment = PdfSharpDrawing.XStringAlignment.Far,
        LineAlignment = PdfSharpDrawing.XLineAlignment.Near
    };
    var centerAlign = new PdfSharpDrawing.XStringFormat()
    {
        Alignment = PdfSharpDrawing.XStringAlignment.Center,
        LineAlignment = PdfSharpDrawing.XLineAlignment.Near
    };

    double lineHeight = 12;
    double y = 10;

    using (var document = new PdfDocument())
    {
        var page = document.AddPage();
        page.Width = PdfSharpDrawing.XUnit.FromPoint(pageWidth);
        page.Height = PdfSharpDrawing.XUnit.FromMillimeter(300);

        using (var gfx = PdfSharpDrawing.XGraphics.FromPdfPage(page))
        {
            var fontTitle = new PdfSharpDrawing.XFont("Courier New", 11, PdfSharpDrawing.XFontStyle.Bold);
            var fontBold = new PdfSharpDrawing.XFont("Courier New", 9, PdfSharpDrawing.XFontStyle.Bold);
            var fontNormal = new PdfSharpDrawing.XFont("Courier New", 8, PdfSharpDrawing.XFontStyle.Regular);
            var fontSmall = new PdfSharpDrawing.XFont("Courier New", 7, PdfSharpDrawing.XFontStyle.Regular);

            double centerX = pageWidth / 2;

            // Header toko (rata tengah)
            gfx.DrawString("WEAR IT FASHION", fontTitle, PdfSharpDrawing.XBrushes.Black, centerX, y, centerAlign);
            y += lineHeight;
            gfx.DrawString("Jl. Raya Dagang No. 88", fontSmall, PdfSharpDrawing.XBrushes.Black, centerX, y, centerAlign);
            y += lineHeight;
            gfx.DrawString("Telp. (021) 1234-5678", fontSmall, PdfSharpDrawing.XBrushes.Black, centerX, y, centerAlign);
            y += lineHeight + 5;

            // Judul struk
            gfx.DrawString("STRUK PEMBAYARAN", fontBold, PdfSharpDrawing.XBrushes.Black, centerX, y, centerAlign);
            y += lineHeight + 3;

            // Info transaksi (rata kiri)
            gfx.DrawString($"No. Transaksi : {idTransaksi}", fontNormal, PdfSharpDrawing.XBrushes.Black, xNama, y, leftAlign);
            y += lineHeight;
            gfx.DrawString($"Tanggal       : {DateTime.Now:dd/MM/yyyy HH:mm:ss}", fontNormal, PdfSharpDrawing.XBrushes.Black, xNama, y, leftAlign);
            y += lineHeight;
            gfx.DrawString($"Kasir         : {Session.NamaLengkap}", fontNormal, PdfSharpDrawing.XBrushes.Black, xNama, y, leftAlign);
            y += lineHeight + 5;

            // Garis pemisah atas
            gfx.DrawLine(PdfSharpDrawing.XPens.Black, leftMargin, y, pageWidth - rightMargin, y);
            y += 5;

            // Header tabel
            gfx.DrawString("Item", fontBold, PdfSharpDrawing.XBrushes.Black, xNama, y, leftAlign);
            gfx.DrawString("Qty", fontBold, PdfSharpDrawing.XBrushes.Black, xQty + colQty / 2, y, centerAlign);
            gfx.DrawString("Harga", fontBold, PdfSharpDrawing.XBrushes.Black, xHarga + colHarga, y, rightAlign);
            gfx.DrawString("Subtotal", fontBold, PdfSharpDrawing.XBrushes.Black, xSubtotal + colSubtotal, y, rightAlign);
            y += lineHeight - 2;
            gfx.DrawLine(PdfSharpDrawing.XPens.Black, leftMargin, y, pageWidth - rightMargin, y);
            y += 3;

            // Baris item
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string nama = row.Cells["Nama Barang"].Value?.ToString() ?? "-";
                if (nama.Length > 20) nama = nama.Substring(0, 17) + "...";
                string qty = row.Cells["Qty"].Value?.ToString() ?? "0";
                string harga = row.Cells["Harga"].Value?.ToString() ?? "0";
                string sub = row.Cells["Subtotal"].Value?.ToString() ?? "0";

                harga = harga.Replace("Rp ", "");
                sub = sub.Replace("Rp ", "");

                gfx.DrawString(nama, fontNormal, PdfSharpDrawing.XBrushes.Black, xNama, y, leftAlign);
                gfx.DrawString(qty, fontNormal, PdfSharpDrawing.XBrushes.Black, xQty + colQty / 2, y, centerAlign);
                gfx.DrawString(harga, fontNormal, PdfSharpDrawing.XBrushes.Black, xHarga + colHarga, y, rightAlign);
                gfx.DrawString(sub, fontNormal, PdfSharpDrawing.XBrushes.Black, xSubtotal + colSubtotal, y, rightAlign);
                y += lineHeight;
            }

            y += 4;
            gfx.DrawLine(PdfSharpDrawing.XPens.Black, leftMargin, y, pageWidth - rightMargin, y);
            y += 5;

            // Rincian total (dua sisi: label kiri, nilai kanan)
            double xTotalLabel = xHarga;               // posisi label (sejajar kolom harga)
            double xTotalValue = xSubtotal + colSubtotal; // posisi nilai rata kanan

            gfx.DrawString("Subtotal:", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalLabel, y, leftAlign);
            gfx.DrawString($"Rp {subtotal:N0}", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalValue, y, rightAlign);
            y += lineHeight;

            if (diskonPersen > 0)
            {
                gfx.DrawString($"Diskon ({diskonPersen}%):", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalLabel, y, leftAlign);
                decimal diskonNilai = subtotal * (diskonPersen / 100);
                gfx.DrawString($"-Rp {diskonNilai:N0}", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalValue, y, rightAlign);
                y += lineHeight;
            }

            gfx.DrawString("Total:", fontBold, PdfSharpDrawing.XBrushes.Black, xTotalLabel, y, leftAlign);
            gfx.DrawString($"Rp {grandTotal:N0}", fontBold, PdfSharpDrawing.XBrushes.Black, xTotalValue, y, rightAlign);
            y += lineHeight;

            gfx.DrawString("Metode:", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalLabel, y, leftAlign);
            gfx.DrawString(metodeBayar, fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalValue, y, rightAlign);
            y += lineHeight;

            if (metodeBayar == "CASH")
            {
                gfx.DrawString("Bayar:", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalLabel, y, leftAlign);
                gfx.DrawString($"Rp {uangBayar:N0}", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalValue, y, rightAlign);
                y += lineHeight;
                gfx.DrawString("Kembali:", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalLabel, y, leftAlign);
                gfx.DrawString($"Rp {kembalian:N0}", fontNormal, PdfSharpDrawing.XBrushes.Black, xTotalValue, y, rightAlign);
                y += lineHeight;
            }

            y += 5;
            gfx.DrawLine(PdfSharpDrawing.XPens.Black, leftMargin, y, pageWidth - rightMargin, y);
            y += 8;

            // Footer (rata tengah)
            gfx.DrawString("Terima kasih telah berbelanja!", fontBold, PdfSharpDrawing.XBrushes.Black, centerX, y, centerAlign);
            y += lineHeight;
            gfx.DrawString("~ Barang yang sudah dibeli tidak dapat", fontSmall, PdfSharpDrawing.XBrushes.Black, centerX, y, centerAlign);
            y += lineHeight;
            gfx.DrawString("  dikembalikan kecuali ada kerusakan ~", fontSmall, PdfSharpDrawing.XBrushes.Black, centerX, y, centerAlign);
        }

        // Dialog simpan PDF
        using (var saveDialog = new SaveFileDialog())
        {
            saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveDialog.FileName = $"Struk_{idTransaksi}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            saveDialog.Title = "Simpan Struk PDF";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                document.Save(saveDialog.FileName);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
            }
            else
            {
                string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Struk_{idTransaksi}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                document.Save(tempFile);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFile) { UseShellExecute = true });
            }
        }
    }
}
    }
}