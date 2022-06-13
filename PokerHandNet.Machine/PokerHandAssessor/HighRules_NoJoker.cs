using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerHandNet.Contract;

namespace PokerHandNet.Machine
{
    /// <summary>
    /// This Poker Hand Assessor works according to the High Ace Rules, and no jokers allowed.
    /// </summary>
    public class HighRules_NoJoker : IPokerHandAssessor
    {
        #region Arrange Cards
        /// <summary>
        /// The cards are arranged in ascending order, so that the Hand Type is easily seen.
        /// If an Ace is to be used as a 1 (before 2) in a Straight, 
        /// then the Ace will be at the beginning, otherwise, it will be at the end.
        /// </summary>
        /// <param name="pokerHand">The poker hand to be arranged.</param>
        void ArrangeCards(PokerHand pokerHand)
        {
            //First determine whether the poker hand is a Straight or a Straight Flush.
            bool straight = IsStraight(pokerHand);

            //Sort ascending
            pokerHand.Sort((pokerCard1, pokerCard2) =>
            pokerCard1.Value.CompareTo(pokerCard2.Value));

            //Move Aces to the end if:
            if (!straight || //Not a straight
                pokerHand[4].Value == CardValue.King)//Straight with a king at the end
            {
                //Move all Aces To the End
                while (pokerHand[0].Value == CardValue.Ace)
                {
                    pokerHand.Add(pokerHand[0]);
                    pokerHand.RemoveAt(0);
                }
            }
        }
        #endregion

        #region Check Poker Hands For Duplicate Cards
        /// <summary>
        /// Checks the poker hands for duplicate cards.
        /// Returns the first poker hands found with duplicate cards.
        /// If a poker hand contains duplicate cards of itself, 
        /// then only that poker hand will be returned.
        /// If cards are duplicated between two poker hands, 
        /// then both these poker hands will be returned.
        /// </summary>
        /// <param name="pokerHands">Poker hands to evaluate.</param>
        /// <returns>Poker hands that contain duplicate cards.</returns>
        PokerHand[] CheckPokerHandsForDuplicateCards(params PokerHand[] pokerHands)
        {
            for (int i = 0; i < pokerHands.Length; i++)
            {
                //Check whether the poker hand contains duplicate cards of itself.
                PokerHand pokerHand = pokerHands[i];
                if (pokerHand.GroupBy(card => new { card.Suit, card.Value }).Count()
                            != pokerHand.Count)
                    return new PokerHand[] { pokerHand };

                for (int ii = i + 1; ii < pokerHands.Length; ii++)
                {
                    //Check whether cards are duplicated between two poker hands.
                    if (new PokerHand[] { pokerHand, pokerHands[ii] }.SelectMany(hand => hand)
                        .GroupBy(card => new { card.Suit, card.Value }).Count() !=
                        pokerHand.Count + pokerHands[ii].Count)
                        return new PokerHand[] { pokerHand, pokerHands[ii] };
                }
            }
            return new PokerHand[0];
        }
        #endregion

        #region Compare Poker Hands
        /// <summary>
        /// Compares poker hands.
        /// </summary>
        /// <param name="pokerHands">Poker hands to compare.</param>
        /// <returns>
        /// A list of Poker Hand Comparison Items ordered ascending by poker hand rank. 
        /// Each comparison item represents a poker hand and provides its Hand Type & Rank.
        /// Winning hands have rank: 1, and will be at the top of the list.
        /// </returns>
        public List<PokerHandComparisonItem> ComparePokerHands(params PokerHand[] pokerHands)
        {
            ValidatePokerHands_private(pokerHands);

            //NOTE: 
            //For better understanding of this code, remember:
            //A PokerHandComparisonItem is a Poker Hand with comparison data.

            //Prepare hand comparison list, including poker hand type.
            //The rank will be calculated later in this function.
            var lstPokerHandComparison = new List<PokerHandComparisonItem>();
            pokerHands.ToList().ForEach(hand => lstPokerHandComparison.Add(
            new PokerHandComparisonItem(hand, DeterminePokerHandType(hand))));

            //Sort ascending by poker hand type.
            lstPokerHandComparison.Sort((comparisonItem1, comparisonItem2) =>
            comparisonItem1.HandType.CompareTo(comparisonItem2.HandType));

            //Group by hand type.
            var handTypeGroups = lstPokerHandComparison.GroupBy(comparisonItem =>
            comparisonItem.HandType).ToList();

            //Compare hands in groups.
            int rank = 0;
            handTypeGroups.ForEach(handTypeGroup =>
            {
                //Get comparison items in this group.
                var comparisonItemsInGroup = handTypeGroup.ToList();

                //Rank must be incremented for every group.
                rank++;

                //Process single hand group.
                if (comparisonItemsInGroup.Count == 1)
                    comparisonItemsInGroup[0].Rank = rank;

                //Process multi hand group.
                else
                {
                    //Sort descending by winning hand. Winning hands are listed first.
                    comparisonItemsInGroup.Sort((comparisonItem1, comparisonItem2) => -1 *
                    CompareHandsOfSameType(
                    comparisonItem1.Hand, comparisonItem2.Hand, comparisonItem1.HandType));

                    //Assign current rank to first hand in group.
                    comparisonItemsInGroup[0].Rank = rank;

                    //Determine rank for subsequent hands in group.
                    //It is helpful that the items are already sorted by winning hand; 
                    //however:
                    //the hands must be compared again to check which hands are equal.
                    //Equal hands must have same rank.
                    for (int i = 1; i < comparisonItemsInGroup.Count; i++)
                    {
                        //Compare current hand with previous hand.
                        var currentComparisonItem = comparisonItemsInGroup[i];
                        if (CompareHandsOfSameType(comparisonItemsInGroup[i - 1].Hand,
                            currentComparisonItem.Hand, currentComparisonItem.HandType) == 1)
                            rank++;//Increment rank if previous hand wins current hand.

                        //Assign current rank to current hand in group.
                        currentComparisonItem.Rank = rank;
                    }
                }
            });

            //Sort ascending by rank.
            lstPokerHandComparison.Sort((comparisonItem1, comparisonItem2) =>
            comparisonItem1.Rank.CompareTo(comparisonItem2.Rank));

            return lstPokerHandComparison;
        }
        #endregion

