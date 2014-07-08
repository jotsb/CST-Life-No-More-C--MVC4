using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SimsTemplate.Models.Mapping;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class TechproContext : DbContext
    {
        static TechproContext()
        {
            Database.SetInitializer<TechproContext>(null);
        }

		public TechproContext()
			: base("Name=techproContext")
		{
		}

        public DbSet<Business> Businesses { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterGrade> CharacterGrades { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<FriendsList> FriendsLists { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Minigame> Minigames { get; set; }
        public DbSet<MinigameInstance> MinigameInstances { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<QuestLog> QuestLogs { get; set; }
        public DbSet<Subforum> Subforums { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BusinessMap());
            modelBuilder.Configurations.Add(new CharacterMap());
            modelBuilder.Configurations.Add(new CharacterGradeMap());
            modelBuilder.Configurations.Add(new ForumPostMap());
            modelBuilder.Configurations.Add(new ForumThreadMap());
            modelBuilder.Configurations.Add(new FriendsListMap());
            modelBuilder.Configurations.Add(new ItemMap());
            modelBuilder.Configurations.Add(new JobMap());
            modelBuilder.Configurations.Add(new LectureMap());
            modelBuilder.Configurations.Add(new MinigameMap());
            modelBuilder.Configurations.Add(new MinigameInstanceMap());
            modelBuilder.Configurations.Add(new QuestMap());
            modelBuilder.Configurations.Add(new QuestLogMap());
            modelBuilder.Configurations.Add(new SubforumMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserMessageMap());
        }
    }
}
