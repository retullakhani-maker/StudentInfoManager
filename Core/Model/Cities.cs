using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Cities
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int StateId { get; set; }
        public States State { get; set; }
    }
}
