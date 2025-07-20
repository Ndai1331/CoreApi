-- Create Table-Valued Function for TT dashboard counts
CREATE OR ALTER FUNCTION TTFunctionDashboardCoSoBiDichBenh()
RETURNS TABLE
AS
RETURN
(
    SELECT
        vsvgh.name AS [LoaiHinhThienTai],
        SUM(csdbct.dien_tich) AS [TongDienTich],
        CAST(0 AS FLOAT) AS [TongSanLuong]
    FROM CoSoBiDichBenhChiTiet csdbct
    INNER JOIN co_so_bi_dich_benh csdb ON csdb.id = csdbct.co_so_bi_dich_benh
    INNER JOIN ViSinhVatGayHai vsvgh ON vsvgh.id = csdbct.vi_sinh_vat_gay_hai
    WHERE csdbct.deleted = 0 AND csdb.deleted = 0
    GROUP BY vsvgh.name
)