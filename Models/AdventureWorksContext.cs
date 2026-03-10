using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NetSQLAPIActions.Models;

public partial class AdventureWorksContext : DbContext
{
    public AdventureWorksContext()
    {
    }

    public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SalesPerson> SalesPeople { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=AdventureWorks;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesPerson>(entity =>
        {
            entity.HasKey(e => e.BusinessEntityId).HasName("PK_SalesPerson_BusinessEntityID");

            entity.ToTable("SalesPerson", "Sales", tb => tb.HasComment("Sales representative current information."));

            entity.HasIndex(e => e.Rowguid, "AK_SalesPerson_rowguid").IsUnique();

            entity.Property(e => e.BusinessEntityId)
                .ValueGeneratedNever()
                .HasComment("Primary key for SalesPerson records. Foreign key to Employee.BusinessEntityID")
                .HasColumnName("BusinessEntityID");
            entity.Property(e => e.Bonus)
                .HasComment("Bonus due if quota is met.")
                .HasColumnType("money");
            entity.Property(e => e.CommissionPct)
                .HasComment("Commision percent received per sale.")
                .HasColumnType("smallmoney");
            entity.Property(e => e.ModifiedDate)
                .HasComment("Date and time the record was last updated.")
                .HasDefaultValueSql("(getdate())", "DF_SalesPerson_ModifiedDate")
                .HasColumnType("datetime");
            entity.Property(e => e.Rowguid)
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.")
                .HasDefaultValueSql("(newid())", "DF_SalesPerson_rowguid")
                .HasColumnName("rowguid");
            entity.Property(e => e.SalesLastYear)
                .HasComment("Sales total of previous year.")
                .HasColumnType("money");
            entity.Property(e => e.SalesQuota)
                .HasComment("Projected yearly sales.")
                .HasColumnType("money");
            entity.Property(e => e.SalesYtd)
                .HasComment("Sales total year to date.")
                .HasColumnType("money")
                .HasColumnName("SalesYTD");
            entity.Property(e => e.TerritoryId)
                .HasComment("Territory currently assigned to. Foreign key to SalesTerritory.SalesTerritoryID.")
                .HasColumnName("TerritoryID");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.BusinessEntityId).HasName("PK_Store_BusinessEntityID");

            entity.ToTable("Store", "Sales", tb => tb.HasComment("Customers (resellers) of Adventure Works products."));

            entity.HasIndex(e => e.Rowguid, "AK_Store_rowguid").IsUnique();

            entity.HasIndex(e => e.SalesPersonId, "IX_Store_SalesPersonID");

            entity.HasIndex(e => e.Demographics, "PXML_Store_Demographics");

            entity.Property(e => e.BusinessEntityId)
                .ValueGeneratedNever()
                .HasComment("Primary key. Foreign key to Customer.BusinessEntityID.")
                .HasColumnName("BusinessEntityID");
            entity.Property(e => e.Demographics)
                .HasComment("Demographic informationg about the store such as the number of employees, annual sales and store type.")
                .HasColumnType("xml");
            entity.Property(e => e.ModifiedDate)
                .HasComment("Date and time the record was last updated.")
                .HasDefaultValueSql("(getdate())", "DF_Store_ModifiedDate")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("Name of the store.");
            entity.Property(e => e.Rowguid)
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.")
                .HasDefaultValueSql("(newid())", "DF_Store_rowguid")
                .HasColumnName("rowguid");
            entity.Property(e => e.SalesPersonId)
                .HasComment("ID of the sales person assigned to the customer. Foreign key to SalesPerson.BusinessEntityID.")
                .HasColumnName("SalesPersonID");

            entity.HasOne(d => d.SalesPerson).WithMany(p => p.Stores).HasForeignKey(d => d.SalesPersonId);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.BusinessEntityId).HasName("PK_Vendor_BusinessEntityID");

            entity.ToTable("Vendor", "Purchasing", tb =>
                {
                    tb.HasComment("Companies from whom Adventure Works Cycles purchases parts or other goods.");
                    tb.HasTrigger("dVendor");
                });

            entity.HasIndex(e => e.AccountNumber, "AK_Vendor_AccountNumber").IsUnique();

            entity.Property(e => e.BusinessEntityId)
                .ValueGeneratedNever()
                .HasComment("Primary key for Vendor records.  Foreign key to BusinessEntity.BusinessEntityID")
                .HasColumnName("BusinessEntityID");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(15)
                .HasComment("Vendor account (identification) number.");
            entity.Property(e => e.ActiveFlag)
                .HasComment("0 = Vendor no longer used. 1 = Vendor is actively used.")
                .HasDefaultValue(true, "DF_Vendor_ActiveFlag");
            entity.Property(e => e.CreditRating).HasComment("1 = Superior, 2 = Excellent, 3 = Above average, 4 = Average, 5 = Below average");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())", "DF_Vendor_ModifiedDate")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("Company name.");
            entity.Property(e => e.PreferredVendorStatus)
                .HasComment("0 = Do not use if another vendor is available. 1 = Preferred over other vendors supplying the same product.")
                .HasDefaultValue(true, "DF_Vendor_PreferredVendorStatus");
            entity.Property(e => e.PurchasingWebServiceUrl)
                .HasMaxLength(1024)
                .HasColumnName("PurchasingWebServiceURL");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
