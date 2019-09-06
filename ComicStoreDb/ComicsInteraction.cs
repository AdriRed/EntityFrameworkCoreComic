using System;
using System.Text;
using System.Linq;
using System.Reflection;
using ComicStoreDb.Classes;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ComicStoreDb
{
    public class ComicsInteraction
    {
        private delegate void Actions();
        public enum MenuLocation
        {
            MainMenu,
            TableMenu,
            ActionMenu,
            CheckData,
            ChooseReg,
            ModifyData
        }
        public enum Table
        {
            Authors,
            Categories,
            Comics
        }

        private Actions[,] actions;
        public MenuLocation Location;
        public Table ActualTable;
        public bool exit;
        private ComicsContext context;

        private Menus menus;

        public ComicsInteraction()
        {
            
        }

        public void Start()
        {
            menus = new Menus(this);
            context = new ComicsContext();
            AssignActions();
            Location = MenuLocation.MainMenu;

            do
            {
                UpdateCollections();
                switch (Location)
                {
                    case MenuLocation.MainMenu:
                        exit = menus.MainMenu();
                        break;
                    case MenuLocation.TableMenu:
                        menus.TableMenu();
                        break;
                    case MenuLocation.ActionMenu:
                        actions[(int)menus.mainMenuSelection, (int)menus.tableSelection]();
                        context.SaveChanges();
                        Location = MenuLocation.MainMenu;
                        break;
                    default:
                        exit = true;
                        break;
                }
            } while (!exit);
        }
        private void UpdateCollections()
        {
            foreach (var item in context.Authors)
            {
                context.Entry(item).Collection(x => x.Functions).Load();
            }
            foreach (var item in context.Comics)
            {
                context.Entry(item).Collection(x => x.Functions).Load();
            }
            foreach (var item in context.Categories)
            {
                context.Entry(item).Collection(x => x.Comics).Load();
            }
        }
        private void AssignActions()
        {
            actions = new Actions[4, 3];
            actions[0, 0] = new Actions(AddAuthor);
            actions[1, 0] = new Actions(ReadAuthors);
            actions[2, 0] = new Actions(UpdateAuthor);
            actions[3, 0] = new Actions(DeleteAuthor);

            actions[0, 1] = new Actions(AddCategory);
            actions[1, 1] = new Actions(ReadCategories);
            actions[2, 1] = new Actions(UpdateCategory);
            actions[3, 1] = new Actions(DeleteCategory);

            actions[0, 2] = new Actions(AddComic);
            actions[1, 2] = new Actions(ReadComics);
            actions[2, 2] = new Actions(UpdateComic);
            actions[3, 2] = new Actions(DeleteComic);
        }

        #region Generic


        public T AddData<T>() where T : IRawData, new()
        {
            T data = new T();
            ModifyData(data);
            return data;
        }
        public void ModifyData<T>(T data) where T : IRawData
        {
            var propNames = data.PropNames();
            var propValues = data.PropValues();

            int selection = 0, totalSelections = propNames.Length + 2;
            StringBuilder actualString = new StringBuilder(propValues[selection]);
            bool exit = false;
            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                Console.WriteLine($"* {menus.mainMenuSelection.ToString()} {menus.tableSelection.ToString()}\n" +
                    "*");
                for (int i = 0; i < propNames.Length; i++)
                {
                    Console.WriteLine($"{(selection == i ? ">" : "*")} {propNames[i]}: {propValues[i]}");
                }
                Console.WriteLine("*\n" +
                    (selection == propNames.Length ? ">" : "*") + " OK\n" +
                    (selection == propNames.Length + 1 ? ">" : "*") + " BACK");


                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow)
                {
                    selection += (key.Key == ConsoleKey.UpArrow) ? -1 : 1;

                    selection += totalSelections;
                    selection %= totalSelections;

                    if (selection >= 0 && selection < propNames.Length)
                    {
                        actualString = new StringBuilder(propValues[selection]);
                    }

                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (actualString.Length > 0)
                    {
                        actualString.Remove(actualString.Length - 1, 1);
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    Location = MenuLocation.MainMenu;
                    exit = true;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    exit = true;
                    if (selection == propNames.Length)
                        Location = MenuLocation.CheckData;
                    else
                        Location = MenuLocation.TableMenu;
                }
                else if (Char.IsLetterOrDigit(key.KeyChar) || Char.IsPunctuation(key.KeyChar) || Char.IsWhiteSpace(key.KeyChar))
                {
                    if (selection >= 0 && selection < propNames.Length)
                    {
                        actualString.Append(key.KeyChar);
                    }
                } 
                if (selection >= 0 && selection < propNames.Length)
                    propValues[selection] = actualString.ToString().Trim();
            } while (!exit);

            data.ConvertFromStringArr(propValues);
        }
        private void PrintSeparator()
        {
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write('-');
            }
            Console.WriteLine();
        }
        private void PrintHeaders(string[] headers, int charsPerField)
        {
            foreach (var item in headers)
            {
                Console.Write(item.PadRight(charsPerField) + "|");
            }
            Console.WriteLine();
            PrintSeparator();
        }
        public void PrintReg(string[] values, int charsPerField)
        {
            foreach (var value in values)
            {
                Console.Write(value.PadRight(charsPerField) + "|");
            }
            Console.WriteLine();
        }
        public void ReadData<T>(ICollection<T> data) where T : IRawData, new()
        {
            string[] headers = (new T()).PropNames();
            int charsPerField = Console.WindowWidth / headers.Length - 1;

            PrintHeaders(headers, charsPerField);
            
            foreach (var item in data)
            {
                string[] values = item.PropValues();
                PrintReg(values, charsPerField);
            }

            Console.ReadKey(true);
        }
        public IQueryable<ITable> GetActualTable()
        {
            var table = (IQueryable)context.GetType().GetProperty(ActualTable.ToString()).GetValue(context);
            return table.Cast<ITable>();
        } 
        public IData[] Query(string property, string value)
        {
            var query = GetActualTable().Where(x => x.Match(property, value)).Select(x => x.GetData());

            return query.ToArray();
        }
        public T FindData<T>(out int id) where T : IRawData, new()
        {
            var prop = ChoosePropFindBy<T>();
            id = -1;
            if (Location != MenuLocation.ChooseReg)
                return default;
            var existingData = ChooseReg<T>(prop, out id);
            if (Location != MenuLocation.ModifyData)
                return default;
            T data = new T();
            data.ConvertFromStringArr(existingData);
            return data;
        }
        private string[] ChooseReg<T>(string findBy, out int id) where T : IRawData, new()
        {
            var propNames = new T().PropNames();
            int charsPerField = (Console.WindowWidth - 2) / propNames.Length - 1;
            int selection = 0;
            id = -1;
            StringBuilder input = new StringBuilder(String.Empty);
            ConsoleKeyInfo inputkey;
            IData[] regs;
            bool exit = false;
            string[] rawdata = null;

            do
            {
                Console.WriteLine("|" + findBy + ": " + input.ToString().PadRight(Console.WindowWidth - findBy.Length - 5));
                
                Console.Write("-|");
                PrintHeaders(propNames, charsPerField);

                regs = (input.ToString() == String.Empty) ? 
                    GetActualTable().Select(x => x.GetData()).ToArray() : 
                    Query(findBy, input.ToString());

                for (int i = 0; i < regs.Length; i++)
                {
                    Console.Write((selection == i + 1 ? ">" : " ") + "|");
                    PrintReg(regs[i].ToStringArr(), charsPerField);
                }

                inputkey = Console.ReadKey(true);

                if (inputkey.Key == ConsoleKey.Escape)
                {
                    Location = MenuLocation.ActionMenu;
                    exit = true;
                }
                else if (inputkey.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input.Remove(input.Length - 1, 1);
                    }
                }
                else if (inputkey.Key == ConsoleKey.UpArrow || inputkey.Key == ConsoleKey.DownArrow)
                {
                    selection += (inputkey.Key == ConsoleKey.DownArrow) ? 1 : -1;
                    selection += regs.Length + 1;
                    selection %= regs.Length + 1;
                }
                else if (inputkey.Key == ConsoleKey.Enter && selection != 0)
                {
                    exit = true;
                    rawdata = regs[selection - 1].ToStringArr();
                    Location = MenuLocation.ModifyData;
                    id = GetActualTable().Where(x => x.GetData().ToStringArr()[0] == rawdata[0]).Select(x => x.Id).First();
                }
                else if ((Char.IsLetterOrDigit(inputkey.KeyChar) || Char.IsPunctuation(inputkey.KeyChar) || Char.IsWhiteSpace(inputkey.KeyChar)) && inputkey.Key != ConsoleKey.Enter)
                {
                    input.Append(inputkey.KeyChar);
                    selection = 0;
                }

                Console.Clear();

            } while (!exit);

            return rawdata;
        }
        private string ChoosePropFindBy<T>() where T : IRawData, new()
        {
            bool valid;
            int input;
            var propNames = new T().PropNames();

            do
            {
                Console.WriteLine("* Choose field to find by\n");
                for (int i = 0; i < propNames.Length; i++)
                {
                    Console.WriteLine($"* {i+1}) {propNames[i]}");
                }
                Console.WriteLine($"* {propNames.Length + 1}) BACK");
                input = Console.ReadKey(true).KeyChar - '1';
                valid = input >= 0 && input <= propNames.Length;
                if (valid)
                {
                    if (input == propNames.Length)
                    {
                        Location = MenuLocation.TableMenu;
                        return null;
                    }
                }
                Console.Clear();
            } while (!valid);

            Location = MenuLocation.ChooseReg;

            return propNames[input];
        }

        #endregion

        #region Add

        private void AddAuthor()
        {
            var data = AddData<AuthorRawData>();
            Console.Clear();
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Author author = new Author(data);
                    context.Authors.Add(author);
                    Console.WriteLine("Added author " + author.Name);
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
                
        }

        private void AddComic()
        {
            var data = AddData<ComicRawData>();
            Console.Clear();
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Comic comic = new Comic(data);
                    context.Comics.Add(comic);

                    context.SaveChanges();

                    //Add functions
                    foreach (var author in Author.SeparateAuthors(data.Authors))
                    {
                        var authorRole = Author.SeparateAuthorRole(author);
                        FunctionRawData function = new FunctionRawData()
                        {
                            Author = authorRole[0],
                            Role = authorRole[1],
                            Comic = comic.Title
                        };
                        context.Functions.Add(new Function(function));
                    }

                    Console.WriteLine("Added comic " + comic.Title);
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }

        private void AddCategory()
        {
            var data = AddData<CategoryRawData>();
            Console.Clear();
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Category category = new Category(data);
                    context.Categories.Add(category);
                    Console.WriteLine("Added category " + category.Name);
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
            
        }
        #endregion

        #region Read
        private void ReadAuthors()
        {
            Console.Clear();
            List<AuthorRawData> data = new List<AuthorRawData>();
            foreach (var single in context.Authors)
            {
                data.Add(new AuthorRawData(single.GetData()));
            }

            ReadData(data);
        }
        private void ReadComics()
        {
            Console.Clear();
            List<ComicRawData> data = new List<ComicRawData>();
            foreach (var single in context.Comics)
            {
                data.Add(new ComicRawData(single.GetData()));
            }

            ReadData(data);
        }
        private void ReadCategories()
        {
            Console.Clear();
            List<CategoryRawData> data = new List<CategoryRawData>();
            foreach (var single in context.Categories)
            {
                data.Add(new CategoryRawData(single.GetData()));
            }

            ReadData(data);
        }
        #endregion

        #region Update
        private void UpdateAuthor()
        {
            int id;
            Console.Clear();
            var data = FindData<AuthorRawData>(out id);
            if (Location != MenuLocation.ModifyData)
                return;
            ModifyData<AuthorRawData>(data);
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    context.Authors.Find(id).SetData(data.Convert());
                    context.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Updated " + data.Name);
                } else
                {
                    Console.Clear();
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
            Console.Clear();
        }

        private void UpdateComic()
        {
            Console.Clear();
            Console.WriteLine("Update comic");
            Console.ReadKey(true);
        }

        private void UpdateCategory()
        {
            Console.Clear();
            Console.WriteLine("Update category");
            Console.ReadKey(true);
        }
        #endregion

        #region Delete
        private void DeleteAuthor()
        {
            Console.Clear();
            Console.WriteLine("Delete author");
            Console.ReadKey(true);
        }

        private void DeleteComic()
        {
            Console.Clear();
            Console.WriteLine("Delete comic");
            Console.ReadKey(true);
        }

        private void DeleteCategory()
        {
            Console.Clear();
            Console.WriteLine("Delete category");
            Console.ReadKey(true);
        }
        #endregion

    }
}