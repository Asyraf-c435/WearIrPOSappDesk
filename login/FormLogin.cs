using System;
using System.Windows.Forms;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponentCustom();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUser.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPass.Focus();
                return;
            }

            if (DatabaseConnection.CekLogin(username, password))
            {
                var userData = DatabaseConnection.AmbilUser(username);

                if (userData.Rows.Count > 0)
                {
                    // Simpan data ke session
                    Session.IsLoggedIn = true;
                    Session.IdUser = Convert.ToInt32(userData.Rows[0]["id_user"]);
                    Session.NamaLengkap = userData.Rows[0]["nama"]?.ToString() ?? "";
                    Session.Username = userData.Rows[0]["username"]?.ToString() ?? "";
                    Session.Role = userData.Rows[0]["role"]?.ToString() ?? "";
                    Session.IdOutlet = Convert.ToInt32(userData.Rows[0]["id_outlet"]);

                    // LOGIKA PERCABANGAN ROLE (owner atau kasir)
                    if (Session.Role.ToLower() == "owner")
                    {
                        OwnerDashboard ownerForm = new OwnerDashboard();
                        ownerForm.Show();
                    }
                    else
                    {
                        Pos posForm = new Pos();
                        posForm.Show();
                    }

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Data user tidak lengkap!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Username atau password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPass.Clear();
                txtPass.Focus();
            }
        }
    }
}