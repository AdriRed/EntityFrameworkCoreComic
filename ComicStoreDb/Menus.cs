using System;
using System.Collections.Generic;
using System.Text;

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
            Exit
        }

        public enum TableSelection
        {
            Author,
            Category,
            Comic,
            Back
        }

        public MainMenuSelection mainMenuSelection;
        public TableSelection tableSelection;

        private int input;
        private bool validInput;

        public bool MainMenu()
        {

            Console.CursorVisible = false;
            do
            {
                Console.Clear();
                Console.WriteLine("* Welcome to the Comic Store.\n" +
                                    "* What do you want to do?\n" +
                                    "*\n" +
                                    "* 1) Add\n" +
                                    "* 2) Read\n" +
                                    "* 3) Update\n" +
                                    "* 4) Delete\n" +
                                    "* 5) Exit");

                input = Console.ReadKey(true).KeyChar - '1';

                validInput = input >= 0 && input <= 4;

                if (validInput)
                {
                    mainMenuSelection = (MainMenuSelection)input;
                    switch (mainMenuSelection)
                    {
                        case MainMenuSelection.Exit:
                            {
                                return true;
                            }
                        default:
                            {
                                sender.Location = ComicsInteraction.MenuLocation.TableMenu;
                                break;
                            }
                    }
                }
            } while (!validInput);

            return false;
        }

        public void TableMenu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("* What do you want to " + mainMenuSelection.ToString().ToLower() + " ?\n" +
                                    "*\n" +
                                    "* 1) Author\n" +
                                    "* 2) Category\n" +
                                    "* 3) Comic\n" +
                                    "* 4) Back");

                input = Console.ReadKey(true).KeyChar - '1';

                validInput = input >= 0 && input <= 3;

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
    }
}
