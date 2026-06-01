using System;
using System.Drawing;
using System.Windows.Forms;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class Pos : Form
    {
      public Pos()
    {
        InitializeComponentCustom();
        txtSearch.TextChanged += txtSearch_TextChanged;

        MuatProdukDariDatabase();
        this.FormClosed += (s, e) => Application.Exit();
        RefreshFooter();
    }

    private void RefreshFooter()
    {
        int jam = DateTime.Now.Hour;
        string salam = jam < 11 ? "Selamat Pagi" :
                       jam < 15 ? "Selamat Siang" :
                       jam < 18 ? "Selamat Sore" : "Selamat Malam";
        
        // Gunakan Session.NamaLengkap (bukan FormLogin.NamaUserLogin)
        string nama = string.IsNullOrWhiteSpace(Session.NamaLengkap) ? "-" : Session.NamaLengkap;
        lblFooterSalam.Text = $"{salam} ☀️";
        lblFooterNama.Text = $"👤 {nama}";
    }

    private void MuatProdukDariDatabase(string keyword = "")
{
    flpProducts.Controls.Clear();
    try
    {
        System.Data.DataTable dt = null!; // Inisialisasi awal

        if (string.IsNullOrWhiteSpace(keyword))
        {
            dt = DatabaseConnection.AmbilProdukAktif(); 
        }
        else
        {
            dt = DatabaseConnection.SearchProduk(keyword);
        }

        // PERBAIKAN UTAMA: Cek apakah data datatable murni null sebelum masuk ke perulangan
        if (dt != null && dt.Rows != null)
        {
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (row == null) continue;

                string kode = row["kode_produk"] != DBNull.Value ? row["kode_produk"].ToString() ?? "" : "";
                string nama = row["nama_produk"] != DBNull.Value ? row["nama_produk"].ToString() ?? "" : "";
                
                    int harga = 0;
                    if (row["harga_jual"] != DBNull.Value)
                {
                    harga = Convert.ToInt32(row["harga_jual"]);
                }

                int stok = 0;
                if (row["stok"] != DBNull.Value)
                {
                    stok = Convert.ToInt32(row["stok"]);
                }

                if (!string.IsNullOrEmpty(kode))
                {
                    BuatCardProduk(kode, nama, harga, stok);
                }
            }
        }
        else
        {
            MessageBox.Show("Data produk tidak ditemukan atau database mengembalikan nilai kosong.", 
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Gagal memuat produk dari database.\n\nDetail: " + ex.Message,
            "Error Database internal", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

      private void txtSearch_TextChanged(object? sender, EventArgs e)
        {
            // 1. Ambil teks, hapus spasi di depan/belakang
            string keyword = txtSearch.Text.Trim();

            // 2. LOGIKA PINTAR:
            // Jika teks kosong ATAU teksnya adalah placeholder, kirim string kosong ("")
            // agar stored procedure menganggapnya sebagai "tampilkan semua" (karena p_keyword = '')
            if (string.IsNullOrWhiteSpace(keyword) || keyword == "Cari nama barang di sini...")
            {
                MuatProdukDariDatabase(""); 
            }
            else
            {
                // Jika teks asli (bukan placeholder), kirim keyword ke database
                MuatProdukDariDatabase(keyword);
            }
        }

        private void TambahAtauUpdateOrder(string kodeProduk, int harga)
        {
            Control[] found = flpOrderList.Controls.Find("cardOrder_" + kodeProduk.Replace(" ", ""), false);

            if (found.Length > 0)
            {
                Panel pnl = (Panel)found[0];
                Label lblQty = (Label)pnl.Controls.Find("lblQty", false)[0];
                Label lblSubtotal = (Label)pnl.Controls.Find("lblSubtotal", false)[0];

                int q = int.Parse(lblQty.Text) + 1;
                lblQty.Text = q.ToString();
                lblSubtotal.Text = $"Rp {(harga * q):N0}";
            }
            else
            {
                BuatCardOrder(kodeProduk, harga);
            }

            HitungUlangTotal();
        }

        private void BuatCardOrder(string kodeProduk, int harga)
        {
            Panel pnlItem = new Panel();
            pnlItem.Name = "cardOrder_" + kodeProduk.Replace(" ", "");
            pnlItem.Size = new Size(315, 80);
            pnlItem.Margin = new Padding(5);
            pnlItem.BackColor = Color.WhiteSmoke;
            pnlItem.Tag = harga;
            pnlItem.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlItem.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            Label lblName = new Label() { Text = kodeProduk, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
            Label lblPrice = new Label() { Text = $"@ Rp {harga:N0}", Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, Location = new Point(10, 32), AutoSize = true };
            Label lblSubtotal = new Label() { Name = "lblSubtotal", Text = $"Rp {harga:N0}", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.HotPink, Location = new Point(10, 52), AutoSize = true };

            Button btnMin = new Button() { Text = "-", Font = new Font("Segoe UI", 10, FontStyle.Bold), Size = new Size(30, 30), Location = new Point(180, 25), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            Label lblQty = new Label() { Name = "lblQty", Text = "1", Font = new Font("Segoe UI", 11, FontStyle.Bold), Size = new Size(35, 30), Location = new Point(215, 25), TextAlign = ContentAlignment.MiddleCenter };
            Button btnPlus = new Button() { Text = "+", Font = new Font("Segoe UI", 10, FontStyle.Bold), Size = new Size(30, 30), Location = new Point(255, 25), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            btnPlus.Click += (s, e) => {
                int q = int.Parse(lblQty.Text) + 1;
                lblQty.Text = q.ToString();
                lblSubtotal.Text = $"Rp {(harga * q):N0}";
                HitungUlangTotal();
            };

            btnMin.Click += (s, e) => {
                int q = int.Parse(lblQty.Text);
                if (q > 1) {
                    q--;
                    lblQty.Text = q.ToString();
                    lblSubtotal.Text = $"Rp {(harga * q):N0}";
                    HitungUlangTotal();
                } else {
                    flpOrderList.Controls.Remove(pnlItem);
                    pnlItem.Dispose();
                    HitungUlangTotal();
                }
            };

            pnlItem.Controls.Add(lblName); pnlItem.Controls.Add(lblPrice); pnlItem.Controls.Add(lblSubtotal);
            pnlItem.Controls.Add(btnMin); pnlItem.Controls.Add(lblQty); pnlItem.Controls.Add(btnPlus);

            flpOrderList.Controls.Add(pnlItem);
        }

        private void HitungUlangTotal()
        {
            int totalItem = 0;
            int totalBayar = 0;

            foreach (Control c in flpOrderList.Controls)
            {
                if (c is Panel pnl)
                {
                    Label lblQ = (Label)pnl.Controls.Find("lblQty", false)[0];
                    int q = int.Parse(lblQ.Text);
                    int hargaSatuan = pnl.Tag is int val ? val : 0;

                    totalItem += q;
                    totalBayar += (hargaSatuan * q);
                }
            }

            lblTotalBarang.Text = $"Total Produk: {totalItem} item";
            lblTotalBayar.Text = $"Total: Rp {totalBayar:N0}";
        }

        private void BtnBayar_Click(object? sender, EventArgs e)
        {
            if (flpOrderList.Controls.Count > 0)
            {
                int totalBelanjaSekarang = int.Parse(lblTotalBayar.Text.Replace("Total: Rp ", "").Replace(",", ""));

                System.Collections.Generic.List<string[]> daftarPesanan = new System.Collections.Generic.List<string[]>();

                foreach (Control c in flpOrderList.Controls)
                {
                    if (c is Panel pnl)
                    {
                        string nama = pnl.Controls[0].Text;
                        int hargaSatuan = pnl.Tag is int val ? val : 0;
                        string qty = pnl.Controls.Find("lblQty", false)[0].Text;
                        string subtotal = pnl.Controls.Find("lblSubtotal", false)[0].Text;

                        daftarPesanan.Add(new string[] { nama, $"Rp {hargaSatuan:N0}", qty, subtotal });
                    }
                }

                Pembayaran formCheckout = new Pembayaran(totalBelanjaSekarang, daftarPesanan, () => {
                    flpOrderList.Controls.Clear();
                    HitungUlangTotal();
                });

                formCheckout.ShowDialog();
            }
            else
            {
                MessageBox.Show("Keranjang order kosong. Silakan pilih produk terlebih dahulu.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnLaporan_Click(object? sender, EventArgs e)
        {
            Laporan formLaporan = new Laporan();
            formLaporan.ShowDialog();
        }

        private void BtnLock_Click(object? sender, EventArgs e)
        {
            Akun formAkun = new Akun();
            formAkun.ShowDialog();
        }

        private void BuatCardProduk(string kode, string namaProduk, int harga, int stok)
        {
            // Simpan kode + harga di Tag (array object)
            var tag = new object[] { kode, harga, stok };

            Panel pnlCard = new Panel() { Size = new Size(135, 110), Margin = new Padding(6), BackColor = Color.White, Cursor = Cursors.Hand };
            pnlCard.Tag = tag;
            pnlCard.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.DarkGray, ButtonBorderStyle.Solid); };

            string labelNama = stok <= 0 ? $"[HABIS]\n{namaProduk}" : namaProduk;
            Label lblNama = new Label() { Text = labelNama, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = stok <= 0 ? Color.Gray : Color.FromArgb(50, 50, 50), Location = new Point(5, 10), Size = new Size(125, 45), TextAlign = ContentAlignment.TopCenter, Cursor = Cursors.Hand };
            Label lblHarga = new Label() { Text = $"Rp {harga:N0}", Font = new Font("Segoe UI", 9, FontStyle.Regular), ForeColor = Color.FromArgb(120, 60, 80), Location = new Point(5, 65), Size = new Size(125, 25), TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };

            pnlCard.Controls.Add(lblNama);
            pnlCard.Controls.Add(lblHarga);

            Action aksiKlikCard = () => {
                if (stok <= 0)
                {
                    MessageBox.Show($"Stok '{namaProduk}' sedang kosong!", "Info Stok", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                TambahAtauUpdateOrder(namaProduk, harga);
            };

            pnlCard.Click += (s, e) => aksiKlikCard();
            lblNama.Click += (s, e) => aksiKlikCard();
            lblHarga.Click += (s, e) => aksiKlikCard();

            flpProducts.Controls.Add(pnlCard);
        }
    }
}