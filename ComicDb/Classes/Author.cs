using System;
using System.Collections.Generic;

namespace ComicDb.Classes
{
    public class Author
    {
        public int Id { get; set; }
        private AuthorData _data { get; set; }
        
        public string Name
        {
            get { return _data.Name; }
            set { _data.Name = value; }
        }
        public string Nationality
        {
            get { return _data.Nationality; }
            set { _data.Nationality = value; }
        }
        public DateTime Birth
        {
            get { return _data.Birth; }
            set { _data.Birth = value; }
        }

        public ICollection<Function> Functions
        {
            get { return _data.Functions; }
            set { _data.Functions = value; }
        }

        public Author()
        {
            _data = new AuthorData();
        }
        public Author(AuthorData data) : this()
        {
            _data = data;
        }
        public Author(AuthorRawData data) : this(data.Convert())
        {

        }
    }

    public class AuthorData
    {
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public string Nationality { get; set; }
        public ICollection<Function> Functions { get; set; }
    }

    public class AuthorRawData
    {
        public AuthorRawData(string name, string birth, string nationality)
        {
            Name = name;
            Birth = birth;
            Nationality = nationality;
        }

        public AuthorRawData(AuthorData data)
        {
            Name = data.Name;
            Nationality = data.Nationality;
            Birth = data.Birth.ToString("yyyy");
        }

        public string Name { get; set; }
        public string Birth { get; set; }
        public string Nationality { get; set; }

        public AuthorData Convert()
        {
            return new AuthorData()
            {
                Name = Name,
                Nationality = Nationality,
                Birth = new DateTime(Int32.Parse(Birth), 1, 1)
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
    
