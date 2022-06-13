using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerHandNet.Contract;


namespace PokerHandNet
{
    class Utilities
    {
        /// <summary>
        /// Converts a card suit to a text symbol.
        /// Example 1: ♠.
        /// Example 2: ♣.
        /// </summary>
        /// <param name="card">This card's suit will be converted.</param>
        /// <returns>Short string representing the card suit.</returns>
        public static string CardSuitToSymbol(Card card)
        {
            return CardSuitToSymbol(card.Suit);
        }

        /// <summary>
        /// Converts a card suit to a text symbol.
        /// Example 1: ♠.
        /// Example 2: ♣.
        /// </summary>
        /// <param name="cardSuit">Card suit to convert.</param>
        /// <returns>Short string representing the card suit.</returns>
        public static string CardSuitToSymbol(CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.Club:
                    return "♣";
                case CardSuit.Diamond:
                    return "♦";
                case CardSuit.Heart:
                    return "♥";
                case CardSuit.Spade:
                    return "♠";
                case CardSuit.Joker:
                    return "J";
            }
            return "";
        }

        /// <summary>
        /// Converts a card to a short string.
        /// Example 1: ♠A.
        /// Example 2: ♣5.
        /// </summary>
        /// <param name="card">Card to convert.</param>
        /// <returns>Short string representing the card.</returns>
        public static string CardToShortString(Card card)
        {
            return CardSuitToSymbol(card) + CardValueToShortString(card);
        }

        /// <summary>
        /// Converts the card value to a short string.
        /// Example 1: A.
        /// Example 2: 5.
        /// </summary>
        /// <param name="card">This card's value will be converted.</param>
        /// <returns>Short string representing the card value.</returns>
        public static string CardValueToShortString(Card card)
        {
            return CardValueToShortString(card.Value);
        }
        /// <summary>
        /// Converts the card value to a short string.
        /// Example 1: A.
        /// Example 2: 5.
        /// </summary>
        /// <param name="cardValue">Card value to convert.</param>
        /// <returns>Short string representing the card value.</returns>
        public static string CardValueToShortString(CardValue cardValue)
        {
            string shortString = (int)cardValue > 1 && (int)cardValue < 11 ?
            ((int)cardValue).ToString() : EnumToTitle(cardValue)[0].ToString();
            return shortString;
        }

        /// <summary>
        /// Converts an enum to a presentable title.
        /// </summary>
        /// <param name="enumToConvert">The enum to be converted.</param>
        /// <returns>A presentable title.</returns>
        public static string EnumToTitle(Enum enumToConvert)
        {
            return System.Text.RegularExpressions.Regex
            .Replace(enumToConvert.ToString(), "[A-Z]", " $0").Trim();
        }

        /// <summary>
        /// Convert poker hands to a short string.
        /// Example 1: ♠A,♣4,♠9,♣5,♠6.
        /// Example 2: ♠A,♣4,♠9,♣5,♠6 - ♥A,♥4,♦9,♦5,♦6.
        /// </summary>
        /// <param name="pokerHands">Poker hands to convert.</param>
        /// <returns>Short string representing the poker hands.</returns>
        public static string PokerHandsToShortString(params PokerHand[] pokerHands)
        {
            StringBuilder sb = new StringBuilder();
            pokerHands.ToList().ForEach(hand => {
                hand.ForEach(card => sb.Append(CardToShortString(card) + ','));

                //Remove last comma
                if (hand.Count > 0)
                    sb.Remove(sb.Length - 1, 1);

                sb.Append(" - ");
            });

            //Remove last: " - "
            if (pokerHands.Length > 0)
                sb.Remove(sb.Length - 3, 3);


            return sb.ToString();
        }
    }
}
