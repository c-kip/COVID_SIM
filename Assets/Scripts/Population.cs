using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a population (e.g. country, continent, world, etc.)
public class Population : MonoBehaviour
{
    private int total;
    private int healthy;
    private int immune;
    private int deceased;
    private Country parent;
    
    //Dictionary storing the values of each stage of infected in either the "standard" or "healing" category (indexes 0 and 1 respectively)
    private Dictionary<Virus.Stages, int[]> infected = new Dictionary<Virus.Stages, int[]>() {
        {Virus.Stages.Asymptotic, new int[]{0,0}},
        {Virus.Stages.Mild, new int[]{0,0}},
        {Virus.Stages.Severe, new int[]{0,0}},
        {Virus.Stages.Deadly, new int[]{0,0}}
    }; 
    
    // Standard constructor for Population object
    public Population(int total, int asymptotic, int mild, int severe, int deadly)
    {
        this.total = total;
        this.infected[Virus.Stages.Asymptotic][0] = asymptotic;
        this.infected[Virus.Stages.Mild][0] = mild;
        this.infected[Virus.Stages.Severe][0] = severe;
        this.infected[Virus.Stages.Deadly][0] = deadly;
        this.healthy = total - asymptotic - mild - severe - deadly;
    }

    // Cycles one day forward in ticks (e.g. days)
    public void cycle()
    {
        //Cycle the state machine (virus stages)
        cycleStages();

        //Increase the number of infected based on non-detected cases
        incInf(Main.calcRandNum(parent.infSpreadRatesLocal[Virus.Stages.Asymptotic] * getAsymptotic(0), 1/*getAsymptotic() / 10*/, 0, getAsymptotic(0) * 5)) ;
        incInf(Main.calcRandNum(parent.infSpreadRatesLocal[Virus.Stages.Mild] * getMild(0), 1/*getMild() / 10*/, 0, getMild(0) * 5));
        incInf(Main.calcRandNum(parent.infSpreadRatesLocal[Virus.Stages.Severe] * getSevere(0), 1/*getSevere() / 10*/, 0, getSevere(0) * 5));
        incInf(Main.calcRandNum(parent.infSpreadRatesLocal[Virus.Stages.Deadly] * getDeadly(0), 1/*getDeadly() / 10*/, 0, getDeadly(0) * 5));
    }

    //Increases the base number of infected (infecting healthy individuals)
    private void incInf(int num)
    {
        healthy -= num;
        this.infected[Virus.Stages.Mild][0] += num;
    }

    //Moves infected between states
    private void moveInf(int num, Virus.Stages start, Virus.Stages end, int type)
    {
        this.infected[start][type] -= num;
        this.infected[end][type] += num;
    }

    //Move infected out of infected state (either immune, deceased, or in another country)
    //0 = immune, 1 = deceased, 2 = moved countries
    public void rmInf(int num, Virus.Stages start, int state, int detected)
    {
        this.infected[start][detected] -= num;
        if (state == 0)
        {
            immune += num;
        } else if (state == 1)
        {
            total -= num;
            deceased += num;
        } else if (state == 2)
        {
            total -= num;
        }
    }

    //Move infected into the infected counters (these are from another country)
    public void addInf(int num, Virus.Stages start, int detected)
    {
        this.infected[start][detected] += num;
        total += num;
    }

    private void detect(int num, Virus.Stages start)
    {
        this.infected[start][0] -= num;
        this.infected[start][1] += num;
    }

