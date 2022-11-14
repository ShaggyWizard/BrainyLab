using System;


public interface IScore
{
    public event Action<int> OnScoreChange;


    public int Score { get; }


    public void AddScore(int amount);
}