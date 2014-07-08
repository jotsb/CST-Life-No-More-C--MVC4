using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SimsTemplate.Models.Mapping
{
    public class FriendsListMap : EntityTypeConfiguration<FriendsList>
    {
        public FriendsListMap()
        {
            // Primary Key
            this.HasKey(t => new { t.id, t.friend_id });

            // Properties
            this.Property(t => t.id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.friend_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.status)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FriendsList");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.friend_id).HasColumnName("friend_id");
            this.Property(t => t.num_requests_sent).HasColumnName("num_requests_sent");
            this.Property(t => t.status).HasColumnName("status");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.FriendsLists)
                .HasForeignKey(d => d.friend_id);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.FriendsLists1)
                .HasForeignKey(d => d.id);

        }
    }
}
