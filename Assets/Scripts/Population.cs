using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a population (e.g. country, continent, world, etc.)
public class Population : MonoBehaviour
{
    private int total;
    //Stages to represent different levels of infection
    private enum Stages
    {
        Asymptotic,
        Mild,
        Severe,
        Deadly
    };
    private Dictionary<Stages, int> infected = new Dictionary<Stages, int>() {
        {Stages.Asymptotic, 0},
        {Stages.Mild, 0},
        {Stages.Severe, 0},
        {Stages.Deadly, 0}
    };

    // Cycles one day forward in ticks (e.g. days)
    public void cycle()
    {

    }
}
