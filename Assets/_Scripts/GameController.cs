using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallHolder[] _holders;
    [SerializeField] private GameObject _winPanel; 

    private BallHolder[] _whiteHolders;
    private BallHolder[] _blackHolders;

    private const int MAGIC_NUMBER = 2;

    public static GameController Instance;

    public Action OnBallPlaced;
    public Action<int> OnBallEjected;
    public BallHolder[] Holders => _holders;

    private void OnEnable()
    {
        OnBallPlaced += CheckWinCondition;
        OnBallEjected += ReassignDragStates;
    }

    private void OnDisable()
    {
        OnBallPlaced -= CheckWinCondition;
        OnBallEjected -= ReassignDragStates;
    }

    public void SendOnBallPlaced() => OnBallPlaced?.Invoke();
    public void SendOnBallEjected(int emptyHolderIndex) => OnBallEjected?.Invoke(emptyHolderIndex);

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
        AssignHolders();
    }

    private void AssignHolders()
    {
        _whiteHolders = new BallHolder[3];
        _blackHolders = new BallHolder[3];

        for (int i = 0; i < _whiteHolders.Length; i++)
            _whiteHolders[i] = _holders[i];

        for (int i = 0; i < _blackHolders.Length; i++)
            _blackHolders[i] = _holders[i + 4];
    }

    private void ReassignDragStates(int emptyHolderIndex)
    {
        ResetDragStates();
        AssignDragStates(emptyHolderIndex);
    }

    private void ResetDragStates()
    {
        for (int i = 0; i < _holders.Length; i++)
        {
            _holders[i].BallDragAllowed = false;
            _holders[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
        }
    }

    private void AssignDragStates(int emptyHolderIndex)
    {
        _holders[emptyHolderIndex].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);

        for (int i = emptyHolderIndex + 1; i <= emptyHolderIndex + MAGIC_NUMBER && i < _holders.Length; i++)
        {
            _holders[i].BallDragAllowed = true;
            _holders[i].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
        }

        for (int i = emptyHolderIndex - 1; i >= emptyHolderIndex - MAGIC_NUMBER && i >= 0; i--)
        {
            _holders[i].BallDragAllowed = true;
            _holders[i].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
        }
    }

    private void CheckWinCondition()
    {
        CheckBallsInHolders(_whiteHolders, out bool whitePlacedCorrectly, BallType.Black);
        CheckBallsInHolders(_blackHolders, out bool blackPlacedCorrectly, BallType.White);

        if (whitePlacedCorrectly && blackPlacedCorrectly)
        {
            _winPanel.SetActive(true);
        }
    }

    private void CheckBallsInHolders(BallHolder[] holders, out bool placedCorrectly, BallType neededType)
    {
        int correctPlacedCount = 0;

        foreach (var holder in holders)
        {
            if (holder.HoldedBall == null)
            {
                placedCorrectly = false;
                return;
            }

            if (holder.HoldedBall.Type == neededType)
                correctPlacedCount++;
        }
        placedCorrectly = correctPlacedCount == holders.Length;
    }

    public void ExitGame() => Application.Quit();
    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
