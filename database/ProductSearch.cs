using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace KasirWearIt.Database
{

    /// <summary>
    /// Wrapper untuk stored procedure sp_search_produk di MySQL.
    /// Usage: DataTable hasil = ProductSearch.Cari(keyword);
    /// </summary>
    public static class ProductSearch
    {
        public static DataTable Cari(string keyword = "")
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // PANGGIL STORED PROCEDURE sp_search_produk
                    using (MySqlCommand cmd = new MySqlCommand("sp_search_produk", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parameter input (sesuaikan nama parameter dengan definisi di MySQL)
                        cmd.Parameters.AddWithValue("p_keyword", string.IsNullOrEmpty(keyword) ? "" : keyword);

                        // Opsional: Parameter output jika ada
                        // cmd.Parameters.Add("p_total_found", MySqlDbType.Int32).Direction = ParameterDirection.Output;

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log atau tampilkan error
                System.Windows.Forms.MessageBox.Show($"Error pencarian produk: {ex.Message}", "Database Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return dt;
        }
    }
}
