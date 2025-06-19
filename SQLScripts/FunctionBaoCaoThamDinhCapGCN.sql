CREATE OR ALTER FUNCTION QLCLFunctionBaoCaoThamDinhCapGCN(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Province INT = NULL,
    @Ward INT = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        FORMAT(qlcl.ngay_tham_dinh, 'yyyy-MM') AS thang,
        COUNT(*) AS tong_co_so_tham_dinh,
        SUM(CASE WHEN qlcl.ket_qua_tham_dinh = 1 THEN 1 ELSE 0 END) AS so_dat,
        SUM(CASE WHEN qlcl.ket_qua_tham_dinh = 2 THEN 1 ELSE 0 END) AS so_khong_dat,
        SUM(CASE WHEN qlcl.ket_qua_tham_dinh = 1 AND qlcl.so_giay_chung_nhan IS NOT NULL AND qlcl.so_giay_chung_nhan != '' THEN 1 ELSE 0 END) AS so_co_so_duoc_cap_gcn,
        CASE WHEN COUNT(*) > 0
        THEN CAST(CAST(SUM(CASE WHEN qlcl.ket_qua_tham_dinh = 1 AND qlcl.so_giay_chung_nhan IS NOT NULL AND qlcl.so_giay_chung_nhan != '' THEN 1 ELSE 0 END) AS DECIMAL(10,2)) / CAST(COUNT(*) AS DECIMAL(10,2)) * 100 AS DECIMAL(5,2))
        ELSE CAST(0 AS DECIMAL(5,2)) END AS ty_le_co_so_duoc_cap_gcn
    FROM
        QLCLCoSoNLTSDuDieuKienATTP qlcl
    WHERE
        qlcl.ngay_tham_dinh IS NOT NULL
        AND (@FromDate IS NULL OR qlcl.ngay_tham_dinh >= @FromDate)
        AND (@ToDate IS NULL OR qlcl.ngay_tham_dinh <= @ToDate)
        AND (@Province IS NULL OR qlcl.province = @Province)
        AND (@Ward IS NULL OR qlcl.ward = @Ward)
        AND qlcl.deleted = 0
        AND qlcl.loai = 1
    GROUP BY
        FORMAT(qlcl.ngay_tham_dinh, 'yyyy-MM')
) 

