using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace KasirWearIt.Database
{
    /// <summary>
    /// Koneksi MySQL untuk aplikasi KasirWearIt.
    /// Sesuaikan SERVER, DATABASE, UID, PWD dengan environment MySQL Anda.
    /// </summary>
    public static class DatabaseConnection
    {
        // ⚠️ CEK DI SINI: Sesuaikan dengan settingan XAMPP / MySQL kamu!
        private const string SERVER   = "localhost";
        private const int    PORT     = 3306; // Ubah ke 3307 jika port XAMPP kamu berubah
        private const string DATABASE = "pos_kasir";
        private const string UID      = "root";
        private const string PWD      = ""; // Isi jika database MySQL kamu menggsakan password

        private static string ConnectionString =>
            $"Server={SERVER};Port={PORT};Database={DATABASE};Uid={UID};Pwd={PWD};" +
            "CharSet=utf8mb4;AllowPublicKeyRetrieval=True;";

        /// <summary>
        /// Buka koneksi MySQL baru. Pemanggil wajib menutup (using) setelah selesai.
        /// </summary>
       public static MySqlConnection GetConnection()
{
    try
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
    catch (Exception ex)
    {
        throw new Exception($"Koneksi ke database gagal!\n\nDetail: {ex.Message}\n\n" +
                           $"Pastikan:\n" +
                           $"1. MySQL Server sedang berjalan\n" +
                           $"2. Database '{DATABASE}' sudah dibuat\n" +
                           $"3. Kredensial login (root/password) benar", ex);
    }
}

        /// <summary>
        /// Eksekusi SELECT → DataTable.
        /// </summary>
        public static DataTable Query(string sql, params MySqlParameter[] args)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand(sql, conn);
                if (args.Length > 0) cmd.Parameters.AddRange(args);
                using var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Detail Error MySQL (Query): {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return new DataTable(); // Mengembalikan data tabel kosong agar tidak memicu Object Reference di luar
            }
        }

      public static (string nama, string username) GetUserInfo(string usernameAktif)
{
    try
    {
        using var conn = GetConnection();
        // Pastikan SELECT hanya mengambil kolom yang ada
        string sql = "SELECT nama, username FROM user WHERE username = @user AND aktif = 1 LIMIT 1";
        
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@user", usernameAktif);
        
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            string dbNama = reader["nama"]?.ToString() ?? "";
            string dbUser = reader["username"]?.ToString() ?? "";
            return (dbNama, dbUser);
        }
        
        return ("", "");
    }
    catch (Exception ex)
    {
        System.Windows.Forms.MessageBox.Show(
            $"Gagal mengambil data user '{usernameAktif}':\n{ex.Message}", 
            "Error Database", 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Error);
        return ("", "");
    }
}

  public static bool UpdateDataAkun(string oldUsername, string oldPassword, string newNama, string newUsername, string newPassword)
{
    try
    {
        using var conn = GetConnection();
        
        // Cek user dan password
        string selectSql = "SELECT id_user, id_outlet, password FROM user WHERE username = @user AND aktif = 1";
        using var selectCmd = new MySqlCommand(selectSql, conn);
        selectCmd.Parameters.AddWithValue("@user", oldUsername);
        
        using var reader = selectCmd.ExecuteReader();
        if (!reader.Read())
        {
            MessageBox.Show($"User '{oldUsername}' tidak ditemukan atau tidak aktif.", 
                           "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        
        string dbPass = reader["password"]?.ToString() ?? "";
        if (dbPass != oldPassword)
        {
            MessageBox.Show("Password lama yang Anda masukkan salah.", 
                           "Verifikasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        
        int idUser = Convert.ToInt32(reader["id_user"]);
        reader.Close();
        
        // Update data
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            string updateSql = @"UPDATE user 
                                SET nama = @nama, 
                                    username = @newuser, 
                                    password = @newpass 
                                WHERE id_user = @id";
            using var updateCmd = new MySqlCommand(updateSql, conn);
            updateCmd.Parameters.AddWithValue("@nama", newNama);
            updateCmd.Parameters.AddWithValue("@newuser", newUsername);
            updateCmd.Parameters.AddWithValue("@newpass", newPassword);
            updateCmd.Parameters.AddWithValue("@id", idUser);
            updateCmd.ExecuteNonQuery();
        }
        else
        {
            string updateSql = @"UPDATE user 
                                SET nama = @nama, 
                                    username = @newuser 
                                WHERE id_user = @id";
            using var updateCmd = new MySqlCommand(updateSql, conn);
            updateCmd.Parameters.AddWithValue("@nama", newNama);
            updateCmd.Parameters.AddWithValue("@newuser", newUsername);
            updateCmd.Parameters.AddWithValue("@id", idUser);
            updateCmd.ExecuteNonQuery();
        }
        
        return true;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error update data akun:\n{ex.Message}", 
                       "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }
}
        /// <summary>
        /// Eksekusi INSERT / UPDATE / DELETE, return affected rows.
        /// </summary>
        public static int Exec(string sql, params MySqlParameter[] args)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand(sql, conn);
                if (args.Length > 0) cmd.Parameters.AddRange(args);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Detail Error MySQL (Exec): {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// Eksekusi query scalar, return object (null-able).
        /// </summary>
        public static object? Scalar(string sql, params MySqlParameter[] args)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand(sql, conn);
                if (args.Length > 0) cmd.Parameters.AddRange(args);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Detail Error MySQL (Scalar): {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Validasi login user.
        /// </summary>
     public static bool CekLogin(string username, string password)
{
    try
    {
        using var conn = GetConnection();
        string sql = "SELECT id_user FROM user WHERE username = @user AND password = @pass AND aktif = 1 LIMIT 1";
        
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@user", username);
        cmd.Parameters.AddWithValue("@pass", password);
        
        using var reader = cmd.ExecuteReader();
        return reader.HasRows;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error login: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }
}
        /// <summary>
        /// Ambil info user berdasarkan username.
        /// </summary>
      public static DataTable AmbilUser(string username)
{
    try
    {
        using var conn = GetConnection();
        string sql = @"SELECT id_user, id_outlet, nama, username, role 
                      FROM user 
                      WHERE username = @u AND aktif = 1";
        
        using var da = new MySqlDataAdapter(sql, conn);
        da.SelectCommand.Parameters.AddWithValue("@u", username);
        
        var dt = new DataTable();
        da.Fill(dt);
        return dt;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error ambil user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return new DataTable();
    }
}

        /// <summary>
        /// Ambil semua produk tanpa status aktif untuk halaman POS.
        /// </summary>
        public static DataTable AmbilProdukAktif()
        {
            // PENTING: Disesuaikan dengan membuang 'WHERE aktif = 1' karena di tabel produk tidak ada kolom tersebut.
            return Query(
                "SELECT kode_produk, nama_produk, harga_jual, stok FROM produk ORDER BY nama_produk");
        }

        /// <summary>
        /// Panggil stored procedure sp_search_produk(keyword).
        /// </summary>
       public static DataTable SearchProduk(string keyword)
        {
            // Menghapus pencarian berdasarkan kolom 'kategori' dan 'aktif' 
            // agar menyesuaikan dengan struktur tabel produk di databasemu.
            string sql = @"
                SELECT kode_produk, nama_produk, harga_jual, stok 
                FROM produk 
                WHERE stok > 0 
                  AND (nama_produk LIKE @kw OR kode_produk LIKE @kw) 
                ORDER BY nama_produk";
                
            return Query(sql, new MySqlParameter("@kw", "%" + keyword + "%"));
        }
        // Sisanya (SimpanTransaksi, LaporanHarian, RingkasanHarian) tetap aman...
      public static int SimpanTransaksi(int idUser, decimal subtotal, decimal diskonPct, decimal totalAkhir, string metodeBayar, decimal uangBayar, decimal kembalian, (string kode, decimal harga, int qty, decimal sub)[] items)
{
    using var conn = new MySqlConnection(ConnectionString);
    conn.Open();
    using var trx = conn.BeginTransaction();
    try
    {
        const string sqlHdr = @"
            INSERT INTO transaksi_jual (id_user, subtotal, diskon_pct, total_akhir, metode_bayar, uang_bayar, kembalian)
            VALUES (@uid, @sub, @dpct, @tot, @met, @bay, @kmb);
            SELECT LAST_INSERT_ID();";
        using var cmdHdr = new MySqlCommand(sqlHdr, conn, trx);
        cmdHdr.Parameters.AddWithValue("@uid", idUser);
        cmdHdr.Parameters.AddWithValue("@sub", subtotal);
        cmdHdr.Parameters.AddWithValue("@dpct", diskonPct);
        cmdHdr.Parameters.AddWithValue("@tot", totalAkhir);
        cmdHdr.Parameters.AddWithValue("@met", metodeBayar);
        cmdHdr.Parameters.AddWithValue("@bay", uangBayar);
        cmdHdr.Parameters.AddWithValue("@kmb", kembalian);
        int idTrans = Convert.ToInt32(cmdHdr.ExecuteScalar());

        // Detail tanpa harga_satuan (karena tabel tidak punya kolom itu)
        const string sqlDtl = "INSERT INTO detail_jual (id_transaksi, kode_produk, jumlah, subtotal) VALUES (@idt, @kp, @jml, @sub)";
        const string sqlStok = "UPDATE produk SET stok = stok - @j WHERE kode_produk = @kp";

        foreach (var item in items)
        {
            using var cmdDtl = new MySqlCommand(sqlDtl, conn, trx);
            cmdDtl.Parameters.AddWithValue("@idt", idTrans);
            cmdDtl.Parameters.AddWithValue("@kp",  item.kode);
            cmdDtl.Parameters.AddWithValue("@jml", item.qty);
            cmdDtl.Parameters.AddWithValue("@sub", item.sub);
            cmdDtl.ExecuteNonQuery();

            using var cmdStok = new MySqlCommand(sqlStok, conn, trx);
            cmdStok.Parameters.AddWithValue("@j",  item.qty);
            cmdStok.Parameters.AddWithValue("@kp", item.kode);
            cmdStok.ExecuteNonQuery();
        }
        trx.Commit();
        return idTrans;
    }
    catch
    {
        trx.Rollback();
        throw;
    }
}

        public static DataTable LaporanHarian(DateTime dari, DateTime sampai)
        {
            return Query(@"
                SELECT DATE(t.tanggal) AS Tanggal, TIME(t.tanggal) AS Jam, p.nama_produk AS Produk, d.jumlah AS Qty, d.subtotal AS Subtotal, t.diskon_pct AS DiskonPct, t.total_akhir AS TotalAkhir, t.metode_bayar AS Metode, u.nama AS Kasir
                FROM transaksi_jual t
                JOIN detail_jual d  ON d.id_transaksi = t.id_transaksi
                JOIN produk p       ON p.kode_produk  = d.kode_produk
                JOIN `user` u       ON u.id_user      = t.id_user
                WHERE t.tanggal BETWEEN @dari AND @sampai ORDER BY t.tanggal DESC",
                new MySqlParameter("@dari", dari), new MySqlParameter("@sampai", sampai));
        }

        public static (decimal totalJual, decimal totalDiskon, decimal totalBersih) RingkasanHarian(DateTime dari, DateTime sampai)
        {
            const string sql = "SELECT COALESCE(SUM(subtotal), 0) AS total_jual, COALESCE(SUM(diskon_pct), 0) AS total_diskon, COALESCE(SUM(total_akhir), 0) AS total_bersih FROM transaksi_jual WHERE tanggal BETWEEN @dari AND @sampai";
            var dt = Query(sql, new MySqlParameter("@dari", dari), new MySqlParameter("@sampai", sampai));
            if (dt == null || dt.Rows.Count == 0) return (0, 0, 0);
            var r = dt.Rows[0];
            return (Convert.ToDecimal(r["total_jual"]), Convert.ToDecimal(r["total_diskon"]), Convert.ToDecimal(r["total_bersih"]));
        }
    }
}