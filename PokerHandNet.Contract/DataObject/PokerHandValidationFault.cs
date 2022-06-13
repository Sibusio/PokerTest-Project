using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHandNet.Contract
{
    public class PokerHandValidationFault
    {
        public PokerHand Hand { get; set; }
        public PokerHandValidationFaultDescription FaultDescription { get; set; }
    }
}
