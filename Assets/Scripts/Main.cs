/*
 * Handles the main countries or continents, and updating the simulation
 * (on a day to day basis).
 * 
 * Note: There are a couple current bugs:
 *      1) Asia's population cannot be represented with an int (need a long)
 *         This would require redoing a large section of the code, which I'll (hopefully)
 *         eventually get around to
 *      2) The randomizer is broken. Badly. Right now it just brute forces it (within a
 *         certain number of attempts) for a random value within a range with specific
 *         distribution. I can't figure out the math to do this better, so IDK.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public static System.Random rng;
    private Population canadians;
    private Country Canada;
    private LinkedList<Country> countries;
    private const int MAX_DAYS = 365;
    private int days;
    private const bool DEBUG = false;
    private string statsCountryName = "";

    //UI elements
    public GameObject statsBackground;
    public Text countryStats;
    public Button northAmericaButton;
    public Button southAmericaButton;
    public Button europeButton;
    public Button africaButton;
    public Button asiaButton;
    public Button oceaniaButton;

    //Debugging tools
    public Text northAmerica;
    public Text southAmerica;
    public Text africa;
    public Text europe;
    public Text asia;
    public Text oceania;
    public Text dayCounter;

    // Start is called before the first frame update
    void Start()
    {
        //Create the randomizer
        rng = new System.Random();

        //Create the countries (or continents) that we'll use
        countries = new LinkedList<Country>();
        countries.AddLast(new Country("North America", new Population(368869647, 1, 0, 0, 0), 10));
        countries.AddLast(new Country("South America", new Population(431969015, 0, 0, 0, 0), 10));
        countries.AddLast(new Country("Europe", new Population(747793556, 0, 0, 0, 0), 10));
        countries.AddLast(new Country("Africa", new Population(1347333004, 0, 0, 0, 0), 10));
        countries.AddLast(new Country("Asia", new Population(1654850282/*4654850282*/, 0, 0, 0, 0), 10));
        countries.AddLast(new Country("Oceania", new Population(38820000, 0, 0, 0, 0), 10));

        //Add transportation routes
        countries.ElementAt(0).addTransportRoute(countries.ElementAt(1), 0.5);
        countries.ElementAt(1).addTransportRoute(countries.ElementAt(0), 0.5);

        //Link the buttons
        northAmericaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(0)); });
        southAmericaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(1)); });
        europeButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(2)); });
        africaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(3)); });
        asiaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(4)); });
        oceaniaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(5)); });

        //UI elements
        statsBackground.SetActive(false);
        countryStats.text = "";

        days = 0;
        StartCoroutine(dailyCycle(days));
    }

    //Create a Coroutine 
    public IEnumerator dailyCycle(int days)
    {
        if (days > MAX_DAYS)
        {
            yield return null;
        }

        //yield on a new YieldInstruction that waits for x seconds.
        yield return new WaitForSeconds(1);

        dayCounter.text = "Day: " + days;

        Population pop;
        String debug;
        //Print out the calculated random number
        foreach (Country place in countries)
        {
            pop = place.getPeople();
            pop.cycle();
            place.transportInf();

            if (DEBUG)
            {
             // Debug information
             debug = place.getName() + "\n" +
                     "Population: " + pop.getTotal() + "\n" +
                     "Healthy: " + pop.getHealthy() + "\n" +
                     "Immune: " + pop.getImmune() + "\n" +
                     "Infected: " + pop.getInfected() + "\n" +
                     "Asymptotic: " + pop.getAsymptotic(2) + " - Undetected:" + pop.getAsymptotic(0) + " - Detected:" + pop.getAsymptotic(1) + "\n" +
                     "Mild: " + pop.getMild(2) + " - Undetected:" + pop.getMild(0) + " - Detected:" + pop.getMild(1) + "\n" +
                     "Severe: " + pop.getSevere(2) + " - Undetected:" + pop.getSevere(0) + " - Detected:" + pop.getSevere(1) + "\n" +
                     "Deadly: " + pop.getDeadly(2) + " - Undetected:" + pop.getDeadly(0) + " - Detected:" + pop.getDeadly(1);

                //Put the debug info on the screen
                switch (place.getName())
                {
                    case "North America":
                        northAmerica.text = debug;
                        break;
                    case "South America":
                        southAmerica.text = debug;
                        break;
                    case "Europe":
                        europe.text = debug;
                        break;
                    case "Africa":
                        africa.text = debug;
                        break;
                    case "Asia":
                        asia.text = debug;
                        break;
                    case "Oceania":
                        oceania.text = debug;
                        break;
                    default:
                        break;
                }
            }
        }

        //Increment the loop counter and go again
        days++;
        StartCoroutine(dailyCycle(days));
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

    //Sets the display menu for country stats
    public void displayCountryStats(Country dispCountry)
    {
        if (statsCountryName == dispCountry.getName())
        {
            statsBackground.SetActive(false);
            countryStats.text = "";
            statsCountryName = "";
        } else
        {
            Population pop = dispCountry.getPeople();
            statsBackground.SetActive(true);
            countryStats.text = dispCountry.getName() + "\n" +
                                "Population: " + pop.getTotal() + "\n" +
                                "Healthy: " + pop.getHealthy() + "\n" +
                                "Immune: " + pop.getImmune() + "\n" +
                                "Infected: " + pop.getInfected() + "\n" +
                                "Asymptotic: " + pop.getAsymptotic(2) + " - Undetected:" + pop.getAsymptotic(0) + " - Detected:" + pop.getAsymptotic(1) + "\n" +
                                "Mild: " + pop.getMild(2) + " - Undetected:" + pop.getMild(0) + " - Detected:" + pop.getMild(1) + "\n" +
                                "Severe: " + pop.getSevere(2) + " - Undetected:" + pop.getSevere(0) + " - Detected:" + pop.getSevere(1) + "\n" +
                                "Deadly: " + pop.getDeadly(2) + " - Undetected:" + pop.getDeadly(0) + " - Detected:" + pop.getDeadly(1);
            statsCountryName = dispCountry.getName();
        }
    }
}
