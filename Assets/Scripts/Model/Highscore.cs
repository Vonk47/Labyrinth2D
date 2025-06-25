using System;


[Serializable]
public class Highscore
{
    public float Time = float.MaxValue;

    public Highscore()
    {
        Time = float.MaxValue;
    }

    public Highscore(float time)
    {
        Time = time;
    }
}
