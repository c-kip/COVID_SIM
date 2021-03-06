﻿/*
 * Handles the main countries or continents, and updating the simulation
 * (on a day to day basis).
 * 
 * Note: There are a few current bugs/issues:
 *      1) Asia's population cannot be represented with an int (need a long)
 *         This would require redoing a large section of the code, which I'll (hopefully)
 *         eventually get around to
 *      2) The randomizer is broken. Badly. Right now it just brute forces it (within a
 *         certain number of attempts) for a random value within a range with specific
 *         distribution. I can't figure out the math to do this better, so IDK.
 *      3) I need to get proper standard deviations for the randomizer.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public static System.Random rng;
    private Population canadians;
    private Country Canada;
    private static LinkedList<Country> countries;
    private const int MAX_DAYS = 365;
    private const float TIME_DELAY = 0.5f;
    private int days;
    private const bool DEBUG = false;
    private string statsCountryName = "";
    public static bool pause = false;
    private bool lastPause = false;

    //UI elements
    public GameObject statsBackground;
    public GameObject canvas;
    public Text countryStats;
    public Button northAmericaButton;
    public Button southAmericaButton;
    public Button europeButton;
    public Button africaButton;
    public Button asiaButton;
    public Button oceaniaButton;
    public Button pauseButton;

    //Animated elements
    private LinkedList<Plane> planes = new LinkedList<Plane>();
    public Image planePrefab;
    public Image dotPrefab;

    //Locations and rotations
    private static Vector3 canvasPos = new Vector3(960f, 540f, 0);
    private Vector3 NA = new Vector3(-482, 216, 0) + canvasPos;
    private Vector3 SA = new Vector3(-305,-128, 0) + canvasPos;
    private Vector3 EU = new Vector3( 134, 268, 0) + canvasPos;
    private Vector3 AF = new Vector3(  68,  25, 0) + canvasPos;
    private Vector3 AS = new Vector3( 423, 252, 0) + canvasPos;
    private Vector3 OC = new Vector3( 642,-201, 0) + canvasPos;

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

        //Create the infection dots
        Image dot1 = Instantiate(dotPrefab, NA, Quaternion.identity);
        dot1.transform.parent = canvas.transform;
        dot1.transform.SetSiblingIndex(5); //Set the dots in front of the maps but behind everything else
        Image dot2 = Instantiate(dotPrefab, SA, Quaternion.identity);
        dot2.transform.parent = canvas.transform;
        dot2.transform.SetSiblingIndex(5);
        Image dot3 = Instantiate(dotPrefab, EU, Quaternion.identity);
        dot3.transform.parent = canvas.transform;
        dot3.transform.SetSiblingIndex(5);
        Image dot4 = Instantiate(dotPrefab, AF, Quaternion.identity);
        dot4.transform.parent = canvas.transform;
        dot4.transform.SetSiblingIndex(5);
        Image dot5 = Instantiate(dotPrefab, AS, Quaternion.identity);
        dot5.transform.parent = canvas.transform;
        dot5.transform.SetSiblingIndex(5);
        Image dot6 = Instantiate(dotPrefab, OC, Quaternion.identity);
        dot6.transform.parent = canvas.transform;
        dot6.transform.SetSiblingIndex(5);

        //Create the countries (or continents) that we'll use
        countries = new LinkedList<Country>();
        countries.AddLast(new Country("North America", new Population(368869647, 0/*11234657*/, 0, 0, 0, dot1), 40, 50));
        countries.AddLast(new Country("South America", new Population(431969015, 0/*8953760*/, 0, 0, 0, dot2), 40, 50));
        countries.AddLast(new Country("Europe", new Population(747793556, 0/*12334693*/, 0, 0, 0, dot3), 50, 60));
        countries.AddLast(new Country("Africa", new Population(1347333004, 0/*8836579*/, 0, 0, 0, dot4), 30, 30));
        countries.AddLast(new Country("Asia", new Population(1654850282/*4654850282*/, 1/*15463820*/, 0, 0, 0, dot5), 60, 50));
        countries.AddLast(new Country("Oceania", new Population(38820000, 0/*6431068*/, 0, 0, 0, dot6), 70, 70));

        //Link each population to it's country/continent
        foreach (Country place in countries)
        {
            place.setPopParent();
        }

        //Add transportation routes connecting all countries
        for (int i = 0; i < countries.Count; i++)
        {
            for (int j = i + 1; j < countries.Count; j++)
            {
                countries.ElementAt(i).addTransportRoute(countries.ElementAt(j), countries.ElementAt(j).getInvHealthRating());
                countries.ElementAt(j).addTransportRoute(countries.ElementAt(i), countries.ElementAt(i).getInvHealthRating());
            }
        }

        //Create a Plane for each transportation route
        Vector3 start;
        Vector3 end;
        Vector3 vectToTarget;
        float angle;
        Quaternion rot;
        Image planeImage;
        foreach (Country place in countries)
        {
            start = findLocation(place);
            foreach (TravelRoute route in place.getTransportRoutes())
            {
                end = findLocation(route.getSecond());
                vectToTarget = end - start;
                angle = Mathf.Atan2(vectToTarget.y, vectToTarget.x) * Mathf.Rad2Deg + 35; //IDK i just guessed a bit and found 35 for the offset
                rot = Quaternion.AngleAxis(angle, Vector3.forward);
                planeImage = Instantiate(planePrefab, start, rot);
                planeImage.transform.parent = canvas.transform;
                planeImage.transform.SetSiblingIndex(5); //Set the planes in front of the maps but behind everything else
                planes.AddLast(new Plane(start, end, rot, planeImage, route.getTravelProb()*600));
            }
        }

        //Link the buttons
        northAmericaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(0)); });
        southAmericaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(1)); });
        europeButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(2)); });
        africaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(3)); });
        asiaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(4)); });
        oceaniaButton.onClick.AddListener(delegate { displayCountryStats(countries.ElementAt(5)); });
        pauseButton.onClick.AddListener(delegate { pauseSim(); });

        //UI elements
        statsBackground.SetActive(false);
        countryStats.text = "";

        days = 0;
        StartCoroutine(dailyCycle());
    }

    //Called once per frame
    public void Update()
    {
        if (!pause)
        {
            foreach (Plane plane in planes)
            {
                plane.movePlane();
            }
        }

        if (!pause && lastPause)
        {
            lastPause = false;
            StartCoroutine(dailyCycle());
        }
        else if (pause && !lastPause)
        {
            lastPause = true;
        }
    }

    //Create a Coroutine 
    public IEnumerator dailyCycle()
    {
        //yield on a new YieldInstruction that waits for x seconds.
        yield return new WaitForSeconds(TIME_DELAY);

        Population pop;
        String debug;
        //Print out the calculated random number
        foreach (Country place in countries)
        {
            pop = place.getPeople();
            pop.cycle();
            place.transportInf();

            //Update the pop-up if needed
            if (place.getName() == statsCountryName)
            {
                countryStats.text = place.ToString();
            }

            //Debug info
            if (DEBUG)
            {
                // Debug information
                debug = place.ToString();

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

        //Increment the day counter and go again
        days++;
        dayCounter.text = "Day: " + days;
        if (pause)
        {
            dayCounter.text += "\nPaused";
        }
        if (!pause && days < MAX_DAYS)
        {
            StartCoroutine(dailyCycle());
        }
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
        } while (tries < 10 && (rngNormal < lowBound || rngNormal > highBound));

        if (tries >= 10)
        {
            return 0;
        }

        return (int)rngNormal;
    }

    //Finds the Vector3 position of a country/continent
    private Vector3 findLocation(Country place)
    {
        switch(place.getName())
        {
            case "North America":
                return NA;
            case "South America":
                return SA;
            case "Europe":
                return EU;
            case "Africa":
                return AF;
            case "Asia":
                return AS;
            case "Oceania":
                return OC;
            default:
                return NA;
        }
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
            countryStats.text = dispCountry.ToString();
            statsCountryName = dispCountry.getName();
        }
    }

    //Returns a country if a name match is found
    public static Country findCountry(string countryName)
    {
        foreach (Country place in countries)
        {
            Debug.Log(place.getName() + "-" + countryName);
            if (place.getName() == countryName)
            {
                return place;
            }
        }
        return null;
    }

    public void pauseSim()
    {
        pause = !pause;
        if (pause)
        {
            pauseButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = ">";
        } else
        {
            pauseButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "||";
        }
    }
}
