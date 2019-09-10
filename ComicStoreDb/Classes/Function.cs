using System;
using System.Linq;

namespace ComicStoreDb.Classes
{
    public class Function : Table
    {
        private FunctionData data { get; set; }

        public string Role
        {
            get { return data.Role; }
            set { data.Role = value; }
        }

        public virtual Comic Comic
        {
            get { return data.Comic; }
            set { data.Comic = value; }
        }

        public virtual Author Author
        {
            get { return data.Author; }
            set { data.Author = value; }
        }

        public Function()
        {
            data = new FunctionData();
        }

        public Function(FunctionData data) : this()
        {
            this.data = data;
        }

        public Function(FunctionRawData data) : this((FunctionData)data.Convert())
        {
        }

        public override Data GetData()
        {
            return data;
        }

        public override void SetData(Data data)
        {
            this.data = (FunctionData)data;
        }

        public override bool Match(string property, string value)
        {
            FunctionRawData rawdata = new FunctionRawData(data);
            return rawdata.GetType().GetProperty(property).GetValue(rawdata).ToString().ToUpper().Contains(value.ToUpper());
        }
    }

    public class FunctionData : Data
    {
        public Comic Comic { get; set; }
        public Author Author { get; set; }
        public string Role { get; set; }

        public override RawData Convert()
        {
            return new FunctionRawData(this);
        }

        public override void Update(RawData data)
        {
            throw new NotImplementedException();
        }
    }

    public class FunctionRawData : RawData
    {
        public FunctionRawData()
        {
            Author = String.Empty;
            Comic = String.Empty;
            Role = String.Empty;
        }

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

        public FunctionRawData(Data data) : this((FunctionData)data)
        {
        }

        public string Author { get; set; }
        public string Comic { get; set; }
        public string Role { get; set; }

        public override Data Convert()
        {
            return new FunctionData()
            {
                Author = ComicsContext.Instance.Authors.Where(x => x.Name == Author).First(),
                Comic = ComicsContext.Instance.Comics.Where(x => x.Title == Comic).First(),
                Role = Role
            };
        }

        public override void ConvertFromStringArr(string[] arr)
        {
            Author = arr[0];
            Comic = arr[1];
            Role = arr[2];
        }

        public override bool Check()
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is FunctionRawData data &&
                   Author == data.Author &&
                   Comic == data.Comic &&
                   Role == data.Role;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Author, Comic, Role);
        }

    }
}