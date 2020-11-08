using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    private string countryName;
    private Population people;
    private LinkedList<Country> transportRoutes;
    private int healthRating;
    private double travelProb;

    public Country(string countryName, Population people, double travelProb)
    {
        this.countryName = countryName;
        this.people = people;
        this.travelProb = travelProb;
        transportRoutes = new LinkedList<Country>();
    }

    public void transportInf()
    {
        people.rmInf(Main.calcRandNum(travelProb * people.getTotal(), 1, 0, people.getTotal()), Virus.Stages.Asymptotic, 2, 0);
    }

    public void addTransportRoute(Country other)
    {
        transportRoutes.AddLast(other);
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
