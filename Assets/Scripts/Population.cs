using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a population (e.g. country, continent, world, etc.)
public class Population : MonoBehaviour
{
    private int total;
    private int healthy;
    //Virus.Stages to represent different levels of infection
    
    private Dictionary<Virus.Stages, int> infected = new Dictionary<Virus.Stages, int>() {
        {Virus.Stages.Asymptotic, 0},
        {Virus.Stages.Mild, 0},
        {Virus.Stages.Severe, 0},
        {Virus.Stages.Deadly, 0}
    }; 
    
    // Standard constructor for Population object
    public Population(int total, int asymptotic, int mild, int severe, int deadly)
    {
        this.total = total;
        this.infected[Virus.Stages.Asymptotic] = asymptotic;
        this.infected[Virus.Stages.Mild] = mild;
        this.infected[Virus.Stages.Severe] = severe;
        this.infected[Virus.Stages.Deadly] = deadly;
        this.healthy = total - asymptotic - mild - severe - deadly;
    }

    // Cycles one day forward in ticks (e.g. days)
    public void cycle()
    {
        //Cycle the state machine (virus stages)


        //Increase the number of infected
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Asymptotic] * getAsymptotic(), 1/*getAsymptotic() / 10*/));
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Mild] * getMild(), 1/*getMild() / 10*/));
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Severe] * getSevere(), 1/*getSevere() / 10*/));
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Deadly] * getDeadly(), 1/*getDeadly() / 10*/));
    }

    private void incInf(int num)
    {
        healthy -= num;
        this.infected[Virus.Stages.Mild] += num;
    }

    private void cycleStages()
    {

    }

    public int getTotal()
    {
        return total;
    }

    public int getHealthy()
    {
        return healthy;
    }

    public int getInfected()
    {
        return getAsymptotic() + getMild() + getSevere() + getDeadly();
    }

    public int getAsymptotic()
    {
        return this.infected[Virus.Stages.Asymptotic];
    }
    public int getMild()
    {
        return this.infected[Virus.Stages.Mild];
    }
    public int getSevere()
    {
        return this.infected[Virus.Stages.Severe];
    }
    public int getDeadly()
    {
        return this.infected[Virus.Stages.Deadly];
    }
}
