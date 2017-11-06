using System;

namespace SolarSystem.Backend.Classes
{
    public class Article
    {
        public long Id { get; }
        public string Name { get;  }
        public AreaCode AreaCode { get; set; }
        
        public Article(long id, string name, AreaCode areaCode)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AreaCode = areaCode;
        }

        
        

        public override string ToString()
        {
            return "ArticleNumber: " + Id + "\nStartArea: " + AreaCode + "\n";
        }
    }
}