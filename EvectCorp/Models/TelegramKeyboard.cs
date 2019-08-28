using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace EvectCorp.Models
{
    public  class TelegramKeyboard
    {
        public  ReplyKeyboardMarkup GetKeyboard(string[][] buttons, bool isOneTime = false)
        {
            int rows = buttons.Length;

            KeyboardButton[][] keyboardButtons =
                new KeyboardButton[rows][];

            for (int row = 0; row < rows; row++)
            {
                keyboardButtons[row] = new KeyboardButton[buttons[row].Length];

                for (int column = 0; column < buttons[row].Length; column++)
                {
                    keyboardButtons[row][column] = buttons[row][column];
                }
            }

            
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = keyboardButtons,
                ResizeKeyboard = true,
                OneTimeKeyboard = isOneTime
            };

            return keyboard;
        }
        
        
        

        public static InlineKeyboardMarkup GetInlineKeyboard(string[][] buttons,
            string[][] callback_data)
        {
            int rows = buttons.Length;
            InlineKeyboardButton[][] keyboardButtons =
                new InlineKeyboardButton[rows][];

            for (int row = 0; row < rows; row++)
            {
                keyboardButtons[row] = new InlineKeyboardButton[buttons[row].Length];

                for (int column = 0; column < buttons[row].Length; column++)
                {
                    keyboardButtons[row][column] = new InlineKeyboardButton
                        {Text = buttons[row][column], CallbackData = callback_data[row][column]};
                }
            }


            var keyboard = new InlineKeyboardMarkup(keyboardButtons);

            return keyboard;
        }
    }
}