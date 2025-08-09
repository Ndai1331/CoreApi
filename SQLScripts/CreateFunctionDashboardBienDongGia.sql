-- Create CreateFunctionDashboardBienDongGia function
CREATE OR ALTER FUNCTION CreateFunctionDashboardBienDongGia(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Province INT = NULL,
    @Ward VARCHAR(500) = NULL,
    @Loai INT = NULL,
    @TenSanPham NVARCHAR(255) = NULL
)
RETURNS TABLE
AS
RETURN
(
    -- Data from QLCLGiaCaNongSan table (Agricultural Products)
    SELECT 
        ngay_ghi_nhan,  -- recording date
        1 as loai,  -- 1: Agricultural products
        QLCLSanPhamSanXuat.name as ten_san_pham, 
        QLCLGiaCaNongSan.nha_cung_cap,
        CONCAT(Provinces.name, ' - ', Wards.name) as dia_diem, -- province name + ward name   
        DonViTinh.name as don_vi_tinh, -- unit name
        gia_mua_vao, 
        gia_ban_ra, 
        CASE 
            WHEN gia_mua_vao > 0 AND gia_mua_vao IS NOT NULL AND gia_ban_ra IS NOT NULL 
            THEN ROUND(((gia_ban_ra - gia_mua_vao) / gia_mua_vao) * 100, 2)
            ELSE 0
        END as bien_dong -- price change percentage (%)
    FROM QLCLGiaCaNongSan
    LEFT JOIN QLCLSanPhamSanXuat ON QLCLSanPhamSanXuat.id = QLCLGiaCaNongSan.san_pham_san_xuat
    LEFT JOIN Provinces ON Provinces.id = QLCLGiaCaNongSan.province
    LEFT JOIN Wards ON Wards.id = QLCLGiaCaNongSan.ward
    LEFT JOIN DonViTinh ON DonViTinh.id = QLCLGiaCaNongSan.don_vi_tinh
    WHERE 
        QLCLGiaCaNongSan.deleted = 0
        AND QLCLSanPhamSanXuat.deleted = 0
        AND (@FromDate IS NULL OR ngay_ghi_nhan >= @FromDate)
        AND (@ToDate IS NULL OR ngay_ghi_nhan <= @ToDate)
        AND (@Province IS NULL OR QLCLGiaCaNongSan.province = @Province)
        AND (@Ward IS NULL OR QLCLGiaCaNongSan.ward IN (SELECT TRIM(value) FROM STRING_SPLIT(@Ward, ',')))
        AND (@Loai IS NULL OR @Loai = 1) -- Filter for agricultural products
        AND (
            @TenSanPham IS NULL
            OR QLCLSanPhamSanXuat.name LIKE '%' + @TenSanPham + '%'
            OR QLCLGiaCaNongSan.nha_cung_cap LIKE '%' + @TenSanPham + '%'
            OR @TenSanPham = '1'
        )

    UNION ALL

    -- Data from QLCLGiaCaVatTuNN table (Agricultural Materials)
    SELECT 
        ngay_ghi_nhan,  -- recording date
        2 as loai,  -- 2: Agricultural materials
        QLCL_VatTuNongNghiep.name as ten_san_pham, 
        QLCLGiaCaVatTuNN.nha_cung_cap,
        CONCAT(Provinces.name, ' - ', Wards.name) as dia_diem, -- province name + ward name   
        DonViTinh.name as don_vi_tinh, -- unit name
        gia_mua_vao, 
        gia_ban_ra, 
        CASE 
            WHEN gia_mua_vao > 0 AND gia_mua_vao IS NOT NULL AND gia_ban_ra IS NOT NULL 
            THEN ROUND(((gia_ban_ra - gia_mua_vao) / gia_mua_vao) * 100, 2)
            ELSE 0
        END as bien_dong -- price change percentage (%)
    FROM QLCLGiaCaVatTuNN
    LEFT JOIN QLCL_VatTuNongNghiep ON QLCL_VatTuNongNghiep.id = QLCLGiaCaVatTuNN.vat_tu_nong_nghiep
    LEFT JOIN Provinces ON Provinces.id = QLCLGiaCaVatTuNN.province
    LEFT JOIN Wards ON Wards.id = QLCLGiaCaVatTuNN.ward
    LEFT JOIN DonViTinh ON DonViTinh.id = QLCLGiaCaVatTuNN.don_vi_tinh
    WHERE 
        QLCLGiaCaVatTuNN.deleted = 0
        AND QLCL_VatTuNongNghiep.deleted = 0
        AND (@FromDate IS NULL OR ngay_ghi_nhan >= @FromDate)
        AND (@ToDate IS NULL OR ngay_ghi_nhan <= @ToDate)
        AND (@Province IS NULL OR QLCLGiaCaVatTuNN.province = @Province)
        AND (@Ward IS NULL OR QLCLGiaCaVatTuNN.ward IN (SELECT TRIM(value) FROM STRING_SPLIT(@Ward, ',')))
        AND (@Loai IS NULL OR @Loai = 2) -- Filter for agricultural materials
        AND (
            @TenSanPham IS NULL
            OR QLCL_VatTuNongNghiep.name LIKE '%' + @TenSanPham + '%'
            OR QLCLGiaCaVatTuNN.nha_cung_cap LIKE '%' + @TenSanPham + '%'
            OR @TenSanPham = '2'
        )
)

