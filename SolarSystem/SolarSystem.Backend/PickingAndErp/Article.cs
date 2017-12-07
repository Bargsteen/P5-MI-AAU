namespace SolarSystem.Backend.PickingAndErp
{
    public class Article
    {
        public Article(int articleNumber, int startArea)
        {
            _articlenumber = articleNumber;
            _startarea = startArea;
        }

        private readonly int _articlenumber;
        private readonly int _startarea;

        public override string ToString()
        {
            return "ArticleNumber: " + _articlenumber + "\n\tStartArea: " + _startarea + "\n";
        }
    }
}
