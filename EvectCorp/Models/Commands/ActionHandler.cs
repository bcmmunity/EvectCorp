#define WORK
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
                builder.AppendLine("Пароль *правильный*");
                builder.AppendLine();
                builder.AppendLine("Вам доступен мой функционал");
                
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Создать новое мероприятие");
#if WORK
                keyboard.AddRow("Изменение тегов (в разработке)");

#else
                keyboard.AddRow("Изменение тегов");
#endif
                
                
                await DatabaseUtils.SetUserAdmin(context, chatId);
                await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForAction);

                await client.SendTextMessageAsync(
                    chatId, 
                    builder.ToString(), 
                    ParseMode.Markdown,
                    replyMarkup: keyboard.Markup);

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
            StringBuilder builder = new StringBuilder();
            
            switch (text)
            {
                case "Создать новое мероприятие":
                    builder.Clear();
                    await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForEventName);

                    builder.AppendLine("*Режим добавления мероприятия*");
                    builder.AppendLine();
                    builder.AppendLine("_Введите название мероприятия_");

                    await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown);
                    break;
                
                case "Изменение тегов":
                    break;
                
                default:
                    await client.SendTextMessageAsync(chatId, "Я вас не понимаю");
                    break;
            }
            
        }

        [UserAction(Actions.WaitingForEventName)]
        public async Task OnWaitingForEventName(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
            StringBuilder builder = new StringBuilder();

            AdminUser user = await DatabaseUtils.GetUserByChatId(context, chatId);

            user.TempEventName = text;

            context.Update(user);
            await context.SaveChangesAsync();

            await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForEventCode);

            builder.AppendLine($"Название \"*{text}*\" сохранено");
            builder.AppendLine();
            builder.AppendLine("_Введите ивент код для участников_");

            await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown);


        }

        [UserAction(Actions.WaitingForEventCode)]
        public async Task OnWaitingEventCode(ApplicationContext context, Message message, 
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
            StringBuilder builder = new StringBuilder();

            if (await DatabaseUtils.CheckEventExists(context, text))
            {
                builder.AppendLine("Мероприятие с таким кодом _уже существует_");
                builder.AppendLine();
                builder.AppendLine("Введите код участника");
                await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown);
            }
            else
            {
                AdminUser user = await DatabaseUtils.GetUserByChatId(context, chatId);

                user.TempEventCode = text;

                context.Update(user);
                await context.SaveChangesAsync();
            
                await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForEventAdminCode);

                builder.AppendLine($"Код участника \"*{text}*\" сохранен");
                builder.AppendLine();
                builder.AppendLine("_Введите ивент код для организаторов_");

                await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown); 
            }
            
            
            
        }

        [UserAction(Actions.WaitingForEventAdminCode)]
        public async Task OnWaitingForEventAdminCode(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
            StringBuilder builder = new StringBuilder();

            if (await DatabaseUtils.CheckEventExists(context, text))
            {
                builder.AppendLine("Мероприятие с таким кодом _уже существует_");
                builder.AppendLine();
                builder.AppendLine("Введите код организтора");
                await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown);
            }
            else
            {
                AdminUser user = await DatabaseUtils.GetUserByChatId(context, chatId);
            
                builder.AppendLine($"Код организатора \"*{text}*\" сохранен");

                await client.SendTextMessageAsync(
                    chatId,
                    builder.ToString(),
                    ParseMode.Markdown);
                
                Event ev = new Event()
                {
                    Name = user.TempEventCode,
                    EventCode = user.TempEventCode,
                    AdminCode = text
                };

                context.Events.Add(ev);
                await context.SaveChangesAsync();

                await DatabaseUtils.ClearUserTempData(context, chatId);
            
                await client.SendTextMessageAsync(
                    chatId, 
                    $"Мероприятие {ev.Name} успешно сохранено", 
                    ParseMode.Default);
            
                await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForAction);
            }

        }

    }
}