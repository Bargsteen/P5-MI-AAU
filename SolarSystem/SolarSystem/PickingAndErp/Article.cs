namespace SolarSystem.PickingAndErp
{
    public class Article
    {
        public Article(int articleNumber, int startArea)
        {
            _articlenumber = articleNumber;
            _startarea = startArea;
        }

        int _articlenumber;
        int _startarea;

        public override string ToString()
        {
            return "ArticleNumber: " + _articlenumber + "\nStartArea: " + _startarea + "\n";
        }
    }
}
