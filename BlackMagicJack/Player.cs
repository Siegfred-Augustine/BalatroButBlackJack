using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagicJack
{
    public class Player
    {
        public String name = "Default Name";
        public int score = 0; //tracks the score of the player.
        public int chips = 0; //tracks players' current chips
        public int cardDraws = 0; //tracks total cards drawn by the player.
        public int evilDraws = 0; //tracks evil cards drawn.
        public int specialDraws = 0; //tracks special cards drawn.
        public int multiplier = 1; //multiplier for chip rewards.
        public int diamondDraws = 0; //can be added for a bonus.
        public int heartDraws = 0; //can be added for a bonus.
        public int spadeDraws = 0; //can be added for a bonus.
        public int clubDraws = 0; //can be added for a bonus.
        public int loreDraws = 0; //triggers a special game over.
        public int currentRoundPoints = 0; //tracks current players' total hand
        public int stands = 0; //calls out a player for being a coward.
        public int highestRound = 0; //tracks a trigger minigame.
        public int blackJacks = 0; //triggers a special text.
        public int busts = 0; //triggers pity chips.
        public int notFollowingInstructions = 0; //triggers all in bet.
        
        public Player(String name)
        {
            this.name = name;
        }
    }
}
