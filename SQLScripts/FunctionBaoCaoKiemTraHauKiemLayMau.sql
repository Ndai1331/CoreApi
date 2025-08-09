
-- Create Table-Valued Function for QLCL Chi Tiet Kiem Tra Hau Kiem ATTP
CREATE OR ALTER FUNCTION QLCLFunctionChiTietKiemTraHauKiemATTP(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Province INT = NULL,
    @Ward VARCHAR(500) = NULL,
    @StringSearch NVARCHAR(500) = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        t.san_pham,
        t.chi_tieu,
        SUM(t.so_mau_khong_dat) AS so_mau_khong_dat,
        SUM(t.so_luong_mau) AS so_luong_mau
    FROM (
        SELECT 
            sp.name AS san_pham,
            ct.so_luong_mau,
            CASE ct.chi_tieu
                WHEN 1 THEN N'Chỉ tiêu vi sinh'
                WHEN 2 THEN N'Chỉ tiêu thuốc BVTV'
                WHEN 3 THEN N'Chỉ tiêu hóa chất, chất bảo quản'
            ELSE N'Không xác định'
            END AS chi_tieu,
            COALESCE(ct.so_mau_khong_dat, 0) so_mau_khong_dat
        FROM
            QLCLKiemTraHauKiemATTPChiTiet ct
            INNER JOIN QLCLKiemTraHauKiemATTP kt ON ct.kiem_tra_hau_kiem_attp = kt.id
            INNER JOIN QLCLSanPhamSanXuat sp ON ct.san_pham = sp.id
        WHERE
            kt.ngay_kiem_tra IS NOT NULL
            AND (@FromDate IS NULL OR kt.ngay_kiem_tra >= @FromDate)
            AND (@ToDate IS NULL OR kt.ngay_kiem_tra <= @ToDate)
            AND (@Province IS NULL OR kt.province = @Province)
            AND (@Ward IS NULL OR kt.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
            AND (
                @StringSearch IS NULL OR sp.name LIKE N'%' + @StringSearch + '%'
                OR ct.chi_tieu_vi_pham LIKE N'%' + @StringSearch + '%'
                OR ct.muc_phat_hien LIKE N'%' + @StringSearch + '%'
            )
            AND ct.deleted = 0
            AND sp.deleted = 0
            AND kt.deleted = 0
    ) AS t
    GROUP BY t.san_pham, t.chi_tieu
)