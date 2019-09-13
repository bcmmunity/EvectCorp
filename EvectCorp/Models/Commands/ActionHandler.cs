#define WORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Evect.Models;
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
                keyboard.AddRow("Вывести все мероприятия");
                
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
                    builder.Clear();

                    builder.AppendLine("Выберите опцию");
                    
                    TelegramKeyboard keyboard = new TelegramKeyboard();
                    keyboard.AddRow("Добавить родительский тег");
                    keyboard.AddRow("Добавить дочерний тег");
                    keyboard.AddRow("Вывести все родительские теги");

                    await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForTagAction);
                    await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown, replyMarkup: keyboard.Markup);

                    break;
                
                case "Вывести все мероприятия":
                    List<Event> events = context.Events.ToList();
                    if (events.Count > 0)
                    {
                        for (var i = 0; i < events.Count; i++)
                        {
                            builder.AppendLine($"{i+1}: {events[i].Name}");
                        }
                    }
                    else
                    {
                        builder.AppendLine("Нету ивентов");
                    }
                    
                    await client.SendTextMessageAsync(chatId, builder.ToString());

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

        [UserAction(Actions.WaitingForEventMemberCode)]
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
                builder.AppendLine("Введите код организатора");
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
                    $"Мероприятие <b>{ev.Name}</b> успешно сохранено", 
                    ParseMode.Html);
            
                await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForAction);
            }

        }

        [UserAction(Actions.WaitingForTagAction)]
        public async Task OnWaitingForTagAction(ApplicationContext context, Message message,
            TelegramBotClient client)
        {            
            var text = message.Text;
            var chatId = message.Chat.Id;
            StringBuilder builder = new StringBuilder();
            switch (text)
            {
                case "Добавить родительский тег":
                    await client.SendTextMessageAsync(
                        chatId, 
                        "Введите название нового тега", 
                        ParseMode.Markdown);
                    await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForParentTag);
                    break;
                
                case "Добавить дочерний тег":

                    var parentTags = Utils.SplitList(2, context.Tags.Where(t => t.Level == 1).ToList());
                    TelegramKeyboard keyboard = new TelegramKeyboard();
                    foreach (var parentTag in parentTags)
                    {
                        keyboard.AddRow(parentTag.Select(e => e.Name));
                    }
                    
                    await client.SendTextMessageAsync(
                        chatId, 
                        "Введите родительский тег к которому надо добавить дочерний", 
                        ParseMode.Markdown, replyMarkup: keyboard.Markup);
                    await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForChoosingParentTag);
                    break;
                
                case "Вывести все родительские теги":
                    builder.Clear();
                    builder.AppendLine("теги:");
                    List<Tag> tags = context.Tags.Where(e => e.Level == 1).ToList();
                    foreach (var tag in tags)
                    {
                        builder.AppendLine(tag.Name);
                    }
                    await client.SendTextMessageAsync(
                        chatId, 
                        builder.ToString(), 
                        ParseMode.Markdown);
                    break;
            }
        }

        [UserAction(Actions.WaitingForParentTag)]
        public async Task OnWaitingParenTag(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
            Tag tag = new Tag()
            {
                ParentTagID = 0,
                Name = text,
                Level = 1
            };

            context.Tags.Add(tag);
            context.SaveChanges();
            await client.SendTextMessageAsync(
                chatId, 
                $"Родительский тег *{tag.Name}* добавлен", 
                ParseMode.Markdown);
            TelegramKeyboard keyboard = new TelegramKeyboard();
            keyboard.AddRow("Добавить родительский тег");
            keyboard.AddRow("Добавить дочерний тег");
            keyboard.AddRow("Вывести все родительские теги");

            await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForTagAction);
            await client.SendTextMessageAsync(chatId, "Выберите опцию", ParseMode.Markdown, replyMarkup: keyboard.Markup);
        }

        [UserAction(Actions.WaitingForChoosingParentTag)]
        public async Task OnWaitingForChoosingParentTag(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
            Tag tag = context.Tags.FirstOrDefault(t => t.Level == 1 && t.Name == text);
            if (tag != null)
            {
                AdminUser user = await DatabaseUtils.GetUserByChatId(context, chatId);
                user.TempParentTag = tag.TagId;
                context.Update(user);
                context.SaveChanges();
                await client.SendTextMessageAsync(chatId, "Введите новый дочерний тег", ParseMode.Markdown);
                await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForNewChildTag);
            }
        }
        
        [UserAction(Actions.WaitingForNewChildTag)]
        public async Task OnWaitingForNewChildTag(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;
            AdminUser user = await DatabaseUtils.GetUserByChatId(context, chatId);
            Tag newChildTag = new Tag()
            {
                ParentTagID = user.TempParentTag,
                Name = text,
                Level = 2
            };
            context.Tags.Add(newChildTag);
            context.SaveChanges();
            
            await DatabaseUtils.ChangeUserAction(context, chatId, Actions.WaitingForTagAction);
            await client.SendTextMessageAsync(chatId, "Выберите опцию", ParseMode.Markdown);
            
        }

    }
}