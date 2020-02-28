using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public PlayerList PlayerList;
    public GameState GameState;


    public GameEvent EndGameEvent;
    public AttackResultEvent AttackResult;
    public PlayerEvent ChangeTurnEvent;

    private int _count = 0;
    public IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        ChangeTurn();
    }

    
    public void ChangeTurn()
    {
        GameState.CurrentPlayer = PlayerList.Players[_count];
        ChangeTurnEvent.Raise(PlayerList.Players[_count]);
        _count = (_count + 1) % 2;

    }

    private void EndGameTest()
    {
        if (PlayerList.Players.Any(p => p.HP <= 0))
        {
            GameState.IsFinished = true;
            EndGameEvent.Raise();
        }
    }

    public void OnAttackDone(Attack att)
    {

        Debug.Log($"Received Attack {att}");
        var hitRoll = Dice.PercentageChance();
        var result = ScriptableObject.CreateInstance<AttackResult>();
        result.IsHit = false;
        result.Attack = att;
        result.Energy = att.AttackMade.Energy;

        if (att.Source.Energy > att.AttackMade.Energy && hitRoll <= att.AttackMade.HitChance)
        {
            result.IsHit = true;
            
            result.Damage = Dice.RangeRoll(att.AttackMade.MinDam, att.AttackMade.MaxDam + 1);

            
            att.Target.HP -= result.Damage;
            
        }

        if (att.Source.Energy > att.AttackMade.Energy)
        {
            att.Source.Energy -= result.Energy;
        }

        AttackResult.Raise(result);

        ChangeTurn();
        EndGameTest();
        
    }
}
