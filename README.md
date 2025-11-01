# Pet Cafe Management System

## 📋 Bối cảnh dự án

Pet Cafe là hệ thống quản lý toàn diện cho các cơ sở chăm sóc thú cưng, giúp tự động hóa và quản lý các hoạt động từ đặt lịch, phân công nhân viên, quản lý ca làm việc đến chăm sóc sức khỏe và tiêm chủng cho thú cưng.

### Mục đích

- Quản lý đặt lịch và slots cho các dịch vụ chăm sóc thú cưng
- Phân công và quản lý nhân viên theo team và ca làm việc
- Quản lý lịch trình và nhiệm vụ hàng ngày cho nhân viên
- Theo dõi sức khỏe, hồ sơ tiêm chủng và lịch tiêm của thú cưng
- Quản lý đơn hàng, dịch vụ và sản phẩm

### Kiến trúc hệ thống

- **Backend**: ASP.NET Core (Clean Architecture)
  - **PetCafe.Domain**: Domain entities và constants
  - **PetCafe.Application**: Business logic và services
  - **PetCafe.Infrastructures**: Database context, repositories, migrations
  - **PetCafe.WebApi**: RESTful API controllers
- **Database**: Entity Framework Core (PostgreSQL)
- **Background Jobs**: Hangfire
- **Pattern**: Repository Pattern, Unit of Work, Dependency Injection

---

## 🎯 Business Rules

### 1. Quản lý Team và Work Shift

#### 1.1. Assign Work Shift cho Team

- ✅ Team phải tồn tại và đang active (`IsActive = true`)
- ✅ Work Shift IDs không được trùng lặp trong input
- ✅ Tất cả Work Shifts phải tồn tại và chưa bị xóa
- ✅ Work Shift không được trùng với Work Shift đã được gán cho team
- ✅ Các Work Shifts được assign cùng lúc không được trùng thời gian trên cùng ngày áp dụng
  - Kiểm tra `ApplicableDays` có giao nhau
  - Kiểm tra `StartTime` và `EndTime` có overlap
- ✅ Work Shift phải có:
  - `StartTime < EndTime`
  - `ApplicableDays` không null và không rỗng
- ✅ Nhân viên trong team không được có lịch trùng thời gian với Work Shift mới
  - Kiểm tra trên tất cả các team mà nhân viên tham gia
  - Kiểm tra từ hôm nay đến cuối tuần (Thứ 2 - Chủ nhật)
  - Chỉ kiểm tra các ngày mà Work Shift có hiệu lực
- ✅ Sau khi assign thành công, tự động tạo `DailySchedule` cho tất cả thành viên trong team từ hôm nay đến cuối tuần (background job)

#### 1.2. Quản lý Team Member

- ✅ Chỉ xử lý các Team Member chưa bị xóa (`IsDeleted = false`)
- ✅ Employee của Team Member phải tồn tại và chưa bị xóa
- ✅ Khi thêm member vào team có Work Shifts, tự động tạo `DailySchedule` từ hôm nay đến cuối tuần
- ✅ Khi xóa member khỏi team, tự động soft delete tất cả `DailySchedule` liên quan

#### 1.3. Remove Work Shift khỏi Team

- ✅ Xóa tất cả `DailySchedule` của các thành viên trong team liên quan đến Work Shift đó
- ✅ Soft delete `TeamWorkShift` record

---

### 2. Quản lý Daily Schedule (Lịch làm việc hàng ngày)

#### 2.1. Trạng thái Daily Schedule

- `PENDING`: Chưa điểm danh
- `PRESENT`: Đã điểm danh - có mặt
- `ABSENT`: Đã điểm danh - vắng mặt
- `EXCUSED`: Đã điểm danh - vắng có phép
- `LATE`: Đã điểm danh - đi muộn

#### 2.2. Tạo Daily Schedule

- ✅ Chỉ tạo cho các ngày trong tuần mà Work Shift có hiệu lực (`ApplicableDays`)
- ✅ Kiểm tra trùng thời gian khi `checkTimeOverlap = true`:
  - Nhân viên không được có lịch trùng thời gian trên cùng ngày
  - Chỉ kiểm tra với các `DailySchedule` chưa bị xóa
- ✅ Không tạo duplicate nếu đã có `DailySchedule` cho cùng TeamMember, WorkShift và Date

#### 2.3. Tính toán tuần làm việc

