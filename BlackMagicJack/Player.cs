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
        public int score = 0;
        public int chips = 0;
        public int cardDraws = 0;
        public int evilDraws = 0;
        public int specialDraws = 0;
        public int multiplier = 1;
        public int diamondDraws = 0;
        public int heartDraws = 0;
        public int spadeDraws = 0;
        public int clubDraws = 0;
        public int loreDraws = 0;
        public int currentRoundPoints = 0;
        
        public Player(String name)
        {
            this.name = name;
        }
    }
}
