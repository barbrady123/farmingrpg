using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    #region Instantiate Crop Prefabs Event
    public static event Action InstantiateCropPrefabsEvent;

    public static void CallInstantiateCropPrefabsEvent()
    {
        if (InstantiateCropPrefabsEvent == null)
            return;

        InstantiateCropPrefabsEvent();
    }
    #endregion

    #region Drop Selected Item Event
    public static event Action DropSelectedItemEvent;

    public static void CallDropSelectedItemEvent()
    {
        if (DropSelectedItemEvent == null)
            return;

        DropSelectedItemEvent();
    }
    #endregion

    #region Remove Selected Item From Inventory Event
    public static event Action RemoveSelectedItemFromInventoryEvent;

    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if (RemoveSelectedItemFromInventoryEvent != null)
        {
            RemoveSelectedItemFromInventoryEvent();
        }
    }
    #endregion

    #region Harvest Action Effect Event
    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

    public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        if (HarvestActionEffectEvent != null)
        {
            HarvestActionEffectEvent(effectPosition, harvestActionEffect);
        }
    }
    #endregion

    #region Inventory Updated Event
    // Inventory Updated Event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }
    #endregion

    #region Movement Event
    // Movement Event
    public static event MovementDelegate MovementEvent;

    // Movement Event Call For Publishers
    public static void CallMovementEvent(
        float xInput,
        float yInput,
        bool isWalking,
        bool isRunning,
        bool isIdle,
        bool isCarrying,
        ToolEffect toolEffect,
        bool isUsingToolRight,
        bool isUsingToolLeft,
        bool isUsingToolUp,
        bool isUsingToolDown,
        bool isLiftingToolRight,
        bool isLiftingToolLeft,
        bool isLiftingToolUp,
        bool isLiftingToolDown,
        bool isPickingRight,
        bool isPickingLeft,
        bool isPickingUp,
        bool isPickingDown,
        bool isSwingingToolRight,
        bool isSwingingToolLeft,
        bool isSwingingToolUp,
        bool isSwingingToolDown,
        bool idleRight,
        bool idleLeft,
        bool idleUp,
        bool idleDown)
    {
        if (EventHandler.MovementEvent != null)
        {
            MovementEvent(
                xInput,
                yInput,
                isWalking,
                isRunning,
                isIdle,
                isCarrying,
                toolEffect,
                isUsingToolRight,
                isUsingToolLeft,
                isUsingToolUp,
                isUsingToolDown,
                isLiftingToolRight,
                isLiftingToolLeft,
                isLiftingToolUp,
                isLiftingToolDown,
                isPickingRight,
                isPickingLeft,
                isPickingUp,
                isPickingDown,
                isSwingingToolRight,
                isSwingingToolLeft,
                isSwingingToolUp,
                isSwingingToolDown,
                idleRight,
                idleLeft,
                idleUp,
                idleDown);
        }
    }
    #endregion

    #region Advance Game Minute Event
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent == null)
            return;

        AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }
    #endregion

    #region Advance Game Hour Event
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent == null)
            return;

        AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }
    #endregion

    #region Advance Game Day Event
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent == null)
            return;

        AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }
    #endregion

    #region Advance Game Season Event
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent == null)
            return;

        AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }
    #endregion

    #region Advance Game Year Event
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent == null)
            return;

        AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }
    #endregion

    #region Before Scene Unload Fade Out Event
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if (BeforeSceneUnloadFadeOutEvent == null)
            return;

        BeforeSceneUnloadFadeOutEvent();
    }
    #endregion

    #region Before Scene Unload Event
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent == null)
            return;

        BeforeSceneUnloadEvent();
    }
    #endregion

    #region After Scene Load Event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent == null)
            return;

        AfterSceneLoadEvent();
    }
    #endregion

    #region After Scene Load Fade In Event
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent == null)
            return;

        AfterSceneLoadFadeInEvent();
    }
    #endregion
}
