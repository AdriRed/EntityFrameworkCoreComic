namespace ComicStoreDb
{
    public interface IData
    {
        string[] ToStringArr();

        void Update(IRawData rawdata);

        IRawData Convert();
    }
}