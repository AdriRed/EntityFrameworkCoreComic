﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicStoreDb.Classes
{
    public class Country : Table
    {
        public Country()
        {
            data = new CountryData();
        }

        public Country(CountryData data) : this()
        {
            this.data = data;
        }

        public Country(CountryRawData data) : this((CountryData)data.Convert())
        {
        }

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
            CountryRawData rawdata = new CountryRawData(data);
            return rawdata.GetType().GetProperty(property).GetValue(rawdata).ToString().ToUpper().Contains(value.ToUpper());
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

        public override RawData Convert()
        {
            return new CountryRawData(this);
        }

        public override void Update(RawData rawdata)
        {
            var data = (CountryRawData)rawdata;
            Name = data.Name;
            Code = data.Code;
        }
    }

    public class CountryRawData : RawData
    {
        public CountryRawData()
        {
            Name = String.Empty;
            Code = String.Empty;
        }

        public CountryRawData(CountryData data) : this()
        {
            Name = data.Name;
            Code = data.Code;
        }

        public CountryRawData(string name, string code) : this()
        {
            Name = name;
            Code = code;
        }

        public CountryRawData(Data data) : this((CountryData)data)
        {
        }

        public string Name { get; set; }
        public string Code { get; set; }

        public override bool Check()
        {
            return Name.Length > 0 && Code.Length > 0;
        }

        public override void ConvertFromStringArr(string[] arr)
        {
            Name = arr[0];
            Code = arr[1];
        }

        public override Data Convert()
        {
            return new CountryData()
            {
                Name = Name,
                Code = Code
            };
        }

        public override bool Equals(object obj)
        {
            return obj is CountryRawData data &&
                   Name == data.Name &&
                   Code == data.Code;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Code);
        }
    }
}