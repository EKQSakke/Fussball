internal class GameScore
{
    public int Team0Score { get; set; }
    public int Team1Score { get; set; }
    public int ScoreTarget { get; set; }

    public GameScore(int scoreTarget)
    {
        ScoreTarget = scoreTarget;
    }

    public void GoalScored(int goalId)
    {
        if (goalId == 0)
        {
            Team0Score++;
        }
        else
        {
            Team1Score++;
        }
    }

    public void Reset()
    {
        Team0Score = 0;
        Team1Score = 0;
    }

    public override string ToString()
    {
        return $"{Team0Score} - {Team1Score}";
    }

    public bool IsGameOver()
    {
        return Team0Score >= ScoreTarget || Team1Score >= ScoreTarget;
    }

    public int GetWinner()
    {
        if (Team0Score > Team1Score)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}