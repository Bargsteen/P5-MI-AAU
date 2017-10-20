using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    class ArticleNumber
    {
        int _articleNum;

        public ArticleNumber(int articleNum)
        {
            _articleNum = articleNum;
        }

        public int ArticleNum { get { return _articleNum; } }

        public override string ToString()
        {
            return _articleNum.ToString();
        }
    }

}
