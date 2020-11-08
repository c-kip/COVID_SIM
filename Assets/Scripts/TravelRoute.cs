using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelRoute
{
    private Country first;
    private Country second;
    private double travelProb;

    public TravelRoute(Country first, Country second, double travelProb)
    {
        this.first = first;
        this.second = second;
        this.travelProb = travelProb;
        Main.planeCount++;
    }

    public Country getFirst()
    {
        return first;
    }

    public Country getSecond()
    {
        return second;
    }
    public double getTravelProb()
    {
        return travelProb;
    }

}
