using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evect.Models;
using Evect.Models.Commands;
using Evect.Models.DB;
using EvectCorp.Models;
using EvectCorp.Models.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Telegram.Bot.Types.User;

namespace EvectCorp.Controllers
{
    public class HomeController : Controller
    {

        private CommandHandler _commandHandler;
        private ActionHandler _actionHandler;
        private Dictionary<Action<ApplicationContext, Message, TelegramBotClient>, string> _commands;
        private Dictionary<Action<ApplicationContext, Message, TelegramBotClient>, Actions> _actions;

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

                User user = await UserDB.GetUserByChatId(db, chatId);

                if (user == null)
                {
                    foreach (var pair in _commands)
                    {
                        if (pair.Value == "/start")
                        {
                            pair.Key(db, message, client);
                        }
                    }

                    return Ok();
                }

                if (!user.IsAuthed)
                {
                    foreach (var pair in _commands)
                    {
                        if (pair.Value == "/start" || pair.Value == "Личный кабинет")
                        {
                            pair.Key(db, message, client);
                        }
                    }

                    return Ok();
                }

                foreach (var pair in _actions)
                {
                    if (pair.Value == user.CurrentAction)
                    {
                        pair.Key(db, message, client);
                    }
                }
            }


            return Ok();
        }
    }
}