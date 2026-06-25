namespace The_Neural_Pitwall;

public static class Scoring
{
    public static double BaseScore(double totalTime)
    {
        // Simplified scoring formula based on lap time, tyre degradation, fuel usage, and pit stop time
        double score = 1000000000 / totalTime; // Base score inversely proportional to total time
        return score;
    }

    public static double FuelBonus(double fuelUsed, double fuelSoftCapLimit)
    {
        // Simplified formula for fuel bonus based on fuel usage and soft cap limit
        double bonus = 1000000 - 1000000 * (1 - (fuelUsed / fuelSoftCapLimit)); // Bonus decreases as fuel usage approaches the soft cap limit
        return bonus;
    }

    public static double TyreBonus(double tyreDegradation, double numberOfBLowouts)
    {
        // Simplified formula for tyre bonus based on tyre degradation and soft cap limit
        double bonus = 100000 * tyreDegradation - 50000 * numberOfBLowouts; // Bonus decreases as tyre degradation approaches the soft cap limit
        return bonus;
    }

    public static double TotalScore(double totalTime, double fuelUsed, double fuelSoftCapLimit, double tyreDegradation, double numberOfBLowouts)
    {
        // Calculate the total score based on base score, fuel bonus, and tyre bonus
        double baseScore = BaseScore(totalTime);
        double fuelBonus = FuelBonus(fuelUsed, fuelSoftCapLimit);
        double tyreBonus = TyreBonus(tyreDegradation, numberOfBLowouts);

        double totalScore = baseScore + fuelBonus + tyreBonus;
        return totalScore;
    }



}