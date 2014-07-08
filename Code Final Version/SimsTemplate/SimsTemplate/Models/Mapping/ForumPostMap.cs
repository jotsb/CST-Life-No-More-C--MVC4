using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class ForumPostMap : EntityTypeConfiguration<ForumPost>
    {
        public ForumPostMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.text)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ForumPost");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.thread_id).HasColumnName("thread_id");
            this.Property(t => t.author_id).HasColumnName("author_id");
            this.Property(t => t.text).HasColumnName("text");
            this.Property(t => t.datetime_posted).HasColumnName("datetime_posted");

            // Relationships
            this.HasRequired(t => t.ForumThread)
                .WithMany(t => t.ForumPosts)
                .HasForeignKey(d => d.thread_id);

        }
    }
}
