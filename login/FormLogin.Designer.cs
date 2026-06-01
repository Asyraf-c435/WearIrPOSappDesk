using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KasirWearIt
{
    partial class FormLogin
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

        // Deklarasi Komponen UI
        private TextBox txtUser = null!;
        private TextBox txtPass = null!;
        private Button btnLogin = null!;
        private CheckBox chkRemember = null!;
        private PictureBox picProfile = null!;

        private void InitializeComponentCustom()
        {
            this.Text = "Login - WEAR IT FASHION";
            this.Size = new Size(850, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true; 
            this.BackColor = Color.White;

            // Garis Aksen Atas
            Panel pnlAccent = new Panel() { Dock = DockStyle.Top, Height = 5, BackColor = Color.FromArgb(44, 62, 80) };
            this.Controls.Add(pnlAccent);

            // Tombol Close
            Label lblClose = new Label() { Text = "✕", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Silver, Cursor = Cursors.Hand, Location = new Point(815, 15), AutoSize = true };
            lblClose.Click += (s, e) => Application.Exit();
            lblClose.MouseEnter += (s, e) => lblClose.ForeColor = Color.Crimson;
            lblClose.MouseLeave += (s, e) => lblClose.ForeColor = Color.Silver;
            this.Controls.Add(lblClose);

            // Profile Picture
            picProfile = new PictureBox() { Size = new Size(90, 90), Location = new Point(380, 105), SizeMode = PictureBoxSizeMode.Zoom };
            try { picProfile.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "resources", "user_icon.png")); }
            catch { picProfile.BackColor = Color.WhiteSmoke; }
            this.Controls.Add(picProfile);

            // Label Brand & Subtitle
            Label lblBrand = new Label() { Text = "WEAR IT FASHION", Font = new Font("Times New Roman", 18, FontStyle.Bold), ForeColor = Color.FromArgb(40, 40, 40), Location = new Point(0, 210), Size = new Size(850, 30), TextAlign = ContentAlignment.MiddleCenter };
            Label lblMasuk = new Label() { Text = "A U T H E N T I C A T I O N", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(150, 150, 150), Location = new Point(0, 245), Size = new Size(850, 20), TextAlign = ContentAlignment.MiddleCenter };
            this.Controls.Add(lblBrand);
            this.Controls.Add(lblMasuk);

            // Divider
            Panel pnlDivider = new Panel() { Size = new Size(400, 1), Location = new Point(225, 280), BackColor = Color.FromArgb(240, 240, 240) };
            this.Controls.Add(pnlDivider);

            // Username Input
            Label lblUser = new Label() { Text = "Username", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(205, 318), AutoSize = true };
            Panel pnlWrapUser = new Panel() { Location = new Point(315, 310), Size = new Size(320, 38), BackColor = Color.White };
            pnlWrapUser.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlWrapUser.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            txtUser = new TextBox() { BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(50, 50, 50), Location = new Point(10, 8), Width = 300 };
            pnlWrapUser.Controls.Add(txtUser);
            this.Controls.Add(lblUser);
            this.Controls.Add(pnlWrapUser);

            // Password Input
            Label lblPass = new Label() { Text = "Password", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(205, 378), AutoSize = true };
            Panel pnlWrapPass = new Panel() { Location = new Point(315, 370), Size = new Size(320, 38), BackColor = Color.White };
            pnlWrapPass.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlWrapPass.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            txtPass = new TextBox() { BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(50, 50, 50), Location = new Point(10, 8), Width = 300, UseSystemPasswordChar = true };
            pnlWrapPass.Controls.Add(txtPass);
            this.Controls.Add(lblPass);
            this.Controls.Add(pnlWrapPass);

            // Checkbox Remember
            chkRemember = new CheckBox() { Text = "Ingat Saya", Font = new Font("Segoe UI", 10), Location = new Point(315, 430), AutoSize = true, ForeColor = Color.FromArgb(120, 120, 120), Cursor = Cursors.Hand };
            this.Controls.Add(chkRemember);

            // Tombol Login (Warna Pink + Efek Hover)
            btnLogin = new Button() { Text = "MASUK", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.HotPink, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(130, 42), Location = new Point(505, 420), Cursor = Cursors.Hand };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.DeepPink;
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.HotPink;
            
            // Menghubungkan event klik ke file logika sebelah (.cs)
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);
        }
    }
}