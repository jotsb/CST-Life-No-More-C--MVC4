using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class CharacterMap : EntityTypeConfiguration<Character>
    {
        public CharacterMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.sex)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.position)
                .HasMaxLength(50);

            this.Property(t => t.datetime_ingame)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Character");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.user_id).HasColumnName("user_id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.sex).HasColumnName("sex");
            this.Property(t => t.grades).HasColumnName("grades");
            this.Property(t => t.money).HasColumnName("money");
            this.Property(t => t.position).HasColumnName("position");
            this.Property(t => t.inventory).HasColumnName("inventory");
            this.Property(t => t.hunger).HasColumnName("hunger");
            this.Property(t => t.sanity).HasColumnName("sanity");
            this.Property(t => t.fun).HasColumnName("fun");
            this.Property(t => t.energy).HasColumnName("energy");
            this.Property(t => t.bladder).HasColumnName("bladder");
            this.Property(t => t.global_score).HasColumnName("global_score");
            this.Property(t => t.datetime_ingame).HasColumnName("datetime_ingame");
            this.Property(t => t.business_level).HasColumnName("business_level");
            this.Property(t => t.job_level).HasColumnName("job_level");

            // Relationships
            this.HasMany(t => t.Items)
                .WithMany(t => t.Characters)
                .Map(m =>
                    {
                        m.ToTable("CharacterItem");
                        m.MapLeftKey("char_id");
                        m.MapRightKey("item_id");
                    });

            this.HasOptional(t => t.Business)
                .WithMany(t => t.Characters)
                .HasForeignKey(d => d.business_level);
            this.HasOptional(t => t.Job)
                .WithMany(t => t.Characters)
                .HasForeignKey(d => d.job_level);
            this.HasRequired(t => t.User)
                .WithMany(t => t.Characters)
                .HasForeignKey(d => d.user_id);

        }
    }
}
