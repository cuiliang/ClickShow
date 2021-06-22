// This code is distributed under MIT license. 
// Copyright (c) 2010-2018 George Mamaladze
// See license.txt or https://mit-license.org/

using System;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using MouseKeyHook.Rx;

namespace ConsoleHook.Rx
{
    internal class DetectCombinations
    {
        public static void Do(AutoResetEvent quit)
        {
            var quitTrigger = Combination.FromString("Control+Q");
            var triggers = new[]
            {
                quitTrigger,
                Combination.TriggeredBy(Keys.H).Alt().Shift(),
                Combination.TriggeredBy(Keys.E).With(Keys.Q).With(Keys.W)
            };


            Hook
                .GlobalEvents()
                .KeyDownObservable()
                .Matching(triggers)
                .ForEachAsync(trigger =>
                {
                    if (trigger == quitTrigger) quit.Set();
                    Console.WriteLine(trigger);
                });

            Console.WriteLine("Press Control+Q to quit.");
            Console.WriteLine("Monitoring folowing combinations:");
            foreach (var name in triggers)
                Console.WriteLine("\t" + name);
        }
    }
}