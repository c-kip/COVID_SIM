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
    //Virus.Stages to represent different levels of infection
    
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
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Asymptotic] * getAsymptotic(0), 1/*getAsymptotic() / 10*/, 0, getAsymptotic(0)*5));
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Mild] * getMild(0), 1/*getMild() / 10*/, 0, getMild(0) * 5));
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Severe] * getSevere(0), 1/*getSevere() / 10*/, 0, getSevere(0) * 5));
        incInf(Main.calcRandNum(Virus.infSpreadRates[Virus.Stages.Deadly] * getDeadly(0), 1/*getDeadly() / 10*/, 0, getDeadly(0) * 5));
    }

    //Increases the base number of infected (infecting healthy individuals)
    private void incInf(int num)
    {
        healthy -= num;
        this.infected[Virus.Stages.Mild][0] += num;
    }

    //Moves infected between states
    private void moveInf(int num, Virus.Stages start, Virus.Stages end)
    {
        this.infected[start][0] -= num;
        this.infected[end][0] += num;
    }

    //Move infected out of infected state (either immune or deceased)
    private void rmInf(int num, Virus.Stages start, bool immunity)
    {
        this.infected[start][0] -= num;
        if (immunity)
        {
            immune += num;
        } else
        {
            total -= num;
            deceased += num;
        }
    }

    private void detect(int num, Virus.Stages start)
    {
        this.infected[start][0] -= num;
        this.infected[start][1] += num;
    }

    private void cycleStages()
    {
        //Map all the possible paths between states

        //Map an increment in stage severity
        moveInf(Main.calcRandNum(Virus.infStageIncRates[Virus.Stages.Asymptotic] * getAsymptotic(0), 1, 0, getAsymptotic(0)), Virus.Stages.Asymptotic, Virus.Stages.Mild);
        moveInf(Main.calcRandNum(Virus.infStageIncRates[Virus.Stages.Mild] * getMild(0), 1, 0, getMild(0)), Virus.Stages.Mild, Virus.Stages.Severe);
        moveInf(Main.calcRandNum(Virus.infStageIncRates[Virus.Stages.Severe] * getSevere(0), 1, 0, getSevere(0)), Virus.Stages.Severe, Virus.Stages.Deadly);
        rmInf(Main.calcRandNum(Virus.infStageIncRates[Virus.Stages.Deadly] * getDeadly(0), 1, 0, getDeadly(0)), Virus.Stages.Deadly, false);

        //Map a decrement in stage severity
        moveInf(Main.calcRandNum(Virus.infStageDecRates[Virus.Stages.Deadly] * getDeadly(0), 1, 0, getDeadly(0)), Virus.Stages.Deadly, Virus.Stages.Severe);
        moveInf(Main.calcRandNum(Virus.infStageDecRates[Virus.Stages.Severe] * getSevere(0), 1, 0, getSevere(0)), Virus.Stages.Severe, Virus.Stages.Mild);
        rmInf(Main.calcRandNum(Virus.infStageDecRates[Virus.Stages.Mild] * getMild(0), 1, 0, getMild(0)), Virus.Stages.Mild, true);
        rmInf(Main.calcRandNum(Virus.infStageDecRates[Virus.Stages.Asymptotic] * getAsymptotic(0), 1, 0, getAsymptotic(0)), Virus.Stages.Asymptotic, true);

        //Map a change from "standard" to "healing"
        detect(Main.calcRandNum(Virus.infDetectRates[Virus.Stages.Asymptotic] * getAsymptotic(0), 1, 0, getAsymptotic(0)), Virus.Stages.Asymptotic);
        detect(Main.calcRandNum(Virus.infDetectRates[Virus.Stages.Severe] * getSevere(0), 1, 0, getSevere(0)), Virus.Stages.Severe);
        detect(Main.calcRandNum(Virus.infDetectRates[Virus.Stages.Mild] * getMild(0), 1, 0, getMild(0)), Virus.Stages.Mild);
        detect(Main.calcRandNum(Virus.infDetectRates[Virus.Stages.Deadly] * getDeadly(0), 1, 0, getDeadly(0)), Virus.Stages.Deadly);

        //Map an increment in "healing" stage severity


        //Map a decrement in "healing" stage severity

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
}
