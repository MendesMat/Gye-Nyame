using System;
using Unity.Behavior;
using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.AI.Nodes
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(
        name: "CanAcquireToken", 
        story: "Checks if [Agent] can get token from [Director]", 
        category: "Conditions", 
        id: "17dee9708f7f3abed890cc34279e13d7")]
    public partial class CanAcquireTokenCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;

        public override bool IsTrue()
        {
            if (Agent == null || Agent.Value == null) return false;
            if (Director == null || Director.Value == null) return false;
            
            IAttackDirector director = Director.Value.GetComponent<IAttackDirector>();
            if (director == null) return false;

            return director.IsTokenAvailableFor(Agent.Value);
        }
    }
}
