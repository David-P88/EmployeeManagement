using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x=> x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x=> x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Salary)
                .HasPrecision(18, 2);

            builder.HasIndex(x => x.EmployeeCode)
                .IsUnique();

            builder.HasIndex(x => x.Email);

            builder.HasOne(x=> x.Department)
                .WithMany(x=> x.Employees)
                .HasForeignKey(x=> x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
                
        }
    }
}
