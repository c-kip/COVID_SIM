using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents the virus
public class Virus
{
    //public const int MAX_SPREAD = 100;
    //Virus.Stages to represent different levels of infection
    public enum Stages
    {
        Asymptotic,
        Mild,
        Severe,
        Deadly
    };
    public static Dictionary<Stages, double> infSpreadRates = new Dictionary<Stages, double>() {
        {Stages.Asymptotic, 0.5},
        {Stages.Mild, 0.1},
        {Stages.Severe, 0.05},
        {Stages.Deadly, 0.01}
    };
    public static Dictionary<Stages, double[]> infStageIncRates = new Dictionary<Stages, double[]>() {
        {Stages.Asymptotic, new double[]{0.1, 0}},
        {Stages.Mild, new double[]{0.01, 0}},
        {Stages.Severe, new double[]{0.1, 0}},
        {Stages.Deadly, new double[]{0, 0}}
    };
    public static Dictionary<Stages, double[]> infStageDecRates = new Dictionary<Stages, double[]>() {
        {Stages.Asymptotic, new double[]{0.1, 0}},
        {Stages.Mild, new double[]{0.1, 0}},
        {Stages.Severe, new double[]{0.05, 0}},
        {Stages.Deadly, new double[]{0, 0}}
    };
    public static Dictionary<Stages, double> infDetectRates = new Dictionary<Stages, double>() {
        {Stages.Asymptotic, 0.01},
        {Stages.Mild, 0.1},
        {Stages.Severe, 0.5},
        {Stages.Deadly, 0.95}
    };
}
