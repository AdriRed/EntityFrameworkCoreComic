using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicDb.Classes
{
    public class Function
    {
        public int Id { get; set; }
        private FunctionData _data { get; set; }

        public string Role
        {
            get { return _data.Role; }
            set { _data.Role = value; }
        }

        public Comic Comic
        {
            get { return _data.Comic; }
            set { _data.Comic = value; }
        }
        public Author Author
        {
            get { return _data.Author; }
            set { _data.Author = value; }
        }

        public Function()
        {
            _data = new FunctionData();
        }
        public Function(FunctionData data) : this()
        {
            _data = data;
        }
        public Function(FunctionRawData data) : this(data.Convert())
        {

        }
    }

    public class FunctionData
    {
        public Comic Comic { get; set; }
        public Author Author { get; set; }
        public string Role { get; set; }
    }

    public class FunctionRawData
    {
        public FunctionRawData(string author, string comic, string role)
        {
            Author = author;
            Comic = comic;
            Role = role; 
        }

        public FunctionRawData(FunctionData data)
        {
            Author = data.Author.Name;
            Comic = data.Comic.Title;
            Role = data.Role;
        }

        public string Author { get; set; }
        public string Comic { get; set; }
        public string Role { get; set; }

        public FunctionData Convert()
        {
            return new FunctionData()
            {
                Author = ComicContext.db.Authors.Where(x => x.Name == Author).First(),
                Comic = ComicContext.db.Comics.Where(x => x.Title == Comic).First(),
                Role = Role
            };
        }
    }
}
