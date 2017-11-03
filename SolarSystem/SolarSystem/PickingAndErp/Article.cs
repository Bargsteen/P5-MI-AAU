using System;
using SolarSystem.Backend.Classes;

namespace SolarSystem.PickingAndErp
{
    public class Article
    {
        public Article(long id, string name, AreaCode areaCode)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AreaCode = areaCode;
        }

        public long Id { get; }
        public string Name { get;  }
        public AreaCode AreaCode { get; set; }
        

        public override string ToString()
        {
            return "ArticleNumber: " + Id + "\nStartArea: " + AreaCode + "\n";
        }
    }
}
