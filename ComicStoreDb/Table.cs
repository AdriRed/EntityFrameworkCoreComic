namespace ComicStoreDb
{
    public abstract class Table
    {
        public int Id { set; get; }

        public abstract Data GetData();

        public abstract void SetData(Data data);

        public abstract bool Match(string property, string value);
    }
}