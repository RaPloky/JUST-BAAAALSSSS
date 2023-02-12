using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallHolder : MonoBehaviour
{
    [SerializeField] private HolderType _holderType;
    [SerializeField] private Ball _holdedBall;

    public bool BallDragAllowed = true;
    public Ball HoldedBall { get => _holdedBall; set => _holdedBall = value; }

    public enum HolderType
    {
        White,
        Black,
        None
    }
}
