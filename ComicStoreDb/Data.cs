namespace ComicStoreDb
{
    public abstract class Data
    {
        public string[] ToStringArr()
        {
            return Convert().PropValues();
        }
        public abstract void Update(RawData rawdata);
        public abstract RawData Convert();
    }
}