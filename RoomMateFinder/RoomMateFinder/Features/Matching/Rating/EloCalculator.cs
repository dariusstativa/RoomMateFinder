namespace RoomMateFinder.Features.Matching.Rating;

public static class EloCalculator
{
    public static int CalculateNewRating(int current, int opponent, bool isWin)
    {
        double expected = 1.0 / (1.0 + Math.Pow(10, (opponent - current) / 400.0));
        int score = isWin ? 1 : 0;
        int k = 32;

        return (int)(current + k * (score - expected));
    }
}