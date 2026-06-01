using System;
using System.Windows.Forms;
using KasirWearIt.Database;

namespace KasirWearIt
{
    public partial class FormLogin : Form
    {
        // Session kasir yang login (diisi setelah login sukses)
        public static int IdUserLogin { get; private set; }
        public static string NamaUserLogin { get; private set; } = "";
        public static string RoleUserLogin { get; private set; } = "";

        public FormLogin()
        {
            InitializeComponentCustom();
        }
        
            private void BtnLogin_Click(object sender, EventArgs e)
        {
            // GANTI: gunakan txtUser dan txtPass (sesuai dengan designer)
            string username = txtUser.Text.Trim();     // <-- GANTI dari txtUsername ke txtUser
            string password = txtPass.Text;            // <-- GANTI dari txtPassword ke txtPass
            
            // Validasi input tidak kosong
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
                // Ambil semua data user
                var userData = DatabaseConnection.AmbilUser(username);
                
                if (userData.Rows.Count > 0)
                {
                    // Simpan ke static properties (backward compatibility)
                    IdUserLogin = Convert.ToInt32(userData.Rows[0]["id_user"]);
                    NamaUserLogin = userData.Rows[0]["nama"]?.ToString() ?? "";
                    RoleUserLogin = userData.Rows[0]["role"]?.ToString() ?? "";
                    
                    // Simpan ke Session class
                    Session.IsLoggedIn = true;
                    Session.IdUser = Convert.ToInt32(userData.Rows[0]["id_user"]);
                    Session.NamaLengkap = userData.Rows[0]["nama"]?.ToString() ?? "";
                    Session.Username = userData.Rows[0]["username"]?.ToString() ?? "";
                    Session.Role = userData.Rows[0]["role"]?.ToString() ?? "";
                    Session.IdOutlet = Convert.ToInt32(userData.Rows[0]["id_outlet"]);
                    
                    // Buka form Pos
                    Pos posForm = new Pos();
                    posForm.Show();
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