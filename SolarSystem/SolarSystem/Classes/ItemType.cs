using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    class ItemType
    {
        ArticleNumber _articleNumber;

        public ItemType(ArticleNumber articleNumber)
        {
            _articleNumber = articleNumber;
        }

        public ArticleNumber ArticleNumber { get { return _articleNumber; } }

    }
}
