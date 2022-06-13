using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerHandNet.Contract;
using PokerHandNet.Machine;


namespace PokerHandNet
{
    //Using Similar Solid Principle Project on CodeProject and then adding unit testing to project
    
    class Program
    {
        
        
        static void Main(string[] args)
        {
            Console.Title = "♠♥♣♦ Hello World - SOLID Poker";
            Console.WriteLine("Testing: Validate Poker Hands");

            //Create hand 1 with duplicate.
            PokerHand hand1 = new PokerHand(
                new Card(CardSuit.Spade, CardValue.Two),
                new Card(CardSuit.Club, CardValue.Seven),
                new Card(CardSuit.Club, CardValue.Seven),
                new Card(CardSuit.Diamond, CardValue.Seven),
                new Card(CardSuit.Heart, CardValue.Seven)
                );

            //Validate hand 1 & Validate.
            ValidatePokerHands_and_UpdateConsole(hand1);
            hand1[1].Suit = CardSuit.Spade;

            //Create hand 2 and duplicate a card from hand 1 & Validate.
            PokerHand hand2 = new PokerHand(
                new Card(CardSuit.Spade, CardValue.Two),
                new Card(CardSuit.Club, CardValue.Queen),
                new Card(CardSuit.Spade, CardValue.King),
                new Card(CardSuit.Diamond, CardValue.Jack),
                new Card(CardSuit.Heart, CardValue.Ace)
                );
            ValidatePokerHands_and_UpdateConsole(hand1, hand2);

            //Change card that was duplicated between the two hands & Validate.
            hand1[0].Suit = CardSuit.Diamond;
            ValidatePokerHands_and_UpdateConsole(hand1, hand2);

            //Place joker in hand 1 & Validate.
            hand1[0].Suit = CardSuit.Joker;
            hand1[0].Value = CardValue.Unspecified;
            ValidatePokerHands_and_UpdateConsole(hand1);

            //Remove a card from hand 1 & Validate.
            hand1.RemoveAt(0);
            ValidatePokerHands_and_UpdateConsole(hand1);


            Console.WriteLine("");
            Console.WriteLine("Testing: Compare Poker Hands");

            //Prepare hands to compare
            //Two Pair
            hand1 = new PokerHand(
                new Card(CardSuit.Spade, CardValue.Two),
                new Card(CardSuit.Club, CardValue.Two),
                new Card(CardSuit.Spade, CardValue.Four),
                new Card(CardSuit.Club, CardValue.Four),
                new Card(CardSuit.Heart, CardValue.Seven)
                );
            //Two Pair
            hand2 = new PokerHand(
                new Card(CardSuit.Diamond, CardValue.Two),
                new Card(CardSuit.Heart, CardValue.Two),
                new Card(CardSuit.Diamond, CardValue.Four),
                new Card(CardSuit.Heart, CardValue.Four),
                new Card(CardSuit.Heart, CardValue.Six)
                );
            //flush
            PokerHand hand3 = new PokerHand(
                new Card(CardSuit.Spade, CardValue.Ace),
                new Card(CardSuit.Spade, CardValue.Three),
                new Card(CardSuit.Spade, CardValue.Queen),
                new Card(CardSuit.Spade, CardValue.King),
                new Card(CardSuit.Spade, CardValue.Ten)
                );
            //flush
            PokerHand hand4 = new PokerHand(
                new Card(CardSuit.Diamond, CardValue.Ace),
                new Card(CardSuit.Diamond, CardValue.Three),
                new Card(CardSuit.Diamond, CardValue.Queen),
                new Card(CardSuit.Diamond, CardValue.King),
                new Card(CardSuit.Diamond, CardValue.Ten)
                );
            //flush
            PokerHand hand5 = new PokerHand(
                new Card(CardSuit.Heart, CardValue.Five),
                new Card(CardSuit.Heart, CardValue.Three),
                new Card(CardSuit.Heart, CardValue.Queen),
                new Card(CardSuit.Heart, CardValue.King),
                new Card(CardSuit.Heart, CardValue.Ten)
                );

            //Compare hands.
            var comparisonItems = assessor.ComparePokerHands(
                hand1, hand2, hand3, hand4, hand5);
            comparisonItems.ForEach(item =>
            Console.WriteLine(
            "Rank: " + item.Rank +
            ", Poker Hand: " + Utilities.PokerHandsToShortString(item.Hand) +
            ", Hand Type: " + Utilities.EnumToTitle(item.HandType)));

            Console.Read();
        }

        static IPokerHandAssessor assessor = new HighRules_NoJoker();

        /// <summary>
        /// Validate poker hands and update console with results.
        /// </summary>
        /// <param name="pokerHands">Poker hands to validate.</param>
        static void ValidatePokerHands_and_UpdateConsole(params PokerHand[] pokerHands)
        {
            var faults = assessor.ValidatePokerHands(pokerHands);
            Console.WriteLine("");
            Console.WriteLine(
                "Validating: " + Utilities.PokerHandsToShortString(pokerHands) + ":");
            Console.WriteLine((faults.Count == 0 ? "Valid" : "Validation Fault: "
                + Utilities.EnumToTitle(faults[0].FaultDescription)));
        }
    }
}