        #region Compare Hands Of Same Type
        /// <summary>
        /// Compares two poker hands of the same poker hand type;
        /// for example: 2 poker hands with hand type Four Of A Kind.
        /// </summary>
        /// <param name="pokerHand1">First poker hand to compare.</param>
        /// <param name="pokerHand2">Second poker hand to compare.</param>
        /// <param name="pokerHandType">Poker Hand Type of the 2 poker hands.</param>
        /// <returns>
        /// Int value indicating the winning hand. 
        /// 1: Hand 1 is the winning hand, 
        /// 0: The two hands are equal, 
        /// -1: Hand 2 is the winning hand.
        /// </returns>
        int CompareHandsOfSameType(PokerHand pokerHand1, PokerHand pokerHand2, PokerHandType pokerHandType)
        {
            //Arrange cards
            ArrangeCards(pokerHand1);
            ArrangeCards(pokerHand2);

            //Compare the hands.
            switch (pokerHandType)
            {
                case PokerHandType.StraightFlush:
                case PokerHandType.Straight:
                    return CompareHandsOfSameType_Helper(pokerHand1[4], pokerHand2[4]);
                case PokerHandType.Flush:
                case PokerHandType.HighCard:
                    for (int i = 4; i >= 0; i--)
                    {
                        int result =
                        CompareHandsOfSameType_Helper(pokerHand1[i], pokerHand2[i]);
                        if (result != 0)
                            return result;
                    }
                    return 0;
            }

            //Find sets of cards with same value: KK QQQ.
            List<Card> hand1SameCardSet1, hand1SameCardSet2;
            FindSetsOfCardsWithSameValue(
            pokerHand1, out hand1SameCardSet1, out hand1SameCardSet2);

            List<Card> hand2SameCardSet1, hand2SameCardSet2;
            FindSetsOfCardsWithSameValue(
            pokerHand2, out hand2SameCardSet1, out hand2SameCardSet2);

            //Continue comparing the hands.
            switch (pokerHandType)
            {
                case PokerHandType.FourOfAKind:
                case PokerHandType.FullHouse:
                case PokerHandType.ThreeOfAKind:
                case PokerHandType.Pair:
                    return CompareHandsOfSameType_Helper(
                        hand1SameCardSet1[0], hand2SameCardSet1[0]);
                case PokerHandType.TwoPair:
                    //Compare first pair
                    int result = CompareHandsOfSameType_Helper(
                        hand1SameCardSet1[0], hand2SameCardSet1[0]);
                    if (result != 0)
                        return result;

                    //Compare second pair
                    result = CompareHandsOfSameType_Helper(
                        hand1SameCardSet2[0], hand2SameCardSet2[0]);
                    if (result != 0)
                        return result;

                    //Compare kickers (side cards)
                    var kicker1 = pokerHand1.Where(card =>
                        !hand1SameCardSet1.Contains(card) &&
                        !hand1SameCardSet2.Contains(card)).ToList()[0];
                    var kicker2 = pokerHand2.Where(card =>
                        !hand2SameCardSet1.Contains(card) &&
                        !hand2SameCardSet2.Contains(card)).ToList()[0];
                    return CompareHandsOfSameType_Helper(kicker1, kicker2);
            }

            //This area of code should not be reached.
            throw new Exception("Hand comparison failed. Check code integrity.");
        }

