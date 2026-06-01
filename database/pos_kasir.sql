-- ================================================================
-- KASIR WEAR IT — DATABASE SCHEMA
-- MySQL / MariaDB
-- Run di phpMyAdmin / MySQL Workbench / CLI mysql
-- ================================================================

CREATE DATABASE IF NOT EXISTS pos_kasir;
USE pos_kasir;

-- ================================================================
-- TABLE: outlet  (opsional — multi-cabang support)
-- ================================================================
CREATE TABLE IF NOT EXISTS outlet (
    id_outlet  INT AUTO_INCREMENT PRIMARY KEY,
    nama_outlet VARCHAR(100) NOT NULL,
    alamat     TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ================================================================
-- TABLE: user  (login + role kasir)
-- ================================================================
CREATE TABLE IF NOT EXISTS `user` (
    id_user    INT AUTO_INCREMENT PRIMARY KEY,
    id_outlet  INT NOT NULL,
    nama       VARCHAR(100) NOT NULL,
    username   VARCHAR(50) NOT NULL UNIQUE,
    `password` VARCHAR(255) NOT NULL,   -- BCrypt / hashed
    role       VARCHAR(30) NOT NULL DEFAULT 'kasir',
    aktif      TINYINT(1) NOT NULL DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (id_outlet) REFERENCES outlet(id_outlet) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ================================================================
-- Default outlet (cabang) harus dibuat duluan sebelum user
-- ================================================================
INSERT INTO outlet (nama_outlet, alamat)
VALUES ('Toko Utama', 'Jl. Sudirman No. 1')
ON DUPLICATE KEY UPDATE nama_outlet = VALUES(nama_outlet);

-- Default admin login (password: admin) -> merujuk ke outlet 1
INSERT INTO `user` (id_outlet, nama, username, `password`, role)
VALUES (1, 'Tasya', 'admin', 'admin', 'admin')
ON DUPLICATE KEY UPDATE nama = VALUES(nama);

-- ================================================================
-- TABLE: produk  (katalog barang)
-- ================================================================
CREATE TABLE IF NOT EXISTS produk (
    kode_produk VARCHAR(20) PRIMARY KEY,
    nama_produk VARCHAR(100) NOT NULL,
    harga_beli  DECIMAL(12,0) NOT NULL DEFAULT 0,
    harga_jual  DECIMAL(12,0) NOT NULL,
    stok        INT NOT NULL DEFAULT 0,
    created_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Sample produk awal
INSERT INTO produk (kode_produk, nama_produk, harga_beli, harga_jual, stok) VALUES
('B01', 'Cardigan Pink',        120000, 180000, 25),
('B02', 'Baju Khimar',         95000,  145000, 30),
('B03', 'Rok Plisket',         80000,  125000, 20),
('B04', 'Gamis Modern',        150000, 220000, 15),
('B05', 'Celana Almamater',    75000,  110000, 40),
('B06', 'Kaos Polos Cotton',   45000,  75000,  50),
('B07', 'Blouse Satin',        85000,  135000, 18),
('B08', 'Kemeja Flanel',       90000,  140000, 22),
('B09', 'Hoodie Zipper',       130000, 195000, 12),
('B10', 'Jaket Denim',         160000, 240000, 10),
('B11', 'Daster Rumahan',      35000,  60000,  35),
('B12', 'Tunik Batik',         110000, 170000, 16),
('B13', 'Celana Kulot',        95000,  145000, 28),
('B14', 'Outer Rajut',         105000, 165000, 14),
('B15', 'Kaos Oblong Pria',    40000,  65000,  45)
ON DUPLICATE KEY UPDATE nama_produk = VALUES(nama_produk);

-- ================================================================
-- TABLE: transaksi_jual  (header transaksi)
-- ================================================================
CREATE TABLE IF NOT EXISTS transaksi_jual (
    id_transaksi INT AUTO_INCREMENT PRIMARY KEY,
    id_user      INT NOT NULL,
    tanggal      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    subtotal     DECIMAL(12,0) NOT NULL DEFAULT 0,
    diskon_pct   DECIMAL(5,2) NOT NULL DEFAULT 0,
    total_akhir  DECIMAL(12,0) NOT NULL DEFAULT 0,
    metode_bayar ENUM('CASH','QRIS') NOT NULL DEFAULT 'CASH',
    uang_bayar   DECIMAL(12,0) NOT NULL DEFAULT 0,
    kembalian    DECIMAL(12,0) NOT NULL DEFAULT 0,

    FOREIGN KEY (id_user) REFERENCES `user`(id_user)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ================================================================
-- TABLE: detail_jual  (baris item per transaksi)
-- ================================================================
CREATE TABLE IF NOT EXISTS detail_jual (
    id_detail    INT AUTO_INCREMENT PRIMARY KEY,
    id_transaksi INT NOT NULL,
    kode_produk  VARCHAR(20) NOT NULL,
    harga_satuan DECIMAL(12,0) NOT NULL,
    jumlah       INT NOT NULL,
    subtotal     DECIMAL(12,0) NOT NULL,

    FOREIGN KEY (id_transaksi) REFERENCES transaksi_jual(id_transaksi) ON DELETE CASCADE,
    FOREIGN KEY (kode_produk)  REFERENCES produk(kode_produk)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ================================================================
-- TABLE: transaksi_beli  (pembelian stok dari supplier)
-- ================================================================
CREATE TABLE IF NOT EXISTS transaksi_beli (
    id_beli       INT AUTO_INCREMENT PRIMARY KEY,
    id_user       INT NOT NULL,
    tanggal       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    nama_supplier VARCHAR(100),
    total_beli    DECIMAL(12,0) NOT NULL DEFAULT 0,

    FOREIGN KEY (id_user) REFERENCES `user`(id_user)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ================================================================
-- TABLE: detail_beli
-- ================================================================
CREATE TABLE IF NOT EXISTS detail_beli (
    id_detail_beli INT AUTO_INCREMENT PRIMARY KEY,
    id_beli        INT NOT NULL,
    kode_produk    VARCHAR(20) NOT NULL,
    jumlah         INT NOT NULL,
    harga_satuan   DECIMAL(12,0) NOT NULL,

    FOREIGN KEY (id_beli)     REFERENCES transaksi_beli(id_beli) ON DELETE CASCADE,
    FOREIGN KEY (kode_produk) REFERENCES produk(kode_produk)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ================================================================
-- VIEW: ringkasan harian (untuk Laporan)
-- ================================================================
CREATE OR REPLACE VIEW v_laporan_harian AS
SELECT
    DATE(t.tanggal)                              AS tanggal,
    TIME(t.tanggal)                              AS jam,
    p.nama_produk,
    d.jumlah,
    d.subtotal,
    t.diskon_pct,
    t.total_akhir,
    t.metode_bayar,
    u.nama                                       AS kasir
FROM transaksi_jual t
JOIN detail_jual d  ON d.id_transaksi = t.id_transaksi
JOIN produk p       ON p.kode_produk = d.kode_produk
JOIN `user` u       ON u.id_user = t.id_user
ORDER BY t.tanggal DESC;