    private void cycleStages()
    {
        //Map all the possible paths between states below
        for (int i = 0; i <= 1; i++)
        {
            //Map an increment in stage severity
            moveInf(Main.calcRandNum(parent.infStageIncRatesLocal[Virus.Stages.Asymptotic][i] * getAsymptotic(i), 1, 0, getAsymptotic(i)), Virus.Stages.Asymptotic, Virus.Stages.Mild, i);
            moveInf(Main.calcRandNum(parent.infStageIncRatesLocal[Virus.Stages.Mild][i] * getMild(i), 1, 0, getMild(i)), Virus.Stages.Mild, Virus.Stages.Severe, i);
            moveInf(Main.calcRandNum(parent.infStageIncRatesLocal[Virus.Stages.Severe][i] * getSevere(i), 1, 0, getSevere(i)), Virus.Stages.Severe, Virus.Stages.Deadly, i);
            rmInf(Main.calcRandNum(parent.infStageIncRatesLocal[Virus.Stages.Deadly][i] * getDeadly(i), 1, 0, getDeadly(i)), Virus.Stages.Deadly, 1, i);

            //Map a decrement in stage severity
            moveInf(Main.calcRandNum(parent.infStageDecRatesLocal[Virus.Stages.Deadly][i] * getDeadly(i), 1, 0, getDeadly(i)), Virus.Stages.Deadly, Virus.Stages.Severe, i);
            moveInf(Main.calcRandNum(parent.infStageDecRatesLocal[Virus.Stages.Severe][i] * getSevere(i), 1, 0, getSevere(i)), Virus.Stages.Severe, Virus.Stages.Mild, i);
            rmInf(Main.calcRandNum(parent.infStageDecRatesLocal[Virus.Stages.Mild][i] * getMild(i), 1, 0, getMild(i)), Virus.Stages.Mild, 0, i);
            rmInf(Main.calcRandNum(parent.infStageDecRatesLocal[Virus.Stages.Asymptotic][i] * getAsymptotic(i), 1, 0, getAsymptotic(i)), Virus.Stages.Asymptotic, 0, i);
        }

        //Map a change from "standard" to "healing"
        detect(Main.calcRandNum(parent.infDetectRatesLocal[Virus.Stages.Asymptotic] * getAsymptotic(0), 1, 0, getAsymptotic(0)), Virus.Stages.Asymptotic);
        detect(Main.calcRandNum(parent.infDetectRatesLocal[Virus.Stages.Severe] * getSevere(0), 1, 0, getSevere(0)), Virus.Stages.Severe);
        detect(Main.calcRandNum(parent.infDetectRatesLocal[Virus.Stages.Mild] * getMild(0), 1, 0, getMild(0)), Virus.Stages.Mild);
        detect(Main.calcRandNum(parent.infDetectRatesLocal[Virus.Stages.Deadly] * getDeadly(0), 1, 0, getDeadly(0)), Virus.Stages.Deadly);
    }

    public int getTotal()
    {
        return total;
    }

    public int getHealthy()
    {
        return healthy;
    }

    public int getImmune()
    {
        return immune;
    }

    public int getDeceased()
    {
        return deceased;
    }

    public int getInfected()
    {
        return getAsymptotic(2) + getMild(2) + getSevere(2) + getDeadly(2);
    }

    public int getAsymptotic(int group)
    {
        if (group == 0)
        {
            return this.infected[Virus.Stages.Asymptotic][0];
        } else if (group == 1)
        {
            return this.infected[Virus.Stages.Asymptotic][1];
        } else
        {
            return this.infected[Virus.Stages.Asymptotic][0] + this.infected[Virus.Stages.Asymptotic][1];
        }
    }
    public int getMild(int group)
    {
        if (group == 0)
        {
            return this.infected[Virus.Stages.Mild][0];
        }
        else if (group == 1)
        {
            return this.infected[Virus.Stages.Mild][1];
        }
        else
        {
            return this.infected[Virus.Stages.Mild][0] + this.infected[Virus.Stages.Mild][1];
        }
    }
    public int getSevere(int group)
    {
        if (group == 0)
        {
            return this.infected[Virus.Stages.Severe][0];
        }
        else if (group == 1)
        {
            return this.infected[Virus.Stages.Severe][1];
        }
        else
        {
            return this.infected[Virus.Stages.Severe][0] + this.infected[Virus.Stages.Severe][1];
        }
    }
    public int getDeadly(int group)
    {
        if (group == 0)
        {
            return this.infected[Virus.Stages.Deadly][0];
        }
        else if (group == 1)
        {
            return this.infected[Virus.Stages.Deadly][1];
        }
        else
        {
            return this.infected[Virus.Stages.Deadly][0] + this.infected[Virus.Stages.Deadly][1];
        }
    }

    public void setParent(Country parent)
    {
        this.parent = parent;
    }
}
