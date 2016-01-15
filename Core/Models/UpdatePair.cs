using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class UpdatePair<T>
    {
        public T Old { get; set; }
        public T New { get; set; }

        public UpdatePair()
        {
        }
 
        public UpdatePair(T oldItem, T newItem)
        {
            Old = oldItem;
            New = newItem;
        }
    }
}
