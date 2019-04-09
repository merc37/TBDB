using UnityEngine;

public class Timer
{
    private float time;
    private float timer;

    public Timer(float time)
    {
        this.time = time;
        timer = time;
    }

    public bool Tick()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Reset();
            return true;
        }
        return false;
    }

    public float GetTime()
    {
        return time;
    }

    public float GetCurrentTime()
    {
        return timer;
    }


    public float GetTimerPercentage()
    {
        return timer / time;
    }

    public void Reset()
    {
        timer = time;
    }
}
