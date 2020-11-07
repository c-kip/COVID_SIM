using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static System.Random rng;
    private Population test;
    private const int MAX_LOOPS = 5;

    // Start is called before the first frame update
    void Start()
    {
        //Create the randomizer
        rng = new System.Random();

        //Create the countries (or continents) that we'll use
        test = new Population(1000, 1, 0, 0, 0);

        int i = 0;
        StartCoroutine(dailyCycle(i));
    }

    //Create a Coroutine 
    public IEnumerator dailyCycle(int i)
    {
        //yield on a new YieldInstruction that waits for x seconds.
        yield return new WaitForSeconds(1);

        //Print out the calculated random number
        test.cycle();
        Debug.Log("Population: " + test.getTotal());
        Debug.Log("   Healthy: " + test.getHealthy());
        Debug.Log("  Infected: " + test.getInfected());
        Debug.Log("Asymptotic: " + test.getAsymptotic());
        Debug.Log("      Mild: " + test.getMild());
        Debug.Log("    Severe: " + test.getSevere());
        Debug.Log("    Deadly: " + test.getDeadly());

        //Increment the loop counter and go again
        i++;
        StartCoroutine(dailyCycle(i));
    }

    // Calculates a random number based on gaussian distribution for the given mean and standard deviation
    // Praise be the coders/mathematicians/staticians that wrote this https://stackoverflow.com/questions/218060/random-gaussian-variables
    public static int calcRandNum(double mean, double stdDev)
    {
        double u1;
        double u2;
        double rngStdNormal;
        double rngNormal;

        // Forces the system to produce a positive number (yes this is a terrible way to do this, however I am a terrible programmer - CHECKMATE)
        do
        {
            u1 = 1.0 - rng.NextDouble(); //uniform(0,1] random doubles
            u2 = 1.0 - rng.NextDouble();
            rngStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            rngNormal = mean + stdDev * rngStdNormal; //random normal(mean,stdDev^2)
        } while (rngNormal < 0);

        return (int)rngNormal;
    }
}
