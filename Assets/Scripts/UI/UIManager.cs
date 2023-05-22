using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehavior<UIManager>
{
    private bool _pauseMenuOn = false;

    [SerializeField]
    private PauseMenuInventoryManagement _pauseMenuInventoryManagement = null;

    [SerializeField]
    private GameObject _pauseMenu = null;

    [SerializeField]
    private GameObject[] _menuTabs = null;

    [SerializeField]
    private Button[] _menuButtons = null;

    [SerializeField]
    private UIInventoryBar _uiInventoryBar = null;

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        _pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_pauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        _uiInventoryBar.DestroyCurrentlyDraggedItems();
        _uiInventoryBar.ClearCurrentlySelectedItem();

        _pauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true;

        Time.timeScale = 0;
        _pauseMenu.SetActive(true);

        // Trigger garbage collector
        GC.Collect();

        // Highlight selected button
        HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        _pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();

        _pauseMenuOn = false;
        Player.Instance.PlayerInputIsDisabled = false;
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
    }

    private void HighlightButtonForSelectedTab()
    {
        foreach (var tab in _menuTabs.WithIndex())
        {
            SetButtonColorActive(_menuButtons[tab.index], tab.item.activeSelf);
        }
    }

    private void SetButtonColorActive(Button button, bool isActive)
    {
        var colors = button.colors;
        colors.normalColor = isActive ? colors.pressedColor : colors.disabledColor;
        button.colors = colors;
    }

    public void SwitchPauseMenuTab(int tabIndex)
    {
        foreach (var tab in _menuTabs.WithIndex())
        {
            tab.item.SetActive(tab.index == tabIndex);
        }

        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
