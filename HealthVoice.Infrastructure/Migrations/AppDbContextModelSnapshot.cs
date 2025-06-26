using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using HealthVoice.Infrastructure.Data;

#nullable disable

namespace HealthVoice.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.6")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("HealthVoice.Domain.Entities.Patient", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uniqueidentifier");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("datetime2");

            b.Property<DateOnly>("DateOfBirth")
                .HasColumnType("date");

            b.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("nvarchar(255)");

            b.Property<string>("FirstName")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            b.Property<string>("LastName")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            b.HasKey("Id");

            b.HasIndex("Email")
                .IsUnique();

            b.ToTable("Patients");
        });
#pragma warning restore 612, 618
    }
} 