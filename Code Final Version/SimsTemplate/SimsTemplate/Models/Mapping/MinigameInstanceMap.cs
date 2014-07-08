using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class MinigameInstanceMap : EntityTypeConfiguration<MinigameInstance>
    {
        public MinigameInstanceMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("MinigameInstance");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.character_id).HasColumnName("character_id");
            this.Property(t => t.minigame_id).HasColumnName("minigame_id");
            this.Property(t => t.score).HasColumnName("score");

            // Relationships
            this.HasRequired(t => t.Character)
                .WithMany(t => t.MinigameInstances)
                .HasForeignKey(d => d.character_id);
            this.HasRequired(t => t.Minigame)
                .WithMany(t => t.MinigameInstances)
                .HasForeignKey(d => d.minigame_id);

        }
    }
}
