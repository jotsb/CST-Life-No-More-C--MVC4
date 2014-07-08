using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class QuestMap : EntityTypeConfiguration<Quest>
    {
        public QuestMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.description)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Quest");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.num_nodes).HasColumnName("num_nodes");
        }
    }
}
