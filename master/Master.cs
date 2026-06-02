using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class Master : Form
    {
        private string _editModeKode = null;

        public Master() : this(null) { }

        public Master(string kodeProduk)
        {
            InitializeComponent();
            _editModeKode = kodeProduk;

            // Pasang event handler
            txtHargaBeli.KeyPress += HanyaBolehAngka_KeyPress;
            txtHargaJual.KeyPress += HanyaBolehAngka_KeyPress;
            txtStokAwal.KeyPress += HanyaBolehAngka_KeyPress;
            btnSimpan.Click += btnSimpan_Click;
            
            // Event Enter untuk memilih semua teks pada Stok Awal
            txtStokAwal.Enter += (s, e) => txtStokAwal.SelectAll();

            if (!string.IsNullOrEmpty(_editModeKode))
            {
                lblTitle.Text = "Produk : Edit";
                LoadDataForEdit();
                txtStokAwal.Enabled = false;
                txtStokAwal.Text = "0";
            }
            else
            {
                txtStokAwal.Text = "0";
            }
        }

        private void LoadDataForEdit()
        {
            string query = "SELECT kode_produk, nama_produk, harga_beli, harga_jual FROM produk WHERE kode_produk = @kode";
            DataTable dt = DatabaseConnection.Query(query, new MySqlParameter("@kode", _editModeKode));
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtKodeProduk.Text = row["kode_produk"].ToString();
                txtKodeProduk.ReadOnly = true;
                txtNamaProduk.Text = row["nama_produk"].ToString();
                txtHargaBeli.Text = row["harga_beli"].ToString();
                txtHargaJual.Text = row["harga_jual"].ToString();
            }
        }

        private async void btnSimpan_Click(object sender, EventArgs e)
        {
            btnSimpan.Enabled = false;
            try
            {
                if (!ValidasiInput()) return;

                string kodeProduk = txtKodeProduk.Text.Trim();
                string namaProduk = txtNamaProduk.Text.Trim();
                decimal hargaBeli = Convert.ToDecimal(txtHargaBeli.Text);
                decimal hargaJual = Convert.ToDecimal(txtHargaJual.Text);
                
                if (!int.TryParse(txtStokAwal.Text.Trim(), out int stokAwal) || stokAwal < 0)
                {
                    MessageBox.Show("Stok awal harus berupa angka positif atau 0.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStokAwal.Focus();
                    return;
                }
                // Batasi maksimal stok
                if (stokAwal > 999999)
                {
                    MessageBox.Show("Stok awal maksimal 999.999", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStokAwal.Focus();
                    return;
                }

                bool success;
                if (string.IsNullOrEmpty(_editModeKode))
                {
                    if (CekKodeProdukSudahAda(kodeProduk))
                    {
                        MessageBox.Show("Kode Produk sudah digunakan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtKodeProduk.Focus();
                        return;
                    }
                    success = SimpanProduk(kodeProduk, namaProduk, hargaBeli, hargaJual, stokAwal);
                }
                else
                {
                    success = UpdateProduk(kodeProduk, namaProduk, hargaBeli, hargaJual);
                }

                if (success)
                {
                    MessageBox.Show("Data produk berhasil disimpan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan sistem: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSimpan.Enabled = true;
            }
        }

        private bool ValidasiInput()
        {
            if (string.IsNullOrWhiteSpace(txtKodeProduk.Text))
            {
                MessageBox.Show("Kode Produk wajib diisi", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKodeProduk.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNamaProduk.Text))
            {
                MessageBox.Show("Nama Produk wajib diisi", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamaProduk.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtHargaJual.Text) || Convert.ToDecimal(txtHargaJual.Text) == 0)
            {
                MessageBox.Show("Harga Jual wajib diisi", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHargaJual.Focus();
                return false;
            }
            if (Convert.ToDecimal(txtHargaBeli.Text) < 0)
            {
                MessageBox.Show("Harga Beli tidak boleh negatif.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHargaBeli.Focus();
                return false;
            }
            if (Convert.ToDecimal(txtHargaJual.Text) < 0)
            {
                MessageBox.Show("Harga Jual tidak boleh negatif.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHargaJual.Focus();
                return false;
            }
            if (!int.TryParse(txtStokAwal.Text, out int stok) || stok < 0)
            {
                MessageBox.Show("Stok awal harus berupa angka positif atau 0.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStokAwal.Focus();
                return false;
            }
            if (stok > 999999)
            {
                MessageBox.Show("Stok awal maksimal 999.999", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStokAwal.Focus();
                return false;
            }
            return true;
        }

        private bool CekKodeProdukSudahAda(string kodeProduk)
        {
            string query = "SELECT COUNT(*) FROM produk WHERE kode_produk = @kode";
            object result = DatabaseConnection.Scalar(query, new MySqlParameter("@kode", kodeProduk));
            int count = (result == null) ? 0 : Convert.ToInt32(result);
            return count > 0;
        }

    private bool SimpanProduk(string kode, string nama, decimal beli, decimal jual, int stokAwal)
{
    using (MySqlConnection conn = DatabaseConnection.GetConnection()) // sudah terbuka
    {
        // ❌ HAPUS baris ini → conn.Open();
        using (MySqlTransaction trans = conn.BeginTransaction())
        {
            try
            {
                // Insert produk
                string queryProduk = @"INSERT INTO produk (kode_produk, nama_produk, harga_beli, harga_jual) 
                                       VALUES (@kode, @nama, @beli, @jual)";
                using (MySqlCommand cmd = new MySqlCommand(queryProduk, conn, trans))
                {
                    cmd.Parameters.AddWithValue("@kode", kode);
                    cmd.Parameters.AddWithValue("@nama", nama);
                    cmd.Parameters.AddWithValue("@beli", beli);
                    cmd.Parameters.AddWithValue("@jual", jual);
                    cmd.ExecuteNonQuery();
                }

                // Ambil semua outlet
                DataTable dtOutlet = new DataTable();
                string outletQuery = "SELECT id_outlet FROM outlet";
                using (MySqlCommand cmdOut = new MySqlCommand(outletQuery, conn, trans))
                {
                    using (var reader = cmdOut.ExecuteReader())
                    {
                        dtOutlet.Load(reader);
                    }
                }

                if (dtOutlet.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada outlet. Tambahkan outlet terlebih dahulu.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trans.Rollback();
                    return false;
                }

                // Insert stok untuk setiap outlet
                string queryStok = @"INSERT INTO stok (id_outlet, kode_produk, qty_sistem, qty_gudang) 
                                     VALUES (@id_outlet, @kode, 0, @stokAwal)";
                using (MySqlCommand cmdStok = new MySqlCommand(queryStok, conn, trans))
                {
                    cmdStok.Parameters.AddWithValue("@kode", kode);
                    cmdStok.Parameters.AddWithValue("@stokAwal", stokAwal);
                    cmdStok.Parameters.Add(new MySqlParameter("@id_outlet", MySqlDbType.Int32));
                    foreach (DataRow row in dtOutlet.Rows)
                    {
                        int idOutlet = Convert.ToInt32(row["id_outlet"]);
                        cmdStok.Parameters["@id_outlet"].Value = idOutlet;
                        cmdStok.ExecuteNonQuery();
                    }
                }

                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show($"Gagal menyimpan produk dan stok: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
        private bool UpdateProduk(string kode, string nama, decimal beli, decimal jual)
        {
            string query = @"UPDATE produk SET nama_produk = @nama, harga_beli = @beli, harga_jual = @jual 
                             WHERE kode_produk = @kode";
            int rows = DatabaseConnection.Exec(query,
                new MySqlParameter("@nama", nama),
                new MySqlParameter("@beli", beli),
                new MySqlParameter("@jual", jual),
                new MySqlParameter("@kode", kode));
            return rows > 0;
        }

        private void HanyaBolehAngka_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}