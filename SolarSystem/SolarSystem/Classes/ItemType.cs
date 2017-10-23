using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class ItemType
    {
        public ArticleNumber ArticleNumber { get; }

        public ItemType(ArticleNumber articleNumber)
        {
            ArticleNumber = articleNumber ?? throw new ArgumentNullException(nameof(articleNumber));
        }
    }
}
