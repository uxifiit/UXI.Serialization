using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Fakes
{
    class CompositeObject
    {
        public SingleIntValue Single { get; set; }
        public MultipleValues Composite { get; set; }
        public double Double { get; set; }
    }
}
