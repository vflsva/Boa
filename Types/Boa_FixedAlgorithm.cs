using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boa
{
    class Boa_FixedAlgorithm : Boa_Algorithm
    {
        public override bool InputIsFixedSize => true;
        public override bool OutputIsFixedSize => true;
    }
}
