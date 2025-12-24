using System.Collections;
using UnityEngine;

public class DrinkState : PlayerState
{
    public DrinkState(PlayerController context) : base(context) { }

    public override void Enter()
    {
        context.Visuals.PlayAnimation("Drink");
        context.TransitionTo(context.Idle);
    }
}