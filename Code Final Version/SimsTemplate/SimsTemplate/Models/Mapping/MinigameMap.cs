using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class MinigameMap : EntityTypeConfiguration<Minigame>
    {
        public MinigameMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.title)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Minigame");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.title).HasColumnName("title");
        }
    }
}
