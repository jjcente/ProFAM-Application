using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FishQuestionDatabase", menuName = "Questions/FishQuestionDatabase")]
public class FishQuestionDatabase : ScriptableObject
{
    [Header("Loaded Questions")]
    public FishQuestion[] questions;

    private List<int> usedQuestionIndexes = new List<int>();
    private HashSet<int> answeredQuestionIndexes = new HashSet<int>();
    private HashSet<int> assignedQuestionIndexes = new HashSet<int>();


    // Call this in FishQuestionManager.Start()
   
    public void LoadQuestions()
    {
        questions = new FishQuestion[]
        {
            new FishQuestion
{
    question = "A factory produces light bulbs. In a random sample of 200 bulbs, 6 are found to be defective. Based on this sample, how many defective bulbs would be expected in a production run of 5,000 bulbs?",
    multipleAnswer = new string[] { "120", "150", "200", "250" },
    correctIndex = 1,
    solution = "Defective rate = 6/200 = 0.03; Expected defectives = 5,000 * 0.03 = 150"
},
new FishQuestion
{
    question = "A hot air balloon is released from the ground and begins to rise at a constant rate. After 3 minutes, it is at an altitude of 180 meters. If the balloon maintains this constant rate, how many more minutes will it take to reach an altitude of 480 meters?",
    multipleAnswer = new string[] { "5 minutes", "6 minutes", "7 minutes", "8 minutes" },
    correctIndex = 0,
    solution = "Rate = Distance/Time; Rate: 180/3=60 m/min; Remaining distance = 480m - 180m = 300; Time = RD/Rate = 300m/60m per min = 5 min"
},
new FishQuestion
{
    question = "    ) of a small rocket in meters after t seconds is modeled by the function H = 20t - 2t². What is the height of the rocket after 5 seconds?",
    multipleAnswer = new string[] { "40 m", "50 m", "60 m", "75 m" },
    correctIndex = 1,
    solution = "Substitute t = 5 into the function. H = 20(5) - 2(5^2) = 100 - 2(25) = 100-50 = 50 meters."
},
new FishQuestion
{
    question = "A bread recipe calls for 300 grams of flour to make a loaf that weighs 450 grams. A baker uses 500 grams of flour for a larger loaf, maintaining the same ingredient proportions. What is the weight of the larger loaf?",
    multipleAnswer = new string[] { "650 g", "700 g", "750 g", "800 g" },
    correctIndex = 2,
    solution = "Flour:Loaf ratio = 300:450 = 2:3; 500 g flour → 2/3*W = 500 → W = 750 g"
},
new FishQuestion
{
    question = "A taxi company charges a flat fee of ₱50 plus ₱15 for every kilometer traveled. What function (C) represents the total cost of a ride for a distance (d) in kilometers, and what is the cost for a 12 km trip?",
    multipleAnswer = new string[] { "C=50d+15; Cost: ₱615", "C=15d+50; Cost: ₱230", "C=65d; Cost: ₱780", "C=15(d+50); Cost: ₱930" },
    correctIndex = 1,
    solution = "C = (cost per km x distance) + flat free  C = 15d + 50; C = 15d + 50  C = 15(12) + 50  C = 180 + 50  C = 230"
},
new FishQuestion
{
    question = "A fully charged phone battery has 100% capacity. After 4 hours of continuous use, the remaining capacity is 60%. If the battery continues to drain at the same constant rate, how many more hours will the battery last until it is completely dead (0% capacity)?",
    multipleAnswer = new string[] { "4 hours", "6 hours", "8 hours", "10 hours" },
    correctIndex = 1,
    solution = "Battery loss rate: (100% - 60%)/4 hrs = 40%/4 hrs = 10% per hour; Time to drain remaining 60%: 60%/10% per hour = 6 hrs"
},
new FishQuestion
{
    question = "The formula to convert temperature from Celsius (C) to Fahrenheit (F) is F = 1.8C + 32. If the temperature is 25℃, what is the temperature in Fahrenheit?",
    multipleAnswer = new string[] { "73℉", "77℉", "80℉", "85℉" },
    correctIndex = 1,
    solution = "Substitute C = 25 into the formula. F = 1.8(25)+32 = 45 + 32 = 77℉"
},
new FishQuestion
{
    question = "The time (T) it takes to fill a pool is inversely proportional to the flow rate (R) of the water pump. A pump with a flow rate of 50 liters per minute fills the pool in 15 hours. How many hours will it take to fill the same pool if the flow rate is increased to 75 liters per minute?",
    multipleAnswer = new string[] { "8 hours", "10 hours", "12 hours", "22.5 hours" },
    correctIndex = 1,
    solution = "Inverse proportionality means T x R is constant (k): k = 15 hours x 50 L/min = 750; New Time: T = 750/75 L/min. = 10 hrs"
},
new FishQuestion
{
    question = "Company X rents a car for ₱800 per day with unlimited mileage. Company Y rents the same car for ₱500 per day plus ₱3 per kilometer traveled. If a person rents the car for one day, at what mileage (in kilometers) will the cost of Company X and Company Y be equal?",
    multipleAnswer = new string[] { "100 km", "120 km", "150 km", "180 km" },
    correctIndex = 0,
    solution = "Set the costs equal: 800 = 500 + 3k  300 = 3k  k = 100 km"
},
new FishQuestion
{
    question = "A scientist is observing a bacteria culture that doubles in population every hour. If the culture starts with 500 bacteria, what will the population be after 4 hours?",
    multipleAnswer = new string[] { "2,000", "4,000", "6,000", "8,000" },
    correctIndex = 3,
    solution = " Population (P) = 500 x 2^t, substitute (P) = 500 x 2^4 = 500 x 16 = 8,000"
},
new FishQuestion
{
    question = "A cell phone company models the percentage of signal strength (P) based on the distance in kilometers (d) from the nearest tower using the formula: P=100-5d^2. At what distance (d) from the tower will the signal strength drop to 55%?",
    multipleAnswer = new string[] { "2 km", "3 km", "4 km", "5 km" },
    correctIndex = 1,
    solution = "Set P = 55 and solve for d: 55 = 100 - 5(d^2) 5(d^2) = 100 - 55  5(d^2) = 45  (d^2) = 9  d = 3 km"
},
new FishQuestion
{
    question = "A fitness tracker estimates calories burned (C) based on minutes of running (M) using the model: C=15M+50 (where 50 is the baseline). How many minutes must a person run to burn a total of 500 calories?",
    multipleAnswer = new string[] { "25 minutes", "30 minutes", "35 minutes", "40 minutes" },
    correctIndex = 1,
    solution = "Set C = 500 and solve for M: 500 = 15M + 50  450 = 15M  M = 450/15 = 30 min"
},
new FishQuestion
{
    question = "An investment grows according to the formula A=P(1+r)^t, where A is the final amount, P is the initial principal, r is the annual rate, and t is the time in years. If P=₱50,000 and r=0.04, how much money is gained after 2 years?",
    multipleAnswer = new string[] { "₱4,080", "₱4,000", "₱54,000", "₱54,080" },
    correctIndex = 0,
    solution = "Final Amount: A = 50,000(1 + 0.04)^2 = 50,000(1.04)^2 = 50,000(1.0816) = ₱54,080; Money Gained: ₱54,080 - ₱50,000 = ₱4,080"
},
new FishQuestion
{
    question = "The strength of a satellite signal (S) is inversely proportional to the square of the distance (d) from the transmitter. This relationship is given by the formula S=k/d^2. If the signal strength is 80 units at a distance of 5 km, what will the signal strength be at a distance of 10 km?",
    multipleAnswer = new string[] { "10 units", "20 units", "40 units", "50 units" },
    correctIndex = 1,
    solution = "Find k: 80 = k/5^2 -> 80 = k/25 -> k = 2,000; Find S at 10 km: S = 2,000/10^2 = 2,000/100 = 20 units"
},
new FishQuestion
{
    question = "A large water tank is being filled at a constant rate of 15 liters per minute. The tank currently contains 300 liters and has a total capacity of 600 liters. How much more time (in minutes) will it take to fill the tank?",
    multipleAnswer = new string[] { "15 minutes", "20 minutes", "30 minutes", "40 minutes" },
    correctIndex = 1,
    solution = "Remaining Volume: 600 L - 300 L = 300 L; Time Required: Time = Volume/Rate -> 300 L/15 L per min = 20 min"
},
new FishQuestion
{
    question = "A family is choosing between two smartphone plans: Plan X: ₱1,500 per month, with 100 minutes of free calls. Extra minutes cost ₱5 each. Plan Y: ₱1,800 per month, with 200 minutes of free calls. Extra minutes cost ₱4 each. If the family uses exactly 200 minutes of calls in a month, what is the cost difference between Plan X and Plan Y?",
    multipleAnswer = new string[] { "₱100", "₱200", "₱300", "₱400" },
    correctIndex = 1,
    solution = "Plan Y Cost (at 200 min): ₱1,800 (since 200 minutes is free); Plan X Cost (at 200 min): ₱1,500 + (100 extra minutes x ₱5/min) = ₱1,500 + ₱500 = ₱2,000; Difference:  ₱2,000 − ₱1,800 = ₱200"
}

        };
    }


