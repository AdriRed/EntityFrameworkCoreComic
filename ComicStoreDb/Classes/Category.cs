using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicStoreDb.Classes
{
    public class Category : Table
    {
        private CategoryData data { get; set; }

        public string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }

        public string Description
        {
            get { return data.Description; }
            set { data.Description = value; }
        }

        public virtual ICollection<Comic> Comics
        {
            get { return data.Comics; }
            set { data.Comics = value; }
        }

        public Category()
        {
            data = new CategoryData();
        }

        public Category(CategoryData data) : this()
        {
            this.data = data;
        }

        public Category(CategoryRawData data) : this(data.Convert())
        {
        }

        public static bool Exist(string categoryName)
        {
            return ComicsContext.Instance.Categories.Any(x => x.Name == categoryName);
        }

        public override Data GetData()
        {
            return data;
        }

        public override void SetData(Data data)
        {
            this.data = (CategoryData)data;
        }

        public override bool Match(string property, string value)
        {
            CategoryRawData rawdata = new CategoryRawData(data);
            return rawdata.GetType().GetProperty(property).GetValue(rawdata).ToString().ToUpper().Contains(value.ToUpper());
        }
    }

    public class CategoryData : Data
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Comic> Comics { get; set; }

        public override RawData Convert()
        {
            return new CategoryRawData(this);
        }

        public override void Update(RawData rawdata)
        {
            var data = ((CategoryRawData)rawdata).Convert();

            Name = data.Name;
            Description = data.Description;
        }
    }

    public class CategoryRawData : RawData
    {
        public CategoryRawData()
        {
            Name = String.Empty;
            Description = String.Empty;
        }

        public CategoryRawData(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public CategoryRawData(CategoryData data)
        {
            Name = data.Name;
            Description = data.Description;
        }

        public CategoryRawData(Data data) : this((CategoryData)data)
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public CategoryData Convert()
        {
            return new CategoryData()
            {
                Name = Name,
                Description = Description
            };
        }

        public override void ConvertFromStringArr(string[] arr)
        {
            Name = arr[0];
            Description = arr[1];
        }

        public override bool Check()
        {
            return Name.Length < 0;
        }

        public override bool Equals(object obj)
        {
            return obj is CategoryRawData data &&
                   Name == data.Name &&
                   Description == data.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description);
        }
    }
}