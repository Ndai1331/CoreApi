ALTER VIEW QLCLViewBaoCaoKiemTraHauKiemATTP AS
SELECT
    qlcl.ngay_kiem_tra AS ngay_kiem_tra,
    FORMAT(qlcl.ngay_kiem_tra, 'yyyy-MM') AS thang,
    qlcl.province,
    qlcl.ward,
    COUNT(DISTINCT qlcl.dot_kiem_tra) AS tong_dot_kiem_tra,
    COUNT(*) AS tong_co_so_kiem_tra,
    SUM(CASE WHEN qlcl.tinh_hinh_vi_pham = 1 THEN 1 ELSE 0 END) AS so_vi_pham,
    SUM(CASE WHEN qlcl.tinh_hinh_vi_pham = 2 THEN 1 ELSE 0 END) AS so_chap_hanh,
    SUM(CASE WHEN qlcl.ket_qua_kiem_tra = 1 THEN 1 ELSE 0 END) AS so_dat,
    SUM(CASE WHEN qlcl.ket_qua_kiem_tra = 2 THEN 1 ELSE 0 END) AS so_khong_dat
FROM
    QLCLKiemTraHauKiemATTP qlcl
WHERE
    qlcl.ngay_kiem_tra IS NOT NULL 
GROUP BY
    qlcl.ngay_kiem_tra,
    FORMAT(qlcl.ngay_kiem_tra, 'yyyy-MM'),
    qlcl.province,
    qlcl.ward
