
-- Create Table-Valued Function for QLCL Detail Co So Kiem Tra Hau Kiem ATTP
CREATE OR ALTER FUNCTION QLCLFunctionDetailCoSoKiemTraHauKiemATTP(
    @ThangNam NVARCHAR(7), -- Format: 'yyyy-MM'
    @Province INT = NULL,
    @Ward VARCHAR(500) = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        cs.id,
        cs.code,
        cs.name,
        cs.dia_chi,
        cs.dien_thoai,
        cs.dai_dien
    FROM
        QLCLKiemTraHauKiemATTP qlcl
        INNER JOIN QLCLCoSoNLTSDuDieuKienATTP cs ON qlcl.co_so = cs.id
    WHERE
        FORMAT(qlcl.ngay_kiem_tra, 'yyyy-MM') = @ThangNam
        AND (@Province IS NULL OR qlcl.province = @Province)
        AND (@Ward IS NULL OR qlcl.ward IN (SELECT TRIM(value) FROM STRING_SPLIT(@Ward, ',')))
        AND qlcl.ngay_kiem_tra IS NOT NULL
        AND qlcl.ket_qua_kiem_tra = 2
        AND qlcl.deleted = 0
        AND cs.deleted = 0
    GROUP BY
        cs.id,
        cs.code,
        cs.name,
        cs.dia_chi,
        cs.dien_thoai,
        cs.dai_dien
) 