namespace ComicStoreDb
{
    public interface IRawData
    {
        string[] PropNames();

        string[] PropValues();

        void ConvertFromStringArr(string[] arr);

        bool Check();
    }
}