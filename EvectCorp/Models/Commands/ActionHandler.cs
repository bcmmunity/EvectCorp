using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Evect.Models.DB;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EvectCorp.Models.Commands
{
    public class ActionHandler
    {
        [UserAction(Actions.WaitingForPassword)]
        public async Task OnWaitingPassword(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
            
            if (text == AppSettings.Password)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Пароль **правильный**");
                builder.AppendLine();
                builder.AppendLine("Вам доступен мой функционал");

                string[][] actions = new[] {new[] {"1", "1", "1", "1", "1", "1", "1", "1", "1", "1"}};
                
                await client.SendTextMessageAsync(
                    chatId, 
                    builder.ToString(),
                    ParseMode.Markdown,
                    replyMarkup: TelegramKeyboard.GetKeyboard(actions, true));
                await DatabaseUtils.SetUserAdmin(context, chatId);
                await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForAction);
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId, 
                    "Пароль неправильный",
                    ParseMode.Markdown);
            }
        }
    }
}