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
        public async Task OnWaitingForPassword(ApplicationContext context, Message message,
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

        [UserAction(Actions.WaitingForAction)]
        public async Task OnWaitingForAction(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
                
            
            
            TelegramKeyboard keyboard = new TelegramKeyboard(true);
            keyboard.AddRow("1", "1", "2");    
            
            
            await client.SendTextMessageAsync(
                chatId, 
                "test",
                ParseMode.Markdown,
                replyMarkup: keyboard.Markup);
        }
    }
}