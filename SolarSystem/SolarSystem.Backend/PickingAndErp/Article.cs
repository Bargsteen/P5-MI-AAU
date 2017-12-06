using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.PickingandERP
{
    public class Article
    {
        public Article(int ArticleNumber, int StartArea)
        {
            _articlenumber = ArticleNumber;
            _startarea = StartArea;
        }

        int _articlenumber;
        int _startarea;

        public override string ToString()
        {
            return "ArticleNumber: " + _articlenumber + "\n\tStartArea: " + _startarea + "\n";
        }
    }
}
