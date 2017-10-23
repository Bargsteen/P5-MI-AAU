namespace SolarSystem.Classes
{
    public class ArticleNumber
    {
        public int ArticleNum { get; }
        
        public ArticleNumber(int articleNum)
        {
            ArticleNum = articleNum;
        }
        
        public override string ToString()
        {
            return ArticleNum.ToString();
        }
    }

}
