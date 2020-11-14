using System.Collections.Generic;

namespace StS
{

    public class Target
    {
        public Player player { get; set; }
        public Enemy enemy { get; set; }
        public List<Enemy> enemyList { get; set; }
    }
}
