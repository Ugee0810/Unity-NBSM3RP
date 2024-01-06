[System.Serializable]
public class TimeRange
{
    public double startTime;
    public double endTime;

    public TimeRange(double start, double end)
    {
        startTime = start;
        endTime = end;
    }
}
