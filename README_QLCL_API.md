# QLCL API Documentation

## Overview
QLCL (Quality Control) API cung cấp các endpoint để truy xuất dữ liệu báo cáo từ các view trong database DRCARE_CORE.

## Database Configuration
- **Server**: 103.151.53.46:1999
- **Database**: DRCARE_CORE
- **User**: sa
- **Password**: CDSL2024$12#903

## API Endpoints

### 1. DynamicQLCLController (Recommended)
Controller động có thể truy vấn bất kỳ view QLCL nào trong database.

#### 1.1 Get All Available Views
```
GET /api/DynamicQLCL/views
```
Trả về danh sách tất cả các view QLCL có sẵn trong database.

**Response:**
```json
{
  "views": [
    {
      "viewName": "QLCLViewBaoCaoKiemTraHauKiemATTP",
      "createDate": "2024-01-01T00:00:00",
      "modifyDate": "2024-01-01T00:00:00",
      "definition": "CREATE VIEW QLCLViewBaoCaoKiemTraHauKiemATTP AS..."
    }
  ],
  "count": 1
}
```

#### 1.2 Get Data from Any View
```
GET /api/DynamicQLCL/view/{viewName}?page=1&pageSize=10
```

**Parameters:**
- `viewName` (path): Tên view trong database
- `page` (query): Số trang (default: 1)
- `pageSize` (query): Kích thước trang (default: 10)

**Response:**
```json
{
  "viewName": "QLCLViewBaoCaoKiemTraHauKiemATTP",
  "data": [
    {
      "thang": "2024-01",
      "province": 1,
      "ward": 1,
      "tong_dot_kiem_tra": 5,
      "tong_co_so_kiem_tra": 25,
      "so_vi_pham": 3,
      "so_chap_hanh": 22,
      "so_dat": 20,
      "so_khong_dat": 5
    }
  ],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

#### 1.3 Get View Structure
```
GET /api/DynamicQLCL/view/{viewName}/structure
```

**Response:**
```json
{
  "viewName": "QLCLViewBaoCaoKiemTraHauKiemATTP",
  "columns": [
    {
      "columnName": "thang",
      "dataType": "varchar",
      "maxLength": 7,
      "isNullable": true,
      "description": "Month in yyyy-MM format"
    }
  ]
}
```

#### 1.4 Get View Summary
```
GET /api/DynamicQLCL/view/{viewName}/summary
```

**Response:**
```json
{
  "viewName": "QLCLViewBaoCaoKiemTraHauKiemATTP",
  "summary": {
    "TotalRecords": 100,
    "tong_dot_kiem_tra_Min": 1,
    "tong_dot_kiem_tra_Max": 10,
    "tong_dot_kiem_tra_Avg": 5.5
  }
}
```

#### 1.5 Execute Custom Query
```
POST /api/DynamicQLCL/query
```

**Request Body:**
```json
{
  "query": "SELECT * FROM QLCLViewBaoCaoKiemTraHauKiemATTP WHERE thang = '2024-01'"
}
```

**Response:**
```json
{
  "query": "SELECT * FROM QLCLViewBaoCaoKiemTraHauKiemATTP WHERE thang = '2024-01'",
  "data": [...],
  "count": 5
}
```

### 2. QLCLBaoCaoKiemTraHauKiemATTPController
Controller cụ thể cho view QLCLViewBaoCaoKiemTraHauKiemATTP (nếu view tồn tại).

#### 2.1 Get All Data
```
GET /api/QLCLBaoCaoKiemTraHauKiemATTP
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Page size (default: 10)
- `thang` (optional): Filter by month (yyyy-MM format)
- `province` (optional): Filter by province ID
- `ward` (optional): Filter by ward ID

#### 2.2 Get By Composite Key
```
GET /api/QLCLBaoCaoKiemTraHauKiemATTP/detail?thang=2024-01&province=1&ward=1
```

#### 2.3 Get Summary Statistics
```
GET /api/QLCLBaoCaoKiemTraHauKiemATTP/summary?thang=2024-01&province=1
```

#### 2.4 Get Data Grouped by Month
```
GET /api/QLCLBaoCaoKiemTraHauKiemATTP/by-month?year=2024&province=1
```

#### 2.5 Get Data Grouped by Province
```
GET /api/QLCLBaoCaoKiemTraHauKiemATTP/by-province?thang=2024-01
```

#### 2.6 Get Latest Data
```
GET /api/QLCLBaoCaoKiemTraHauKiemATTP/latest?limit=10
```

## Setup Instructions

### 1. Check Existing Views
Chạy script SQL để kiểm tra các view hiện có:
```sql
-- Chạy file: SQLScripts/CheckExistingViews.sql
```

### 2. Create View (if needed)
Nếu view chưa tồn tại, chạy script tạo view:
```sql
-- Chạy file: SQLScripts/CreateQLCLViewBaoCaoKiemTraHauKiemATTP.sql
```

### 3. Update Connection String
Đảm bảo connection string trong `appsettings.json` đã được cấu hình đúng:

```json
{
  "connectionStrings": {
    "UserDbConnectionString": "Server=103.151.53.46,1999;Database=DRCARE_CORE;User Id=sa;Password=CDSL2024$12#903; TrustServerCertificate=True"
  }
}
```

### 4. Build and Run
```bash
dotnet build
dotnet run
```

## Usage Examples

