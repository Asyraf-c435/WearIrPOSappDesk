using System;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class MasterData : Form, IEmbeddableForm
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmbedded { get; set; } = false;

        public MasterData()
        {
            InitializeComponent();
            LoadOutlets();
        }

        private void LoadOutlets()
        {
            try
            {
                string query = "SELECT id_outlet, nama_outlet FROM outlet ORDER BY nama_outlet";
                DataTable dt = DatabaseConnection.Query(query);
                cbOutlet.DataSource = dt;
                cbOutlet.DisplayMember = "nama_outlet";
                cbOutlet.ValueMember = "id_outlet";
                if (dt.Rows.Count > 0)
                {
                    cbOutlet.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat outlet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // HANYA SATU method LoadProducts
        private void LoadProducts()
        {
            try
            {
                if (cbOutlet.SelectedValue == null) return;
                int idOutlet;
                try
                {
                    idOutlet = Convert.ToInt32(cbOutlet.SelectedValue);
                }
                catch
                {
                    if (cbOutlet.SelectedItem is DataRowView drv)
                        idOutlet = Convert.ToInt32(drv["id_outlet"]);
                    else
                    {
                        MessageBox.Show("Outlet tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string keyword = txtSearch.Text.Trim();
                string query = @"
                    SELECT p.kode_produk, p.nama_produk, p.harga_jual,
                           COALESCE(s.qty_sistem, 0) AS qty_sistem
                    FROM produk p
                    LEFT JOIN stok s ON p.kode_produk = s.kode_produk AND s.id_outlet = @outlet
                    WHERE (@keyword = '' OR p.kode_produk LIKE @kw OR p.nama_produk LIKE @kw)
                    ORDER BY p.nama_produk";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@outlet", idOutlet),
                    new MySqlParameter("@keyword", keyword),
                    new MySqlParameter("@kw", "%" + keyword + "%")
                };
                DataTable dt = DatabaseConnection.Query(query, parameters);

                dgvProduk.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    int idx = dgvProduk.Rows.Add(
                        row["kode_produk"].ToString(),
                        row["nama_produk"].ToString(),
                        row["qty_sistem"].ToString(),
                        Convert.ToDecimal(row["harga_jual"]).ToString("N0"),
                        "✏️ Edit",
                        "🗑️ Hapus"
                    );
                    dgvProduk.Rows[idx].Tag = row["kode_produk"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat produk: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CbOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOutlet.SelectedValue == null) return;
            // Gunakan BeginInvoke untuk memastikan binding selesai
            this.BeginInvoke(new Action(() => LoadProducts()));
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void BtnTambah_Click(object sender, EventArgs e)
        {
            using (var formMaster = new Master())
            {
                formMaster.ShowDialog();
                LoadProducts();
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void DgvProduk_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string kodeProduk = dgvProduk.Rows[e.RowIndex].Tag?.ToString();
            if (string.IsNullOrEmpty(kodeProduk)) return;

            if (dgvProduk.Columns[e.ColumnIndex].Name == "Edit")
            {
                using (var formMaster = new Master(kodeProduk))
                {
                    formMaster.ShowDialog();
                    LoadProducts();
                }
            }
            else if (dgvProduk.Columns[e.ColumnIndex].Name == "Hapus")
            {
                DialogResult res = MessageBox.Show($"Hapus produk '{dgvProduk.Rows[e.RowIndex].Cells["NamaProduk"].Value}' ?",
                    "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    HapusProduk(kodeProduk);
                    LoadProducts();
                }
            }
        }

        private void HapusProduk(string kodeProduk)
        {
            try
            {
                string query = "DELETE FROM produk WHERE kode_produk = @kode";
                int rows = DatabaseConnection.Exec(query, new MySqlParameter("@kode", kodeProduk));
                if (rows > 0)
                    MessageBox.Show("Produk berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Gagal menghapus produk.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}   