        #region Helper
        /// <summary>
        /// This function eliminates boilerplate code when comparing poker cards,
        /// and returns an int value indicating the winning hand.
        /// </summary>
        /// <param name="pokerHand1_card">Poker hand 1's card.</param>
        /// <param name="pokerHand2_card">Poker hand 2's card.</param>
        /// <returns>Int value indicating the winning hand. 
        /// 1: Hand 1 is the winning hand, 
        /// 0: The two hands are equal, 
        /// -1: Hand 2 is the winning hand.</returns>
        int CompareHandsOfSameType_Helper(Card pokerHand1_card, Card pokerHand2_card)
        {
            //Get card int values.
            //This is convenient for use in this function.
            //This is also necessary to ensure the actual card's value remains unchanged.
            int pokerHand1_cardIntValue = (int)pokerHand1_card.Value;
            int pokerHand2_cardIntValue = (int)pokerHand2_card.Value;

            //Aces are always treated as high aces in this function.
            //Low aces are never passed to this function. 
            if (pokerHand1_card.Value == CardValue.Ace)
                pokerHand1_cardIntValue += (int)CardValue.King;
            if (pokerHand2_card.Value == CardValue.Ace)
                pokerHand2_cardIntValue += (int)CardValue.King;

            //Compare and return result.
            return pokerHand1_cardIntValue > pokerHand2_cardIntValue ? 1 :
                pokerHand1_cardIntValue == pokerHand2_cardIntValue ? 0 : -1;
        }
        #endregion
        #endregion

        #region Find Sets of Cards With Same Value
        /// <summary>
        /// Finds sets of cards that have the same value.
        /// This is necessary to identify the followig Hand Types: 
        /// Four of a Kind, Full House, Three of a Kind, Two-Pair, Pair
        /// </summary>
        /// <param name="pokerHand">The poker hand to be evaluated.</param>
        /// <param name="sameValueSet1">First set of cards found with the same value.</param>
        /// <param name="sameValueSet2">Second set of cards found with the same value.</param>
        void FindSetsOfCardsWithSameValue(PokerHand pokerHand, out List<Card> sameValueSet1, out List<Card> sameValueSet2)
        {
            //Arrange the cards in logical order.
            ArrangeCards(pokerHand);

            //Find sets of cards with the same value.
            int index = 0;
            sameValueSet1 = FindSetsOfCardsWithSameValue_Helper(pokerHand, ref index);
            sameValueSet2 = FindSetsOfCardsWithSameValue_Helper(pokerHand, ref index);
        }
        List<Card> FindSetsOfCardsWithSameValue_Helper(PokerHand pokerHand_ArrangedCorrectly, ref int index)
        {
            List<Card> sameCardSet = new List<Card>();
            for (; index < 4; index++)
            {
                Card currentCard = pokerHand_ArrangedCorrectly[index];
                Card nextCard = pokerHand_ArrangedCorrectly[index + 1];
                if (currentCard.Value == nextCard.Value)
                {
                    if (sameCardSet.Count == 0)
                        sameCardSet.Add(currentCard);
                    sameCardSet.Add(nextCard);
                }
                else if (sameCardSet.Count > 0)
                {
                    index++;
                    break;
                }
            }
            return sameCardSet;
        }
        #endregion

        #region Determine Poker Hand Type
        /// <summary>
        /// Determines the poker hand type. For example: Straight Flush or Four of a Kind.
        /// </summary>
        /// <param name="pokerHand">The poker hand to be evaluated.</param>
        /// <returns>The poker hand type.
        /// For example: Straight Flush or Four of a Kind.</returns>
        public PokerHandType DeterminePokerHandType(PokerHand pokerHand)
        {
            ValidatePokerHands_private(pokerHand);

            //Check whether all cards are in the same suit
            bool allSameSuit = pokerHand.GroupBy(card => card.Suit).Count() == 1;

            //Check whether the Poker Hand Type is: Straight
            bool straight = IsStraight(pokerHand);

            //Determine Poker Hand Type
            if (allSameSuit && straight)
                return PokerHandType.StraightFlush;

            if (allSameSuit)
                return PokerHandType.Flush;

            if (straight)
                return PokerHandType.Straight;

            //Find sets of cards with the same value.
            //Example: QQQ KK
            List<Card> sameCardSet1, sameCardSet2;
            FindSetsOfCardsWithSameValue(pokerHand, out sameCardSet1, out sameCardSet2);

            //Continue Determining Poker Hand Type
            if (sameCardSet1.Count == 4)
                return PokerHandType.FourOfAKind;

            if (sameCardSet1.Count + sameCardSet2.Count == 5)
                return PokerHandType.FullHouse;

            if (sameCardSet1.Count == 3)
                return PokerHandType.ThreeOfAKind;

            if (sameCardSet1.Count + sameCardSet2.Count == 4)
                return PokerHandType.TwoPair;

            if (sameCardSet1.Count == 2)
                return PokerHandType.Pair;

            return PokerHandType.HighCard;
        }
        #endregion

