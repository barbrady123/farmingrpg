using UnityEngine;

public class NPCScheduleEvent
{
    public int Hour;

    public int Minute;

    public int Priority;

    public int Day;

    public Weather Weather;

    public Season Season;

    public SceneName ToSceneName;

    public GridCoordinate ToGridCoordinate;

    public Direction NPCFacingDirectionAtDestination = Direction.None;

    public AnimationClip AnimationAtDestination;

    public int Time => (this.Hour * 100) + this.Minute;

    public NPCScheduleEvent() { }

    public NPCScheduleEvent(
        int hour,
        int minute,
        int priority,
        int day,
        Weather weather,
        Season season,
        SceneName toSceneName,
        GridCoordinate toGridCoordinate,
        AnimationClip animationAtDestination)
    {
        this.Hour = hour;
        this.Minute = minute;
        this.Priority = priority;
        this.Day = day;
        this.Weather = weather;
        this.Season = season;
        this.ToSceneName = toSceneName;
        this.ToGridCoordinate = toGridCoordinate;
        this.AnimationAtDestination = animationAtDestination;
    }

    public override string ToString()
    {
        return $"Time: {this.Time}, Priority: {this.Priority}, Day: {this.Day}, Weather: {this.Weather}, Season: {this.Season}";
    }
}
