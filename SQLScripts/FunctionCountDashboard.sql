-- Create Table-Valued Function for TT dashboard counts
CREATE OR ALTER FUNCTION TTFunctionCountDashboard(
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
        (
            SELECT
                COUNT(1)
            FROM CoSoSanXuatPhanBon
            WHERE deleted = 0
            AND (@FromDate IS NULL OR ngay_cap_gcn >= @FromDate)
            AND (@ToDate IS NULL OR ngay_cap_gcn <= @ToDate)
            AND (@Province IS NULL OR province = @Province)
            AND (@Ward IS NULL OR ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
        ) AS CoSoSanXuatPhanBon,
        (
            SELECT
                COUNT(1)
            FROM CoSoDuDieuKienBuonBanPhanBon
            WHERE deleted = 0
            AND (@FromDate IS NULL OR ngay_cap >= @FromDate)
            AND (@ToDate IS NULL OR ngay_cap <= @ToDate)
            AND (@Province IS NULL OR province = @Province)
            AND (@Ward IS NULL OR ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
        ) AS CoSoDuDieuKienBuonBanPhanBon,
        (
            SELECT
                COUNT(1)
            FROM CoSoSanXuatThuocBVTV
            WHERE deleted = 0
            AND (@FromDate IS NULL OR ngay_cap >= @FromDate)
            AND (@ToDate IS NULL OR ngay_cap <= @ToDate)
            AND (@Province IS NULL OR province = @Province)
            AND (@Ward IS NULL OR ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
        ) AS CoSoSanXuatThuocBVTV,
        (
            SELECT
                COUNT(1)
            FROM CoSoKinhDoanhThuocBVTV
            WHERE deleted = 0
            AND (@FromDate IS NULL OR ngay_cap >= @FromDate)
            AND (@ToDate IS NULL OR ngay_cap <= @ToDate)
            AND (@Province IS NULL OR province = @Province)
            AND (@Ward IS NULL OR ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
        ) AS CoSoKinhDoanhThuocBVTV,
        (
            SELECT
                COUNT(1)
            FROM ViPhamSXKDThuocBVTV vpsx
            LEFT JOIN CoSoSanXuatThuocBVTV cssx ON cssx.id = vpsx.co_so_san_xuat_thuoc_bvtv
            LEFT JOIN CoSoKinhDoanhThuocBVTV cskd ON cskd.id = vpsx.co_so_kinh_doanh_thuoc_bvtv
            WHERE vpsx.deleted = 0
            AND (@FromDate IS NULL OR vpsx.ngay_phat_hien >= @FromDate)
            AND (@ToDate IS NULL OR vpsx.ngay_phat_hien <= @ToDate)
            AND (@Province IS NULL OR cssx.province = @Province OR cskd.province = @Province)
            AND (
                @Ward IS NULL
                OR cssx.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ','))
                OR cskd.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ','))
            )
        ) AS ViPhamSanXuatKinhDoanhThuocBVTV
)
