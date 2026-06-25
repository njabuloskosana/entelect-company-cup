namespace The_Neural_Pitwall;

public static class Formulas
{
    public static double MaxCornerSpeed(double cornerRadius, double tyreFriction, double crawlConstant)
    {
        // Simplified formula for cornering speed
        double g = 9.81; // acceleration due to gravity in m/s^2
        double corneringSpeed = Math.Sqrt(tyreFriction * g * cornerRadius) + crawlConstant;
        return corneringSpeed;
    }
    public static double StraightDegradation(double degradationRateType, double degradationRate, double distance)
    {
        // Simplified formula for straight-line speed degradation
        double degradedUsed = degradationRateType*degradationRate*distance;
        return degradedUsed;
    }

    public static double CornerDegradation(double degradationRateType, double degradationRate, double radius, double speed)
    {
        // Simplified formula for cornering speed degradation
        double degradedUsed = degradationRateType*degradationRate*(speed*speed/radius);
        return degradedUsed;
    }

    public static double BrakeDegradation(double degradationRateType, double degradationRate, double initialspeed, double finalspeed)
    {
        // Simplified formula for braking speed degradation
        double speedDifference = (initialspeed/100)*(initialspeed/100) - (finalspeed/100)*(finalspeed/100);
        double degradedUsed = degradationRateType*degradationRate*speedDifference;
        return degradedUsed;
    }

    public static double TyreFriction(double baseFrictionCoef, double tyreDeg, double weatherMuliplier)
    {
        // Simplified formula for tyre friction based on compound, temperature, and pressure
        double friction = (baseFrictionCoef - tyreDeg) * weatherMuliplier;
        return friction;
    }

    public static double CrawlConstant(double baseCrawl, double tyreDeg, double weatherMuliplier)
    {
        // Simplified formula for crawl constant based on compound, temperature, and pressure
        double crawl = (baseCrawl - tyreDeg) * weatherMuliplier;
        return crawl;
    }

    public static double FuelUsage(double baseFuelConsumptionRate, double dragFuelConsumptionRate, double distance, double initialSpeed, double finalSpeed)
    {
        // Simplified formula for fuel usage based on consumption rate and distance
        double fuelUsed = (baseFuelConsumptionRate + dragFuelConsumptionRate * (initialSpeed + finalSpeed)/2  * (initialSpeed + finalSpeed)/2 ) * distance;
        return fuelUsed;
    }

    public static double PitStopTime(double basePitStopTime, double tyreChangeTime, double fuelRefillTime)
    {
        // Simplified formula for pit stop time based on base time and additional factors
        double totalPitStopTime = basePitStopTime + tyreChangeTime + fuelRefillTime ;
        return totalPitStopTime;
    }

    public static double RefuelTime(double fuelRefillRate, double fuelAmount)
    {
        // Simplified formula for refuel time based on refill rate and amount
        double refuelTime = fuelAmount / fuelRefillRate;
        return refuelTime;
    }


}
