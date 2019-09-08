using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicStoreDb.Classes
{
    public class Category : ITable
    {
        public int Id { get; set; }
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

        public IData GetData()
        {
            return data;
        }

        public void SetData(IData data)
        {
            this.data = (CategoryData)data;
        }

        public bool Match(string property, string value)
        {
            CategoryRawData rawdata = new CategoryRawData(data);
            return rawdata.GetType().GetProperty(property).GetValue(rawdata).ToString().ToUpper().Contains(value.ToUpper());
        }
    }

    public class CategoryData : IData
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Comic> Comics { get; set; }

        public IRawData Convert()
        {
            return new CategoryRawData(this);
        }

        public string[] ToStringArr()
        {
            return Convert().PropValues();
        }

        public void Update(IRawData rawdata)
        {
            var data = ((CategoryRawData)rawdata).Convert();

            Name = data.Name;
            Description = data.Description;
        }
    }

    public class CategoryRawData : IRawData
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

        public CategoryRawData(IData data) : this((CategoryData)data)
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
            Name = arr[0];
            Description = arr[1];
        }

        public bool Check()
        {
            return true;
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