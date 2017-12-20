using System;

namespace SolarSystem.Backend.Solution.Simulation.Orders
{
    public class Article
    {
        public long Id { get; set; }
        public string Name { get;  }
        public AreaCode AreaCode { get; set; }
        
        public Article(long id, string name, AreaCode areaCode)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AreaCode = areaCode;
        }

        protected bool Equals(Article other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return "ArticleNumber: " + Id + "\nStartArea: " + AreaCode + "\n";
        }
    }
}