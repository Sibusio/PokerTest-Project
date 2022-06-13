using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHandNet.Contract
{
    public class PokerHandComparisonItem
    {
        public PokerHand Hand { get; set; }
        public PokerHandType HandType { get; set; }
        public int Rank { get; set; }

        public PokerHandComparisonItem()
        { }
        public PokerHandComparisonItem(PokerHand Hand)
        {
            this.Hand = Hand;
        }
        public PokerHandComparisonItem(PokerHand Hand, PokerHandType HandType)
        {
            this.Hand = Hand;
            this.HandType = HandType;
        }
        public PokerHandComparisonItem(PokerHand Hand, PokerHandType HandType, int Rank)
        {
            this.Hand = Hand;
            this.HandType = HandType;
            this.Rank = Rank;
        }
    }
}
