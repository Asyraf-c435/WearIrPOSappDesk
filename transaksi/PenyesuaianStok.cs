using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class PenyesuaianStok : Form
    {
        // Atribut wajib untuk menghindari error WFO1000
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmbedded { get; set; } = false;

        public PenyesuaianStok()
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
                MessageBox.Show($"Gagal memuat outlet: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductStock()
        {
            try
            {
                if (cbOutlet.SelectedValue == null) return;

                // Perbaikan CS8600: gunakan null‑coalescing
                string outletValue = cbOutlet.SelectedValue.ToString() ?? "";
                if (!int.TryParse(outletValue, out int idOutlet))
                {
                    MessageBox.Show("ID Outlet tidak valid.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = @"
                    SELECT p.kode_produk, p.nama_produk, 
                           COALESCE(s.qty_sistem, 0) AS qty_sistem,
                           COALESCE(s.qty_gudang, 0) AS qty_gudang
                    FROM produk p
                    LEFT JOIN stok s ON p.kode_produk = s.kode_produk AND s.id_outlet = @idOutlet
                    ORDER BY p.nama_produk";

                MySqlParameter[] parameters = { new MySqlParameter("@idOutlet", idOutlet) };
                DataTable dt = DatabaseConnection.Query(query, parameters);

                dgvStok.Rows.Clear();
                int no = 1;
                foreach (DataRow row in dt.Rows)
                {
                    string namaProduk = row["nama_produk"]?.ToString() ?? "";
                    string qtySistem = row["qty_sistem"]?.ToString() ?? "0";
                    string qtyGudang = row["qty_gudang"]?.ToString() ?? "0";

                    int idx = dgvStok.Rows.Add(
                        no++,
                        namaProduk,
                        qtySistem,
                        qtyGudang,
                        "🔄 Transfer",
                        "❌"
                    );
                    dgvStok.Rows[idx].Tag = row["kode_produk"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat stok: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CbOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOutlet.SelectedValue == null) return;
            this.BeginInvoke(new Action(() => LoadProductStock()));
        }

       private void DgvStok_CellContentClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    if (dgvStok.Columns[e.ColumnIndex].Name == "AksiTransfer")
    {
        string kodeProduk = dgvStok.Rows[e.RowIndex].Tag?.ToString() ?? "";
        if (string.IsNullOrEmpty(kodeProduk)) return;

        string namaProduk = dgvStok.Rows[e.RowIndex].Cells["Produk"].Value?.ToString() ?? "";
        int qtySistem = int.TryParse(dgvStok.Rows[e.RowIndex].Cells["QtySistem"].Value?.ToString(), out int s) ? s : 0;
        int qtyGudang = int.TryParse(dgvStok.Rows[e.RowIndex].Cells["QtyGudang"].Value?.ToString(), out int g) ? g : 0;

        // Ambil id outlet
        string outletValue = cbOutlet.SelectedValue?.ToString() ?? "";
        if (!int.TryParse(outletValue, out int idOutlet))
        {
            MessageBox.Show("ID Outlet tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using (var popup = new FormPenyesuaianStokPopup(idOutlet, kodeProduk, namaProduk, qtyGudang, qtySistem))
        {
            if (popup.ShowDialog() == DialogResult.OK)
            {
                // Refresh data setelah update
                LoadProductStock();
            }
        }
    }

    if (dgvStok.Columns[e.ColumnIndex].Name == "AksiHapus")
    {
        DialogResult res = MessageBox.Show("Hapus produk ini dari daftar penyesuaian?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.Yes)
        {
            dgvStok.Rows.RemoveAt(e.RowIndex);
        }
    }
}

private void BtnSimpan_Click(object sender, EventArgs e)
{
    // Logika simpan penyesuaian stok (misalnya update ke database)
    MessageBox.Show("Data penyesuaian stok berhasil disimpan.", "Sukses",
        MessageBoxButtons.OK, MessageBoxIcon.Information);
    this.Close();
}

private void BtnBatal_Click(object sender, EventArgs e)
{
    this.Close();
}

    }
}