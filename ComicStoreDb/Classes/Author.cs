using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicStoreDb.Classes
{
    public class Author : ITable
    {
        public int Id { get; set; }
        private AuthorData data { get; set; }

        public string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }

        public string Nationality
        {
            get { return data.Nationality; }
            set { data.Nationality = value; }
        }

        public DateTime Birth
        {
            get { return data.Birth; }
            set { data.Birth = value; }
        }

        public virtual ICollection<Function> Functions
        {
            get { return data.Functions; }
            set { data.Functions = value; }
        }

        public Author()
        {
            data = new AuthorData();
        }

        public Author(AuthorData data) : this()
        {
            this.data = data;
        }

        public Author(AuthorRawData data) : this(data.Convert())
        {
        }

        public static bool Exist(string name)
        {
            return ComicsContext.Instance.Authors.Any(x => x.Name.ToUpper() == name.ToUpper());
        }

        public static string[] SeparateAuthorRole(string authorRole)
        {
            string[] separated = authorRole.Split('/');
            for (int i = 0; i < separated.Length; i++)
            {
                separated[i] = separated[i].Trim();
            }
            return separated;
        }

        public static string[] SeparateAuthors(string allAuthors)
        {
            string[] separated = allAuthors.Split(',');
            for (int i = 0; i < separated.Length; i++)
            {
                separated[i] = separated[i].Trim();
            }
            return separated;
        }

        public IData GetData()
        {
            return data;
        }

        public void SetData(IData data)
        {
            this.data = (AuthorData)data;
        }

        public bool Match(string property, string value)
        {
            AuthorRawData rawdata = new AuthorRawData(data);
            return rawdata.GetType().GetProperty(property).GetValue(rawdata).ToString().ToUpper().Contains(value.ToUpper());
        }
    }

    public class AuthorData : IData
    {
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public string Nationality { get; set; }
        public ICollection<Function> Functions { get; set; }

        public IRawData Convert()
        {
            return new AuthorRawData(this);
        }

        public string[] ToStringArr()
        {
            return Convert().PropValues();
        }

        public void Update(IRawData rawdata)
        {
            var data = ((AuthorRawData)rawdata).Convert();

            Name = data.Name;
            Birth = data.Birth;
            Nationality = data.Nationality;
        }
    }

    public class AuthorRawData : IRawData
    {
        public AuthorRawData()
        {
            Name = String.Empty;
            Birth = String.Empty;
            Nationality = String.Empty;
        }

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

        public AuthorRawData(IData data) : this((AuthorData)data)
        {
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

        public string[] PropNames()
        {
            return GetType().GetProperties().Select(x => x.Name).ToArray();
        }

        public string[] PropValues()
        {
            return Array.ConvertAll(
                                GetType().GetProperties().Select(x => x.GetValue(this)).ToArray(),
                                x => x?.ToString() ?? String.Empty)
                ;
        }

        public void ConvertFromStringArr(string[] arr)
        {
            Name = arr[0];
            Birth = arr[1];
            Nationality = arr[2];
        }

        public bool Check()
        {
            int foo;
            return Int32.TryParse(Birth, out foo) && foo >= 0 && foo < 10000;
        }

        public override bool Equals(object obj)
        {
            return obj is AuthorRawData data &&
                   Name == data.Name &&
                   Birth == data.Birth &&
                   Nationality == data.Nationality;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Birth, Nationality);
        }
    }
}