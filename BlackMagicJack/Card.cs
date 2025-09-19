using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagicJack
{
    
    public enum eRarity
    {
        Boring,
        Special,
        Evil,
        Bonus,
        IDK
    }
    public static class RNG
    {
        public static Random random = new Random();
    }
    public class Card
    {
        public String suite;
        public String value;
        public eRarity rarity;
        public virtual void special(Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck) {}

        public static String valueConverter(int value)
        {
            switch(value){
                case 1:
                    return "Ace";
                case 11:
                    return "Jack";
                case 12:
                    return "Queen";
                case 13:
                    return "King";
                case 14:
                    return "Joker";
                default:
                    return Convert.ToString(value);
            }
        }
    }


    class NormalCard : Card {
        public NormalCard(String value, String suite)
        {
            this.value = value;
            this.suite = suite;
            this.rarity = eRarity.Boring;
        }
        
        public override void special(Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck)
        {
            String[] messages =
            {
                "This card does nothing",
                "Literally a boring card",
                "What did you expect to happen?"
            };

            Console.WriteLine(messages[RNG.random.Next(messages.Length)]);
        }
        public static void initializeDeck(List<Card> deck)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    deck.Add(new NormalCard(valueConverter(i + 1), "Diamonds"));
                    deck.Add(new NormalCard(valueConverter(i + 1), "Hearts"));
                    deck.Add(new NormalCard(valueConverter(i + 1), "Spades"));
                    deck.Add(new NormalCard(valueConverter(i + 1), "Clubs"));
                }
            }
        }
    }
    class EvilCard : Card
    {
        public void Action(int index, Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck)
        {
            switch (index)
            {
                case 0:
                    Console.WriteLine("Something funny happened.");
                    for(int i = 0; i < 15; i++)
                    {
                        evilDeck.Add(new EvilCard(eRarity.Evil, valueConverter(14), "Blacks"));
                    }
                    break;
                case 1:
                    Console.WriteLine("Say goodbye to 20 cards.");
                    EvilCard.cardStealer(normalDeck);
                    break;
                case 2:
                    Console.WriteLine("Good luck trying to win now (-1 multiplier)");
                    if (player.multiplier < 1)
                        break;
                    else
                        player.multiplier--;
                    break;
                case 3:
                    Console.WriteLine("You don't deserve bonuses (no bonus cards)");
                    while (bonusDeck.Count != 0)
                    {
                        bonusDeck.RemoveAt(0);
                    }
                    break;
                case 4:
                    Console.WriteLine("I'm feeling generous today (+1 multiplier)");
                    player.multiplier += 1;
                    break;
                case 5:
                    Console.WriteLine("Evil Deck Refill!");
                    evilDeck.Clear();
                    EvilCard.initializeDeck(evilDeck);
                    break;
                case 6:
                    Console.WriteLine("Deck Refill!");
                    normalDeck.Clear();
                    NormalCard.initializeDeck(normalDeck);
                    break;
                case 7:
                    Console.WriteLine("Bonus Deck Refill!");
                    BonusCard.initialize(bonusDeck);
                    break;
                case 8:
                    Console.WriteLine("ACES!! in ur deck!!");
                    int rand = RNG.random.Next(2, 10);
                    for (int i = 0; i < rand; i++)
                    {
                        normalDeck.Add(new NormalCard("Ace", "Spades"));
                    }
                    break;
                case 9:
                    Console.WriteLine("Go spoil urself bbg <3 (+1 chip)");
                    player.chips++;
                    break;
                case 10:
                    player.storyLineTrigger = true;
                    break;
                }
            }

        public EvilCard(eRarity rarity, String value, String suite)
        {
            this.rarity = rarity;
            this.suite = suite;
            this.value = value;
        }
        static eRarity evilRarityAssigner()
        {
            int rarity = RNG.random.Next(100);

            if(rarity < 1)
            {
                return eRarity.IDK;
            }
            if (rarity < 20)
            {
                return eRarity.Evil;
            }
            else
            {
                return eRarity.Special;
            }
        }
        public static void initializeDeck(List<Card> deck)
        {
            for (int i = 0; i < 13; i++)
            {
                deck.Add(new EvilCard(evilRarityAssigner(), valueConverter(i + 1), "Diamonds"));
                deck.Add(new EvilCard(evilRarityAssigner(), valueConverter(i + 1), "Hearts"));
                deck.Add(new EvilCard(evilRarityAssigner(), valueConverter(i + 1), "Spades"));
                deck.Add(new EvilCard(evilRarityAssigner(), valueConverter(i + 1), "Clubs"));
            }
        }
        public override void special(Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck)
        {
            if(this.rarity == eRarity.Evil)
            {
                Action(RNG.random.Next(4), player, normalDeck, evilDeck, bonusDeck);
            }
            if(this.rarity == eRarity.Special)
            {
                Action(RNG.random.Next(4, 10), player, normalDeck, evilDeck, bonusDeck);
            }if(this.rarity == eRarity.IDK)
            {
                Action(10, player, normalDeck, evilDeck, bonusDeck);
            }
        }
        public static void cardStealer(List<Card> deck)
        {
            for(int i = 0; i < 20; i++)
            {
                int index = RNG.random.Next(deck.Count);
                deck.RemoveAt(index);
            }
        }
    }
    class BonusCard : Card
    {
        public void BonusAction(int index, Player player, List<Card> deck)
        {
            switch (index)
            {
                case 0:
                    if(player.chips == 0)
                    {
                        player.chips += 2;
                    }
                    else
                    {
                        player.chips *= 2;
                    }
                    Console.WriteLine("Your chips are doubled!!");
                    break;
                case 1:
                    player.chips += player.heartDraws + player.diamondDraws;
                    Console.WriteLine("Red is your lucky color!!");
                    break;
                case 2:
                    player.chips += player.spadeDraws + player.clubDraws;
                    Console.WriteLine("You look better in black!");
                    break;
                case 3:
                    player.chips += 200;
                    Console.WriteLine("JACKPOT");
                    break;
                case 4:
                    int pos = RNG.random.Next(player.availableItems.Count);
                    if (pos == 0)
                    {
                        Console.WriteLine("You know what you have to do.");
                        break;
                    }
                    Items item = player.availableItems[pos];
                    
                    player.playerInventory.Add(item);
                    player.availableItems.RemoveAt(pos);
                    Console.WriteLine($"{item.itemName} has been added to your inventory.");
                    break;
            }
        }

        public BonusCard(String suite, String value)
        {
            this.suite = suite;
            this.value = value;
            this.rarity = eRarity.Bonus;
        }
        public static void initialize(List<Card> deck)
        {
            String[] suites = { "Diamonds", "Hearts", "Spades", "Clubs" };

            for(int i = 0; i<RNG.random.Next(15); i++)
            {
                deck.Add(new BonusCard(suites[RNG.random.Next(4)], valueConverter(RNG.random.Next(1,14))));
            }
        }
        public override void special(Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck)
        {
            BonusAction(RNG.random.Next(5), player, bonusDeck);
        }

    }
}
