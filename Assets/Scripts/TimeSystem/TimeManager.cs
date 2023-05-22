using System;
using UnityEngine;

public class TimeManager : SingletonMonobehavior<TimeManager>, ISaveable
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

    private string _iSaveableUniqueID;

    private GameObjectSave _gameObjectSave;

    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);
    }

    protected override void Awake()
    {
        base.Awake();

        _iSaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        _gameObjectSave = new GameObjectSave();
    }

    private void Update()
    {
        if (!_gameClockPaused)
        {
            GameTick();
        }
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
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

                            if (_gameYear > 9999)
                            {
                                _gameYear = 1;
                            }

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

    public void TestAdvanceGameMinute()
    {
        for (int x = 0; x < 60; x++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int x = 0; x < 1440; x++)
        {
            TestAdvanceGameMinute();
        }
    }

    private void BeforeSceneUnloadFadeOut()
    {
        _gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn()
    {
        _gameClockPaused = false;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Remove(this);
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since the time manager is on the persistent scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since the time manager is on the persistent scene
    }

    public GameObjectSave ISaveableSave()
    {
        _gameObjectSave.SceneData.Remove(Global.Scenes.Persistent);

        var sceneSave = new SceneSave();

        sceneSave.IntDictionary.Add(Global.Saves.GameYear, _gameYear);
        sceneSave.IntDictionary.Add(Global.Saves.GameDay, _gameDay);
        sceneSave.IntDictionary.Add(Global.Saves.GameHour, _gameHour);
        sceneSave.IntDictionary.Add(Global.Saves.GameMinute, _gameMinute);
        sceneSave.IntDictionary.Add(Global.Saves.GameSecond, _gameSecond);

        sceneSave.StringDictionary.Add(Global.Saves.GameDayOfWeek, _gameDayOfWeek);
        sceneSave.StringDictionary.Add(Global.Saves.GameSeason, _gameSeason.ToString());

        _gameObjectSave.SceneData.Add(Global.Scenes.Persistent, sceneSave);

        return _gameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (!gameSave.GameObjectData.TryGetValue(_iSaveableUniqueID, out var gameObjectSave))
            return;

        _gameObjectSave = gameObjectSave;

        if (!gameObjectSave.SceneData.TryGetValue(Global.Scenes.Persistent, out var sceneSave))
            return;

        _gameYear = sceneSave.IntDictionary[Global.Saves.GameYear];
        _gameDay = sceneSave.IntDictionary[Global.Saves.GameDay];
        _gameHour = sceneSave.IntDictionary[Global.Saves.GameHour];
        _gameMinute = sceneSave.IntDictionary[Global.Saves.GameMinute];
        _gameSecond = sceneSave.IntDictionary[Global.Saves.GameSecond];

        _gameDayOfWeek = sceneSave.StringDictionary[Global.Saves.GameDayOfWeek];

        if (Enum.TryParse<Season>(sceneSave.StringDictionary[Global.Saves.GameSeason], out Season season))
        {
            _gameSeason = season;
        }

        _gameTick = 0;

        EventHandler.CallAdvanceGameMinuteEvent(_gameYear, _gameSeason, _gameDay, _gameDayOfWeek, _gameHour, _gameMinute, _gameSecond);
    }
}
