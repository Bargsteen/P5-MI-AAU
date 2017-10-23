using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
