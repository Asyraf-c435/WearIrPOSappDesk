using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class Akun
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private PictureBox picProfile = null!;
        
        // Komponen Read-Only (Atas)
        private TextBox txtCurrentNama = null!;
        private TextBox txtCurrentUsername = null!;
        private TextBox txtCurrentPassword = null!;
        
        // Komponen Edit (Bawah)
        private TextBox txtPassLama = null!;
        private TextBox txtNewNama = null!;
        private TextBox txtNewUsername = null!;
        private TextBox txtNewPassword = null!;
        private Button btnSimpan = null!;
        private Button btnBatal = null!;

        private void InitializeComponentCustom()
        {
            this.Text = "Pengaturan Akun";
            this.Size = new Size(450, 780); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            
            this.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            // ==========================================
            // HEADER PINK
            // ==========================================
            Panel pnlHeader = new Panel() { Height = 50, Dock = DockStyle.Top, BackColor = Color.FromArgb(224, 158, 172) };
            Label lblTitle = new Label() { Text = "🔒 Pengaturan Akun", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 60), AutoSize = true, Location = new Point(15, 12) };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            int startX = 40;
            int startY = 65;

            // FOTO PROFIL
            picProfile = new PictureBox() { Size = new Size(80, 80), Location = new Point((this.Width - 80) / 2, startY), SizeMode = PictureBoxSizeMode.Zoom };
            try { picProfile.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "resources", "user_icon.png")); }
            catch { picProfile.BackColor = Color.WhiteSmoke; }
            this.Controls.Add(picProfile);
            
            startY += 95; 

            // ==========================================
            // BAGIAN 1: READ ONLY (INFO AKUN)
            // ==========================================
            Label lblTitleInfo = new Label() { Text = "Informasi Akun Saat Ini", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.HotPink, Location = new Point(startX, startY), AutoSize = true };
            this.Controls.Add(lblTitleInfo);
            startY += 30;

            // Current Nama 
            Label lblNama = new Label() { Text = "Nama Lengkap", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(startX, startY), AutoSize = true };
            txtCurrentNama = new TextBox() { 
                Font = new Font("Segoe UI", 11), 
                Location = new Point(startX, startY + 20), 
                Width = 370, 
                ReadOnly = true, 
                BackColor = Color.WhiteSmoke, 
                ForeColor = Color.DimGray 
            };
            this.Controls.Add(lblNama); this.Controls.Add(txtCurrentNama);
            startY += 55;

            // Current Username
            Label lblUser = new Label() { Text = "Username", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(startX, startY), AutoSize = true };
            txtCurrentUsername = new TextBox() { 
                Font = new Font("Segoe UI", 11), 
                Location = new Point(startX, startY + 20), 
                Width = 370, 
                ReadOnly = true, 
                BackColor = Color.WhiteSmoke, 
                ForeColor = Color.DimGray 
            };
            this.Controls.Add(lblUser); this.Controls.Add(txtCurrentUsername);
            startY += 55;

            // Current Password
            Label lblPass = new Label() { Text = "Password", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(startX, startY), AutoSize = true };
            txtCurrentPassword = new TextBox() { Font = new Font("Segoe UI", 11), Location = new Point(startX, startY + 20), Width = 370, ReadOnly = true, BackColor = Color.WhiteSmoke, ForeColor = Color.DimGray, UseSystemPasswordChar = true };
            this.Controls.Add(lblPass); this.Controls.Add(txtCurrentPassword);
            startY += 60;

            // Divider
            Panel pnlDivider = new Panel() { Size = new Size(370, 1), Location = new Point(startX, startY), BackColor = Color.FromArgb(220, 220, 220) };
            this.Controls.Add(pnlDivider);
            startY += 20;

            // ==========================================
            // BAGIAN 2: EDIT / UBAH AKUN
            // ==========================================
            Label lblTitleEdit = new Label() { Text = "Perbarui Data Akun", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.HotPink, Location = new Point(startX, startY), AutoSize = true };
            this.Controls.Add(lblTitleEdit);
            startY += 35;

            // Password Lama
            Label lblPassLama = new Label() { Text = "Password Lama (Wajib Diisi)", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(232, 65, 24), Location = new Point(startX, startY), AutoSize = true };
            txtPassLama = new TextBox() { Font = new Font("Segoe UI", 11), Location = new Point(startX, startY + 20), Width = 370, UseSystemPasswordChar = true };
            this.Controls.Add(lblPassLama); this.Controls.Add(txtPassLama);
            startY += 60;

            // Nama Baru
            Label lblNewNama = new Label() { Text = "Nama Lengkap Baru", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(startX, startY), AutoSize = true };
            txtNewNama = new TextBox() { Font = new Font("Segoe UI", 11), Location = new Point(startX, startY + 20), Width = 370 };
            this.Controls.Add(lblNewNama); this.Controls.Add(txtNewNama);
            startY += 60;

            // Username Baru
            Label lblNewUser = new Label() { Text = "Username Baru", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(startX, startY), AutoSize = true };
            txtNewUsername = new TextBox() { Font = new Font("Segoe UI", 11), Location = new Point(startX, startY + 20), Width = 370 };
            this.Controls.Add(lblNewUser); this.Controls.Add(txtNewUsername);
            startY += 60;

            // Password Baru
            Label lblNewPass = new Label() { Text = "Password Baru (Kosongkan jika tak diubah)", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(startX, startY), AutoSize = true };
            txtNewPassword = new TextBox() { Font = new Font("Segoe UI", 11), Location = new Point(startX, startY + 20), Width = 370, UseSystemPasswordChar = true };
            this.Controls.Add(lblNewPass); this.Controls.Add(txtNewPassword);
            startY += 70;

            // Tombol Aksi
            btnSimpan = new Button() { Text = "Simpan", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.HotPink, ForeColor = Color.White, Location = new Point(startX, startY), Size = new Size(180, 45), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.Click += BtnSimpan_Click;

            btnBatal = new Button() { Text = "Batal", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.Silver, ForeColor = Color.White, Location = new Point(startX + 190, startY), Size = new Size(180, 45), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnBatal.FlatAppearance.BorderSize = 0;
            btnBatal.Click += (s, e) => this.Close();

            this.Controls.Add(btnSimpan);
            this.Controls.Add(btnBatal);
        }
    }
}