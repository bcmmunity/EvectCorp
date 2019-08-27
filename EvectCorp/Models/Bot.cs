using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Evect.Models;
using Evect.Models.DB;
using EvectCorp.Models.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EvectCorp.Models
{
    public static class Bot
    {
        private static TelegramBotClient _client;


        private static Dictionary<Action<ApplicationContext, Message, TelegramBotClient>, string > _commandsList = new Dictionary<Action<ApplicationContext,Message, TelegramBotClient>, string>();
        private static Dictionary<Action<ApplicationContext,Message, TelegramBotClient>, Actions> _actionList =  new Dictionary<Action<ApplicationContext,Message, TelegramBotClient>, Actions>();
        
        public static Dictionary<Action<ApplicationContext,Message, TelegramBotClient>, string> Commands => _commandsList;
        public static Dictionary<Action<ApplicationContext,Message, TelegramBotClient>, Actions > ActionList => _actionList;
        

        
        
        
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (_client != null)
            {
                return _client;
            }
            
            
            Assembly assembly = Assembly.GetAssembly(typeof(Actions));

            
            var commandsMethodInfo = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(TelegramCommand), false).Length > 0)
                .ToList();
            
            CommandHandler _commandHandler = new CommandHandler();
            ActionHandler _actionHandler = new ActionHandler();
            
            foreach (var methodInfo in commandsMethodInfo)
            {
                Action<ApplicationContext, Message, TelegramBotClient> a = 
                    (Action<ApplicationContext, Message, TelegramBotClient>) Delegate.CreateDelegate(typeof(Action<ApplicationContext,Message, TelegramBotClient>), _commandHandler, methodInfo);

                string c = methodInfo.GetCustomAttribute<TelegramCommand>().StringCommand;
                _commandsList.Add(a, c);
            }
            
            var actionMethodInfo = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(UserAction), false).Length > 0)
                .ToList();
           
            
            foreach (var methodInfo in actionMethodInfo)
            {
                Action<ApplicationContext,Message, TelegramBotClient> a = 
                                    (Action<ApplicationContext,Message, TelegramBotClient>) Delegate.CreateDelegate(typeof(Action<ApplicationContext,Message, TelegramBotClient>),_actionHandler ,methodInfo);
                
                Actions act = methodInfo.GetCustomAttribute<UserAction>().Action;
                _actionList.Add(a, act);
            }

            
            _client = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url, "api/message/update");
            await _client.SetWebhookAsync(hook);

            return _client;
        }
    }
}