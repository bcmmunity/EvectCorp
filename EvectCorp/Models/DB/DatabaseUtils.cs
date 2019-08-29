using System.Linq;
using System.Threading.Tasks;
using EvectCorp.Models;
using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public static class DatabaseUtils
    {

        public static async Task<AdminUser> GetUserByChatId(ApplicationContext context, long chatId)
        {
            return await context.Admins.FirstOrDefaultAsync(a => a.TelegramId == chatId);
        }

        public static async Task<bool> IsUserAdmin(ApplicationContext context, long chatId)
        {
            AdminUser user = await GetUserByChatId(context, chatId);
            if (user != null)
            {
                return user.IsAdmin;
            }

            return false;
        }

        public static async Task SetUserAdmin(ApplicationContext context, long chatId)
        {
            AdminUser user = await GetUserByChatId(context, chatId);
            if (user != null)
            {
                user.IsAdmin = true;
                context.Admins.Update(user);
                await context.SaveChangesAsync();
            }
        }


        public static async Task ChangeUserAction(ApplicationContext context, long chatId, Actions newAction)
        {
            AdminUser user = await GetUserByChatId(context, chatId);
            if (user != null)
            {
                user.CurrentAction = newAction;
                context.Admins.Update(user);
                await context.SaveChangesAsync();
            }
        }

        public static async Task AddUser(ApplicationContext context, long chatId)
        {
            AdminUser user = new AdminUser()
            {
                TelegramId = chatId
            };

            await context.Admins.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public static async Task ClearUserTempData(ApplicationContext context, long chatId)
        {
            AdminUser user = await GetUserByChatId(context, chatId);
            user.TempEventName = default;
            user.TempEventCode = default;

            context.Admins.Update(user);
            await context.SaveChangesAsync();

        }

        public static async Task<bool> CheckEventExists(ApplicationContext context, string code)
        {
            return await context.Events.FirstOrDefaultAsync(e => e.EventCode == code || e.AdminCode == code) != null;
        }
        


    }
}