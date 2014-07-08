using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class UserMessageMap : EntityTypeConfiguration<UserMessage>
    {
        public UserMessageMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.subject)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.message)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("UserMessage");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.sender_id).HasColumnName("sender_id");
            this.Property(t => t.recipient_id).HasColumnName("recipient_id");
            this.Property(t => t.datetime_sent).HasColumnName("datetime_sent");
            this.Property(t => t.subject).HasColumnName("subject");
            this.Property(t => t.message).HasColumnName("message");
            this.Property(t => t.message_read).HasColumnName("message_read");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserMessages)
                .HasForeignKey(d => d.recipient_id);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.UserMessages1)
                .HasForeignKey(d => d.sender_id);

        }
    }
}
