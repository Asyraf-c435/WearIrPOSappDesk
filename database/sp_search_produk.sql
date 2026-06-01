-- ===========================================================
-- Stored procedure: sp_search_produk
-- Dipanggil dari C#: DatabaseConnection.SearchProduk(keyword)
-- Param input  : p_keyword VARCHAR  (kosong => tampilkan semua aktif)
-- Kolom output HARUS sama dengan yang dibaca Pos.cs:
--   kode_produk, nama_produk, harga_jual, stok, kategori
-- ===========================================================
DELIMITER $$

DROP PROCEDURE IF EXISTS sp_search_produk$$
CREATE PROCEDURE sp_search_produk(IN p_keyword VARCHAR(100))
BEGIN
    SET p_keyword = IFNULL(TRIM(p_keyword), '');

    SELECT
        kode_produk,
        nama_produk,
        harga_jual,
        stok,
        kategori
    FROM produk
    WHERE aktif = 1
      AND stok > 0
      AND (
            p_keyword = ''
         OR nama_produk LIKE CONCAT('%', p_keyword, '%')
         OR kode_produk LIKE CONCAT('%', p_keyword, '%')
         OR kategori  LIKE CONCAT('%', p_keyword, '%')
      )
    ORDER BY nama_produk;
END$$

DELIMITER ;
