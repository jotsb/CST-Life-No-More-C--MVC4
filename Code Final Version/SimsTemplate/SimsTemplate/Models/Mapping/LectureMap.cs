using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class LectureMap : EntityTypeConfiguration<Lecture>
    {
        public LectureMap()
        {
            // Primary Key
            this.HasKey(t => new { t.char_id, t.minigame_id });

            // Properties
            this.Property(t => t.char_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.minigame_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("Lecture");
            this.Property(t => t.char_id).HasColumnName("char_id");
            this.Property(t => t.minigame_id).HasColumnName("minigame_id");
            this.Property(t => t.lecture_attended).HasColumnName("lecture_attended");
            this.Property(t => t.week).HasColumnName("week");

            // Relationships
            this.HasRequired(t => t.Character)
                .WithMany(t => t.Lectures)
                .HasForeignKey(d => d.char_id);
            this.HasRequired(t => t.Minigame)
                .WithMany(t => t.Lectures)
                .HasForeignKey(d => d.minigame_id);

        }
    }
}
