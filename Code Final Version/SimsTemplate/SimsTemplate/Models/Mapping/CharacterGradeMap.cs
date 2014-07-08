using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class CharacterGradeMap : EntityTypeConfiguration<CharacterGrade>
    {
        public CharacterGradeMap()
        {
            // Primary Key
            this.HasKey(t => t.char_id);

            // Properties
            this.Property(t => t.char_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("CharacterGrades");
            this.Property(t => t.char_id).HasColumnName("char_id");
            this.Property(t => t.programming_methods).HasColumnName("programming_methods");
            this.Property(t => t.web_development).HasColumnName("web_development");
            this.Property(t => t.discrete_mathematics).HasColumnName("discrete_mathematics");
            this.Property(t => t.business_communications).HasColumnName("business_communications");

            // Relationships
            this.HasRequired(t => t.Character)
                .WithOptional(t => t.CharacterGrade);

        }
    }
}
