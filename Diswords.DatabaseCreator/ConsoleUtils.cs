using System;

namespace Diswords.DatabaseCreator
{
    public static class ConsoleUtils
    {
        public static int WaitForChoice(params string[] texts)
        {
            for (var index = 0; index < texts.Length; index++)
            {
                var sentence = texts[index];
                Console.WriteLine($"{index + 1}. {sentence}");
            }

            while (true)
            {
                Console.Write("> ");

                var input = Console.ReadLine();
                if (!int.TryParse(input, out var choice))
                    Console.WriteLine("Invalid string.");
                else if (choice < 1 || choice > texts.Length)
                    Console.WriteLine("Invalid choice.");
                else
                    return choice;
            }
        }

        public static string WaitForInput(string before, Func<string, bool> checker)
        {
            while (true)
            {
                Console.Write(before);
                var input = Console.ReadLine();
                if (!checker(input))
                    Console.WriteLine("Invalid input!");
                else
                    return input;
            }
        }
    }
}