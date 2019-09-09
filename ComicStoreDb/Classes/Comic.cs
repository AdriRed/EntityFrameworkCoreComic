using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ComicStoreDb.Classes
{
    public class Comic : ITable
    {
        public int Id { get; set; }
        private ComicData data { get; set; }

        public string Title
        {
            get { return data.Title; }
            set { data.Title = value; }
        }

        public string Description
        {
            get { return data.Description; }
            set { data.Description = value; }
        }

        public DateTime PublicationDate
        {
            get { return data.PublicationDate; }
            set { data.PublicationDate = value; }
        }

        public int Pages
        {
            get { return data.Pages; }
            set { data.Pages = value; }
        }

        public virtual ICollection<Function> Functions
        {
            get { return data.Functions; }
            set { data.Functions = value; }
        }

        public virtual Category Category
        {
            get { return data.Category; }
            set { data.Category = value; }
        }

        public Comic()
        {
            data = new ComicData();
        }

        public Comic(ComicData data) : this()
        {
            this.data = data;
        }

        public Comic(ComicRawData data) : this(data.Convert())
        {
        }

        public static bool Exsit(string title)
        {
            return ComicsContext.Instance.Comics.Any(x => x.Title.ToUpper() == title.ToUpper());
        }

        public IData GetData()
        {
            return data;
        }

        public void SetData(IData data)
        {
            this.data = (ComicData)data;
        }

        public bool Match(string property, string value)
        {
            ComicRawData rawdata = new ComicRawData(data);
            return rawdata.GetType().GetProperty(property).GetValue(rawdata).ToString().ToUpper().Contains(value.ToUpper());
        }
    }

    public class ComicData : IData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Pages { get; set; }

        public ICollection<Function> Functions { get; set; }
        public Category Category { get; set; }

        public IRawData Convert()
        {
            return new ComicRawData(this);
        }

        public string[] ToStringArr()
        {
            return Convert().PropValues();
        }

        public void Update(IRawData rawdata)
        {
            var data = ((ComicRawData)rawdata).Convert();

            Title = data.Title;
            Description = data.Description;
            PublicationDate = data.PublicationDate;
            Pages = data.Pages;

            Category = data.Category;
        }
    }

    public class ComicRawData : IRawData
    {
        public ComicRawData()
        {
            Title = String.Empty;
            Description = String.Empty;
            PublicationDate = String.Empty;
            Pages = String.Empty;
            Authors = String.Empty;
            Category = String.Empty;
        }

        public ComicRawData(string title, string description, string publicationDate,
            string pages, string authors, string category) : this()
        {
            Title = title;
            Description = description;
            PublicationDate = publicationDate;
            Pages = pages;
            Authors = authors;
            Category = category;
        }

        public ComicRawData(ComicData data) : this()
        {
            List<string> functions = new List<string>();
            foreach (var item in data.Functions)
            {
                functions.Add(item.Author.Name + " / " + item.Role);
            }

            Title = data.Title;
            Description = data.Description;
            PublicationDate = data.PublicationDate.ToString("d");
            Pages = data.Pages.ToString();
            Category = data.Category.Name;
            Authors = String.Join(", ", functions);
        }

        public ComicRawData(IData data) : this((ComicData)data)
        {
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string PublicationDate { get; set; }
        public string Pages { get; set; }
        public string Authors { get; set; }
        public string Category { get; set; }

        public ComicData Convert()
        {
            return new ComicData()
            {
                Title = Title,
                Description = Description,
                PublicationDate = PublicationDate.Split('/').ToInt().ToDate(),
                Pages = Int32.Parse(Pages),
                Category = ComicsContext.Instance.Categories.Where(x => x.Name == Category).FirstOrDefault()
            };
        }

        public string[] PropNames()
        {
            return GetType().GetProperties().Select(x => x.Name).ToArray();
        }

        public string[] PropValues()
        {
            return Array.ConvertAll(
                                GetType().GetProperties().Select(x => x.GetValue(this)).ToArray(),
                                x => x?.ToString() ?? string.Empty)
                ;
        }

        public void ConvertFromStringArr(string[] arr)
        {
            Title = arr[0];
            Description = arr[1];
            PublicationDate = arr[2];
            Pages = arr[3];
            Authors = arr[4];
            Category = arr[5];
        }

        public bool Check()
        {
            DateTime date;
            int pages;

            if (!Int32.TryParse(Pages, out pages))
                return false;
            foreach (var item in Author.SeparateAuthors(Authors))
            {
                if (!Author.Exist(Author.SeparateAuthorRole(item)[0]))
                    return false;
            }

            return DateTime.TryParse(PublicationDate, out date) &&
                Int32.TryParse(Pages, out pages) &&
                Classes.Category.Exist(Category) &&
                pages > 0;
        }

        public override bool Equals(object obj)
        {
            return obj is ComicRawData data &&
                   Title == data.Title &&
                   Description == data.Description &&
                   PublicationDate == data.PublicationDate &&
                   Pages == data.Pages &&
                   Authors == data.Authors &&
                   Category == data.Category;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Description, PublicationDate, Pages, Authors, Category);
        }
    }
}