// This code is distributed under MIT license. 
// Copyright (c) 2010-2018 George Mamaladze
// See license.txt or https://mit-license.org/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleHook
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var selector = new Dictionary<string, Action<Action>>
            {
                {"1. Log keys", LogKeys.Do},
                {"2. Detect key combinations", DetectCombinations.Do},
                {"3. Detect key sequences", DetectSequences.Do},
                {"Q. Quit", Exit}
            };

            Action<Action> action = null;

            while (action == null)
            {
                Console.WriteLine("Please select one of these:");
                foreach (var selectorKey in selector.Keys)
                    Console.WriteLine(selectorKey);
                var ch = Console.ReadKey(true).KeyChar;
                action = selector
                    .Where(p => p.Key.StartsWith(ch.ToString()))
                    .Select(p => p.Value).FirstOrDefault();
            }
            Console.WriteLine("--------------------------------------------------");

            action(Application.Exit);

            Application.Run(new ApplicationContext());
        }


        private static void Exit(Action quit)
        {
            Application.Exit();
        }
    }
}