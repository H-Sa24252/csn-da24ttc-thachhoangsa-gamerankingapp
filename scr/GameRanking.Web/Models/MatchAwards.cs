namespace GameRanking.Web.Models;

public static class MatchAwards
{
    public static int Score(bool won) => won ? 50 : 5;
    public static int Exp(bool won) => won ? 100 : 20;
}
