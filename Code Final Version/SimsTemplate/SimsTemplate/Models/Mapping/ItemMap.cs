using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class ItemMap : EntityTypeConfiguration<Item>
    {
        public ItemMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.title)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.money_boost)
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Item");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.title).HasColumnName("title");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.energy_boost).HasColumnName("energy_boost");
            this.Property(t => t.hunger_boost).HasColumnName("hunger_boost");
            this.Property(t => t.sanity_boost).HasColumnName("sanity_boost");
            this.Property(t => t.fun_boost).HasColumnName("fun_boost");
            this.Property(t => t.bladder_boost).HasColumnName("bladder_boost");
            this.Property(t => t.money_boost).HasColumnName("money_boost");
        }
    }
}
