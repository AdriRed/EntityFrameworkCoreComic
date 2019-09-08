namespace ComicStoreDb
{
    public interface ITable
    {
        IData GetData();

        void SetData(IData data);

        bool Match(string property, string value);

        int Id { get; }
    }
}