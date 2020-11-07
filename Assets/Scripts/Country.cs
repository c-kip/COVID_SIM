using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    private Population people;
    private LinkedList<Country> transportRoutes;
    private int healthRating;

    public Country(Population people, int healthRating)
    {
        this.people = people;
        transportRoutes = new LinkedList<Country>();
        this.healthRating = healthRating;
    }

    public void addTransportRoute(Country other)
    {
        transportRoutes.AddLast(other);
    }
}
