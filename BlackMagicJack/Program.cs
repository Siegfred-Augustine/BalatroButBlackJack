// See https://aka.ms/new-console-template for more information
using System;

namespace BlackMagicJack {
    class Program {
        static void Main(string[] args) {

            Player player = new Player("Default Name");
            player.storyline = Items.intro(player);

            Console.Clear();

            Program program = new Program();
            Console.WriteLine("Welcome to a Normal Game of BlackJack. You and the computer starts with 2 cards. The computer will show one of it's cards to you." +
                                "You win when the total value of your cards reach 21 (Black Jack) or is closest to 21. Face cards are treated with a value of 10 and Aces are " +
                                "treated as 11 or 1 depending on the count. You can draw more cards but \'bust\' if your total goes above 21.\n");
            Console.WriteLine("Add the amount of chips you are willing to bet and earn a point to add to your score each time you win a round. Good luck!\n");
            Console.Write("Your name --> ");
            String name = Console.ReadLine();

            if (name == null || name == "")
                name = "Someone Hiding";
            player.name = name;
            program.gameStart(player);

        }
        void gameStart(Player player)
        {
            
            player.chips = 100;
            int playerBet = 0;
            int totalCardCount = 0;
            int roundNum = 0;
            int aces = 0;

            List<Card> normalDeck = new List<Card>();
            List<Card> evilDeck = new List<Card>();
            List<Card> bonusDeck = new List<Card>();

            NormalCard.initializeDeck(normalDeck);
            EvilCard.initializeDeck(evilDeck);
            BonusCard.initialize(bonusDeck);

            totalCardCount = normalDeck.Count + evilDeck.Count + bonusDeck.Count;

            while (totalCardCount > 15 && player.chips > 0)
            {
                Console.Clear();
                Program.checkTriggers(player);

                Console.Clear();
                player.currentRoundPoints = 0;
                roundNum++;
                Console.WriteLine($"Hi {player.name}, here are your current stats.\nCards Drawn : {player.cardDraws}\nBlackJacks :  {player.blackJacks}\nBusts : {player.busts}");
                Console.WriteLine($"Current score: {player.score}");
                Console.WriteLine($"\nRound {roundNum}");
                Console.WriteLine($"You currently have {player.chips} chips.");
                Console.WriteLine($"There are currently {totalCardCount} cards.");
                Console.WriteLine($"There are {normalDeck.Count} Normal Cards, {evilDeck.Count} Evil Cards and {bonusDeck.Count} Bonus Cards.");
                Console.WriteLine();

                List<Card> playerHand = new List<Card>();
                List<Card> computerHand = new List<Card>();
                playerBet = 0;

                bool isRoundWin = roundStart(player, normalDeck, evilDeck, bonusDeck, playerHand, ref playerBet, computerHand, ref aces);

                if (isRoundWin)
                {
                    Console.WriteLine("Congrats on winning");
                    Console.WriteLine("You won " + (playerBet * player.multiplier + playerBet) + " chips.");
                    player.chips += (playerBet * player.multiplier + playerBet);
                    player.score++;
                }
                else
                {
                    Console.WriteLine("You Lose");
                    Console.WriteLine("");
                    Console.WriteLine();
                    if(player.score != 0)
                        player.score--;
                }
                totalCardCount = normalDeck.Count + evilDeck.Count + bonusDeck.Count;

                String answer = "";
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("Do you want to play another round?[y/n]");
                    Console.Write("--> ");
                    try
                    {
                        answer = Console.ReadLine().ToLower();
                        if (answer == "y" || answer == "n")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("There are literally only two options ffs.");
                            player.notFollowingInstructions++;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Ur getting on my nerves");
                        player.notFollowingInstructions++;
                    }
                }
                if (answer == "y")
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Game Over.");
            Console.WriteLine("Your Score is: " + player.score);
        }
        void incrementCardCounter(Player player, Card card)
        {
            player.cardDraws++;
            switch (card.suite)
            {
                case "Diamonds": player.diamondDraws++; break;
                case "Hearts": player.heartDraws++; break;
                case "Spades": player.spadeDraws++; break;
                case "Clubs": player.clubDraws++; break;
            }

            switch (card.rarity)
            {
                case eRarity.Special: player.specialDraws++; break;
                case eRarity.Evil: player.evilDraws++; break;
            }
        }
        bool roundStart(Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck, List<Card> playerHand, ref int playerBet, List<Card> computerHand, ref int aces)
        {
            int playerHandValue = 0;
            int computerHandValue = 0;
            aces = 0;


            while (true)
            {
                if (player.notFollowingInstructions > 3)
                {
                    Console.WriteLine("Since ur not following instructions, you're betting everything this round.");
                    playerBet = player.chips;
                    player.notFollowingInstructions = 0;
                    break;
                }
                Console.WriteLine("How much are you betting?");
                Console.Write("--> ");

                try
                {
                    playerBet = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Gang, at least put a solid number in there.");
                    player.notFollowingInstructions++;
                    continue;
                }
                if (playerBet == 0)
                {
                    Console.WriteLine("Ur broke LMAO.");
                    break;
                }
                else if (playerBet > player.chips)
                {
                    Console.WriteLine("You don't have that kinda cash on u.");
                    continue;
                }
                else
                {
                    player.chips -= playerBet;
                    Console.WriteLine("No turning back now.");
                    Console.WriteLine();
                    break;
                }
            }
            Console.WriteLine();

            for (int i = 0; i < 2; i++)
            {
                playerHand.Add(draw(normalDeck));
                player.cardDraws++;
                computerHand.Add(draw(normalDeck));
            }
            foreach (Card card in playerHand)
            {
                Thread.Sleep(2000);
                Console.WriteLine("You drew the " + card.value + " of " + card.suite + ".");
                addPoints(card, player);
            }

            foreach (Card card in computerHand)
            {
                addPoints(card, ref computerHandValue, ref aces);
            }
            Card firstComCard = computerHand[0];
            Thread.Sleep(2000);
            Console.WriteLine("The computer drew the " + firstComCard.value + " of " + firstComCard.suite);

            while (player.currentRoundPoints != 21 && player.currentRoundPoints != 0)
            {
                if (!playerDraw(player, normalDeck, evilDeck, bonusDeck, playerHand))
                {
                    break;
                }
            }

            if (player.currentRoundPoints == 21)
            {
                Console.WriteLine();
                Console.WriteLine("BLACKJAAAAACKKKK RAHHHHH!!!");
                Console.WriteLine();
                player.blackJacks++;
                return true;
            }
            else if (player.currentRoundPoints == 0)
            {
                Console.WriteLine();
                Console.WriteLine("It's a BUST!");
                Console.WriteLine();
                player.busts++;
                return false;
            }

            while (computerDraw(normalDeck, computerHand, ref computerHandValue, ref aces));

                for (int i = 1; i < computerHand.Count; i++)
                {
                    Thread.Sleep(3000);
                    Console.WriteLine($"The computer drew the {computerHand[i].value} of {computerHand[i].suite}");
                }
            Console.WriteLine("The computer scored " + computerHandValue);
            playerHandValue = player.currentRoundPoints;

            Console.WriteLine();

            if (computerHandValue > playerHandValue || computerHandValue == playerHandValue)
            {
                return false;
            }
            return true;
        }

