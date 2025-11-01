using UnityEngine;
using System;
using System.Collections.Generic;

public class QuestionDatabase : MonoBehaviour
{
    public static QuestionDatabase Instance { get; private set; }
    public Question[] questions;

    private List<int> usedQuestionIndexes = new List<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadQuestions();
    }

    void LoadQuestions()
    {
        TextAsset json = Resources.Load<TextAsset>("questions"); // load questions.json from Resources
        if (json == null)
        {
            Debug.LogError("Failed to load questions.json");
            return;
        }

        questions = JsonUtility.FromJson<Wrapper>("{\"questions\":" + json.text + "}").questions;
        Debug.Log("Loaded " + questions.Length + " questions");
    }

    [Serializable]
    private class Wrapper
    {
        public Question[] questions;
    }

    public Question GetRandomQuestion()
    {
        if (questions == null || questions.Length == 0) return null;

        // If all questions were used, return null
        if (usedQuestionIndexes.Count >= questions.Length) return null;

        int idx;
        do
        {
            idx = UnityEngine.Random.Range(0, questions.Length);
        } while (usedQuestionIndexes.Contains(idx));

        usedQuestionIndexes.Add(idx);
        return questions[idx];
    }

    // Call this to reset questions for a new round
    public void ResetQuestions()
    {
        usedQuestionIndexes.Clear();
    }

    public bool AllQuestionsUsed()
{
    return usedQuestionIndexes.Count >= questions.Length;
}
}
