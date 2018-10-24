using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    public class TagWithLineNumEqualityComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag x, Tag y)
        {
            return x.EqualsWithPosition(y);
        }

        public int GetHashCode(Tag obj)
        {
            return obj.GetHashCode();
        }
    }
}
