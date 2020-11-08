using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    private string countryName;
    private Population people;
    private LinkedList<TravelRoute> transportRoutes;
    private double healthRating;
    private double invHealthRating;

    public static Dictionary<Virus.Stages, double> infSpreadRates = new Dictionary<Virus.Stages, double>();
    public static Dictionary<Virus.Stages, double[]> infStageIncRates = new Dictionary<Virus.Stages, double[]>();
    public static Dictionary<Virus.Stages, double[]> infStageDecRates = new Dictionary<Virus.Stages, double[]>();
    public static Dictionary<Virus.Stages, double> infDetectRates = new Dictionary<Virus.Stages, double>();

    public Country(string countryName, Population people, double healthRating)
    {
        this.countryName = countryName;
        this.people = people;
        this.healthRating = healthRating;
        this.invHealthRating = Math.Pow(healthRating, -1);
        transportRoutes = new LinkedList<TravelRoute>();
        
        foreach (Virus.Stages stage in Enum.GetValues(typeof(Virus.Stages)))
        {
            infSpreadRates.Add(stage, Virus.infSpreadRates[stage] * invHealthRating);
            infStageIncRates.Add(stage, new double[] { Virus.infStageIncRates[stage][0] * invHealthRating, Virus.infStageIncRates[stage][1] * invHealthRating });
            infStageDecRates.Add(stage, new double[] { Virus.infStageDecRates[stage][0] * invHealthRating, Virus.infStageDecRates[stage][1] * invHealthRating });
            infDetectRates.Add(stage, Virus.infDetectRates[stage] * invHealthRating);
        }
    }

    //Transport random numbers of gaussian distributed passengers between countries
    public void transportInf()
    {
        int travellers = 0;
        foreach (TravelRoute route in transportRoutes)
        {
            Population otherPeople;
            if (route.getFirst().getName() != this.countryName)
            {
                otherPeople = route.getFirst().getPeople();
            } else
            {
                otherPeople = route.getSecond().getPeople();
            }
            for (int i = 0; i <= 1; i++)
            {
                //Asymptomatic
                travellers = Main.calcRandNum(route.getTravelProb() * people.getAsymptotic(i), 1, 0, people.getAsymptotic(i));
                people.rmInf(travellers, Virus.Stages.Asymptotic, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Asymptotic, i);

                //Mild
                travellers = Main.calcRandNum(route.getTravelProb() * people.getMild(i), 1, 0, people.getMild(i));
                people.rmInf(travellers, Virus.Stages.Mild, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Mild, i);

                //Severe
                travellers = Main.calcRandNum(route.getTravelProb() * people.getSevere(i), 1, 0, people.getSevere(i));
                people.rmInf(travellers, Virus.Stages.Severe, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Severe, i);

                //Deadly
                travellers = Main.calcRandNum(route.getTravelProb() * people.getDeadly(i), 1, 0, people.getDeadly(i));
                people.rmInf(travellers, Virus.Stages.Deadly, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Deadly, i);
            }
        }
    }

    //Add a transport route to this country
    public void addTransportRoute(Country other, double travelProb)
    {
        transportRoutes.AddLast(new TravelRoute(this, other, travelProb));
    }

    public LinkedList<TravelRoute> getTransportRoutes()
    {
        return transportRoutes;
    }

    public Population getPeople()
    {
        return people;
    }

    public string getName()
    {
        return countryName;
    }

    public bool Equals(Country other)
    {
        if (other == null)
        {
            return false;
        }
        if (this.countryName == other.countryName)
        {
            return true;
        }
        return false;
    }

    
    public override string ToString()
    {
        return this.getName() + "\n" +
                "Health Rating: " + healthRating + "\n" +
                "Population: " + people.getTotal() + "\n" +
                "Healthy: " + people.getHealthy() + "\n" +
                "Immune: " + people.getImmune() + "\n" +
                "Infected: " + people.getInfected() + "\n" +
                "Asymptotic: " + people.getAsymptotic(2) + " - Undetected:" + people.getAsymptotic(0) + " - Detected:" + people.getAsymptotic(1) + "\n" +
                "Mild: " + people.getMild(2) + " - Undetected:" + people.getMild(0) + " - Detected:" + people.getMild(1) + "\n" +
                "Severe: " + people.getSevere(2) + " - Undetected:" + people.getSevere(0) + " - Detected:" + people.getSevere(1) + "\n" +
                "Deadly: " + people.getDeadly(2) + " - Undetected:" + people.getDeadly(0) + " - Detected:" + people.getDeadly(1);
    }
}
