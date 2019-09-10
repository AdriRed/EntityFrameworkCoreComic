using ComicStoreDb.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicStoreDb
{
    public class ComicsInteraction
    {
        public enum MenuLocation
        {
            MainMenu,
            TableMenu,
            ActionMenu,
            StatisticsMenu,
            StatisticViewer,
            CheckData,
            ChooseReg,
            SelectedData
        }

        public enum Table
        {
            Authors,
            Categories,
            Comics,
            PublishingHouses,
            Countries
        }

        public enum RegStyle
        {
            VerticalFields,
            HorizontalTable
        }

        private Action[,] actions;
        private Action[] statistics;
        public MenuLocation Location;
        public Table ActualTable;
        private bool exit;
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
                UpdateContext();
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

                    case MenuLocation.StatisticsMenu:
                        menus.StatisticsMenu();
                        break;

                    case MenuLocation.StatisticViewer:
                        statistics[(int)menus.statisticSelection]();
                        Location = MenuLocation.MainMenu;
                        break;

                    default:
                        exit = true;
                        break;
                }
                Console.Clear();
            } while (!exit);

            context.Dispose();
        }

        private void UpdateContext()
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
            foreach (var item in context.Countries)
            {
                context.Entry(item).Collection(x => x.PublishingHouses).Load();
                context.Entry(item).Collection(x => x.Authors).Load();
            }
            foreach (var item in context.PublishingHouses)
            {
                context.Entry(item).Collection(x => x.Comics).Load();
            }
        }

        private void AssignActions()
        {
            actions = new Action[4, 3];
            actions[0, 0] = new Action(AddAuthor);
            actions[1, 0] = new Action(ReadAuthors);
            actions[2, 0] = new Action(UpdateAuthor);
            actions[3, 0] = new Action(DeleteAuthor);

            actions[0, 1] = new Action(AddCategory);
            actions[1, 1] = new Action(ReadCategories);
            actions[2, 1] = new Action(UpdateCategory);
            actions[3, 1] = new Action(DeleteCategory);

            actions[0, 2] = new Action(AddComic);
            actions[1, 2] = new Action(ReadComics);
            actions[2, 2] = new Action(UpdateComic);
            actions[3, 2] = new Action(DeleteComic);

            statistics = new Action[4];
            statistics[0] = new Action(ComicsPerCategory);
            statistics[1] = new Action(ComicsPerAuthor);
            statistics[2] = new Action(LongestComics);
            statistics[3] = new Action(AuthorsPerNationality);
        }

        #region Generic

        public T AddData<T>() where T : RawData, new()
        {
            T data = new T();
            ModifyData(data);
            return data;
        }

        public void ModifyData<T>(T data) where T : RawData
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
                    var selector = (i == selection) ? ">" : "*";
                    Console.WriteLine($"{selector} {propNames[i]}: {propValues[i]}");
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
                    if (selection >= propNames.Length)
                    {
                        exit = true;
                        if (selection == propNames.Length)
                            Location = MenuLocation.CheckData;
                        else
                            Location = MenuLocation.TableMenu;
                    }
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

        private void PrintHeaders(string[] headers, string pre, string post)
        {
            int charsPerField = Console.WindowWidth / headers.Length - pre.Length - post.Length - 1;
            foreach (var item in headers)
            {
                Console.Write(pre);
                if (item.Length <= charsPerField)
                    Console.Write(item.PadRight(charsPerField));
                else
                    Console.Write(item.PadRight(charsPerField).Substring(0, charsPerField - 3) + "...");
                Console.Write(post);
            }
            Console.WriteLine();
            PrintSeparator();
        }

        public void PrintReg(RawData rawData, string pre, string post, RegStyle style)
        {
            var propVals = rawData.PropValues();

            switch (style)
            {
                case RegStyle.HorizontalTable:

                    int charsPerField = Console.WindowWidth / propVals.Length - pre.Length - post.Length - 1;
                    foreach (var value in propVals)
                    {
                        Console.Write(pre);
                        if (value.Length <= charsPerField)
                            Console.Write(value.PadRight(charsPerField));
                        else
                            Console.Write(value.PadRight(charsPerField).Substring(0, charsPerField - 3) + "...");
                        Console.Write(post);
                    }
                    Console.WriteLine();
                    break;

                case RegStyle.VerticalFields:

                    var propNames = rawData.PropNames();
                    for (int i = 0; i < propVals.Length; i++)
                    {
                        Console.WriteLine(pre + propNames[i] + ": " + propVals[i]);
                    }

                    break;
            }
        }

        public void ReadData<T>(ICollection<T> data) where T : RawData, new()
        {
            string[] headers = (new T()).PropNames();
            int charsPerField = Console.WindowWidth / headers.Length - 1;

            PrintHeaders(headers, String.Empty, "|");

            foreach (var item in data)
            {
                PrintReg(item, String.Empty, "|", RegStyle.HorizontalTable);
            }

            Console.ReadKey(true);
        }

        public IQueryable<ComicStoreDb.Table> GetActualTable()
        {
            var table = (IQueryable)context.GetType().GetProperty(ActualTable.ToString()).GetValue(context);
            return table.Cast<ComicStoreDb.Table>();
        }

        public Data[] Query(string property, string value)
        {
            var query = GetActualTable().Where(x => x.Match(property, value)).Select(x => x.GetData());

            return query.ToArray();
        }

        public T FindData<T>(out int id) where T : RawData, new()
        {
            var prop = ChoosePropFindBy<T>();
            id = -1;
            if (Location != MenuLocation.ChooseReg)
                return default;
            var reg = ChooseReg<T>(prop, out id);
            if (Location != MenuLocation.SelectedData)
                return default;
            return (T)reg;
        }

        private RawData ChooseReg<T>(string findBy, out int id) where T : RawData, new()
        {
            var propNames = new T().PropNames();
            int selection = 0;
            id = -1;
            StringBuilder input = new StringBuilder(String.Empty);
            ConsoleKeyInfo inputkey;
            Data[] regs;
            bool exit = false;
            RawData rawdata = null;

            do
            {
                regs = (input.ToString() == String.Empty) ?
                    GetActualTable().Select(x => x.GetData()).ToArray() :
                    Query(findBy, input.ToString());

                Console.WriteLine("-|" + findBy + ": " + input.ToString().PadRight(Console.WindowWidth - findBy.Length - 5));

                Console.Write("-|");
                PrintHeaders(propNames, String.Empty, "|");

                for (int i = 0; i < regs.Length; i++)
                {
                    var selector = (selection == i + 1 ? ">" : " ");
                    Console.Write(selector);
                    PrintReg(regs[i].Convert(), "|", "", RegStyle.HorizontalTable);
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
                    rawdata = regs[selection - 1].Convert();
                    Location = MenuLocation.SelectedData;
                    id = GetActualTable().Where(x => x.GetData().Convert().Equals(rawdata)).Select(x => x.Id).First();
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

        private string ChoosePropFindBy<T>() where T : RawData, new()
        {
            bool valid;
            int input;
            var propNames = new T().PropNames();

            do
            {
                Console.WriteLine("* Choose field to find by\n");
                for (int i = 0; i < propNames.Length; i++)
                {
                    Console.WriteLine($"* {i + 1}) {propNames[i]}");
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

        private bool EnsureDelete(RawData reg)
        {
            bool exit = false;
            bool ensure = false;
            int selection = 0;
            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("* DO YOU WANT TO DELETE THIS?");
                Console.WriteLine("*");
                PrintReg(reg, "* ", "", RegStyle.VerticalFields);
                Console.WriteLine("*");
                Console.WriteLine($"* {(selection == 0 ? "[NO]" : " NO ")} {(selection == 1 ? "[YES]" : " YES ")}");
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow)
                {
                    selection += (key.Key == ConsoleKey.RightArrow) ? 1 : -1;
                    selection += 2;
                    selection %= 2;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    exit = true;
                    ensure = selection == 1;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    exit = true;
                    Location = MenuLocation.MainMenu;
                }
                Console.Clear();
            } while (!exit);

            return ensure;
        }

        #endregion Generic

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

        private void AddPublishingHouse()
        {
            var data = AddData<PublishingHouseRawData>();
            Console.Clear();
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    PublishingHouse p = new PublishingHouse(data);
                    context.PublishingHouses.Add(p);
                    Console.WriteLine("Added " + p.Name);
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }

        private void AddCountry()
        {
            var data = AddData<CountryRawData>();
            Console.Clear();
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Country c = new Country(data);
                    context.Countries.Add(c);
                    Console.WriteLine("Added " + c.Name);
                } else
                {
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }

        #endregion Add

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

        private void ReadPublishingHouses()
        {
            Console.Clear();
            List<PublishingHouseRawData> data = new List<PublishingHouseRawData>();
            foreach (var single in context.PublishingHouses)
            {
                data.Add(new PublishingHouseRawData(single.GetData()));
            }

            ReadData(data);
        }

        private void ReadCountries()
        {
            Console.Clear();
            List<CountryRawData> data = new List<CountryRawData>();
            foreach (var single in context.Countries)
            {
                data.Add(new CountryRawData(single.GetData()));
            }

            ReadData(data);
        }

        #endregion Read

        #region Update

        private void UpdateAuthor()
        {
            int id;
            Console.Clear();
            var data = FindData<AuthorRawData>(out id);
            if (Location != MenuLocation.SelectedData)
                return;
            ModifyData(data);
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Author target = context.Authors.Find(id);
                    target.GetData().Update(data);

                    context.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Updated " + data.Name);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }

        private void UpdateComic()
        {
            int id;
            Console.Clear();
            var data = FindData<ComicRawData>(out id);
            if (Location != MenuLocation.SelectedData)
                return;
            ModifyData(data);
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Comic target = context.Comics.Find(id);

                    //Reset Functions
                    context.Functions.RemoveRange(context.Functions.Where(x => x.Comic.Id == id));

                    foreach (var item in Author.SeparateAuthors(data.Authors))
                    {
                        FunctionRawData newFunction;
                        string[] separated = Author.SeparateAuthorRole(item);
                        newFunction = new FunctionRawData(separated[0], target.Title, separated[1]);
                        context.Functions.Add(new Function(newFunction));
                    }

                    target.GetData().Update(data);
                    context.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Updated " + data.Title);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }

        private void UpdateCategory()
        {
            int id;
            Console.Clear();
            var data = FindData<CategoryRawData>(out id);
            if (Location != MenuLocation.SelectedData)
                return;
            ModifyData(data);
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Category target = context.Categories.Find(id);

                    target.GetData().Update(data);

                    context.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Updated " + data.Name);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }

        private void UpdatePublishingHouse()
        {
            int id;
            Console.Clear();
            var data = FindData<PublishingHouseRawData>(out id);
            if (Location != MenuLocation.SelectedData)
                return;
            ModifyData(data);
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    PublishingHouse target = context.PublishingHouses.Find(id);

                    target.GetData().Update(data);

                    context.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Updated " + data.Name);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }
        private void UpdateCoutnry()
        {
            int id;
            Console.Clear();
            var data = FindData<CountryRawData>(out id);
            if (Location != MenuLocation.SelectedData)
                return;
            ModifyData(data);
            if (Location == MenuLocation.CheckData)
            {
                if (data.Check())
                {
                    Country target = context.Countries.Find(id);

                    target.GetData().Update(data);

                    context.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Updated " + data.Name);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("ERROR");
                }
                Console.ReadKey(true);
            }
        }
        #endregion Update

        #region Delete

        private void DeleteAuthor()
        {
            int id;
            Console.Clear();
            string field = ChoosePropFindBy<AuthorRawData>();
            var reg = ChooseReg<AuthorRawData>(field, out id);
            if (Location == MenuLocation.SelectedData)
                if (EnsureDelete(reg))
                {
                    Author target = context.Authors.Find(id);

                    if (target.Functions.Count > 0)
                    {
                        context.Authors.Remove(target);

                        context.SaveChanges();
                        Console.WriteLine("Deleted " + target.Name);
                        Console.ReadKey(true);
                    } else
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine($"Some comics already have this Author ({target.Name})");
                        foreach (var item in target.Functions)
                        {
                            var data = new FunctionRawData(target.GetData());
                            Console.WriteLine($"\t {data.Comic} as {data.Role}");
                        }
                    }
                    
                }
        }

        private void DeleteComic()
        {
            int id;
            Console.Clear();
            string field = ChoosePropFindBy<ComicRawData>();
            var reg = ChooseReg<ComicRawData>(field, out id);
            if (Location == MenuLocation.SelectedData)
                if (EnsureDelete(reg))
                {
                    Comic target = context.Comics.Find(id);

                    //Cascade ok
                    context.Functions.RemoveRange(context.Functions.Where(x => x.Comic == target));
                    context.Comics.Remove(target);

                    context.SaveChanges();
                    Console.WriteLine("Deleted " + target.Title);
                    Console.ReadKey(true);
                }
        }

        private void DeleteCategory()
        {
            int id;
            Console.Clear();
            string field = ChoosePropFindBy<CategoryRawData>();
            var reg = ChooseReg<CategoryRawData>(field, out id);
            if (Location == MenuLocation.SelectedData)
                if (EnsureDelete(reg))
                {
                    Category target = context.Categories.Find(id);
                    if (target.Comics.Count > 0)
                    {
                        context.Categories.Remove(target);

                        context.SaveChanges();
                        Console.WriteLine("Deleted " + target.Name);

                    } else
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine($"Some comics already have this category ({target.Name})!");
                        foreach (var item in target.Comics)
                        {
                            Console.WriteLine($"\t{item.Title}");
                        }
                    }
                    Console.ReadKey(true);
                }
        }

        private void DeletePublishingHouse()
        {
            int id;
            Console.Clear();
            string field = ChoosePropFindBy<AuthorRawData>();
            var reg = ChooseReg<AuthorRawData>(field, out id);
            if (Location == MenuLocation.SelectedData)
                if (EnsureDelete(reg))
                {
                    PublishingHouse target = context.PublishingHouses.Find(id);

                    if (target.Comics.Count > 0)
                    {
                        context.PublishingHouses.Remove(target);

                        context.SaveChanges();
                        Console.WriteLine("Deleted " + target.Name);
                    } else
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine("Some comics already have this Publishing House");
                        foreach (var item in target.Comics)
                        {
                            Console.WriteLine($"\t{item.Title}");
                        }
                    }
                    Console.ReadKey(true);
                }
        }

        private void DeleteCountry()
        {
            int id;
            Console.Clear();
            string field = ChoosePropFindBy<ComicRawData>();
            var reg = ChooseReg<ComicRawData>(field, out id);
            if (Location == MenuLocation.SelectedData)
                if (EnsureDelete(reg))
                {
                    Country target = context.Countries.Find(id);

                    if (target.Authors.Count > 0 && target.PublishingHouses.Count > 0)
                    {
                        context.Countries.Remove(target);

                        context.SaveChanges();
                        Console.WriteLine("Deleted " + target.Name);
                    }
                    else
                    {
                        Console.WriteLine("ERROR");
                        if (target.Authors.Count > 0)
                        {
                            Console.WriteLine($"Some authors are from this Country ({target.Name})");
                            foreach (var item in target.Authors)
                            {
                                Console.WriteLine($"\t{item.Name}");
                            }
                        }

                        if (target.Authors.Count > 0)
                        {
                            Console.WriteLine($"Some publishing houses are from this Country ({target.Name})");
                            foreach (var item in target.PublishingHouses)
                            {
                                Console.WriteLine($"\t{item.Name}");
                            }
                        }
                    }
                    Console.ReadKey(true);
                }
        }

        #endregion Delete

        #region Statistics

        private void ComicsPerCategory()
        {
            var query = context.Comics.GroupBy(x => x.Category);
            foreach (var category in query)
            {
                int ncomics = category.Count();
                Console.WriteLine(category.Key.Name + ": " + ncomics);
            }

            Console.ReadKey(true);
        }

        private void ComicsPerAuthor()
        {
            var query = from comic in context.Comics
                        from function in context.Functions
                        from author in context.Authors
                        where function.Comic == comic
                        where function.Author == author
                        group function by author into authorGroup
                        select authorGroup;

            foreach (var author in query)
            {
                Console.WriteLine(author.Key.Name + ": " + author.Count());
                foreach (var comic in author)
                {
                    Console.WriteLine("\t" + comic.Comic.Title + " as " + comic.Role);
                }
            }

            Console.ReadKey(true);
        }

        private void LongestComics()
        {
            var query = (from comic in context.Comics
                        orderby comic.Pages descending
                        select comic).Take(3);

            foreach (var item in query)
            {
                Console.WriteLine(item.Title + ": " + item.Pages);
            }

            Console.ReadKey(true);
        }

        private void AuthorsPerNationality()
        {
            var query = from author in context.Authors
                        group author by author.Nationality into nationality
                        select nationality;

            foreach (var nationality in query)
            {
                Console.WriteLine(nationality.Key + ": " + nationality.Count());
                foreach (var author in nationality)
                {
                    Console.WriteLine("\t" + author.Name);
                }
            }

            Console.ReadKey(true);
        }

        #endregion Statistics
    }
}