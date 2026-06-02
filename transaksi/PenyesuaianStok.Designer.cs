using System;
using System.Drawing;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class PenyesuaianStok
    {
        private ComboBox cbOutlet;
        private DataGridView dgvStok;
        private Button btnSimpan, btnBatal;

        private void InitializeComponent()
        {
            this.Text = "Penyesuaian Stok - KasirWearIt";
            this.Size = new Size(950, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int startY = 30;

            // Title
            Label lblTitle = new Label
            {
                Text = "✏️ Penyesuaian Stok (Transfer Gudang → Sistem)",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 50, 60),
                Location = new Point(25, startY),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            startY += 50;

            // Label Outlet
            Label lblOutlet = new Label
            {
                Text = "Outlet",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Gray,
                Location = new Point(25, startY),
                AutoSize = true
            };
            this.Controls.Add(lblOutlet);

            // ComboBox Outlet
            cbOutlet = new ComboBox
            {
                Location = new Point(25, startY + 20),
                Width = 280,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White
            };
            cbOutlet.SelectedIndexChanged += CbOutlet_SelectedIndexChanged;
            this.Controls.Add(cbOutlet);
            startY += 60;

            // DataGridView
            dgvStok = new DataGridView
            {
                Location = new Point(25, startY),
                Size = new Size(880, 480),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(255, 200, 210)
            };

            // Kolom
            dgvStok.Columns.Add("No", "No");
            dgvStok.Columns.Add("Produk", "Produk");
            dgvStok.Columns.Add("QtySistem", "Qty di Sistem");
            dgvStok.Columns.Add("QtyGudang", "Qty di Gudang");

            DataGridViewButtonColumn btnTransfer = new DataGridViewButtonColumn
            {
                Name = "AksiTransfer",
                HeaderText = "Aksi Transfer",
                Text = "🔄 Transfer",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat
            };
            dgvStok.Columns.Add(btnTransfer);

            DataGridViewButtonColumn btnHapus = new DataGridViewButtonColumn
            {
                Name = "AksiHapus",
                HeaderText = "Hapus",
                Text = "❌",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat
            };
            dgvStok.Columns.Add(btnHapus);

            dgvStok.EnableHeadersVisualStyles = false;
            dgvStok.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 210, 220);
            dgvStok.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 40, 50);
            dgvStok.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvStok.ColumnHeadersHeight = 40;
            dgvStok.RowTemplate.Height = 35;
            dgvStok.CellContentClick += DgvStok_CellContentClick;

            this.Controls.Add(dgvStok);

            int buttonY = startY + dgvStok.Height + 20;

            // Tombol Batal
            btnBatal = new Button
            {
                Text = "Batal",
                Location = new Point(620, buttonY),
                Size = new Size(130, 42),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.Silver,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnBatal.FlatAppearance.BorderSize = 0;
            btnBatal.Click += BtnBatal_Click;
            this.Controls.Add(btnBatal);

            // Tombol Simpan
            btnSimpan = new Button
            {
                Text = "Simpan",
                Location = new Point(770, buttonY),
                Size = new Size(130, 42),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(174, 225, 225),
                ForeColor = Color.FromArgb(40, 60, 60),
                Cursor = Cursors.Hand
            };
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.Click += BtnSimpan_Click;
            this.Controls.Add(btnSimpan);
        }
    }
}