- Tuần được tính từ Thứ 2 đến Chủ nhật
- Nếu hôm nay là Chủ nhật, tính từ Thứ 2 tuần hiện tại (không phải tuần sau)
- Công thức: `daysFromMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7`

---

### 3. Quản lý Slot và Booking

#### 3.1. Tạo Slot

- ✅ Slot phải thuộc một Task đang active
- ✅ Thời gian slot phải nằm trong thời gian làm việc của Team:
  - Team phải có Work Shift bao phủ `StartTime` và `EndTime` của slot
  - Ngày áp dụng của slot phải nằm trong `ApplicableDays` của Work Shift
- ✅ Không được trùng slot với các slot hiện có:
  - Cùng Area, Team, hoặc PetGroup
  - Cùng `DayOfWeek`
  - Thời gian overlap
- ✅ Nếu Task có `ServiceId`, slot sẽ tự động có `ServiceId` và `ServiceStatus = UNAVAILABLE`
- ✅ `MaxCapacity` phải > 0
- ✅ Khi tạo slot recurring, tự động tạo `DailyTasks` cho các ngày còn lại trong tuần (background job)

#### 3.2. Đặt lịch (Booking)

- ✅ Slot phải:
  - Đang available (`ServiceStatus = AVAILABLE`)
  - Area, Team, Service đang active và chưa bị xóa
  - Task đang active
  - Phù hợp với ngày đặt lịch:
    - Recurring: `DayOfWeek` khớp với ngày đặt
    - Non-recurring: `SpecificDate` khớp với ngày đặt
  - `StartTime <= BookingDate.TimeOfDay <= EndTime`
- ✅ Kiểm tra capacity: Số lượng booking hiện tại < `MaxCapacity`
- ✅ Không được đặt quá `MaxCapacity` cho cùng một slot trong cùng ngày

#### 3.3. Slot Status

- `AVAILABLE`: Có thể đặt lịch
- `UNAVAILABLE`: Không thể đặt lịch
- `CANCELLED`: Đã hủy
- `MAINTENANCE`: Đang bảo trì

---

### 4. Quản lý Daily Task

#### 4.1. Trạng thái Daily Task

- `SCHEDULED`: Đã lên lịch
- `IN_PROGRESS`: Đang thực hiện
- `COMPLETED`: Đã hoàn thành
- `CANCELLED`: Đã hủy
- `MISSED`: Đã bỏ lỡ
- `SKIPPED`: Đã bỏ qua

#### 4.2. Tự động tạo Daily Tasks

- ✅ Từ Slot recurring: Tạo `DailyTasks` cho các ngày còn lại trong tuần từ ngày mai đến Chủ nhật
- ✅ Từ Slot non-recurring: Tạo `DailyTask` cho `SpecificDate`
- ✅ Tự động tạo cho toàn bộ tuần (Thứ 2 - Chủ nhật) cho các slot recurring vào đầu tuần

#### 4.3. Auto Assign Tasks

- Background job chạy tự động
- Tạo `DailyTasks` cho tất cả slots recurring trong tuần hiện tại
- Chỉ tạo cho các ngày chưa có `DailyTask`

---

### 5. Quản lý Booking Status

#### 5.1. Trạng thái Booking

- `PENDING`: Đang chờ xác nhận
- `CONFIRMED`: Đã xác nhận
- `IN_PROGRESS`: Đang thực hiện
- `COMPLETED`: Đã hoàn thành
- `CANCELLED`: Đã hủy

---

### 6. Quản lý Task

#### 6.1. Trạng thái Task

- `ACTIVE`: Đang hoạt động
- `INACTIVE`: Không hoạt động

#### 6.2. Priority Task

- `LOW`: Thấp
- `MEDIUM`: Trung bình
- `HIGH`: Cao
- `URGENT`: Khẩn cấp

---

### 7. Quản lý Team Status

#### 7.1. Team Status

- `ACTIVE`: Team đang hoạt động
- `INACTIVE`: Team không hoạt động

#### 7.2. Team Validation

- ✅ Chỉ có thể assign work shift cho team đang active (`IsActive = true`)
- ✅ Khi xóa team, tự động soft delete:
  - Tất cả `DailySchedule` của các thành viên trong team
  - Tất cả `TeamWorkShift` liên quan

---

### 8. Quản lý Work Shift

#### 8.1. Work Shift Validation

