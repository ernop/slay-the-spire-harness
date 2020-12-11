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

        EnemyAttack = 12,
        EnemyBuff = 13,
        EnemyStatusAttack = 14,
        EnemyDied = 15,
        EndEnemyTurn = 16,

        StartFight = 20,
        WonFight = 21,
        LostFight = 22,
        TooLong = 25,


    }
}