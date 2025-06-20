CREATE OR ALTER FUNCTION FunctionSoDotKiemTraTheoThang(
    @Year INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 1 THEN kthk.dot_kiem_tra END) as t1,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 2 THEN kthk.dot_kiem_tra END) as t2,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 3 THEN kthk.dot_kiem_tra END) as t3,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 4 THEN kthk.dot_kiem_tra END) as t4,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 5 THEN kthk.dot_kiem_tra END) as t5,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 6 THEN kthk.dot_kiem_tra END) as t6,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 7 THEN kthk.dot_kiem_tra END) as t7,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 8 THEN kthk.dot_kiem_tra END) as t8,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 9 THEN kthk.dot_kiem_tra END) as t9,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 10 THEN kthk.dot_kiem_tra END) as t10,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 11 THEN kthk.dot_kiem_tra END) as t11,
       COUNT(DISTINCT CASE WHEN MONTH(kthk.ngay_kiem_tra) = 12 THEN kthk.dot_kiem_tra END) as t12
    FROM
        QLCLKiemTraHauKiemATTP kthk
    WHERE
        kthk.deleted = 0
        AND YEAR(kthk.ngay_kiem_tra) = @Year
) 