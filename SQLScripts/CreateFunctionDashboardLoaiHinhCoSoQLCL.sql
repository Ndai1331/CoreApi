-- Create FunctionDashboardLoaiHinhCoSoQLCL function
CREATE OR ALTER FUNCTION FunctionDashboardLoaiHinhCoSoQLCL(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        lhcs.code AS code,
        lhcs.name AS name,
        COUNT(cs.id) AS so_luong_co_so
    FROM
        QLCLLoaiHinhCoSo lhcs
        LEFT JOIN QLCLCoSoCheBienNLTS cs ON lhcs.id = cs.loai_hinh_co_so
    WHERE
        lhcs.deleted = 0
        AND (cs.deleted = 0 OR cs.deleted IS NULL)
    GROUP BY
        lhcs.code,
        lhcs.name
    ORDER BY
        so_luong_co_so DESC,
        lhcs.name
) 