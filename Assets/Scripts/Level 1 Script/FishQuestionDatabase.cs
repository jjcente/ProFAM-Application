using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FishQuestionDatabase", menuName = "Questions/FishQuestionDatabase")]
public class FishQuestionDatabase : ScriptableObject
{
    [Header("Loaded Questions")]
    public FishQuestion[] questions;

    private List<int> usedQuestionIndexes = new List<int>();

    // Call this in FishQuestionManager.Start()
   
    public void LoadQuestions()
    {
        questions = new FishQuestion[]
        {
            new FishQuestion
            {
                question = "A taxi company charges a flat fee of ₱50 plus ₱15 for every kilometer traveled. What function (C) represents the total cost of a ride for a distance (d) in kilometers, and what is the cost for a 12 km trip?",
                multipleAnswer = new string[] { "C=50d+15; Cost: ₱615", "C=15d+50; Cost: ₱230", "C=65d; Cost: ₱780", "C=15(d+50); Cost: ₱930" },
                correctIndex = 1,
                solution = "C=15d+50; 12 km trip: 15*12+50=₱230"
            },
            new FishQuestion
            {
                question = "A hot air balloon is released from the ground and begins to rise at a constant rate. After 3 minutes, it is at an altitude of 180 meters. If the balloon maintains this constant rate, how many more minutes will it take to reach an altitude of 480 meters?",
                multipleAnswer = new string[] { "5 minutes", "6 minutes", "7 minutes", "8 minutes" },
                correctIndex = 0,
                solution = "Rate: 180/3=60 m/min. Total time: 480/60=8 min. Remaining: 8-3=5 min"
            },
            new FishQuestion
            {
                question = "Company X rents a car for ₱800 per day with unlimited mileage. Company Y rents the same car for ₱500 per day plus ₱3 per kilometer traveled. If a person rents the car for one day, at what mileage (in kilometers) will the cost of Company X and Company Y be equal?",
                multipleAnswer = new string[] { "100 km", "120 km", "150 km", "180 km" },
                correctIndex = 0,
                solution = "800=500+3k ⇒ k=100 km"
            },
            new FishQuestion
            {
                question = "The strength of a satellite signal (S) is inversely proportional to the square of the distance (d) from the transmitter. This relationship is given by the formula S=k/d^2. If the signal strength is 80 units at a distance of 5 km, what will the signal strength be at a distance of 10 km?",
                multipleAnswer = new string[] { "10 units", "20 units", "40 units", "50 units" },
                correctIndex = 1,
                solution = "k=80*5^2=2000; S=2000/10^2=20 units"
            },
            new FishQuestion
            {
                question = "A fully charged phone battery has 100% capacity. After 4 hours of continuous use, the remaining capacity is 60%. If the battery continues to drain at the same constant rate, how many more hours will the battery last until it is completely dead (0% capacity)?",
                multipleAnswer = new string[] { "4 hours", "6 hours", "8 hours", "10 hours" },
                correctIndex = 1,
                solution = "Loss rate: 40%/4h=10%/h. Remaining 60% / 10%=6 hours"
            },
            new FishQuestion
            {
                question = "A cell phone company models the percentage of signal strength (P) based on the distance in kilometers (d) from the nearest tower using the formula: P=100-5d^2. At what distance (d) from the tower will the signal strength drop to 55%?",
                multipleAnswer = new string[] { "2 km", "3 km", "4 km", "5 km" },
                correctIndex = 1,
                solution = "55=100-5d^2 ⇒ 5d^2=45 ⇒ d=3 km"
            },
            new FishQuestion
            {
                question = "A family is choosing between two smartphone plans: Plan X: ₱1,500 per month, with 100 minutes of free calls. Extra minutes cost ₱5 each. Plan Y: ₱1,800 per month, with 200 minutes of free calls. Extra minutes cost ₱4 each. If the family uses exactly 200 minutes of calls in a month, what is the cost difference between Plan X and Plan Y?",
                multipleAnswer = new string[] { "₱100", "₱200", "₱300", "₱400" },
                correctIndex = 1,
                solution = "Plan X: 1500+100*5=2000; Plan Y: 1800; Difference=200"
            },
            new FishQuestion
            {
                question = "The time (T) it takes to fill a pool is inversely proportional to the flow rate (R) of the water pump. A pump with a flow rate of 50 liters per minute fills the pool in 15 hours. How many hours will it take to fill the same pool if the flow rate is increased to 75 liters per minute?",
                multipleAnswer = new string[] { "8 hours", "10 hours", "12 hours", "22.5 hours" },
                correctIndex = 1,
                solution = "T*R=k=15*50=750; New T=750/75=10 hours"
            },
            new FishQuestion
            {
                question = "A fitness tracker estimates calories burned (C) based on minutes of running (M) using the model: C=15M+50 (where 50 is the baseline). How many minutes must a person run to burn a total of 500 calories?",
                multipleAnswer = new string[] { "25 minutes", "30 minutes", "35 minutes", "40 minutes" },
                correctIndex = 1,
                solution = "500=15M+50 ⇒ M=30 minutes"
            },
            new FishQuestion
            {
                question = "An investment grows according to the formula A=P(1+r)^t, where A is the final amount, P is the initial principal, r is the annual rate, and t is the time in years. If P=₱50,000 and r=0.04, how much money is gained after 2 years?",
                multipleAnswer = new string[] { "₱4,080", "₱4,000", "₱54,000", "₱54,080" },
                correctIndex = 0,
                solution = "Final A=50,000*(1.04)^2=54,080; Gain=4,080"
            },
        };
    }

    public FishQuestion GetRandomQuestion()
    {
        if (questions == null || questions.Length == 0)
            return null;

        if (usedQuestionIndexes.Count >= questions.Length)
            usedQuestionIndexes.Clear(); // reset after all used

        int idx;
        do
        {
            idx = UnityEngine.Random.Range(0, questions.Length);
        } while (usedQuestionIndexes.Contains(idx));

        usedQuestionIndexes.Add(idx);
        return questions[idx];
    }

    public bool AllQuestionsUsed()
{
    return usedQuestionIndexes.Count >= questions.Length;
}

public void ResetQuestions()
{
    usedQuestionIndexes.Clear();
}
}