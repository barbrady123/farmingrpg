using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehavior<TimeManager>
{
    private int _gameYear = 1;

    private Season _gameSeason = Season.Spring;

    private int _gameDay = 1;

    private int _gameHour = 6;

    private int _gameMinute = 30;

    private int _gameSecond = 0;

    private string _gameDayOfWeek = "Mon";

    private bool _gameClockPaused = false;

    private float _gameTick = 0f;

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);
    }

    private void Update()
    {
        if (!_gameClockPaused)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        _gameTick += Time.deltaTime;

        if (_gameTick >= Settings.SecondsPerGameSecond)
        {
            _gameTick -= Time.deltaTime;
            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        _gameSecond++;

        if (_gameSecond > 59)
        {
            _gameSecond = 0;
            _gameMinute++;

            if (_gameMinute > 59)
            {
                _gameMinute = 0;
                _gameHour++;

                if (_gameHour > 23)
                {
                    _gameHour = 0;
                    _gameDay++;

                    if (_gameDay > 30)
                    {
                        _gameDay = 1;

                        int season = (int)_gameSeason;
                        season++;
                        _gameSeason = (Season)season;

                        if (season > 3)
                        {
                            season = 0;
                            _gameSeason = (Season)season;
                            _gameYear++;
                            EventHandler.CallAdvanceGameYearEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);
                    }

                    // This should occur prior to this, otherwise this string is off for the year/season events :(
                    _gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);

            Debug.Log($"Year: {_gameYear} Season: {_gameSeason} Day: {_gameDay} Hour: {_gameHour} Minute: {_gameMinute}");
        }

        // Call to advance game second event would go here if required
    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)_gameSeason) * 30) + _gameDay;
        int dayOfWeek = totalDays % 7;

        return (dayOfWeek) switch
        {
            0 => "Sun",
            1 => "Mon",
            2 => "Tue",
            3 => "Wed",
            4 => "Thu",
            5 => "Fri",
            6 => "Sat",
            _ => ""
        };
    }
}
