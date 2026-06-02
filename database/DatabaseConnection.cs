using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace KasirWearIt.Database
{
    public static class DatabaseConnection
    {
        private const string SERVER = "localhost";
        private const int PORT = 3306;
        private const string DATABASE = "pos_kasir";
        private const string UID = "root";
        private const string PWD = "";

        private static string ConnectionString =>
            $"Server={SERVER};Port={PORT};Database={DATABASE};Uid={UID};Pwd={PWD};" +
            "CharSet=utf8mb4;AllowPublicKeyRetrieval=True;";

        // GetConnection sudah membuka koneksi
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
                                   $"Pastikan:\n1. MySQL Server sedang berjalan\n" +
                                   $"2. Database '{DATABASE}' sudah dibuat\n" +
                                   $"3. Kredensial login (root/password) benar", ex);
            }
        }

        // Query SELECT -> DataTable
        public static DataTable Query(string sql, params MySqlParameter[] args)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new MySqlCommand(sql, conn);
                if (args.Length > 0) cmd.Parameters.AddRange(args);
                using var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error Query: {ex.Message}", "Database Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return new DataTable();
            }
        }

        // Exec INSERT/UPDATE/DELETE -> affected rows
        public static int Exec(string sql, params MySqlParameter[] args)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new MySqlCommand(sql, conn);
                if (args.Length > 0) cmd.Parameters.AddRange(args);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error Exec: {ex.Message}", "Database Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }

        // Scalar -> single value
        public static object Scalar(string sql, params MySqlParameter[] args)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new MySqlCommand(sql, conn);
                if (args.Length > 0) cmd.Parameters.AddRange(args);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error Scalar: {ex.Message}", "Database Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }

        // Login check
        public static bool CekLogin(string username, string password)
        {
            try
            {
                string sql = "SELECT id_user FROM user WHERE username = @user AND password = @pass AND aktif = 1 LIMIT 1";
                var result = Scalar(sql, new MySqlParameter("@user", username), new MySqlParameter("@pass", password));
                return result != null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error login: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        // Ambil info user
        public static DataTable AmbilUser(string username)
        {
            string sql = @"SELECT id_user, id_outlet, nama, username, role FROM user WHERE username = @u AND aktif = 1";
            return Query(sql, new MySqlParameter("@u", username));
        }

        // Get user info (nama, username)
       public static (string nama, string username) GetUserInfo(string username)
{
    var dt = AmbilUser(username);
    if (dt.Rows.Count > 0)
    {
        string nama = dt.Rows[0]["nama"]?.ToString() ?? "";
        string user = dt.Rows[0]["username"]?.ToString() ?? "";
        return (nama, user);
    }
    return ("", "");
}

        // Update akun
        public static bool UpdateDataAkun(string oldUsername, string oldPassword, string newNama, string newUsername, string newPassword)
        {
            try
            {
                // Cek user lama
                string checkSql = "SELECT id_user, password FROM user WHERE username = @user AND aktif = 1";
                var dt = Query(checkSql, new MySqlParameter("@user", oldUsername));
                if (dt.Rows.Count == 0) return false;
                string dbPass = dt.Rows[0]["password"].ToString();
                if (dbPass != oldPassword) return false;
                int idUser = Convert.ToInt32(dt.Rows[0]["id_user"]);

                string updateSql;
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    updateSql = "UPDATE user SET nama = @nama, username = @newuser, password = @newpass WHERE id_user = @id";
                    return Exec(updateSql,
                        new MySqlParameter("@nama", newNama),
                        new MySqlParameter("@newuser", newUsername),
                        new MySqlParameter("@newpass", newPassword),
                        new MySqlParameter("@id", idUser)) > 0;
                }
                else
                {
                    updateSql = "UPDATE user SET nama = @nama, username = @newuser WHERE id_user = @id";
                    return Exec(updateSql,
                        new MySqlParameter("@nama", newNama),
                        new MySqlParameter("@newuser", newUsername),
                        new MySqlParameter("@id", idUser)) > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // Produk aktif
        public static DataTable AmbilProdukAktif()
        {
            return Query("SELECT kode_produk, nama_produk, harga_jual, stok FROM produk ORDER BY nama_produk");
        }

        public static DataTable SearchProduk(string keyword)
        {
            string sql = @"SELECT kode_produk, nama_produk, harga_jual, stok FROM produk 
                           WHERE (nama_produk LIKE @kw OR kode_produk LIKE @kw) ORDER BY nama_produk";
            return Query(sql, new MySqlParameter("@kw", "%" + keyword + "%"));
        }

        // Update stok gudang
        public static bool UpdateStockGudang(int idOutlet, string kodeProduk, int qtyBaru)
        {
            string sql = @"INSERT INTO stok (id_outlet, kode_produk, qty_sistem, qty_gudang)
                           VALUES (@outlet, @kode, 0, @qty)
                           ON DUPLICATE KEY UPDATE qty_gudang = @qty";
            return Exec(sql,
                new MySqlParameter("@outlet", idOutlet),
                new MySqlParameter("@kode", kodeProduk),
                new MySqlParameter("@qty", qtyBaru)) > 0;
        }

        // Simpan transaksi (perbaiki juga)
        public static int SimpanTransaksi(int idUser, decimal subtotal, decimal diskonPct, decimal totalAkhir, string metodeBayar, decimal uangBayar, decimal kembalian, (string kode, decimal harga, int qty, decimal sub)[] items)
        {
            using var conn = GetConnection();
            using var trx = conn.BeginTransaction();
            try
            {
                // Ambil id_outlet user
                int idOutlet = 0;
                string queryOutlet = "SELECT id_outlet FROM user WHERE id_user = @idUser";
                using (var cmdOutlet = new MySqlCommand(queryOutlet, conn, trx))
                {
                    cmdOutlet.Parameters.AddWithValue("@idUser", idUser);
                    var result = cmdOutlet.ExecuteScalar();
                    if (result != null) idOutlet = Convert.ToInt32(result);
                    else throw new Exception("User tidak memiliki outlet.");
                }

                // Insert header
                const string sqlHdr = @"INSERT INTO transaksi_jual (id_user, subtotal, diskon_pct, total_akhir, metode_bayar, uang_bayar, kembalian)
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

                // Insert detail & update stok
                const string sqlDtl = "INSERT INTO detail_jual (id_transaksi, kode_produk, jumlah, subtotal) VALUES (@idt, @kp, @jml, @sub)";
                const string sqlUpdateStok = "UPDATE stok SET qty_sistem = qty_sistem - @jml WHERE id_outlet = @outlet AND kode_produk = @kp";
                foreach (var item in items)
                {
                    using var cmdDtl = new MySqlCommand(sqlDtl, conn, trx);
                    cmdDtl.Parameters.AddWithValue("@idt", idTrans);
                    cmdDtl.Parameters.AddWithValue("@kp", item.kode);
                    cmdDtl.Parameters.AddWithValue("@jml", item.qty);
                    cmdDtl.Parameters.AddWithValue("@sub", item.sub);
                    cmdDtl.ExecuteNonQuery();

                    using var cmdStok = new MySqlCommand(sqlUpdateStok, conn, trx);
                    cmdStok.Parameters.AddWithValue("@jml", item.qty);
                    cmdStok.Parameters.AddWithValue("@kp", item.kode);
                    cmdStok.Parameters.AddWithValue("@outlet", idOutlet);
                    int rows = cmdStok.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        string sqlInsertStok = "INSERT INTO stok (id_outlet, kode_produk, qty_sistem, qty_gudang) VALUES (@outlet, @kp, 0, 0)";
                        using var cmdInsert = new MySqlCommand(sqlInsertStok, conn, trx);
                        cmdInsert.Parameters.AddWithValue("@outlet", idOutlet);
                        cmdInsert.Parameters.AddWithValue("@kp", item.kode);
                        cmdInsert.ExecuteNonQuery();
                        cmdStok.ExecuteNonQuery();
                    }
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

        // Laporan penjualan lengkap
        public static DataTable GetLaporanPenjualanLengkap(DateTime dari, DateTime sampai)
        {
            DateTime sampaiFull = sampai.Date.AddDays(1);
            string sql = @"
                SELECT 
                    DATE(t.tanggal) AS Tanggal,
                    TIME(t.tanggal) AS Jam,
                    p.nama_produk AS Produk,
                    dt.jumlah AS Qty,
                    p.harga_jual AS HargaJual,
                    dt.subtotal AS Subtotal,
                    t.diskon_pct AS DiskonPersen,
                    COALESCE(p.harga_beli, 0) AS HargaBeli,
                    (dt.subtotal - (dt.jumlah * COALESCE(p.harga_beli, 0))) AS Laba
                FROM transaksi_jual t
                JOIN detail_jual dt ON t.id_transaksi = dt.id_transaksi
                JOIN produk p ON dt.kode_produk = p.kode_produk
                WHERE t.tanggal >= @dari AND t.tanggal < @sampai
                ORDER BY t.tanggal, p.nama_produk";
            return Query(sql, new MySqlParameter("@dari", dari), new MySqlParameter("@sampai", sampaiFull));
        }

        public static (decimal grossSales, decimal totalDiskon, decimal netSales, decimal totalHpp, decimal labaKotor, int totalTransaksi) GetRingkasanLaporan(DateTime dari, DateTime sampai)
        {
            DateTime sampaiFull = sampai.Date.AddDays(1);
            string sql = @"
                SELECT 
                    COALESCE(SUM(dt.subtotal), 0) AS GrossSales,
                    COALESCE(SUM(t.total_akhir), 0) AS NetSales,
                    COALESCE(SUM(dt.jumlah * p.harga_beli), 0) AS TotalHpp,
                    COALESCE(SUM(dt.subtotal - (dt.jumlah * p.harga_beli)), 0) AS LabaKotor,
                    COUNT(DISTINCT t.id_transaksi) AS TotalTransaksi
                FROM transaksi_jual t
                JOIN detail_jual dt ON t.id_transaksi = dt.id_transaksi
                JOIN produk p ON dt.kode_produk = p.kode_produk
                WHERE t.tanggal >= @dari AND t.tanggal < @sampai";
            var dt = Query(sql, new MySqlParameter("@dari", dari), new MySqlParameter("@sampai", sampaiFull));
            if (dt.Rows.Count == 0) return (0, 0, 0, 0, 0, 0);
            var row = dt.Rows[0];
            decimal gross = Convert.ToDecimal(row["GrossSales"]);
            decimal net = Convert.ToDecimal(row["NetSales"]);
            decimal diskon = gross - net;
            return (gross, diskon, net, Convert.ToDecimal(row["TotalHpp"]), Convert.ToDecimal(row["LabaKotor"]), Convert.ToInt32(row["TotalTransaksi"]));
        }

        
// ===================== METHOD YANG HILANG (DIPANGGIL OLEH FORM LAIN) =====================

/// <summary>
/// Mengambil ringkasan harian (Gross Sales, Net Sales, Total Pax / transaksi) untuk hari ini.
/// Dipanggil oleh OwnerDashboard.
/// </summary>
public static (decimal grossSales, decimal netSales, int totalPax) GetSummaryToday()
{
    DateTime today = DateTime.Today;
    DateTime tomorrow = today.AddDays(1);
    var dt = GetDailySales(today, tomorrow);
    if (dt.Rows.Count == 0)
        return (0, 0, 0);
    var row = dt.Rows[0];
    decimal gross = Convert.ToDecimal(row["TotalJual"]);
    decimal net = Convert.ToDecimal(row["TotalBersih"]);
    int pax = Convert.ToInt32(row["JumlahTransaksi"]);
    return (gross, net, pax);
}

/// <summary>
/// Mengambil data penjualan per hari dalam rentang tanggal.
/// Kolom: Tanggal, TotalJual, TotalBersih, JumlahTransaksi
/// </summary>
public static DataTable GetDailySales(DateTime dari, DateTime sampai)
{
    DateTime sampaiFull = sampai.Date.AddDays(1);
    string sql = @"
        SELECT 
            DATE(tanggal) AS Tanggal,
            COALESCE(SUM(subtotal), 0) AS TotalJual,
            COALESCE(SUM(total_akhir), 0) AS TotalBersih,
            COUNT(*) AS JumlahTransaksi
        FROM transaksi_jual
        WHERE tanggal >= @dari AND tanggal < @sampai
        GROUP BY DATE(tanggal)
        ORDER BY Tanggal";
    return Query(sql,
        new MySqlParameter("@dari", dari),
        new MySqlParameter("@sampai", sampaiFull));
}

/// <summary>
/// Mencari tanggal dengan pendapatan tertinggi dan terendah dalam 30 hari terakhir.
/// Dipanggil oleh OwnerDashboard.
/// </summary>
public static (DateTime peakDate, decimal peakIncome, DateTime lowDate, decimal lowIncome) GetPeakLowDates(int daysBack = 30)
{
    DateTime start = DateTime.Today.AddDays(-daysBack);
    DateTime end = DateTime.Today.AddDays(1);
    var dt = GetDailySales(start, end);
    if (dt.Rows.Count == 0)
        return (DateTime.Today, 0, DateTime.Today, 0);
    DataRow peakRow = dt.Rows[0];
    DataRow lowRow = dt.Rows[0];
    foreach (DataRow row in dt.Rows)
    {
        decimal income = Convert.ToDecimal(row["TotalBersih"]);
        if (income > Convert.ToDecimal(peakRow["TotalBersih"]))
            peakRow = row;
        if (income < Convert.ToDecimal(lowRow["TotalBersih"]))
            lowRow = row;
    }
    return (
        Convert.ToDateTime(peakRow["Tanggal"]),
        Convert.ToDecimal(peakRow["TotalBersih"]),
        Convert.ToDateTime(lowRow["Tanggal"]),
        Convert.ToDecimal(lowRow["TotalBersih"])
    );
}

/// <summary>
/// Versi lama dari laporan penjualan (tanpa HPP/Laba). Dipanggil oleh Laporan.cs.
/// </summary>
public static DataTable GetLaporanPenjualan(DateTime dari, DateTime sampai)
{
    // Gunakan method GetLaporanPenjualanLengkap dan buang kolom HPP/Laba jika perlu
    DataTable dtLengkap = GetLaporanPenjualanLengkap(dari, sampai);
    // Buat tabel sederhana sesuai dengan ekspektasi Laporan.cs
    DataTable result = new DataTable();
    result.Columns.Add("Tanggal", typeof(DateTime));
    result.Columns.Add("Jam", typeof(TimeSpan));
    result.Columns.Add("Produk", typeof(string));
    result.Columns.Add("Qty", typeof(int));
    result.Columns.Add("Subtotal", typeof(decimal));
    result.Columns.Add("DiskonPct", typeof(decimal));
    // (Opsional: tambah kolom lain jika diperlukan)
    foreach (DataRow row in dtLengkap.Rows)
    {
        result.Rows.Add(
            row["Tanggal"],
            row["Jam"],
            row["Produk"],
            row["Qty"],
            row["Subtotal"],
            row["DiskonPersen"]
        );
    }
    return result;
}

/// <summary>
/// Update stok gudang DAN stok sistem sekaligus. Dipanggil oleh FormPenyesuaianStokPopup.
/// </summary>
public static bool UpdateStockBoth(int idOutlet, string kodeProduk, int qtySistemBaru, int qtyGudangBaru)
{
    string sql = @"
        INSERT INTO stok (id_outlet, kode_produk, qty_sistem, qty_gudang)
        VALUES (@outlet, @kode, @qtySistem, @qtyGudang)
        ON DUPLICATE KEY UPDATE 
            qty_sistem = @qtySistem,
            qty_gudang = @qtyGudang";
    MySqlParameter[] parameters = {
        new MySqlParameter("@outlet", idOutlet),
        new MySqlParameter("@kode", kodeProduk),
        new MySqlParameter("@qtySistem", qtySistemBaru),
        new MySqlParameter("@qtyGudang", qtyGudangBaru)
    };
    return Exec(sql, parameters) > 0;
}
    }
}