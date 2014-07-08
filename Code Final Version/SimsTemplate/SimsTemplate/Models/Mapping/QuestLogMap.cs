using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class QuestLogMap : EntityTypeConfiguration<QuestLog>
    {
        public QuestLogMap()
        {
            // Primary Key
            this.HasKey(t => new { t.char_id, t.quest_id });

            // Properties
            this.Property(t => t.char_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.quest_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("QuestLog");
            this.Property(t => t.char_id).HasColumnName("char_id");
            this.Property(t => t.quest_id).HasColumnName("quest_id");
            this.Property(t => t.current_node).HasColumnName("current_node");
            this.Property(t => t.accumulated_value).HasColumnName("accumulated_value");

            // Relationships
            this.HasRequired(t => t.Character)
                .WithMany(t => t.QuestLogs)
                .HasForeignKey(d => d.char_id);
            this.HasRequired(t => t.Quest)
                .WithMany(t => t.QuestLogs)
                .HasForeignKey(d => d.quest_id);

        }
    }
}
