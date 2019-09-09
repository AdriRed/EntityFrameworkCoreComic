using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicStoreDb.Classes
{
    public class Country : Table
    {
        private CountryData data { get; set; }

        public string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }

        public string Code
        {
            get { return data.Code; }
            set { data.Code = value; }
        }

        public virtual ICollection<Author> Authors
        {
            get { return data.Authors; }
            set { data.Authors = value; }
        }
        
        public virtual ICollection<PublishingHouse> PublishingHouses
        {
            get { return data.PublishingHouses; }
            set { data.PublishingHouses = value; }
        }

        public override Data GetData()
        {
            return data;
        }

        public override bool Match(string property, string value)
        {
            throw new NotImplementedException();
        }

        public override void SetData(Data data)
        {
            this.data = (CountryData)data;
        }

        public static bool Exist(string name)
        {
            return ComicsContext.Instance.Countries.Any(x => x.Name.ToUpper() == name.ToUpper());
        }
    }

    public class CountryData : Data
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public ICollection<Author> Authors { get; set; }
        public ICollection<PublishingHouse> PublishingHouses { get; set; }
    }

    public class CountryRawData : RawData
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
