using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHandNet.Contract
{
    public class Card
    {
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }

        public Card()
        { }
        public Card(CardSuit Suit, CardValue Value)
        {
            this.Suit = Suit;
            this.Value = Value;
        }
    }
}
