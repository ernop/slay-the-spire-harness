namespace StS
{

    public enum FightActionEnum
    {
        PlayCard = 1,
        Potion = 2,
        EndTurn = 3,
        StartTurn = 4,

        StartTurnEffect = 5,
        EndTurnEffect = 6,
        EndTurnDeckEffect = 7,
        EndTurnOtherEffect = 8,

        StartFightEffect = 9,
        EndFightEffect = 10,

        EnemyMove = 12,
        
        EnemyDied = 15,
        
        EndEnemyTurn = 17,

        StartFight = 20,
        WonFight = 21,
        LostFight = 22,
        
        TooLong = 25,
        NotInitialized = 99,
    }
}