    public FishQuestion GetRandomQuestion()
    {
        if (questions == null || questions.Length == 0)
            return null;

        List<int> available = new List<int>();
        for (int i = 0; i < questions.Length; i++)
        {
            if (!answeredQuestionIndexes.Contains(i) && !assignedQuestionIndexes.Contains(i))
                available.Add(i);
        }

        if (available.Count == 0)
            return null; // no question available

        int randomIndex = UnityEngine.Random.Range(0, available.Count);
        int questionIndex = available[randomIndex];

        assignedQuestionIndexes.Add(questionIndex); // mark as currently assigned
        return questions[questionIndex];
    }

public bool AllQuestionsUsed()
{
    return answeredQuestionIndexes.Count >= (questions != null ? questions.Length : 0);
}
    

 public void MarkQuestionAsAnswered(FishQuestion q)
    {
        int index = Array.IndexOf(questions, q);
        if (index >= 0)
        {
            answeredQuestionIndexes.Add(index);
            assignedQuestionIndexes.Remove(index); // remove from live-assigned set
            Debug.Log($"Marked question {index} as answered: {q.question}");
        }
    }


   public void ResetQuestions()
{
    usedQuestionIndexes.Clear();
    answeredQuestionIndexes.Clear();
    assignedQuestionIndexes.Clear(); // <-- add this
    Debug.Log("✅ Questions have been fully reset.");
}
    public int QuestionsRemaining()
{
    if (questions == null) return 0;
    return questions.Length - answeredQuestionIndexes.Count - assignedQuestionIndexes.Count;
}
}