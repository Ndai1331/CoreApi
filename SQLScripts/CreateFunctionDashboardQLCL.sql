-- Create FunctionDashboardQLCL function
CREATE OR ALTER FUNCTION FunctionDashboardQLCL(
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
        (SELECT COUNT(*) FROM QLCLCoSoCheBienNLTS WHERE deleted = 0 
           AND (@Province IS NULL OR province = @Province)
                AND (@Ward IS NULL OR ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
        ) AS so_luong_co_so,
        
        (SELECT
            SUM(CASE WHEN qlcl.ket_qua_tham_dinh = 1 AND qlcl.so_giay_chung_nhan IS NOT NULL AND qlcl.so_giay_chung_nhan != '' THEN 1 ELSE 0 END)
        FROM
            QLCLCoSoNLTSDuDieuKienATTP qlcl
            
        WHERE
            qlcl.ngay_tham_dinh IS NOT NULL
            AND (@FromDate IS NULL OR qlcl.ngay_tham_dinh >= @FromDate)
            AND (@ToDate IS NULL OR qlcl.ngay_tham_dinh <= @ToDate)
            AND (@Province IS NULL OR qlcl.province = @Province OR qlcl.province = @Province)
            AND (@Ward IS NULL OR qlcl.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')) OR qlcl.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
            AND qlcl.deleted = 0
            AND qlcl.loai = 1) AS so_luong_co_so_dat_chung_nhan,
        
        (SELECT
            COUNT(*)
        FROM
            QLCLCoSoViPhamATTP qlcl
            left join QLCLCoSoCheBienNLTS co_so_che_bien_nlts on qlcl.co_so_che_bien_nlts = co_so_che_bien_nlts.id
            left join QLCLCoSoNLTSDuDieuKienATTP co_so_nlts_du_dieu_kien_attp on qlcl.co_so_nlts_du_dieu_kien_attp = co_so_nlts_du_dieu_kien_attp.id
        WHERE
            qlcl.ngay_ghi_nhan IS NOT NULL
            AND (@FromDate IS NULL OR qlcl.ngay_ghi_nhan >= @FromDate)
            AND (@ToDate IS NULL OR qlcl.ngay_ghi_nhan <= @ToDate)
            AND (@Province IS NULL OR co_so_che_bien_nlts.province = @Province OR co_so_nlts_du_dieu_kien_attp.province = @Province)
            AND (@Ward IS NULL OR co_so_che_bien_nlts.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')) OR co_so_nlts_du_dieu_kien_attp.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
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
                AND (@Province IS NULL OR kthk.province = @Province)
                AND (@Ward IS NULL OR kthk.ward IN (SELECT value FROM STRING_SPLIT(@Ward, ',')))
                AND kthk.deleted = 0
            GROUP BY 
                YEAR(kthk.ngay_kiem_tra) * 100 + MONTH(kthk.ngay_kiem_tra)
        ) AS monthly_summary) AS so_dot_kiem_tra
) 