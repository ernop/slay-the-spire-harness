using System;
using System.Collections.Generic;

namespace StS
{
    public class GameContext
    {
        List<Enemy> Enemies = new List<Enemy>();
        public void Died(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Enemy:
                    Enemies.Remove((Enemy)entity);
                    break;
                case EntityType.Player:
                    GameOver();
                    break;
                default:
                    break;
            };
        }

        public void GameOver()
        {
            Console.WriteLine("Died");
        }
    }
}
