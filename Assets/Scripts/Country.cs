using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    private string countryName;
    private Population people;
    private LinkedList<TravelRoute> transportRoutes;
    private int healthRating;
    private double travelProb;

    public Country(string countryName, Population people, double travelProb)
    {
        this.countryName = countryName;
        this.people = people;
        this.travelProb = travelProb;
        transportRoutes = new LinkedList<TravelRoute>();
    }

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
                Debug.Log("Route from " + this.countryName + " to " + route.getSecond().getName());
                otherPeople = route.getSecond().getPeople();
            }
            for (int i = 0; i <= 1; i++)
            {
                //Asymptomatic
                travellers = Main.calcRandNum(travelProb * people.getAsymptotic(i), 1, 0, people.getAsymptotic(i));
                people.rmInf(travellers, Virus.Stages.Asymptotic, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Asymptotic, i);

                //Mild
                travellers = Main.calcRandNum(travelProb * people.getMild(i), 1, 0, people.getMild(i));
                people.rmInf(travellers, Virus.Stages.Mild, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Mild, i);

                //Severe
                travellers = Main.calcRandNum(travelProb * people.getSevere(i), 1, 0, people.getSevere(i));
                people.rmInf(travellers, Virus.Stages.Severe, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Severe, i);

                //Deadly
                travellers = Main.calcRandNum(travelProb * people.getDeadly(i), 1, 0, people.getDeadly(i));
                people.rmInf(travellers, Virus.Stages.Deadly, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Deadly, i);
            }
        }
    }

    public void addTransportRoute(Country other, double travelProb)
    {
        transportRoutes.AddLast(new TravelRoute(this, other, travelProb));
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
}
