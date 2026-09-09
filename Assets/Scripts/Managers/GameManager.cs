using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int PlayerScore { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void IncreaseScore()
    {
        PlayerScore++;
    }
}