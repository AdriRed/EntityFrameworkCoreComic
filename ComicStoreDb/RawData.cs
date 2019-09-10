using System;
using System.Linq;

namespace ComicStoreDb
{
    public abstract class RawData
    {
        public string[] PropNames()
        {
            return GetType().GetProperties().Select(x => x.Name).ToArray();
        }

        public string[] PropValues()
        {
            var type = GetType();
            //////////////////FALLA AQUI
                var props = type.GetProperties();
            //////////////////FALLA AQUI
            var query = props.Select(x => x.GetValue(this));
            var array = query.ToArray();
            var strarray = Array.ConvertAll(array, x => x?.ToString() ?? String.Empty);

            return strarray;
        }
        public abstract void ConvertFromStringArr(string[] arr);
        public abstract bool Check();

        public abstract Data Convert();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}