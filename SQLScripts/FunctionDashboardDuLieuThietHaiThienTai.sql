-- Create Table-Valued Function for TT dashboard counts
CREATE OR ALTER FUNCTION TTFunctionDashboardDuLieuThietHaiThienTai(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Province INT = NULL,
    @Ward VARCHAR(500) = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        lhtt.name AS [LoaiHinhThienTai],
        CAST(ISNULL(SUM(ttttct.dien_tich), 0) AS DECIMAL(18,2)) AS [TongDienTich],
        CAST(ISNULL(SUM(ttttct.san_luong), 0) AS DECIMAL(18,2)) AS [TongSanLuong]
    FROM DuLieuThietHaiThienTai tttt
    INNER JOIN LoaiHinhThienTai lhtt ON lhtt.id = tttt.loai_hinh_thien_tai
    LEFT JOIN DuLieuThietHaiThienTaiChiTiet ttttct 
        ON ttttct.du_lieu_thiet_hai_thien_tai = tttt.id 
        AND (ttttct.deleted IS NULL OR ttttct.deleted = 0)
    WHERE tttt.deleted = 0
    AND (@FromDate IS NULL OR tttt.ngay_ghi_nhan >= @FromDate)
    AND (@ToDate IS NULL OR tttt.ngay_ghi_nhan <= @ToDate)
    AND (@Province IS NULL OR tttt.province = @Province)
    AND (@Ward IS NULL OR tttt.ward IN (SELECT TRIM(value) FROM STRING_SPLIT(@Ward, ',')))
    GROUP BY lhtt.name
)