using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static System.Random rng;
    private Population canadians;
    private Country Canada;
    private const int MAX_LOOPS = 20;

    // Start is called before the first frame update
    void Start()
    {
        //Create the randomizer
        rng = new System.Random();

        //Create the countries (or continents) that we'll use
        canadians = new Population(1000, 1, 0, 0, 0);

        int i = 0;
        StartCoroutine(dailyCycle(i));
    }

    //Create a Coroutine 
    public IEnumerator dailyCycle(int i)
    {
        if (i > MAX_LOOPS)
        {
            yield return null;
        }

        //yield on a new YieldInstruction that waits for x seconds.
        yield return new WaitForSeconds(1);

        //Print out the calculated random number
        canadians.cycle();
        Debug.Log("Population: " + canadians.getTotal());
        Debug.Log("   Healthy: " + canadians.getHealthy());
        Debug.Log("    Immune: " + canadians.getImmune());
        Debug.Log("  Infected: " + canadians.getInfected());
        Debug.Log("Asymptotic: " + canadians.getAsymptotic(2) + " 0: " + canadians.getAsymptotic(0) + " 1: " + canadians.getAsymptotic(1));
        Debug.Log("      Mild: " + canadians.getMild(2) + " 0: " + canadians.getMild(0) + " 1: " + canadians.getMild(1));
        Debug.Log("    Severe: " + canadians.getSevere(2) + " 0: " + canadians.getSevere(0) + " 1: " + canadians.getSevere(1));
        Debug.Log("    Deadly: " + canadians.getDeadly(2) + " 0: " + canadians.getDeadly(0) + " 1: " + canadians.getDeadly(1));

        //Increment the loop counter and go again
        i++;
        StartCoroutine(dailyCycle(i));
    }

    // Calculates a random number based on gaussian distribution for the given mean and standard deviation
    // Praise be the coders/mathematicians/staticians that wrote this https://stackoverflow.com/questions/218060/random-gaussian-variables
    public static int calcRandNum(double mean, double stdDev, int lowBound, int highBound)
    {
        if (lowBound >= highBound)
        {
            return 0;
        }

        double u1;
        double u2;
        double rngStdNormal;
        double rngNormal;

        int tries = 0;
        // Forces the system to produce a positive number (yes this is a terrible way to do this, however I am a terrible programmer - CHECKMATE)
        do
        {
            u1 = 1.0 - rng.NextDouble(); //uniform(0,1] random doubles
            u2 = 1.0 - rng.NextDouble();
            rngStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            rngNormal = mean + stdDev * rngStdNormal; //random normal(mean,stdDev^2)
            tries++;
        } while (tries < 5 && (rngNormal < lowBound || rngNormal > highBound));

        if (tries >= 5)
        {
            return 0;
        }

        return (int)rngNormal;
    }
}
