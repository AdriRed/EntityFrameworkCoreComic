namespace ComicStoreDb
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ComicsInteraction comicsInteraction = new ComicsInteraction();
            comicsInteraction.Start();

            /*
             * Welcome to the Comic Store.
             * What do you want to do?
             * 1) Add
             * 2) Read
             * 3) Update
             * 4) Delete
             * 5) Exit
             *
             * ---------
             *
             * What do you want to (Choose) ?
             *
             * 1) Author
             * 2) Comic
             * 3) Category
             * 4) Back
             *
             * ---------
                 * NEW AUTHOR
                 *
                 * Name:
                 * Nationality:
                 * Birth:
                 *
                 *
                 * (Confirmation)
                 * ---------
                 * NEW COMIC:
                 *
                 * Title:
                 * Description:
                 * Publication date:
                 * Pages:
                 * Category:
                 * Authors (Separated by comma):   //Foreach author, create new CREATOR registry
                 *
                 *
                 * (Confirmation)
                 * ---------
                 * NEW CATEGORY:
                 *
                 * Name:
                 *
                 *
                 * (Confirmation)
                 * ---------
             *
             *
             *
             */
        }
    }
}