### JavaScript/Fetch
```javascript
// Get all available views
const viewsResponse = await fetch('/api/DynamicQLCL/views');
const views = await viewsResponse.json();

// Get data from specific view
const dataResponse = await fetch('/api/DynamicQLCL/view/QLCLViewBaoCaoKiemTraHauKiemATTP?page=1&pageSize=10');
const data = await dataResponse.json();

// Get view structure
const structureResponse = await fetch('/api/DynamicQLCL/view/QLCLViewBaoCaoKiemTraHauKiemATTP/structure');
const structure = await structureResponse.json();

// Execute custom query
const queryResponse = await fetch('/api/DynamicQLCL/query', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    query: "SELECT * FROM QLCLViewBaoCaoKiemTraHauKiemATTP WHERE thang = '2024-01'"
  })
});
const queryResult = await queryResponse.json();
```

### C# HttpClient
```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("https://your-api-url/");

// Get available views
var viewsResponse = await client.GetAsync("api/DynamicQLCL/views");
var views = await viewsResponse.Content.ReadFromJsonAsync<dynamic>();

// Get data from view
var dataResponse = await client.GetAsync("api/DynamicQLCL/view/QLCLViewBaoCaoKiemTraHauKiemATTP?page=1&pageSize=10");
var data = await dataResponse.Content.ReadFromJsonAsync<dynamic>();

// Execute custom query
var queryRequest = new { query = "SELECT * FROM QLCLViewBaoCaoKiemTraHauKiemATTP WHERE thang = '2024-01'" };
var queryResponse = await client.PostAsJsonAsync("api/DynamicQLCL/query", queryRequest);
var queryResult = await queryResponse.Content.ReadFromJsonAsync<dynamic>();
```

## Security Features

### DynamicQLCLController
- ✅ SQL injection prevention
- ✅ View name validation
- ✅ Query type restriction (SELECT only)
- ✅ QLCL view filtering
- ✅ Parameterized queries

### Error Handling
API trả về các HTTP status codes sau:
- `200 OK`: Thành công
- `400 Bad Request`: Lỗi tham số hoặc validation
- `404 Not Found`: Không tìm thấy dữ liệu
- `500 Internal Server Error`: Lỗi server

## Notes
- **DynamicQLCLController** được khuyến nghị sử dụng vì tính linh hoạt cao
- Controller có thể làm việc với bất kỳ view QLCL nào trong database
- Tự động phát hiện cấu trúc view và trả về dữ liệu phù hợp
- Hỗ trợ pagination, filtering, và custom queries
- Tất cả các endpoint đều hỗ trợ CORS
- Dữ liệu được trả về theo định dạng JSON

## Database View Structure

### QLCLViewBaoCaoKiemTraHauKiemATTP
View này tổng hợp dữ liệu từ bảng `QLCLKiemTraHauKiemATTP` theo tháng, tỉnh/thành phố và quận/huyện.

**Columns:**
- `thang` (string): Tháng báo cáo (yyyy-MM format)
- `province` (int): ID tỉnh/thành phố
- `ward` (int): ID quận/huyện
- `tong_dot_kiem_tra` (int): Tổng số đợt kiểm tra
- `tong_co_so_kiem_tra` (int): Tổng số cơ sở kiểm tra
- `so_vi_pham` (int): Số cơ sở vi phạm
- `so_chap_hanh` (int): Số cơ sở chấp hành
- `so_dat` (int): Số cơ sở đạt
- `so_khong_dat` (int): Số cơ sở không đạt

## Setup Instructions

### 1. Create Database View
Chạy script SQL trong file `SQLScripts/CreateQLCLViews.sql` để tạo view trong database.

### 2. Update Connection String
Đảm bảo connection string trong `appsettings.json` đã được cấu hình đúng:

```json
{
  "connectionStrings": {
    "UserDbConnectionString": "Server=103.151.53.46,1999;Database=DRCARE_CORE;User Id=sa;Password=CDSL2024$12#903; TrustServerCertificate=True"
  }
}
```

### 3. Build and Run
```bash
dotnet build
dotnet run
```

## Error Handling
API trả về các HTTP status codes sau:
- `200 OK`: Thành công
- `404 Not Found`: Không tìm thấy dữ liệu
- `500 Internal Server Error`: Lỗi server

## Example Usage

### JavaScript/Fetch
```javascript
// Get all data with pagination
const response = await fetch('/api/QLCLBaoCaoKiemTraHauKiemATTP?page=1&pageSize=10');
const data = await response.json();

// Get summary statistics
const summaryResponse = await fetch('/api/QLCLBaoCaoKiemTraHauKiemATTP/summary?thang=2024-01');
const summary = await summaryResponse.json();
```

### C# HttpClient
```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("https://your-api-url/");

// Get all data
var response = await client.GetAsync("api/QLCLBaoCaoKiemTraHauKiemATTP?page=1&pageSize=10");
var data = await response.Content.ReadFromJsonAsync<dynamic>();

// Get summary
var summaryResponse = await client.GetAsync("api/QLCLBaoCaoKiemTraHauKiemATTP/summary?thang=2024-01");
var summary = await summaryResponse.Content.ReadFromJsonAsync<dynamic>();
```

## Notes
- Tất cả các endpoint đều hỗ trợ CORS
- Dữ liệu được trả về theo định dạng JSON
- Pagination được áp dụng cho các endpoint trả về danh sách dữ liệu
- Các filter có thể được kết hợp để lọc dữ liệu theo nhiều tiêu chí 