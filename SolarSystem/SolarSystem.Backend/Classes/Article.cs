using System;

namespace SolarSystem.Backend.Classes
{
    public class Article : IComparable
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


        public override bool Equals(object obj)
        {
            if (!(obj is Article otherArticle))
            {
                return false;
            }
            return Id.Equals(otherArticle.Id);
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

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Article otherArticle = obj as Article;
            if (otherArticle != null) 
                return this.Id.CompareTo(otherArticle.Id);
            else
                throw new ArgumentException("Object is not an Article");
        }
    }
}