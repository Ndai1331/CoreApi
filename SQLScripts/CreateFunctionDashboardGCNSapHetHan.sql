-- Create FunctionDashboardGCNSapHetHan function
CREATE OR ALTER FUNCTION FunctionDashboardGCNSapHetHan(
    @Province INT = NULL,
    @Ward INT = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        cs.id,
        cs.name,
        cs.so_giay_chung_nhan,
        cs.ngay_het_hieu_luc,
        DATEDIFF(DAY, GETDATE(), cs.ngay_het_hieu_luc) AS so_ngay_con_lai
    FROM
        QLCLCoSoNLTSDuDieuKienATTP cs
    WHERE
        cs.deleted = 0
        AND cs.so_giay_chung_nhan IS NOT NULL
        AND cs.so_giay_chung_nhan != ''
        AND cs.ngay_het_hieu_luc IS NOT NULL
        AND cs.ngay_het_hieu_luc >= GETDATE()
        AND cs.ngay_het_hieu_luc <= DATEADD(MONTH, 1, GETDATE())
        AND (@Province IS NULL OR cs.province = @Province)
        AND (@Ward IS NULL OR cs.ward = @Ward)
    ORDER BY
        cs.ngay_het_hieu_luc ASC
) 