- ✅ `StartTime` phải nhỏ hơn `EndTime`
- ✅ `ApplicableDays` không được null hoặc rỗng
- ✅ Không được trùng thời gian với Work Shift khác trên cùng ngày áp dụng
- ✅ Khi tạo/update, kiểm tra duplicate dựa trên:
  - Ngày áp dụng giao nhau
  - Thời gian overlap

---

### 9. Soft Delete Pattern

Hệ thống sử dụng Soft Delete pattern cho tất cả entities:

- ✅ Tất cả entities kế thừa `BaseEntity` có `IsDeleted` flag
- ✅ Mặc định, các repository chỉ query entities chưa bị xóa (`IsDeleted = false`)
- ✅ Khi xóa, set `IsDeleted = true` thay vì xóa vĩnh viễn
- ✅ Các validation và business logic phải kiểm tra `IsDeleted` khi cần:
  - TeamMembers
  - Employees
  - DailySchedules
  - TeamWorkShifts
  - WorkShifts
  - Slots
  - Tasks

---

### 10. Background Jobs (Hangfire)

Các tác vụ được thực hiện trong background:

- ✅ Tạo `DailySchedule` cho team members khi assign work shift
- ✅ Tạo `DailyTasks` từ slots khi tạo/cập nhật slot
- ✅ Auto assign tasks cho tuần mới
- ✅ Tự động thay đổi trạng thái tasks

#### Retry Policy

- Tạo `DailySchedule` và `DailyTasks`: Có retry (mặc định 3 lần)
- Các job khác: Tùy theo yêu cầu

---

### 11. Data Consistency Rules

#### 11.1. Referential Integrity

- ✅ Khi xóa Team → Xóa DailySchedule liên quan
- ✅ Khi xóa TeamMember → Xóa DailySchedule của member đó
- ✅ Khi xóa TeamWorkShift → Xóa DailySchedule liên quan đến work shift đó
- ✅ Khi xóa WorkShift → Xóa TeamWorkShift và DailySchedule liên quan

#### 11.2. Cascade Operations

- Xóa Team → Soft delete TeamMembers → Soft delete DailySchedules
- Xóa TeamMember → Soft delete DailySchedules
- Xóa TeamWorkShift → Soft delete DailySchedules

---

## 📊 Kiến trúc dữ liệu chính

### Core Entities

- **Team**: Nhóm nhân viên
- **TeamMember**: Thành viên trong team
- **WorkShift**: Ca làm việc
- **TeamWorkShift**: Liên kết team và work shift
- **DailySchedule**: Lịch làm việc hàng ngày của nhân viên
- **Slot**: Khung giờ dịch vụ
- **Task**: Nhiệm vụ
- **DailyTask**: Nhiệm vụ hàng ngày
- **CustomerBooking**: Đặt lịch của khách hàng
- **Pet**: Thú cưng
- **PetGroup**: Nhóm thú cưng
- **VaccinationSchedule**: Lịch tiêm chủng
- **Order**: Đơn hàng

---

## 🔒 Security & Validation

- ✅ Input validation: Kiểm tra duplicate IDs, null values, empty collections
- ✅ Business validation: Kiểm tra trạng thái active, trùng lịch, capacity
- ✅ Soft delete validation: Luôn filter `IsDeleted = false` trong queries
- ✅ Time validation: Kiểm tra thời gian hợp lệ (StartTime < EndTime)
- ✅ Date validation: Kiểm tra ngày áp dụng của work shifts và slots

---

## 📝 Notes

- Tất cả thời gian sử dụng UTC
- Tuần được tính từ Thứ 2 đến Chủ nhật
- Background jobs được xử lý bất đồng bộ để không block API responses
- Hệ thống hỗ trợ recurring và non-recurring slots/tasks

---

## 🚀 Tính năng nổi bật

1. **Tự động hóa lịch trình**: Tự động tạo lịch làm việc và nhiệm vụ từ templates
2. **Quản lý ca làm việc linh hoạt**: Hỗ trợ nhiều ca làm việc cho mỗi team với validation chặt chẽ
3. **Phòng chống trùng lịch**: Tự động kiểm tra và ngăn chặn trùng lịch giữa các team và nhân viên
4. **Quản lý capacity**: Kiểm tra và giới hạn số lượng booking cho mỗi slot
5. **Background processing**: Xử lý các tác vụ nặng trong background để tối ưu performance
