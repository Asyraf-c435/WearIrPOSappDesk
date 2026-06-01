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

            string nama = string.IsNullOrWhiteSpace(Session.NamaLengkap) ? "-" : Session.NamaLengkap;
            lblFooterSalam.Text = $"{salam} ☀️";
            lblFooterNama.Text = $"👤 {nama}";
        }

        private void MuatProdukDariDatabase(string keyword = "")
        {
            flpProducts.Controls.Clear();
            try
            {
                System.Data.DataTable dt = null!;

                if (string.IsNullOrWhiteSpace(keyword) || keyword == "Cari nama barang di sini...")
                {
                    dt = DatabaseConnection.AmbilProdukAktif();
                }
                else
                {
                    dt = DatabaseConnection.SearchProduk(keyword);
                }

                if (dt != null && dt.Rows != null)
                {
                    foreach (System.Data.DataRow databaseRow in dt.Rows)
                    {
                        string kodeProduk = databaseRow["kode_produk"].ToString() ?? "-";
                        string namaProduk = databaseRow["nama_produk"].ToString() ?? "Tanpa Nama";
                        decimal harga = Convert.ToDecimal(databaseRow["harga_jual"]);
                        int stok = Convert.ToInt32(databaseRow["stok"]);

                        Panel pnlCard = new Panel() { Size = new Size(135, 100), BackColor = Color.White, Margin = new Padding(8), Cursor = Cursors.Hand };
                        string tag = $"{namaProduk}|{harga}";
                        pnlCard.Tag = tag;
                        pnlCard.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.DarkGray, ButtonBorderStyle.Solid); };

                        string labelNama = stok <= 0 ? $"[HABIS]\n{namaProduk}" : namaProduk;
                        Label lblNama = new Label() { Text = labelNama, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = stok <= 0 ? Color.Gray : Color.FromArgb(50, 50, 50), Location = new Point(5, 10), Size = new Size(125, 45), TextAlign = ContentAlignment.TopCenter, Cursor = Cursors.Hand };
                        Label lblHarga = new Label() { Text = $"Rp {harga:N0}", Font = new Font("Segoe UI", 9, FontStyle.Regular), ForeColor = Color.FromArgb(120, 60, 80), Location = new Point(5, 65), Size = new Size(125, 25), TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };

                        pnlCard.Controls.Add(lblNama);
                        pnlCard.Controls.Add(lblHarga);

                        string kProd = kodeProduk;
                        string nProd = namaProduk;
                        decimal hJual = harga;
                        int sToko = stok;

                        Action aksiKlikCard = () => {
                            if (sToko <= 0)
                            {
                                MessageBox.Show($"Stok '{nProd}' sedang kosong!", "Info Stok", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            TambahAtauUpdateOrder(kProd, nProd, hJual);
                        };

                        pnlCard.Click += (s, e) => aksiKlikCard();
                        lblNama.Click += (s, e) => aksiKlikCard();
                        lblHarga.Click += (s, e) => aksiKlikCard();

                        flpProducts.Controls.Add(pnlCard);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat produk pakaian: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object? sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword) || keyword == "Cari nama barang di sini...")
            {
                MuatProdukDariDatabase("");
            }
            else
            {
                MuatProdukDariDatabase(keyword);
            }
        }

        private void TambahAtauUpdateOrder(string kodeProduk, string namaProduk, decimal harga)
        {
            int price = (int)harga;
            string panelName = "order_" + kodeProduk;

            // Cek apakah barang sudah ada di keranjang
            foreach (Control ctrl in flpOrderList.Controls)
            {
                if (ctrl is Panel pnlItem && pnlItem.Name == panelName)
                {
                    // Ambil komponen di dalam panel
                    Label lblQty = (Label)pnlItem.Controls.Find("lblQty", false)[0];
                    Label lblSubtotal = (Label)pnlItem.Controls.Find("lblSubtotal", false)[0];
                    int currentQty = int.Parse(lblQty.Text);
                    int newQty = currentQty + 1;

                    lblQty.Text = newQty.ToString();
                    lblSubtotal.Text = $"Rp {(price * newQty):N0}";

                    HitungUlangTotal();
                    return;
                }
            }

            // Jika belum ada, buat card baru
            BuatCardOrder(kodeProduk, namaProduk, price);
        }

        private void BuatCardOrder(string kodeProduk, string namaProduk, int harga)
        {
            Panel pnlItem = new Panel();
            pnlItem.Name = "order_" + kodeProduk;
            pnlItem.Size = new Size(315, 80);
            pnlItem.Margin = new Padding(5);
            pnlItem.BackColor = Color.WhiteSmoke;
            pnlItem.Tag = harga; // simpan harga satuan
            pnlItem.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlItem.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            Label lblName = new Label() { Text = namaProduk, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
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
                if (q > 1)
                {
                    q--;
                    lblQty.Text = q.ToString();
                    lblSubtotal.Text = $"Rp {(harga * q):N0}";
                    HitungUlangTotal();
                }
                else
                {
                    flpOrderList.Controls.Remove(pnlItem);
                    pnlItem.Dispose();
                    HitungUlangTotal();
                }
            };

            pnlItem.Controls.Add(lblName);
            pnlItem.Controls.Add(lblPrice);
            pnlItem.Controls.Add(lblSubtotal);
            pnlItem.Controls.Add(btnMin);
            pnlItem.Controls.Add(lblQty);
            pnlItem.Controls.Add(btnPlus);

            flpOrderList.Controls.Add(pnlItem);
            HitungUlangTotal();
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
                // Ambil kode produk dari nama panel (hapus prefix "order_")
                string kodeProduk = pnl.Name.StartsWith("order_") ? pnl.Name.Substring(6) : "-";
                string nama = pnl.Controls[0].Text;
                int hargaSatuan = pnl.Tag is int val ? val : 0;
                string qty = pnl.Controls.Find("lblQty", false)[0].Text;
                string subtotal = pnl.Controls.Find("lblSubtotal", false)[0].Text;

                // Kirim 5 data: Kode, Nama, Harga (string), Qty, Subtotal
                daftarPesanan.Add(new string[] { kodeProduk, nama, $"Rp {hargaSatuan:N0}", qty, subtotal });
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
    }
}