using System.Collections.Generic;
using System.Linq;

namespace EvectCorp.Models
{
    public static class Utils
    {
        /// <summary>
        /// Разбивает изначальный список на энное количество списков опреленной длины
        /// </summary>
        /// <param name="width">длина списка </param>
        /// <param name="list">изначальный список</param>
        public static List<List<T>> SplitList<T>(int width, List<T> list)
        {
            List<List<T>> splited = new List<List<T>>();
            if (list.Count <= width)
            {
                splited.Add(list);
                return splited;
            }

            int numberOfLists = list.Count / width;

            for (int i = 0; i < numberOfLists; i++) 
            { 
                List<T> newList = list.Skip(i * width).Take(width).ToList();
                splited.Add(newList); 
            }
            
            return splited.Take(splited.Count - 1).ToList();


        }
    }
}