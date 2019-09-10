using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicStoreDb.Classes
{
    public class PublishingHouse : Table
    {
        private PublishingHouseData data { get; set; }

        public string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }

        public virtual Country Country
        {
            get { return data.Country; }
            set { data.Country = value; }
        }

        public override Data GetData()
        {
            return data;
        }

        public override void SetData(Data data)
        {
            this.data = (PublishingHouseData)data;
        }

        public override bool Match(string property, string value)
        {
            PublishingHouseRawData rawdata = new PublishingHouseRawData(data);
            return rawdata.GetType().GetProperty(property).GetValue(rawdata).ToString().ToUpper().Contains(value.ToUpper());
        }

        public static bool Exist(string name)
        {
            return ComicsContext.Instance.PublishingHouses.Any(x => x.Name.ToUpper() == name.ToUpper());
        }
    }

    public class PublishingHouseData : Data
    {
        public string Name { get; set; }
        public Country Country { get; set; }

        public override RawData Convert()
        {
            throw new NotImplementedException();
        }

        public override void Update(RawData rawdata)
        {
            var data = (PublishingHouseRawData)rawdata;
            Name = data.Name;
            Country = ComicsContext.Instance.Countries.Where(x => x.Name.ToUpper() == data.Country.ToUpper()).FirstOrDefault();
        }
    }

    public class PublishingHouseRawData : RawData
    {
        public PublishingHouseRawData()
        {
            Name = String.Empty;
            Country = String.Empty;
        }

        public PublishingHouseRawData(PublishingHouseData data)
        {
            Name = data.Name;
            Country = data.Country.Name;
        }
        public PublishingHouseRawData(string name, string country)
        {
            Name = name;
            Country = country;
        }

        public string Name { get; set; }
        public string Country { get; set; }

        public override bool Check()
        {
            return Name.Length > 0 && Classes.Country.Exist(Country);
        }

        public override void ConvertFromStringArr(string[] arr)
        {
            Name = arr[0];
            Country = arr[1];
        }
    }
}
