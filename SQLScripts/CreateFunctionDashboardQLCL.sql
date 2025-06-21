-- Create FunctionDashboardQLCL function
CREATE OR ALTER FUNCTION FunctionDashboardQLCL(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        (SELECT COUNT(*) FROM QLCLCoSoCheBienNLTS WHERE deleted = 0) AS so_luong_co_so,
        
        (SELECT
            SUM(CASE WHEN qlcl.ket_qua_tham_dinh = 1 AND qlcl.so_giay_chung_nhan IS NOT NULL AND qlcl.so_giay_chung_nhan != '' THEN 1 ELSE 0 END)
        FROM
            QLCLCoSoNLTSDuDieuKienATTP qlcl
        WHERE
            qlcl.ngay_tham_dinh IS NOT NULL
            AND (@FromDate IS NULL OR qlcl.ngay_tham_dinh >= @FromDate)
            AND (@ToDate IS NULL OR qlcl.ngay_tham_dinh <= @ToDate)
            AND qlcl.deleted = 0
            AND qlcl.loai = 1) AS so_luong_co_so_dat_chung_nhan,
        
        (SELECT
            COUNT(*)
        FROM
            QLCLCoSoViPhamATTP qlcl
        WHERE
            qlcl.ngay_ghi_nhan IS NOT NULL
            AND (@FromDate IS NULL OR qlcl.ngay_ghi_nhan >= @FromDate)
            AND (@ToDate IS NULL OR qlcl.ngay_ghi_nhan <= @ToDate)
            AND qlcl.deleted = 0) AS so_luong_vu_vi_pham,
        
        -- Đếm số đợt kiểm tra trong từng tháng và tổng hợp lại
        (SELECT
            SUM(monthly_inspections)
        FROM (
            SELECT 
                YEAR(kthk.ngay_kiem_tra) * 100 + MONTH(kthk.ngay_kiem_tra) AS year_month,
                COUNT(DISTINCT kthk.dot_kiem_tra) AS monthly_inspections
            FROM
                QLCLKiemTraHauKiemATTP kthk
            WHERE
                kthk.ngay_kiem_tra IS NOT NULL
                AND kthk.dot_kiem_tra IS NOT NULL
                AND kthk.dot_kiem_tra != ''
                AND (@FromDate IS NULL OR kthk.ngay_kiem_tra >= @FromDate)
                AND (@ToDate IS NULL OR kthk.ngay_kiem_tra <= @ToDate)
                AND kthk.deleted = 0
            GROUP BY 
                YEAR(kthk.ngay_kiem_tra) * 100 + MONTH(kthk.ngay_kiem_tra)
        ) AS monthly_summary) AS so_dot_kiem_tra
) 