using System;

namespace ComicStoreDb
{
    public class Menus
    {
        private ComicsInteraction sender;

        public Menus(ComicsInteraction sender)
        {
            this.sender = sender;
        }

        public enum MainMenuSelection
        {
            Add, //0
            Read, //1
            Update,
            Delete,
            Statistics,
            Exit
        }

        public enum TableSelection
        {
            Author,
            Category,
            Comic,
            PublishingHouse,
            Country,
            Back
        }

        public enum StatisticsSelection
        {
            ComicsPerCategory,
            ComicsPerAuthor,
            LongestComics,
            AuthorsPerNationality,
            Back
        }

        public MainMenuSelection mainMenuSelection { get; private set; }
        public TableSelection tableSelection { get; private set; }
        public StatisticsSelection statisticSelection { get; private set; }

        private int input;
        private bool validInput;

        public bool MainMenu()
        {
            var enumNames = Enum.GetNames(typeof(MainMenuSelection));
            Console.CursorVisible = false;
            bool exit = false;

            do
            {
                Console.WriteLine("* Welcome to the Comic Store.\n" +
                                    "* What do you want to do?\n" +
                                    "*");
                int count = 1;
                foreach (var item in enumNames)
                {
                    Console.WriteLine($"* {count++}) {item}");
                }

                input = Console.ReadKey(true).KeyChar - '1';

                validInput = input >= 0 && input < enumNames.Length;

                if (validInput)
                {
                    mainMenuSelection = (MainMenuSelection)input;
                    switch (mainMenuSelection)
                    {
                        case MainMenuSelection.Exit:
                            {
                                exit = true;
                                break;
                            }
                        case MainMenuSelection.Statistics:
                            {
                                sender.Location = ComicsInteraction.MenuLocation.StatisticsMenu;
                                break;
                            }
                        default:
                            {
                                sender.Location = ComicsInteraction.MenuLocation.TableMenu;
                                break;
                            }
                    }
                }
                Console.Clear();
            } while (!validInput);

            return exit;
        }

        public void TableMenu()
        {
            var enumNames = Enum.GetNames(typeof(TableSelection));

            do
            {
                Console.Clear();
                Console.WriteLine("* What do you want to " + mainMenuSelection.ToString().ToLower() + "?\n" +
                                    "*");

                int count = 1;
                foreach (var item in enumNames)
                {
                    Console.WriteLine($"* {count++}) {item}");
                }

                input = Console.ReadKey(true).KeyChar - '1';

                validInput = input >= 0 && input < enumNames.Length;

                if (validInput)
                {
                    tableSelection = (TableSelection)input;
                    switch (tableSelection)
                    {
                        case TableSelection.Back:
                            sender.Location = ComicsInteraction.MenuLocation.MainMenu;
                            break;

                        default:
                            sender.Location = ComicsInteraction.MenuLocation.ActionMenu;
                            sender.ActualTable = (ComicsInteraction.Table)tableSelection;
                            break;
                    }
                }
            } while (!validInput);
        }

        public void StatisticsMenu()
        {
            var enumNames = Enum.GetNames(typeof(StatisticsSelection)).AddWhitespaces();
            bool exit = false;
            bool validInput;
            int input;
            StatisticsSelection selection;
            do
            {
                Console.WriteLine("* What statistic do you want to visualize?\n" +
                    "*");

                int count = 1;
                foreach (var item in enumNames)
                {
                    Console.WriteLine($"* {count++}) {item}");
                }

                input = Console.ReadKey(true).KeyChar - '1';

                validInput = input >= 0 && input < enumNames.Length;

                if (validInput)
                {
                    selection = (StatisticsSelection)input;
                    exit = true;
                    switch (selection)
                    {
                        case StatisticsSelection.Back:
                            sender.Location = ComicsInteraction.MenuLocation.MainMenu;
                            break;

                        default:
                            statisticSelection = selection;
                            sender.Location = ComicsInteraction.MenuLocation.StatisticViewer;
                            break;
                    }
                }
                Console.Clear();
            } while (!exit);
        }
    }
}