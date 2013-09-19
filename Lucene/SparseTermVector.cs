using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene
{
    public class SparseTermVector<T>
    {
        private Dictionary<T, int> _elements = new Dictionary<T, int>(); 

        public SparseTermVector(T[] keys)
        {
            foreach (T key in keys)
            {
                _elements.Add(key, 0); 
            }
        }
    }
}
