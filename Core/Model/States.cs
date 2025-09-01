using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class States
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Cities> Cities { get; set; } = new List<Cities>();
    }
}
