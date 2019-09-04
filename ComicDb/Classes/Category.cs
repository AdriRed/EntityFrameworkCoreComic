using System;
using System.Collections.Generic;
using System.Text;

namespace ComicDb.Classes
{
    public class Category
    {
        public int Id { get; set; }
        private CategoryData _data { get; set; }

        public string Name
        {
            get { return _data.Name; }
            set { _data.Name = value; }
        }
        public string Description
        {
            get { return _data.Description; }
            set { _data.Description = value; }
        }

        public Category()
        {
            _data = new CategoryData();
        }
        public Category(CategoryData data) : this()
        {
            _data = data;
        }
        public Category(CategoryRawData data) : this(data.Convert())
        {

        }
    }
    public class CategoryData
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class CategoryRawData
    {
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
    }
}
