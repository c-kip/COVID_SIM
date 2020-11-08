using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents the virus
public class Virus
{
    //public const int MAX_SPREAD = 100;
    private static double infRate = 0.01;
    private static double asymptRate = 0.22;
    private static double mildRate = 0.624; //0.8*0.78
    private static double severeRate = 0.1092; //0.14*0.78
    private static double deadlyRate = 0.039; //0.05*0.78
    private static int avgDailyContact = 1000; //assume direct or indirect contact with 10 people per day

    //Virus.Stages to represent different levels of infection
    public enum Stages
    {
        Asymptotic,
        Mild,
        Severe,
        Deadly
    };
    public static Dictionary<Stages, double> infSpreadRates = new Dictionary<Stages, double>() {
        {Stages.Asymptotic, infRate * asymptRate * avgDailyContact},
        {Stages.Mild, infRate * mildRate * avgDailyContact},
        {Stages.Severe, infRate * severeRate * avgDailyContact},
        {Stages.Deadly, infRate * deadlyRate * avgDailyContact}
    };
    //Need data
    public static Dictionary<Stages, double[]> infStageIncRates = new Dictionary<Stages, double[]>() {
        {Stages.Asymptotic, new double[]{0.75, 0.75}}, //0.75 confirmed, but others need data
        {Stages.Mild, new double[]{0.1, 0.1}},
        {Stages.Severe, new double[]{0.1, 0.1}},
        {Stages.Deadly, new double[]{0.004, 0.004}} //4% of total cases, should be adjusted
    };
    //Need data
    public static Dictionary<Stages, double[]> infStageDecRates = new Dictionary<Stages, double[]>() {
        {Stages.Asymptotic, new double[]{0.04, 0.04}},
        {Stages.Mild, new double[]{0.04, 0.04}},
        {Stages.Severe, new double[]{0.01, 0.01}},
        {Stages.Deadly, new double[]{0.01, 0.01}}
    };
    //Need data
    public static Dictionary<Stages, double> infDetectRates = new Dictionary<Stages, double>() {
        {Stages.Asymptotic, 0.001},
        {Stages.Mild, 0.02},
        {Stages.Severe, 0.11}, //Test avg. success rate appears to be ~80% across tests
        {Stages.Deadly, 0.14} //These values assume about a 7 day wait for results
    };
}
