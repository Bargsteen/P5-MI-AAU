using System;

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