        #region Is Straight
        /// <summary>
        /// Determines whether the card values are in sequence. 
        /// The hand type would then be either Straight or Straight Flush.
        /// </summary>
        /// <param name="pokerHand">The poker hand to be evaluated.</param>
        /// <returns>Boolean indicating whether the card values are in sequence.</returns>
        bool IsStraight(PokerHand pokerHand)
        {
            //Sort ascending
            pokerHand.Sort((pokerCard1, pokerCard2) =>
            pokerCard1.Value.CompareTo(pokerCard2.Value));

            //Determines whether the card values are in sequence.
            return
                //Check whether the last 4 cards are in sequence.
                pokerHand[1].Value == pokerHand[2].Value - 1 &&
                pokerHand[2].Value == pokerHand[3].Value - 1 &&
                pokerHand[3].Value == pokerHand[4].Value - 1
                &&
                (
                //Check that the first two cards are in sequence
                pokerHand[0].Value == pokerHand[1].Value - 1
                //or the first card is an Ace and the last card is a King.
                || pokerHand[0].Value == CardValue.Ace && pokerHand[4].Value == CardValue.King
                );
        }
        #endregion

        #region Validate Poker Hands
        /// <summary>
        /// Checks that poker hands have 5 cards, no jokers and no duplicate cards.
        /// Retuns the first validation faults found. 
        /// Does not continue with further validations after validation faults are found.
        /// </summary>
        /// <param name="pokerHands">The poker hands to validate.</param>
        /// <returns>List of Poker Hand Validation Faults</returns>
        public List<PokerHandValidationFault> ValidatePokerHands(params PokerHand[] pokerHands)
        {
            List<PokerHandValidationFault> faults = new List<PokerHandValidationFault>();

            //Check card count.
            var pokerHandsWithWrongCardCount =
                        pokerHands.Where(hand => hand.Count != 5).ToList();
            if (pokerHandsWithWrongCardCount.Count > 0)
            {
                pokerHandsWithWrongCardCount.ForEach(hand =>
                faults.Add(new PokerHandValidationFault
                {
                    FaultDescription = PokerHandValidationFaultDescription.WrongCardCount,
                    Hand = hand
                }));
                return faults;
            }

            //Look for jokers.
            foreach (PokerHand hand in pokerHands)
            {
                var jokers = hand.Where(card => card.Suit == CardSuit.Joker);
                if (jokers.Count() > 0)
                {
                    faults.Add(new PokerHandValidationFault
                    {
                        FaultDescription =
                        PokerHandValidationFaultDescription.JokersNotAllowed,
                        Hand = hand
                    });
                    return faults;
                }
            }

            //Look for duplicates.
            List<PokerHand> pokerHandsWithDuplicates =
                        CheckPokerHandsForDuplicateCards(pokerHands).ToList();
            pokerHandsWithDuplicates.ForEach(hand => faults.Add(new PokerHandValidationFault
            {
                FaultDescription = PokerHandValidationFaultDescription.HasDuplicateCards,
                Hand = hand
            }));
            return faults;
        }

        /// <summary>
        /// Validate poker hands and throw an argument exception if validation fails.
        /// The public methods of this class expect valid poker hands and an exception must be 
        /// thrown in case of an invalid poker hand.
        /// Subscribers of this class's functionality can call the ValidatePokerHands function
        /// to validate the poker hands without an exception being thrown.
        /// The calling method name is automatically detected and included in the exception.
        /// </summary>
        /// <param name="pokerHands">Poker hands to validate.</param>
        void ValidatePokerHands_private(params PokerHand[] pokerHands)
        {
            var validationFaults = ValidatePokerHands(pokerHands);
            if (validationFaults.Count > 0)
            {
                string callingMethodName =
                            new System.Diagnostics.StackFrame(1).GetMethod().Name;
                throw new ArgumentException(
                "Poker hands failed validation: " +
                Utilities.EnumToTitle(validationFaults[0].FaultDescription) +
                " Call the ValidatePokerHands method for detailed validation feedback.",
                callingMethodName);
            }
        }
        #endregion
    }
}
