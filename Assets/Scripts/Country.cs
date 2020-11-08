using System;
using System.Collections.Generic;

public class Country
{
    //Class variables
    private string countryName;
    private Population people;
    private LinkedList<TravelRoute> transportRoutes;
    private double healthRating; //1 to 100 (higher is better)
    private double hospitalRating; //1 to 100 (higher is better)
    private double invHealthRating;
    private double invHospitalRating;

    //Stats to use for spread, stages transfer, and detection rates
    public Dictionary<Virus.Stages, double> infSpreadRatesLocal = new Dictionary<Virus.Stages, double>();
    public Dictionary<Virus.Stages, double[]> infStageIncRatesLocal = new Dictionary<Virus.Stages, double[]>();
    public Dictionary<Virus.Stages, double[]> infStageDecRatesLocal = new Dictionary<Virus.Stages, double[]>();
    public Dictionary<Virus.Stages, double> infDetectRatesLocal = new Dictionary<Virus.Stages, double>();

    public Country(string countryName, Population people, double healthRating, double hospitalRating)
    {
        this.countryName = countryName;
        this.people = people;
        this.healthRating = healthRating;
        this.hospitalRating = hospitalRating;
        this.invHealthRating = Math.Pow(healthRating, -1);
        this.invHospitalRating = Math.Pow(hospitalRating, -1);
        transportRoutes = new LinkedList<TravelRoute>();

        foreach (Virus.Stages stage in Enum.GetValues(typeof(Virus.Stages)))
        {
            //If the stage is severe or deadly, and the virus is detected then use hospital ratings
            if (stage == Virus.Stages.Severe || stage == Virus.Stages.Deadly)
            {
                infSpreadRatesLocal.Add(stage, Virus.infSpreadRates[stage] * invHealthRating);
                infStageIncRatesLocal.Add(stage, new double[] { Virus.infStageIncRates[stage][0] * invHealthRating, Virus.infStageIncRates[stage][1] * invHospitalRating });
                infStageDecRatesLocal.Add(stage, new double[] { Virus.infStageDecRates[stage][0] * invHealthRating, Virus.infStageDecRates[stage][1] * invHospitalRating });
                infDetectRatesLocal.Add(stage, Virus.infDetectRates[stage] * invHealthRating);
            } else
            { //Otherwise, use the standard health rating of the country/continent
                infSpreadRatesLocal.Add(stage, Virus.infSpreadRates[stage] * invHealthRating);
                infStageIncRatesLocal.Add(stage, new double[] { Virus.infStageIncRates[stage][0] * invHealthRating, Virus.infStageIncRates[stage][1] * invHealthRating });
                infStageDecRatesLocal.Add(stage, new double[] { Virus.infStageDecRates[stage][0] * invHealthRating, Virus.infStageDecRates[stage][1] * invHealthRating });
                infDetectRatesLocal.Add(stage, Virus.infDetectRates[stage] * invHealthRating);
            }
        }
    }

    public void setPopParent()
    {
        this.people.setParent(this);
    }

    //Transport random numbers of gaussian distributed passengers between countries
    public void transportInf()
    {
        int travellers;
        foreach (TravelRoute route in transportRoutes)
        {
            Population otherPeople;
            if (route.getFirst().getName() != this.countryName)
            {
                otherPeople = route.getFirst().getPeople();
            } else
            {
                otherPeople = route.getSecond().getPeople();
            }
            for (int i = 0; i <= 1; i++)
            {
                //Asymptomatic
                travellers = Main.calcRandNum(route.getTravelProb() * people.getAsymptotic(i), 1, 0, people.getAsymptotic(i));
                people.rmInf(travellers, Virus.Stages.Asymptotic, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Asymptotic, i);

                //Mild
                travellers = Main.calcRandNum(route.getTravelProb() * people.getMild(i), 1, 0, people.getMild(i));
                people.rmInf(travellers, Virus.Stages.Mild, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Mild, i);

                //Severe
                travellers = Main.calcRandNum(route.getTravelProb() * people.getSevere(i), 1, 0, people.getSevere(i));
                people.rmInf(travellers, Virus.Stages.Severe, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Severe, i);

                //Deadly
                travellers = Main.calcRandNum(route.getTravelProb() * people.getDeadly(i), 1, 0, people.getDeadly(i));
                people.rmInf(travellers, Virus.Stages.Deadly, 2, i);
                otherPeople.addInf(travellers, Virus.Stages.Deadly, i);
            }
        }
    }

    //Add a transport route to this country
    public void addTransportRoute(Country other, double travelProb)
    {
        transportRoutes.AddLast(new TravelRoute(this, other, travelProb));
    }

    public LinkedList<TravelRoute> getTransportRoutes()
    {
        return transportRoutes;
    }

    public Population getPeople()
    {
        return people;
    }

    public string getName()
    {
        return countryName;
    }

    public double getInvHealthRating()
    {
        return invHealthRating;
    }

    public bool Equals(Country other)
    {
        if (other == null)
        {
            return false;
        }
        if (this.countryName == other.countryName)
        {
            return true;
        }
        return false;
    }

    
    public override string ToString()
    {
        return this.getName() + "\n" +
                "Health Rating: " + healthRating + "\n" +
                "Hospital Rating: " + hospitalRating + "\n" +
                "Population: " + people.getTotal() + "\n" +
                "Deceased: " + people.getDeceased() + "\n" +
                "Healthy: " + people.getHealthy() + "\n" +
                "Immune: " + people.getImmune() + "\n" +
                "Infected: " + people.getInfected() + "\n" +
                "Asymptotic: " + people.getAsymptotic(2) + " - Undetected:" + people.getAsymptotic(0) + " - Detected:" + people.getAsymptotic(1) + "\n" +
                "Mild: " + people.getMild(2) + " - Undetected:" + people.getMild(0) + " - Detected:" + people.getMild(1) + "\n" +
                "Severe: " + people.getSevere(2) + " - Undetected:" + people.getSevere(0) + " - Detected:" + people.getSevere(1) + "\n" +
                "Deadly: " + people.getDeadly(2) + " - Undetected:" + people.getDeadly(0) + " - Detected:" + people.getDeadly(1);
    }
}
