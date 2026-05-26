using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    protected StateMachineControler SMC;

    public void StartState(StateMachineControler stateMachineControler)
    {
        SMC = stateMachineControler;

        StartOverride();
    }

    protected virtual void StartOverride()
    {
        
    }

    public void OnUpdate()
    {
        UpdateOverride();
    }

    protected virtual void UpdateOverride()
    {
        
    }


}
