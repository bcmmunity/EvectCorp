using EvectCorp.Models;
using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public sealed class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppSettings.DatabaseConnectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.TelegramId);
            
            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    UserId = 1,
                    TelegramId = 12312312,
                    FirstName = "artem",
                    LastName = "kim",
                    Email = "moranmr8@gmail.com"
                });
            
            modelBuilder.Entity<Event>()
                .HasData(
                    new Event { 
                        EventId = 1,
                        Name = "Тестовое мероприятие, оч крутое", 
                        Info = "Крутое мероприятия для разномастных разработчиков", 
                        EventCode = "event_kim",
                        AdminCode = "event_admin",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    }, 
                    new Event { 
                        EventId = 2,
                        Name = "Второе тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim2",
                        AdminCode = "event_admin2",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    },
                    new Event { 
                        EventId = 3,
                        Name = "Третье тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim3",
                        AdminCode = "event_admin3",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    },
                    new Event { 
                        EventId = 4,
                        Name = "Четвертое тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim4",
                        AdminCode = "event_admin4",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    },
                    new Event { 
                        EventId = 5,
                        Name = "Пятое тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim5",
                        AdminCode = "event_admin5",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    },
                    new Event { 
                        EventId = 6,
                        Name = "Шестое тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim6",
                        AdminCode = "event_admin6",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    }
                    );

            modelBuilder.Entity<UserEvent>()
                .HasKey(us => new {us.EventId, us.UserEventId});
            
            modelBuilder.Entity<UserEvent>()
                .HasOne(us => us.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(k => k.EventId);
                
            
            modelBuilder.Entity<UserEvent>()
                .HasOne(us => us.User)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(k => k.UserEventId);
                


        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}