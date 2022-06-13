using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHandNet.Contract
{
    public class PokerHand : List<Card>
    {
        public PokerHand()
        { }
        public PokerHand(params Card[] pokerhandCards)
        {
            AddRange(pokerhandCards);
        }
    }
}
