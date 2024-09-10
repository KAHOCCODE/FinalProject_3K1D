using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_3K1D.Models;

public partial class QlrapPhimContext : DbContext
{
    public QlrapPhimContext()
    {
    }

    public QlrapPhimContext(DbContextOptions<QlrapPhimContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChucVu> ChucVus { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LichChieu> LichChieus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Phim> Phims { get; set; }

    public virtual DbSet<PhongChieu> PhongChieus { get; set; }

    public virtual DbSet<Rap> Raps { get; set; }

    public virtual DbSet<TheLoai> TheLoais { get; set; }

    public virtual DbSet<Ve> Ves { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
    public DbSet<Order> Orders { get; set; }
    public virtual DbSet<DanhGia> DanhGias { get; set; }
    public virtual DbSet<GiaHoan> GiaHoans { get; set; }
    public virtual DbSet<Food> Foods { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-0U55SOV\\SQLEXPRESS;Initial Catalog=QLRapPhim;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChucVu>(entity =>
        {
            entity.HasKey(e => e.IdChucVu);

            entity.ToTable("ChucVu");

            entity.Property(e => e.IdChucVu)
                .ValueGeneratedNever()
                .HasColumnName("idChucVu");
            entity.Property(e => e.TenChucVu)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.IdKhachHang);

            entity.ToTable("KhachHang");

            entity.Property(e => e.IdKhachHang)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idKhachHang");
            entity.Property(e => e.Cccd)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HoTen)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.PassKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PassKH");
            entity.Property(e => e.Sdt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.UserKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserKH");
        });

        modelBuilder.Entity<LichChieu>(entity =>
        {
            entity.HasKey(e => e.IdLichChieu).HasName("PK__LichChie__A56F50C2523BC244");

            entity.ToTable("LichChieu");

            entity.Property(e => e.IdLichChieu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idLichChieu");
            entity.Property(e => e.GiaVe).HasColumnType("money");
            entity.Property(e => e.GioChieu).HasColumnType("datetime");
            entity.Property(e => e.IdPhim)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idPhim");
            entity.Property(e => e.IdPhongChieu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idPhongChieu");

            entity.HasOne(d => d.IdPhimNavigation).WithMany(p => p.LichChieus)
                .HasForeignKey(d => d.IdPhim)
                .HasConstraintName("FK_LichChieu_Phim");

            entity.HasOne(d => d.IdPhongChieuNavigation).WithMany(p => p.LichChieus)
                .HasForeignKey(d => d.IdPhongChieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichChieu__idPho__4AB81AF0");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.IdNhanVien).HasName("PK_NhanV");

            entity.ToTable("NhanVien");

            entity.Property(e => e.IdNhanVien)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idNhanVien");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HoTen)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdChucVu).HasColumnName("idChucVu");
            entity.Property(e => e.IdRap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idRap");
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.PassNv)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PassNV");
            entity.Property(e => e.Sdt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.UserNv)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserNV");

            entity.HasOne(d => d.IdChucVuNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.IdChucVu)
                .HasConstraintName("FK_NhanVien_ChucVu");

            entity.HasOne(d => d.IdRapNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.IdRap)
                .HasConstraintName("FK_NhanVien_Rap");
        });

        modelBuilder.Entity<Phim>(entity =>
        {
            entity.HasKey(e => e.IdPhim).HasName("PK__Phim__BFC6F6835535CAE4");

            entity.ToTable("Phim");

            entity.Property(e => e.IdPhim)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idPhim");
            entity.Property(e => e.ApPhich).HasMaxLength(50);
            entity.Property(e => e.DaoDien).HasMaxLength(100);
            entity.Property(e => e.DinhDangPhim)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(1000);
            entity.Property(e => e.NamSx).HasColumnName("NamSX");
            entity.Property(e => e.QuocGiaSanXuat).HasMaxLength(50);
            entity.Property(e => e.TenPhim).HasMaxLength(100);

            entity.HasMany(d => d.IdTheLoais).WithMany(p => p.IdPhims)
                .UsingEntity<Dictionary<string, object>>(
                    "PhanLoaiPhim",
                    r => r.HasOne<TheLoai>().WithMany()
                        .HasForeignKey("IdTheLoai")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PhanLoaiP__idThe__4D94879B"),
                    l => l.HasOne<Phim>().WithMany()
                        .HasForeignKey("IdPhim")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PhanLoaiP__idPhi__4CA06362"),
                    j =>
                    {
                        j.HasKey("IdPhim", "IdTheLoai");
                        j.ToTable("PhanLoaiPhim");
                        j.IndexerProperty<string>("IdPhim")
                            .HasMaxLength(50)
                            .IsUnicode(false)
                            .HasColumnName("idPhim");
                        j.IndexerProperty<string>("IdTheLoai")
                            .HasMaxLength(50)
                            .IsUnicode(false)
                            .HasColumnName("idTheLoai");
                    });
        });

        modelBuilder.Entity<PhongChieu>(entity =>
        {
            entity.HasKey(e => e.IdPhongChieu).HasName("PK__PhongChi__A985F624FD920998");

            entity.ToTable("PhongChieu");

            entity.Property(e => e.IdPhongChieu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idPhongChieu");
            entity.Property(e => e.IdRap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idRap");
            entity.Property(e => e.TenPhong).HasMaxLength(100);

            entity.HasOne(d => d.IdRapNavigation).WithMany(p => p.PhongChieus)
                .HasForeignKey(d => d.IdRap)
                .HasConstraintName("FK_PhongChieu_Rap");
        });

        modelBuilder.Entity<Rap>(entity =>
        {
            entity.HasKey(e => e.IdRap).HasName("PK_Rap1");

            entity.ToTable("Rap");

            entity.Property(e => e.IdRap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idRap");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TenRap)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TheLoai>(entity =>
        {
            entity.HasKey(e => e.IdTheLoai).HasName("PK__TheLoai__890D7EC86D98E64D");

            entity.ToTable("TheLoai");

            entity.Property(e => e.IdTheLoai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idTheLoai");
            entity.Property(e => e.MoTa).HasMaxLength(100);
            entity.Property(e => e.TenTheLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<Ve>(entity =>
        {
            entity.HasKey(e => e.IdVe).IsClustered(false);

            entity.ToTable("Ve");

            entity.HasIndex(e => e.IdVe, "IX_Ve").IsClustered();

            entity.Property(e => e.IdVe).ValueGeneratedNever();
            entity.Property(e => e.IdKhachHang)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idKhachHang");
            entity.Property(e => e.IdLichChieu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idLichChieu");
            entity.Property(e => e.MaGheNgoi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayMua).HasColumnType("datetime");
            entity.Property(e => e.TienBanVe).HasColumnType("money");

            entity.HasOne(d => d.IdKhachHangNavigation).WithMany(p => p.Ves)
                .HasForeignKey(d => d.IdKhachHang)
                .HasConstraintName("FK_Ve_KhachHang");

            entity.HasOne(d => d.IdLichChieuNavigation).WithMany(p => p.Ves)
                .HasForeignKey(d => d.IdLichChieu)
                .HasConstraintName("FK_Ve_LichChieu");
        });
        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.IdKhuyenMai);

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.IdKhuyenMai)
                .ValueGeneratedNever()
                .HasColumnName("idKhuyenMai");

            entity.Property(e => e.TenKhuyenMai)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.GiaTri)
                .HasColumnType("money");

            entity.Property(e => e.NgayBatDau)
                .HasColumnType("datetime");

            entity.Property(e => e.NgayKetThuc)
                .HasColumnType("datetime");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
