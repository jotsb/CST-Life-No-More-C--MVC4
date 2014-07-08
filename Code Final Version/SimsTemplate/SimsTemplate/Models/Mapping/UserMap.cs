using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.username)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.firstname)
                .HasMaxLength(50);

            this.Property(t => t.lastname)
                .HasMaxLength(50);

            this.Property(t => t.email)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.password)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.role)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.sex)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.location)
                .HasMaxLength(50);

            this.Property(t => t.image_url)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("User");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.username).HasColumnName("username");
            this.Property(t => t.firstname).HasColumnName("firstname");
            this.Property(t => t.lastname).HasColumnName("lastname");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.password).HasColumnName("password");
            this.Property(t => t.role).HasColumnName("role");
            this.Property(t => t.datetime_registered).HasColumnName("datetime_registered");
            this.Property(t => t.age).HasColumnName("age");
            this.Property(t => t.sex).HasColumnName("sex");
            this.Property(t => t.location).HasColumnName("location");
            this.Property(t => t.image_url).HasColumnName("image_url");
            this.Property(t => t.is_banned).HasColumnName("is_banned");
            this.Property(t => t.current_character).HasColumnName("current_character");

            // Relationships
            this.HasOptional(t => t.Character)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.current_character);

        }
    }
}
