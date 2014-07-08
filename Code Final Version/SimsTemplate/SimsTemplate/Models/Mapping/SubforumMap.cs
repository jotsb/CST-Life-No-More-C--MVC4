using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class SubforumMap : EntityTypeConfiguration<Subforum>
    {
        public SubforumMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.title)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.user_access)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Subforum");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.title).HasColumnName("title");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.user_access).HasColumnName("user_access");
        }
    }
}
