using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class ForumThreadMap : EntityTypeConfiguration<ForumThread>
    {
        public ForumThreadMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.title)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ForumThread");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.subforum_id).HasColumnName("subforum_id");
            this.Property(t => t.author_id).HasColumnName("author_id");
            this.Property(t => t.title).HasColumnName("title");
            this.Property(t => t.num_hits).HasColumnName("num_hits");
            this.Property(t => t.datetime_posted).HasColumnName("datetime_posted");

            // Relationships
            this.HasRequired(t => t.Subforum)
                .WithMany(t => t.ForumThreads)
                .HasForeignKey(d => d.subforum_id);
            this.HasRequired(t => t.User)
                .WithMany(t => t.ForumThreads)
                .HasForeignKey(d => d.author_id);

        }
    }
}
