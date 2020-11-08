using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    private string countryName;
    private Population people;
    private LinkedList<Country> transportRoutes;
    private int healthRating;

    public Country(string countryName, Population people, int healthRating)
    {
        this.countryName = countryName;
        this.people = people;
        this.healthRating = healthRating;
        transportRoutes = new LinkedList<Country>();
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
}
