using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHandNet.Contract
{
    
        public enum CardSuit
        {
            Club = 0,
            Diamond = 1,
            Heart = 2,
            Spade = 3,
            Joker = 4
        }

        public enum CardValue
        {
            Unspecified = 0,
            Ace = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Jack = 11,
            Queen = 12,
            King = 13
        }

    public enum PokerHandType
    {
        FiveOfAKind = 1,
        StraightFlush = 2,
        FourOfAKind = 3,
        FullHouse = 4,
        Flush = 5,
        Straight = 6,
        ThreeOfAKind = 7,
        TwoPair = 8,
        Pair = 9,
        HighCard = 10
    }

    public enum PokerHandValidationFaultDescription
    {
        HasDuplicateCards = 1,
        JokersNotAllowed = 2,
        WrongCardCount = 3
    }


}
