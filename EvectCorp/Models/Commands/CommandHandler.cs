using System.Threading.Tasks;
using Evect.Models;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EvectCorp.Models.Commands
{
    public class CommandHandler
    {

        [TelegramCommand("/start")]
        public async Task OnStart(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            await DatabaseUtils.AddUser(context, chatId);
            
            await client.SendTextMessageAsync(
                chatId,
                "Добро пожаловать, введите администраторский пароль",
                ParseMode.Markdown);
            await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForPassword);
        }

        
     
      
       


    } 
}