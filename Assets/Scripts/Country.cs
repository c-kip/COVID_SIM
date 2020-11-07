using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    private int totalPopulation;
    private int populationDensity;
    private int healthRating;
    private LinkedList<Country> transportRoutes;

    public Country(int totalPopulation, int populationDensity, int healthRating)
    {
        this.totalPopulation = totalPopulation;
        this.populationDensity = populationDensity;
        this.healthRating = healthRating;
    }

    public void addTransportRoute(Country other)
    {
        transportRoutes.AddLast(other);
    }
}
