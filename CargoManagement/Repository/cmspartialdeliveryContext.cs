using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CargoManagement.Repository
{
    public partial class cmspartialdeliveryContext : DbContext
    {
        public cmspartialdeliveryContext(DbContextOptions<cmspartialdeliveryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Bookingfile> Bookingfiles { get; set; } = null!;
        public virtual DbSet<Bookingitem> Bookingitems { get; set; } = null!;
        public virtual DbSet<Bookingitemsdistribution> Bookingitemsdistributions { get; set; } = null!;
        public virtual DbSet<Bookingpayment> Bookingpayments { get; set; } = null!;
        public virtual DbSet<Bookingstatus> Bookingstatuses { get; set; } = null!;
        public virtual DbSet<Bookingtransaction> Bookingtransactions { get; set; } = null!;
        public virtual DbSet<Boxtype> Boxtypes { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Customeraddress> Customeraddresses { get; set; } = null!;
        public virtual DbSet<Customerdiscount> Customerdiscounts { get; set; } = null!;
        public virtual DbSet<Customertransaction> Customertransactions { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Exceptionlog> Exceptionlogs { get; set; } = null!;
        public virtual DbSet<Expensetype> Expensetypes { get; set; } = null!;
        public virtual DbSet<Hub> Hubs { get; set; } = null!;
        public virtual DbSet<Hubtype> Hubtypes { get; set; } = null!;
        public virtual DbSet<Journey> Journeys { get; set; } = null!;
        public virtual DbSet<Journeyexpense> Journeyexpenses { get; set; } = null!;
        public virtual DbSet<Journeyitem> Journeyitems { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Shipmentmode> Shipmentmodes { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Userrole> Userroles { get; set; } = null!;
        public virtual DbSet<Usertype> Usertypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=Doitnew*1;database=cmspartialdelivery", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");

                entity.HasIndex(e => e.BookingId, "BookingId")
                    .IsUnique();

                entity.HasIndex(e => e.CurrentHubId, "CurrentHubId");

                entity.HasIndex(e => e.CustomerAddressId, "CustomerAddressId");

                entity.HasIndex(e => e.CustomerId, "CustomerId");

                entity.HasIndex(e => e.JourneyId, "FK_JourneyId");

                entity.HasIndex(e => e.NextHubId, "NextHubId");

                entity.HasIndex(e => e.OriginHubId, "OriginHubId");

                entity.HasIndex(e => e.ReceipientCustomerAddressId, "ReceipientCustomerAddressId");

                entity.HasIndex(e => e.ReceipientCustomerId, "ReceipientCustomerId");

                entity.HasIndex(e => e.StatusId, "StatusId");

                entity.HasIndex(e => e.ShipmentMode, "booking_fkshipment_idx");

                entity.Property(e => e.BookingId).HasMaxLength(100);

                entity.Property(e => e.ClosingRemarks).HasMaxLength(200);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.FreightCharges).HasPrecision(18, 2);

                entity.Property(e => e.HandlingCharges).HasPrecision(18, 2);

                entity.Property(e => e.InvoiceNumber).HasMaxLength(45);

                entity.Property(e => e.IsCash).HasDefaultValueSql("'1'");

                entity.Property(e => e.IsClosed).HasDefaultValueSql("false");

                entity.Property(e => e.IsEmailNotificationSent).HasDefaultValueSql("'1'");

                entity.Property(e => e.NetAmnount).HasPrecision(18, 2);

                entity.Property(e => e.Notes).HasMaxLength(200);

                entity.Property(e => e.PaidDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentMode)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.PaymentRemarks).HasMaxLength(100);

                entity.Property(e => e.RoundOffAmnount).HasPrecision(18, 2);

                entity.Property(e => e.ShipmentMode).HasMaxLength(2);

                entity.Property(e => e.StatusId)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                entity.Property(e => e.TotalAmountReceived)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.TotalDiscountGiven)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CurrentHub)
                    .WithMany(p => p.BookingCurrentHubs)
                    .HasForeignKey(d => d.CurrentHubId)
                    .HasConstraintName("booking_ibfk_7");

                entity.HasOne(d => d.CustomerAddress)
                    .WithMany(p => p.BookingCustomerAddresses)
                    .HasForeignKey(d => d.CustomerAddressId)
                    .HasConstraintName("booking_ibfk_2");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BookingCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("booking_ibfk_1");

                entity.HasOne(d => d.Journey)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.JourneyId)
                    .HasConstraintName("FK_JourneyId");

                entity.HasOne(d => d.NextHub)
                    .WithMany(p => p.BookingNextHubs)
                    .HasForeignKey(d => d.NextHubId)
                    .HasConstraintName("booking_ibfk_6");

                entity.HasOne(d => d.OriginHub)
                    .WithMany(p => p.BookingOriginHubs)
                    .HasForeignKey(d => d.OriginHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("booking_ibfk_5");

                entity.HasOne(d => d.ReceipientCustomerAddress)
                    .WithMany(p => p.BookingReceipientCustomerAddresses)
                    .HasForeignKey(d => d.ReceipientCustomerAddressId)
                    .HasConstraintName("booking_ibfk_4");

                entity.HasOne(d => d.ReceipientCustomer)
                    .WithMany(p => p.BookingReceipientCustomers)
                    .HasForeignKey(d => d.ReceipientCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("booking_ibfk_3");

                entity.HasOne(d => d.ShipmentModeNavigation)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ShipmentMode)
                    .HasConstraintName("booking_fkshipment");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("booking_ibfk_8");
            });

            modelBuilder.Entity<Bookingfile>(entity =>
            {
                entity.ToTable("bookingfiles");

                entity.HasIndex(e => e.BookingId, "BookingId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.FileName).HasMaxLength(100);

                entity.Property(e => e.Remarks).HasMaxLength(100);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Bookingfiles)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookingfiles_ibfk_1");
            });

            modelBuilder.Entity<Bookingitem>(entity =>
            {
                entity.ToTable("bookingitems");

                entity.HasIndex(e => e.BookingId, "BookingId");

                entity.HasIndex(e => e.BoxTypeId, "BoxTypeId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Bookingitems)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookingitems_ibfk_1");

                entity.HasOne(d => d.BoxType)
                    .WithMany(p => p.Bookingitems)
                    .HasForeignKey(d => d.BoxTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookingitems_ibfk_2");
            });

            modelBuilder.Entity<Bookingitemsdistribution>(entity =>
            {
                entity.ToTable("bookingitemsdistribution");

                entity.HasIndex(e => e.BookingItemId, "BookingItemId");

                entity.HasIndex(e => e.HubId, "HubId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.BookingItem)
                    .WithMany(p => p.Bookingitemsdistributions)
                    .HasForeignKey(d => d.BookingItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BookingItemId");

                entity.HasOne(d => d.Hub)
                    .WithMany(p => p.Bookingitemsdistributions)
                    .HasForeignKey(d => d.HubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HubId");
            });

            modelBuilder.Entity<Bookingpayment>(entity =>
            {
                entity.ToTable("bookingpayment");

                entity.HasIndex(e => e.BookingId, "BookingId");

                entity.HasIndex(e => e.BookingItemId, "BookingItemId");

                entity.HasIndex(e => e.JourneyId, "JourneyId");

                entity.HasIndex(e => e.JourneyItemId, "JourneyItemId");

                entity.HasIndex(e => e.Paidby, "paidby_fk5");

                entity.Property(e => e.AdditionalCharge)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Discount)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.IsPaymentCompleted).HasDefaultValueSql("'0'");

                entity.Property(e => e.Paidby).HasColumnName("paidby");

                entity.Property(e => e.Paiddate)
                    .HasColumnType("datetime")
                    .HasColumnName("paiddate");

                entity.Property(e => e.PaymentMode)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.TotalAmountPaid).HasPrecision(18, 2);

                entity.Property(e => e.TotalAmountToPay).HasPrecision(18, 2);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Bookingpayments)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("booking_p_fk1");

                entity.HasOne(d => d.BookingItem)
                    .WithMany(p => p.Bookingpayments)
                    .HasForeignKey(d => d.BookingItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookingitems_p_fk2");

                entity.HasOne(d => d.Journey)
                    .WithMany(p => p.Bookingpayments)
                    .HasForeignKey(d => d.JourneyId)
                    .HasConstraintName("journey_p_fk3");

                entity.HasOne(d => d.JourneyItem)
                    .WithMany(p => p.Bookingpayments)
                    .HasForeignKey(d => d.JourneyItemId)
                    .HasConstraintName("journeyItems_p_fk4");

                entity.HasOne(d => d.PaidbyNavigation)
                    .WithMany(p => p.Bookingpayments)
                    .HasForeignKey(d => d.Paidby)
                    .HasConstraintName("paidby_fk5");
            });

            modelBuilder.Entity<Bookingstatus>(entity =>
            {
                entity.ToTable("bookingstatus");

                entity.Property(e => e.Id)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(25);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Bookingtransaction>(entity =>
            {
                entity.ToTable("bookingtransactions");

                entity.HasIndex(e => e.BookingId, "BookingId");

                entity.HasIndex(e => e.CurrentHubId, "CurrentHubId");

                entity.HasIndex(e => e.JourneyId, "FK_JourneyId");

                entity.HasIndex(e => e.NextHubId, "NextHubId");

                entity.HasIndex(e => e.OriginHubId, "OriginHubId");

                entity.HasIndex(e => e.StatusId, "StatusId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.FileName).HasMaxLength(100);

                entity.Property(e => e.Remarks).HasMaxLength(100);

                entity.Property(e => e.StatusId)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Bookingtransactions)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BookingTransactions_ibfk_1");

                entity.HasOne(d => d.CurrentHub)
                    .WithMany(p => p.BookingtransactionCurrentHubs)
                    .HasForeignKey(d => d.CurrentHubId)
                    .HasConstraintName("BookingTransactions_ibfk_7");

                entity.HasOne(d => d.Journey)
                    .WithMany(p => p.Bookingtransactions)
                    .HasForeignKey(d => d.JourneyId)
                    .HasConstraintName("BookingTransactions_FK_JourneyId");

                entity.HasOne(d => d.NextHub)
                    .WithMany(p => p.BookingtransactionNextHubs)
                    .HasForeignKey(d => d.NextHubId)
                    .HasConstraintName("BookingTransactions_ibfk_6");

                entity.HasOne(d => d.OriginHub)
                    .WithMany(p => p.BookingtransactionOriginHubs)
                    .HasForeignKey(d => d.OriginHubId)
                    .HasConstraintName("BookingTransactions_ibfk_5");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Bookingtransactions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BookingTransactions_ibfk_8");
            });

            modelBuilder.Entity<Boxtype>(entity =>
            {
                entity.ToTable("boxtype");

                entity.HasIndex(e => e.Name, "Name")
                    .IsUnique();

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.HasIndex(e => e.HubId, "HubId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.CreditLimit)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.IsCreditAllowed).HasDefaultValueSql("'0'");

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.OutstandingCredit)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Hub)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.HubId)
                    .HasConstraintName("customer_ibfk_1");
            });

            modelBuilder.Entity<Customeraddress>(entity =>
            {
                entity.ToTable("customeraddresses");

                entity.HasIndex(e => e.CustomerId, "CustomerId");

                entity.HasIndex(e => e.DistrictId, "DistrictId");

                entity.HasIndex(e => e.LocationId, "LocationId");

                entity.HasIndex(e => e.StateId, "StateId");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Landmark).HasMaxLength(200);

                entity.Property(e => e.Mobile).HasMaxLength(15);

                entity.Property(e => e.Pincode)
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Customeraddresses)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("customeraddresses_ibfk_4");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Customeraddresses)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("customeraddresses_ibfk_2");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Customeraddresses)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("customeraddresses_ibfk_3");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Customeraddresses)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("customeraddresses_ibfk_1");
            });

            modelBuilder.Entity<Customerdiscount>(entity =>
            {
                entity.ToTable("customerdiscounts");

                entity.HasIndex(e => e.CustomerId, "CustomerId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);

                entity.Property(e => e.DiscountBookingIds).HasColumnType("text");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Customerdiscounts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CustomerId_fk1");
            });

            modelBuilder.Entity<Customertransaction>(entity =>
            {
                entity.ToTable("customertransactions");

                entity.HasIndex(e => e.CustomerId, "CustomerId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.NewAmount).HasPrecision(18, 2);

                entity.Property(e => e.PreviousAmount).HasPrecision(18, 2);

                entity.Property(e => e.TransactionAmount).HasPrecision(18, 2);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Customertransactions)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("customertransactions_ibfk_1");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("district");

                entity.HasIndex(e => e.StateId, "StateID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("district_ibfk_1");
            });

            modelBuilder.Entity<Exceptionlog>(entity =>
            {
                entity.ToTable("exceptionlogs");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ControllerName)
                    .HasMaxLength(45)
                    .HasDefaultValueSql("'null'");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FunctionName)
                    .HasMaxLength(45)
                    .HasDefaultValueSql("'null'");
            });

            modelBuilder.Entity<Expensetype>(entity =>
            {
                entity.ToTable("expensetype");

                entity.HasIndex(e => e.Name, "Name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Hub>(entity =>
            {
                entity.ToTable("hub");

                entity.HasIndex(e => e.DistrictId, "DistrictId");

                entity.HasIndex(e => e.HubTypeId, "HubTypeId");

                entity.HasIndex(e => e.LocationId, "LocationId");

                entity.HasIndex(e => e.StateId, "StateId");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Pincode)
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Hubs)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("hub_ibfk_2");

                entity.HasOne(d => d.HubType)
                    .WithMany(p => p.Hubs)
                    .HasForeignKey(d => d.HubTypeId)
                    .HasConstraintName("hub_ibfk_4");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Hubs)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("hub_ibfk_3");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Hubs)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("hub_ibfk_1");
            });

            modelBuilder.Entity<Hubtype>(entity =>
            {
                entity.ToTable("hubtype");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Journey>(entity =>
            {
                entity.ToTable("journey");

                entity.HasIndex(e => e.DestinationHubId, "DestinationHubId");

                entity.HasIndex(e => e.OriginHubId, "OriginHubId");

                entity.HasIndex(e => e.CreatorHubId, "journey_ibfk_4");

                entity.Property(e => e.ContainerId).HasMaxLength(45);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.CreatorHubId).HasDefaultValueSql("'2'");

                entity.Property(e => e.DateOfJourney).HasColumnType("datetime");

                entity.Property(e => e.IsLocal)
                    .IsRequired()
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Notes).HasMaxLength(200);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatorHub)
                    .WithMany(p => p.JourneyCreatorHubs)
                    .HasForeignKey(d => d.CreatorHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journey_ibfk_4");

                entity.HasOne(d => d.DestinationHub)
                    .WithMany(p => p.JourneyDestinationHubs)
                    .HasForeignKey(d => d.DestinationHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journey_ibfk_2");

                entity.HasOne(d => d.OriginHub)
                    .WithMany(p => p.JourneyOriginHubs)
                    .HasForeignKey(d => d.OriginHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journey_ibfk_1");
            });

            modelBuilder.Entity<Journeyexpense>(entity =>
            {
                entity.ToTable("journeyexpenses");

                entity.HasIndex(e => e.ExpenseTypeId, "ExpenseTypeId");

                entity.HasIndex(e => e.JourneyId, "JourneyId");

                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Notes).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.ExpenseType)
                    .WithMany(p => p.Journeyexpenses)
                    .HasForeignKey(d => d.ExpenseTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journeyexpenses_ibfk_2");

                entity.HasOne(d => d.Journey)
                    .WithMany(p => p.Journeyexpenses)
                    .HasForeignKey(d => d.JourneyId)
                    .HasConstraintName("journeyexpenses_ibfk_1");
            });

            modelBuilder.Entity<Journeyitem>(entity =>
            {
                entity.ToTable("journeyitems");

                entity.HasIndex(e => e.DestinationHubId, "DestinationHubId");

                entity.HasIndex(e => e.JourneyId, "JourneyId");

                entity.HasIndex(e => e.OriginHubId, "OriginHubId");

                entity.HasIndex(e => e.BookingItemId, "journeydetails_ibfk_4");

                entity.HasIndex(e => e.ItemDistributionId, "journeydetails_ibfk_5");

                entity.Property(e => e.Action)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Notes).HasMaxLength(200);

                entity.Property(e => e.PaidAmount).HasPrecision(18, 2);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentMode).HasMaxLength(5);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.BookingItem)
                    .WithMany(p => p.Journeyitems)
                    .HasForeignKey(d => d.BookingItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journeydetails_ibfk_4");

                entity.HasOne(d => d.DestinationHub)
                    .WithMany(p => p.JourneyitemDestinationHubs)
                    .HasForeignKey(d => d.DestinationHubId)
                    .HasConstraintName("journeydetails_ibfk_2");

                entity.HasOne(d => d.ItemDistribution)
                    .WithMany(p => p.Journeyitems)
                    .HasForeignKey(d => d.ItemDistributionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journeydetails_ibfk_5");

                entity.HasOne(d => d.Journey)
                    .WithMany(p => p.Journeyitems)
                    .HasForeignKey(d => d.JourneyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journeydetails_ibfk_3");

                entity.HasOne(d => d.OriginHub)
                    .WithMany(p => p.JourneyitemOriginHubs)
                    .HasForeignKey(d => d.OriginHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("journeydetails_ibfk_1");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("location");

                entity.HasIndex(e => e.DistrictId, "DistrictId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Pincode)
                    .HasMaxLength(6)
                    .HasColumnName("pincode")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("location_ibfk_1");
            });

            modelBuilder.Entity<Shipmentmode>(entity =>
            {
                entity.HasKey(e => e.ShipmentId)
                    .HasName("PRIMARY");

                entity.ToTable("shipmentmode");

                entity.Property(e => e.ShipmentId).HasMaxLength(2);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ShipmentName).HasMaxLength(45);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("state");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.UserId, "UserId")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "Username")
                    .IsUnique();

                entity.HasIndex(e => e.HubId, "user_hub_id-fk");

                entity.HasIndex(e => e.UserTypeId, "user_ibfk_1");

                entity.HasIndex(e => e.UserRoleId, "user_infkrole_idx");

                entity.Property(e => e.AlternativeMobile).HasMaxLength(15);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Email).HasMaxLength(30);

                entity.Property(e => e.Image).HasMaxLength(100);

                entity.Property(e => e.Mobile).HasMaxLength(15);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(30);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(30);

                entity.HasOne(d => d.Hub)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.HubId)
                    .HasConstraintName("user_hub_id-fk");

                entity.HasOne(d => d.UserRole)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserRoleId)
                    .HasConstraintName("user_infkrole");

                entity.HasOne(d => d.UserType)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserTypeId)
                    .HasConstraintName("user_ibfk_1");
            });

            modelBuilder.Entity<Userrole>(entity =>
            {
                entity.ToTable("userroles");

                entity.HasIndex(e => e.Id, "Id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserRoleName, "UserRoleName_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserRoleName).HasMaxLength(45);
            });

            modelBuilder.Entity<Usertype>(entity =>
            {
                entity.ToTable("usertype");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("'1'");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
