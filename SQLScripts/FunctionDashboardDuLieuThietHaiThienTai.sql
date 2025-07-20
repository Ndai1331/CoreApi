-- Create Table-Valued Function for TT dashboard counts
CREATE OR ALTER FUNCTION TTFunctionDashboardDuLieuThietHaiThienTai()
RETURNS TABLE
AS
RETURN
(
    SELECT
        lhtt.name AS [LoaiHinhThienTai],
        SUM(ttttct.dien_tich) AS [TongDienTich],
        SUM(ttttct.san_luong) AS [TongSanLuong]
    FROM DuLieuThietHaiThienTai tttt
    INNER JOIN LoaiHinhThienTai lhtt ON lhtt.id = tttt.loai_hinh_thien_tai
    LEFT JOIN DuLieuThietHaiThienTaiChiTiet ttttct 
        ON ttttct.du_lieu_thiet_hai_thien_tai = tttt.id 
        AND (ttttct.deleted IS NULL OR ttttct.deleted = 0)
    WHERE tttt.deleted = 0
    GROUP BY lhtt.name
)