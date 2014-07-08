using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class JobMap : EntityTypeConfiguration<Job>
    {
        public JobMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.value)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Job");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.value).HasColumnName("value");
        }
    }
}
