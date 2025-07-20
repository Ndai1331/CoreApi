-- Create Table-Valued Function for TT dashboard counts
CREATE OR ALTER FUNCTION TTFunctionCountDashboard()
RETURNS TABLE
AS
RETURN
(
    SELECT
        (SELECT COUNT(1) FROM CoSoSanXuatPhanBon WHERE deleted = 0) AS CoSoSanXuatPhanBon,
        (SELECT COUNT(1) FROM CoSoDuDieuKienBuonBanPhanBon WHERE deleted = 0) AS CoSoDuDieuKienBuonBanPhanBon,
        (SELECT COUNT(1) FROM CoSoSanXuatThuocBVTV WHERE deleted = 0) AS CoSoSanXuatThuocBVTV,
        (SELECT COUNT(1) FROM CoSoKinhDoanhThuocBVTV WHERE deleted = 0) AS CoSoKinhDoanhThuocBVTV,
        (SELECT COUNT(1) FROM ViPhamSXKDThuocBVTV WHERE deleted = 0) AS ViPhamSanXuatKinhDoanhThuocBVTV
)
