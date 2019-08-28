﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evect.Models.DB;
using EvectCorp.Models;
using EvectCorp.Models.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EvectCorp.Controllers
{
    public class HomeController : Controller
    {
        private CommandHandler _commandHandler;
        private ActionHandler _actionHandler;
        private Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, string> _commands;
        private Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, Actions> _actions;

        public HomeController(ApplicationContext db)
        {
            _commandHandler = new CommandHandler();
            _actionHandler = new ActionHandler();

            _commands = Bot.Commands;
            _actions = Bot.ActionList;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("api/message/update")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update == null)
                return Ok();

            
            using (ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>()))
            {
                var message = update.Message;
                var client = new TelegramBotClient(AppSettings.Key);
                var chatId = message.Chat.Id;
                var text = message.Text;

                AdminUser user = await DatabaseUtils.GetUserByChatId(db, chatId);

                if (user == null)
                {
                    await DatabaseUtils.AddUser(db, chatId);
                }
                else
                {
                    if (user.IsAdmin)
                    {
                        foreach (var pair in _actions)
                        {
                            if (pair.Value == user.CurrentAction)
                            {
                                await pair.Key(db, message, client);
                            }
                        }   
                    }
                    else
                    {
                        await _actions
                            .Where(u => u.Value == Actions.WaitingForPassword)
                            .Select(u => u.Key)
                            .First()(db, message, client);
                    }
                }
                
                if (text == "/start")
                {
                    await client.SendTextMessageAsync(
                        chatId, 
                        "Добро пожаловать, введите администраторский пароль",
                        ParseMode.Markdown);
                    await DatabaseUtils.ChangeUserAction(db, chatId, Actions.WaitingForPassword);
                }

                

                return Ok();
            }


        }
    }
}