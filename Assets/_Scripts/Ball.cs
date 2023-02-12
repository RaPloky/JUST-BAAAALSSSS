using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    [SerializeField] private Transform _ballTrans;
    [SerializeField] private BallType _ballType;
    [SerializeField] private Transform _currentHolder;

    public BallType Type { get => _ballType; set => _ballType = value; }

    private GameController _gameController;
    private BallHolder CurrentHolderComponent => _currentHolder.GetComponent<BallHolder>();

    private void Start()
    {
        _gameController = GameController.Instance;
    }

    private void OnMouseDrag()
    {
        if (!CurrentHolderComponent.BallDragAllowed)
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _ballTrans.position = new Vector3(mousePosition.x, mousePosition.y, _ballTrans.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var component = collision.GetComponent<BallHolder>();

        if (component == null)
            return;

        if (component.HoldedBall == null)
        {
            CurrentHolderComponent.HoldedBall = null;

            component.HoldedBall = gameObject.GetComponent<Ball>();
            _gameController.SendOnBallEjected(Array.IndexOf(_gameController.Holders, CurrentHolderComponent));

            _currentHolder = collision.gameObject.transform;

            _gameController.SendOnBallPlaced();
        }
    }

    private void OnMouseUp()
    {
        _ballTrans.position = new Vector3(_currentHolder.position.x, _currentHolder.position.y, _ballTrans.position.z);
    }
}

public enum BallType
{
    None,
    White,
    Black
}
