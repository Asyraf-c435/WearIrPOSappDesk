using System;
using System.Windows.Forms;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class Akun : Form
    {
        private string _usernameAktif;

     public Akun()
        {
            // Cek session
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Session login tidak ditemukan! Silakan login kembali.", 
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            
            InitializeComponentCustom();
            MuatDataSaatIni();
        }

        private void MuatDataSaatIni()
        {
            try
            {
                // Ambil dari Session yang sudah tersimpan
                string namaLengkap = Session.NamaLengkap;
                string username = Session.Username;
                
                if (string.IsNullOrEmpty(namaLengkap) && string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("Data user tidak ditemukan di session!", 
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 1. ISI BAGIAN INFORMASI AKUN (READ-ONLY)
                txtCurrentNama.Text = string.IsNullOrEmpty(namaLengkap) ? "Nama Tidak Ditemukan" : namaLengkap;
                txtCurrentUsername.Text = string.IsNullOrEmpty(username) ? "Username Tidak Ditemukan" : username;
                txtCurrentPassword.Text = "••••••••";

                // 2. ISI BAGIAN INPUT MODIFIKASI DATA (EDITABLE)
                txtNewNama.Text = namaLengkap;
                txtNewUsername.Text = username;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BtnSimpan_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassLama.Text))
            {
                MessageBox.Show("Silakan masukkan Password Lama Anda untuk memverifikasi perubahan!", 
                               "Validasi Diperlukan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassLama.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewNama.Text) || string.IsNullOrWhiteSpace(txtNewUsername.Text))
            {
                MessageBox.Show("Nama Lengkap dan Username tidak boleh kosong!", 
                               "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool isSuccess = DatabaseConnection.UpdateDataAkun(
                    Session.Username,      // old username dari session
                    txtPassLama.Text.Trim(), 
                    txtNewNama.Text.Trim(), 
                    txtNewUsername.Text.Trim(), 
                    txtNewPassword.Text.Trim()
                );

                if (!isSuccess)
                {
                    MessageBox.Show("Gagal menyimpan! Password Lama Anda salah atau data user tidak ditemukan.", 
                                   "Otorisasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassLama.Clear();
                    txtPassLama.Focus();
                    return;
                }

                // Update session dengan data baru
                Session.NamaLengkap = txtNewNama.Text.Trim();
                Session.Username = txtNewUsername.Text.Trim();
                
                // HAPUS ATAU COMMENT BARIS INI (karena setter-nya private)
                // FormLogin.NamaUserLogin = txtNewNama.Text.Trim();
                // Ganti dengan ini jika perlu update static property di FormLogin
                // Tapi lebih baik tidak usah, karena kita pakai Session

                MessageBox.Show("Data akun berhasil diperbarui!\n\nSilakan Login kembali menggunakan kredensial baru Anda.", 
                               "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.Close();
                Application.Restart(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data.\n\nDetail: " + ex.Message, 
                               "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}