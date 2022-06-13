using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHandNet.Contract
{
        public interface IPokerHandAssessor
        {
        List<PokerHandComparisonItem> ComparePokerHands(params PokerHand[] pokerHands);
        PokerHandType DeterminePokerHandType(PokerHand pokerHand);
        List<PokerHandValidationFault> ValidatePokerHands(params PokerHand[] pokerHands);
    }
    
}
