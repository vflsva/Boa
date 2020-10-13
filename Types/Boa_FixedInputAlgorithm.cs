using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boa
{
    class Boa_Un FixedInputAlgorithm : Boa_Algorithm
    {
        public override bool InputIsFixedSize => true;

        public Boa_FixedInputAlgorithm(SolveAlgorithm solveAlgorithm, int inputDimension)
        {
            this.solveAlgorithm = solveAlgorithm;
            InputDimension = inputDimension;
        }

        protected override bool TryMatchInputDimension(int outputToMatch)
        {
            if (outputToMatch == InputDimension) return true;
            return false;
        }

        protected override bool TryMatchOutputDimension(int inputToMatch)
        {
            OutputDimension = inputToMatch;
            return true;
        }

    }
}
