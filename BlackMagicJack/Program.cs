// See https://aka.ms/new-console-template for more information
using System;

namespace BlackMagicJack {
    class Program {
        static void Main(string[] args) {

            Program program = new Program();
            program.gameStart();

        }
        void gameStart()
        {
            Console.WriteLine("Welcome to a Normal Game of BlackJack. Enter your name:");
            Console.Write("--> ");
            String name = Console.ReadLine();
            if (name == null || name == "")
                name = "This person has no name";
            Player player = new Player(name);
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
                    Console.WriteLine("You won " + (playerBet * player.multiplier + playerBet));
                    player.chips += (playerBet * player.multiplier + playerBet);
                    player.score++;
                }
                else
                {
                    Console.WriteLine("You Lose");
                    Console.WriteLine("You're not for this lil bro");
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
                case eRarity.IDK: player.loreDraws++; break;
                case eRarity.Special: player.specialDraws++; break;
                case eRarity.Evil: player.evilDraws++; break;
            }
        }
        bool roundStart(Player player, List<Card> normalDeck, List<Card> evilDeck, List<Card> bonusDeck, List<Card> playerHand, ref int playerBet, List<Card> computerHand, ref int aces)
        {
            int playerHandValue = 0;
            int computerHandValue = 0;


            while (true)
            {
                if (player.notFollowingInstructions > 3)
                {
                    Console.WriteLine("Since ur not following instructions, Y=you're betting everything this round.");
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
                Console.WriteLine("You drew the " + card.value + " of " + card.suite + ".");
                addPoints(card, player);
            }
            foreach (Card card in computerHand)
            {
                computerHandValue = addPoints(card, computerHandValue, ref aces);
            }
            Card firstComCard = computerHand[0];
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
                return true;
            }
            else if (player.currentRoundPoints == 0)
            {
                Console.WriteLine();
                Console.WriteLine("It's a BUST!");
                Console.WriteLine();
                return false;
            }

            while (computerDraw(normalDeck, computerHand, ref computerHandValue, ref aces));
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
            if (currentPoints == 21)
            {
                Console.WriteLine();
                Console.WriteLine("BlackJack for me lil bro");
                Console.WriteLine();
                return false;
            }

            if (currentPoints < 17)
            {
                Card card = draw(normalDeck);
                computerHand.Add(card);
                currentPoints = addPoints(card, currentPoints, ref aces);
                Console.WriteLine($"The computer drew the {card.value} of {card.suite}");
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
            Console.WriteLine();

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
            Card currentCard = null;
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
            currentCard.special(player, normalDeck, evilDeck, bonusDeck);
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

            if (card.value == "Jack" || card.value == "Queen" || card.value == "King")
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

        public int addPoints(Card card, int currentPoints, ref int aces)
        {

            if (currentPoints > 21)
            {
                currentPoints = 0;
            }
            else if (currentPoints == 21)
            {
                return currentPoints;
            }
            else if (card.value == "Jack" || card.value == "Queen" || card.value == "King")
            {
                currentPoints += 10;
            }
            else if (card.value == "Ace")
            {
                aces++;
                currentPoints += 11;
                while (currentPoints > 21 && aces > 0)
                {
                    currentPoints -= 10;
                    aces--;
                }
            }
            else
            {
                currentPoints += Convert.ToInt32(card.value);
            }

            if (currentPoints > 21)
            {
                currentPoints = 0;
            }
            return currentPoints;
        }

    void checkTriggers(Player player)
        {

            if (player.cardDraws > 100 && !player.isTriggered1)
            {
                Console.WriteLine("");
                Console.WriteLine("Touch grass or something gahdahm.");
                player.isTriggered1 = true;
            }
            if (player.loreDraws >= 5 && !player.isTriggered6)
            {
                //add special ending
                player.isTriggered6 = true;
            }
            if (player.stands > 50 && !player.isTriggered2)
            {
                Console.WriteLine("");
                Console.WriteLine("Maybe you should stop being a lil b***** and stop standing so much.");
                player.isTriggered2 = true;
            }
            if (player.highestRound > 8 && !player.isTriggered3)
            {
                //trigger minigame
                player.isTriggered3 = true;
            }
            if (player.blackJacks > 10 && !player.isTriggered4)
            {
                Console.WriteLine("");
                Console.WriteLine("Ur always getting lucky these days have this (+1 multiplier).");
                player.multiplier++;
                player.isTriggered4 = true;
            }
            if (player.busts > 10 && !player.isTriggered5)
            {
                Console.WriteLine("");
                Console.WriteLine("Ok this maybe unfair. Have this to keep going +100 chips).");
                player.chips += 100;
                player.isTriggered5 = true;
            }
        } 
    } 
}
