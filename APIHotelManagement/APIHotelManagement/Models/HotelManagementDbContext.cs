using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIHotelManagement.Models;

public partial class HotelManagementDbContext : DbContext
{
    public HotelManagementDbContext()
    {
    }

    public HotelManagementDbContext(DbContextOptions<HotelManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationService> ReservationServices { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(localdb)\\MsSqlLocalDb;Integrated Security=true;Trusted_Connection=True;Database=HotelManagementDB;TrustServerCertificate=yes");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            //convert int to string (Enum)
            modelBuilder.Entity<Staff>()
           .Property(g => g.Gender)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<Staff>()
           .Property(g => g.WorkStatus)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<Reservation>()
           .Property(g => g.ReservationStatus)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<Guest>()
           .Property(g => g.IdProofType)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<Payment>()
           .Property(g => g.PaymentStatus)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<Payment>()
           .Property(g => g.PaymentMethod)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<Bill>()
           .Property(g => g.PaymentStatus)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<User>()
           .Property(g => g.UserRole)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            modelBuilder.Entity<Room>()
           .Property(g => g.RoomStatus)
           .HasConversion<string>(); // Store Enum as string in SQL Server

            entity.HasKey(e => e.BillId).HasName("PK__Bill__D706DDB304D6E5F9");

            entity.ToTable("Bill", tb => tb.HasTrigger("trg_CalculateTax"));

            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.BillingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("billing_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payment_status");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.Tax)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("tax");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Bills)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_Reservations");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__C2232422D1576CEA");

            entity.ToTable("Department");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartmentDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("department_description");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("department_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.GuestId).HasName("PK__Guest__19778E3526671A97");

            entity.ToTable("Guest");

            entity.HasIndex(e => e.IdProofNumber, "UQ__Guest__3929920F28800457").IsUnique();

            entity.HasIndex(e => e.ContactNumber, "UQ__Guest__A1D1BF21FF588F87").IsUnique();

            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.ContactNumber).HasColumnName("contact_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GuestName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("guest_name");
            entity.Property(e => e.IdProofNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("id_proof_number");
            entity.Property(e => e.IdProofType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("id_proof_type");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__B59ACC4979022AF2");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.BestBefore)
                .HasDefaultValue(new DateOnly(9999, 12, 31))
                .HasColumnName("best_before");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ItemName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("item_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EA1D617112");

            entity.ToTable("Payment", tb => tb.HasTrigger("trg_CalculatePaymentAmount"));

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payment_status");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_Reservation_Payment");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__31384C29F4C9D89D");

            entity.ToTable("Reservation");

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.CheckIn)
                .HasColumnType("datetime")
                .HasColumnName("check_in");
            entity.Property(e => e.CheckOut)
                .HasColumnType("datetime")
                .HasColumnName("check_out");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.NumberOfGuest).HasColumnName("number_of_guest");
            entity.Property(e => e.ReservationStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("reservation_status");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Guest).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("FK_Guest");

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Room");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Users");
        });

        modelBuilder.Entity<ReservationService>(entity =>
        {
            entity.HasKey(e => new { e.ReservationId, e.ServiceId }).HasName("PK__Reservat__D2D897A3852CCEAB");

            entity.ToTable("ReservationService");

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Reservation).WithMany(p => p.ReservationServices)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_Reservation");

            entity.HasOne(d => d.Service).WithMany(p => p.ReservationServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Services");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__19675A8AD7C232EC");

            entity.ToTable("Room");

            entity.HasIndex(e => e.RoomNumber, "UQ__Room__FE22F61B28E9EB78").IsUnique();

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.RoomNumber).HasColumnName("room_number");
            entity.Property(e => e.RoomStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("room_status");
            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_RoomType");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__RoomType__42395E84A02305C6");

            entity.ToTable("RoomType");

            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.RoomDescription)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("room_description");
            entity.Property(e => e.RoomRate)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("room_rate");
            entity.Property(e => e.RoomTypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("room_type_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__3E0DB8AF962C12DB");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.AvailabilityStatus)
                .HasDefaultValue(true)
                .HasColumnName("availability_status");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ServiceDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("service_description");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("service_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__1963DD9C7CE93910");

            entity.HasIndex(e => e.ContactNumber, "UQ__Staff__A1D1BF214AB19FD4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Staff__AB6E61643DB53CFF").IsUnique();

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.ContactNumber).HasColumnName("contact_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.HireDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("hire_date");
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salary");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.StaffName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("staff_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("work_status");

            entity.HasOne(d => d.Department).WithMany(p => p.Staff)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Department");

            entity.HasOne(d => d.Service).WithMany(p => p.Staff)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Service");

            entity.HasOne(d => d.User).WithMany(p => p.Staff)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F2346E201");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("user_name");
            entity.Property(e => e.UserRole)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
