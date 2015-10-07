using GTA;

public class Timer
{
    #region Public Variables
    private bool enabled;
    public bool Enabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    private int interval;
    public int Interval
    {
        get { return interval; }
        set { interval = value; }
    }
    #endregion

    private int waiter;
    public int Waiter
    {
        get { return waiter; }
        set { waiter = value; }
    }

    public Timer(int interval)
    {
        this.interval = interval;
        waiter = 0;
        enabled = false;
    }

    public Timer()
    {
        interval = 0;
        waiter = 0;
        enabled = false;
    }

    public void Start()
    {
        waiter = Game.GameTime + interval;
        enabled = true;
    }

    public void Reset()
    {
        waiter = Game.GameTime + interval;
    }
}