        bool computerDraw(List<Card> normalDeck, List<Card> computerHand, ref int currentPoints, ref int aces)
        {
            if (currentPoints == 0)
            {
                Console.WriteLine();
                Console.WriteLine("I Busted :(");
                Console.WriteLine();
                return false;
            }
            else if (currentPoints == 21)
            {
                Console.WriteLine();
                Console.WriteLine("BlackJack for me lil bro");
                Console.WriteLine();
                return false;
            }

            else if (currentPoints < 17)
            {
                Card card = draw(normalDeck);
                computerHand.Add(card);
                addPoints(card, ref currentPoints, ref aces);
                return true;
            }
            return false;
        }

        bool playerDraw(Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck, List<Card> playerHand)
        {
            Console.WriteLine();
            Console.WriteLine("Draw a card from one of these decks:");
            Console.WriteLine("Type [N] to draw from the \'Normal Deck\'.");
            Console.WriteLine("Type [E] to draw from the \'Evil Deck\'.");
            Console.WriteLine("Type [B] to draw from the \'Bonus Deck\'.");
            Console.WriteLine("Type [S] to stand.");
            Console.WriteLine("Choose wisely gang u got this fr.");
            Console.Write("Your answer --> ");
            String answer = "";

            while (true)
            {
                try
                {
                    answer = Console.ReadLine().ToLower();
                    if (answer != "n" && answer != "e" && answer != "b" && answer != "s")
                    {
                        Console.WriteLine("That's not in the choices, gang. Try again will you?");
                        Console.Write("Your answer --> ");
                        player.notFollowingInstructions++;
                    }
                    else
                    {
                        break;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("At least try to answer it gang </3.");
                    Console.Write("Your answer --> ");
                }
            }
            Console.WriteLine();
            Card currentCard = null;

            Console.WriteLine("...");
            Thread.Sleep(3000);
            switch (answer)
            {
                case "n":
                    currentCard = draw(normalDeck);

                    break;
                case "e":
                    currentCard = draw(evilDeck);
                    break;
                case "b":
                    if (player.chips < 5)
                    {
                        Console.WriteLine("You're too broke for this");
                        return true;
                    }
                    Console.WriteLine("Buy a Bonus Card for 5 chips?[y/n]");
                    while (true)
                    {
                        try
                        {
                            answer = Console.ReadLine().ToLower();

                            if (answer == "y" || answer == "n")
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("It is literally a yes or no question gang, just pick one");
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("You're testing my patience fr.");
                        }
                    }
                    if (answer == "y")
                    {
                        player.chips -= 5;
                        currentCard = draw(bonusDeck);
                    }
                    break;
                case "s":
                    player.stands++;
                    return false;

            }
            if (currentCard == null)
            {
                return false;
            }

            Console.WriteLine("You have drawn the " + currentCard.value + " of " + currentCard.suite);
            incrementCardCounter(player, currentCard);
            playerHand.Add(currentCard);

            if (currentCard.value == "Joker")
            {
                while (normalDeck.Count > 21)
                {
                    normalDeck.RemoveAt(RNG.random.Next(normalDeck.Count - 1));
                }
            }
            else { currentCard.special(player, normalDeck, evilDeck, bonusDeck); }

            addPoints(currentCard, player);

            if (player.currentRoundPoints == 0)
            {
                return false;
            }
            if (player.currentRoundPoints == 21)
            {
                return false;
            }

            player.cardDraws++;
            Console.WriteLine("Current Points : " + Convert.ToString(player.currentRoundPoints));

            return true;
        }

        public Card draw(List<Card> deck)
        {
            if (deck.Count == 0)
            {
                Console.WriteLine("That deck is empty!");
                return null;
            }

            int index = RNG.random.Next(deck.Count);
            Card card = deck[index];
            deck.RemoveAt(index);
            return card;
        }

        public void addPoints(Card card, Player player)
        {
            int currentPoints = player.currentRoundPoints;

            if (card.value == "Jack" || card.value == "Queen" || card.value == "King" || card.value == "Joker")
            {
                currentPoints += 10;
            }
            else if (card.value == "Ace")
            {
                currentPoints += 11;
                player.aces++;
            }
            else
            {
                currentPoints += Convert.ToInt32(card.value);
            }
            while(currentPoints > 21 && player.aces > 0)
            {
                currentPoints -= 10;
                player.aces--;
            }

            player.currentRoundPoints = currentPoints;

            if (player.currentRoundPoints > 21)
            {
                player.currentRoundPoints = 0;
            }
            return;
        }


        public void addPoints(Card card, ref int currentPoints, ref int aces)
        {
            if (card.value == "Jack" || card.value == "Queen" || card.value == "King" || card.value == "Joker")
            {
                currentPoints += 10;
            }
            else if (card.value == "Ace")
            {
                aces++;
                currentPoints += 11;
            }
            else
            {
                currentPoints += Convert.ToInt32(card.value);
            }

            while (currentPoints > 21 && aces > 0)
            {
                currentPoints -= 10;
                aces--;
            }
            if (currentPoints > 21)
                currentPoints = 0;
        }


        public static void checkTriggers(Player player)
        {
            if (player.storyLineTrigger && player.availableItems.Count != 0)
            {
                Items.itemTrigger(player.availableItems[0], player);
                player.storyLineTrigger = false;
                return;
            }
            if (player.cardDraws > 200 && !player.playerInventory.Any(i => i.itemID == 2))
            {
                Items item = new Items("Briefcase", "An empty black briefcase.", 2);
                player.playerInventory.Add(item);
                item.briefCaseTrigger();
            }
            if (player.busts >= 50 && !player.playerInventory.Any(i => i.itemID == 6))
            {
                Items item = new Items("Glock 17", "A gun with a bullet in the chamber.", 6);
                player.playerInventory.Add(item);
                item.gunTrigger();
            }
            if (player.evilDraws > 100 && !player.playerInventory.Any(i => i.itemID == 8))
            {
                Items item = new Items("Bloody Diamond Ring", "This isn't yours", 8);
                player.playerInventory.Add(item);
                item.ringTrigger();
            }
            if (player.chips > 5000 && player.availableItems.Any(i => i.itemID == 1))
            {
                Items item = player.availableItems.FirstOrDefault(i => i.itemID == 1);
                player.playerInventory.Add(item);
                Items.hospitalBillTrigger(player);
                player.availableItems.Remove(player.availableItems.FirstOrDefault(i => i.itemID == 1));
            }
            if (player.blackJacks > 50 && player.availableItems.Any(i => i.itemID == 9))
            {
                Items item = player.availableItems.FirstOrDefault(i => i.itemID == 9);
                player.playerInventory.Add(item);
                Items.hennyBottleTrigger(player);
                player.availableItems.Remove(player.availableItems.FirstOrDefault(i => i.itemID == 9));
            }
            if (player.specialDraws > 50 && player.availableItems.Any(i => i.itemID == 3))
            {
                Items item = player.availableItems.FirstOrDefault(i => i.itemID == 3);
                player.playerInventory.Add(item);
                Items.recitalInvTrigger(player);
                player.availableItems.Remove(player.availableItems.FirstOrDefault(i => i.itemID == 3));
            }
            if (player.diamondDraws > 50 && player.availableItems.Any(i => i.itemID == 4))
            {
                Items item = player.availableItems.FirstOrDefault(i => i.itemID == 4);
                player.playerInventory.Add(item);
                Items.gasCanTrigger(player);
                player.availableItems.Remove(player.availableItems.FirstOrDefault(i => i.itemID == 4));
            }
            if (player.clubDraws > 50 && player.availableItems.Any(i => i.itemID == 5))
            {
                Items item = player.availableItems.FirstOrDefault(i => i.itemID == 5);
                player.playerInventory.Add(item);
                Items.matchTrigger(player);
                player.availableItems.Remove(player.availableItems.FirstOrDefault(i => i.itemID == 5));
            }
            if (player.score > 20 && player.availableItems.Any(i => i.itemID == 7))
            {
                Items item = player.availableItems.FirstOrDefault(i => i.itemID == 7);
                player.playerInventory.Add(item);
                Items.carKeyTrigger(player);
                player.availableItems.Remove(player.availableItems.FirstOrDefault(i => i.itemID == 7));
            }
        } 
    } 
}
