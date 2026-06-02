using System;
using System.Drawing;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class MasterData
    {
        private ComboBox cbOutlet;
        private TextBox txtSearch;
        private DataGridView dgvProduk;
        private Button btnTambah, btnRefresh;

        private void InitializeComponent()
        {
            this.Text = "Master Data Produk";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Panel filter
            Panel pnlFilter = new Panel { Height = 80, Dock = DockStyle.Top, BackColor = Color.WhiteSmoke };
            Label lblOutlet = new Label { Text = "Outlet:", Location = new Point(20, 30), AutoSize = true };
            cbOutlet = new ComboBox { Location = new Point(80, 27), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cbOutlet.SelectedIndexChanged += CbOutlet_SelectedIndexChanged;

            Label lblSearch = new Label { Text = "Cari:", Location = new Point(320, 30), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(370, 27), Width = 200 };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnTambah = new Button { Text = "+ Tambah", Location = new Point(620, 25), Size = new Size(100, 30), BackColor = Color.FromArgb(174, 225, 225), FlatStyle = FlatStyle.Flat };
            btnTambah.Click += BtnTambah_Click;
            btnRefresh = new Button { Text = "⟳ Refresh", Location = new Point(730, 25), Size = new Size(100, 30), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += BtnRefresh_Click;

            pnlFilter.Controls.Add(lblOutlet);
            pnlFilter.Controls.Add(cbOutlet);
            pnlFilter.Controls.Add(lblSearch);
            pnlFilter.Controls.Add(txtSearch);
            pnlFilter.Controls.Add(btnTambah);
            pnlFilter.Controls.Add(btnRefresh);
            this.Controls.Add(pnlFilter);

            // DataGridView
            dgvProduk = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(940, 520),
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dgvProduk.Columns.Add("Kode", "Kode Produk");
            dgvProduk.Columns.Add("NamaProduk", "Nama Produk");
            dgvProduk.Columns.Add("Stok", "Stok");
            dgvProduk.Columns.Add("Harga", "Harga Jual");
            dgvProduk.Columns.Add("Edit", "Edit");
            dgvProduk.Columns.Add("Hapus", "Hapus");

            // PERBAIKAN: gunakan DataGridViewAutoSizeColumnMode (singular) untuk kolom
            dgvProduk.Columns["Edit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvProduk.Columns["Hapus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvProduk.CellContentClick += DgvProduk_CellContentClick;

            this.Controls.Add(dgvProduk);
        }
    }
}