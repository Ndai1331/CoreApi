CREATE OR ALTER FUNCTION QLCLFunctionBaoCaoCoSoKhongCapGCN(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Province INT = NULL,
    @Ward VARCHAR(500) = NULL,
    @ThangNam NVARCHAR(7) = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT DISTINCT
        qlcl.id,
        qlcl.code,
        qlcl.name,
        qlcl.dia_chi,
        qlcl.dien_thoai,
        qlcl.dai_dien
    FROM
        QLCLCoSoNLTSDuDieuKienATTP qlcl
    WHERE
        qlcl.ngay_tham_dinh IS NOT NULL
        AND (@FromDate IS NULL OR qlcl.ngay_tham_dinh >= @FromDate)
        AND (@ToDate IS NULL OR qlcl.ngay_tham_dinh <= @ToDate)
        AND (@Province IS NULL OR qlcl.province = @Province)
        AND (@Ward IS NULL OR qlcl.ward IN (SELECT TRIM(value) FROM STRING_SPLIT(@Ward, ',')))
        AND qlcl.deleted = 0
        AND qlcl.loai = 1
        AND (qlcl.so_giay_chung_nhan IS NULL OR qlcl.so_giay_chung_nhan = ''
         OR qlcl.ket_qua_tham_dinh = 2)
        AND (@ThangNam IS NULL OR FORMAT(qlcl.ngay_tham_dinh, 'yyyy-MM') = @ThangNam)
) 

