
-- Create Table-Valued Function for QLCL Chi Tiet Kiem Tra Hau Kiem ATTP
CREATE OR ALTER FUNCTION QLCLFunctionChiTietKiemTraHauKiemATTP(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Province INT = NULL,
    @Ward INT = NULL,
    @StringSearch NVARCHAR(500) = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        ct.id,
        sp.name AS san_pham,
        ct.so_luong_mau,
        CASE ct.chi_tieu
            WHEN 1 THEN N'Chỉ tiêu vi sinh'
            WHEN 2 THEN N'Chỉ tiêu thuốc BVTV'
            WHEN 3 THEN N'Chỉ tiêu hóa chất, chất bảo quản'
        ELSE ''
        END AS chi_tieu,
        CASE ct.so_mau_khong_dat
            WHEN NULL THEN 0
            ELSE ct.so_mau_khong_dat
        END AS so_mau_khong_dat,
        ct.chi_tieu_vi_pham,
        ct.muc_phat_hien
    FROM
        QLCLKiemTraHauKiemATTPChiTiet ct
        INNER JOIN QLCLKiemTraHauKiemATTP kt ON ct.kiem_tra_hau_kiem_attp = kt.id
        INNER JOIN QLCLSanPhamSanXuat sp ON ct.san_pham = sp.id
    WHERE
        kt.ngay_kiem_tra IS NOT NULL
        AND (@FromDate IS NULL OR kt.ngay_kiem_tra >= @FromDate)
        AND (@ToDate IS NULL OR kt.ngay_kiem_tra <= @ToDate)
        AND (@Province IS NULL OR kt.province = @Province)
        AND (@Ward IS NULL OR kt.ward = @Ward)
        AND (@StringSearch IS NULL OR sp.name LIKE N'%' + @StringSearch + '%'
            OR ct.chi_tieu_vi_pham LIKE N'%' + @StringSearch + '%'
            OR ct.muc_phat_hien LIKE N'%' + @StringSearch + '%')
        AND ct.deleted = 0
) 