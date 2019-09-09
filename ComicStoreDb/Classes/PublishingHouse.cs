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
            throw new NotImplementedException();
        }

        public override void SetData(Data data)
        {
            throw new NotImplementedException();
        }

        public override bool Match(string property, string value)
        {
            throw new NotImplementedException();
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
    }

    public class PublishingHouseRawData : RawData
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
