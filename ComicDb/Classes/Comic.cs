using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicDb.Classes
{
    public class Comic
    {
        /*  Obtener roles de cada autor:
         *      Cómo obtener autores:
         *         "Autor1 / Rol, Autor2 / Rol ..."
         *      Input -> Split(,) -> Authors -> Foreach(Trim())
         *          Obtienes {"Autor1 / Rol", "Autor2 / Rol", ...}
         *      Authors -> Foreach -> Split('/') -> authorRole -> Foreach(Trim())
         *          Obtienes de cada uno {"Autor1", "Rol"}
         */

        public int Id { get; set; }
        private ComicData _data { get; set; }

        public string Title
        {
            get { return _data.Title; }
            set { _data.Title = value; }
        }
        public string Description
        {
            get { return _data.Description; }
            set { _data.Description = value; }
        }
        public DateTime PublicationDate
        {
            get { return _data.PublicationDate; }
            set { _data.PublicationDate = value; }
        }
        public int Pages
        {
            get { return _data.Pages; }
            set { _data.Pages = value; }
        }

        public ICollection<Function> Functions
        {
            get { return _data.Functions; }
            set { _data.Functions = value; }
        }
        public Category Category
        {
            get { return _data.Category; }
            set { _data.Category = value; }
        }

        public Comic()
        {
            _data = new ComicData();
        }
        public Comic(ComicData data) : this()
        {
            _data = data;
        }
        public Comic(ComicRawData data) : this(data.Convert())
        {

        }
    }

    public class ComicData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Pages { get; set; }

        public ICollection<Function> Functions { get; set; }
        public Category Category { get; set; }
    }

    public class ComicRawData
    {
        public ComicRawData(string title, string description, string publicationDate, 
            string pages, string authors, string category)
        {
            Title = title;
            Description = description;
            PublicationDate = publicationDate;
            Pages = pages;
            Authors = authors;
            Category = category;
        }

        public ComicRawData(ComicData data)
        {
            Title = data.Title;
            Description = data.Description;
            PublicationDate = data.PublicationDate.ToString("dd/mm/yyyy");
            Pages = data.Pages.ToString();
            Category = data.Category.Name;
            Authors = String.Join(", ", data.Functions);
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
                Category = ComicContext.db.Categories.Where(x => x.Name == Category).FirstOrDefault()
            };
        }
    }
}
