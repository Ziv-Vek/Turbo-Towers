using BehaviorDesigner.Runtime;
using UnityEngine;
using Tooltip = BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Turbo Towers")]
[TaskDescription("Rotate (yaw) agent's turret to aim at the target")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}RotateTowardsIcon.png")]

public class RotateToTarget : Action
{
    public SharedBodyPart target;
    //public float angleThreshold = 5f;
    public SharedTransform head;
    
    [Tooltip.Tooltip("The agent is done rotating when the angle is less than this value")]
    [UnityEngine.Serialization.FormerlySerializedAs("rotationEpsilon")]
    public SharedFloat m_RotationEpsilon = 0.5f;
    [Tooltip.Tooltip("The maximum number of angles the agent can rotate in a single tick")]
    [UnityEngine.Serialization.FormerlySerializedAs("maxLookAtRotationDelta")]
    public SharedFloat m_MaxLookAtRotationDelta = 1;
    [Tooltip.Tooltip("Should the rotation only affect the Y axis?")]
    [UnityEngine.Serialization.FormerlySerializedAs("onlyY")]
    public SharedBool m_OnlyY;
    [Tooltip.Tooltip("If target is null then use the target rotation")]
    [UnityEngine.Serialization.FormerlySerializedAs("targetRotation")]
    public SharedVector3 m_TargetRotation;

        public override TaskStatus OnUpdate()
        {
            var rotation = Target();
            // Return a task status of success once we are done rotating
            if (Quaternion.Angle(head.Value.rotation, rotation) < m_RotationEpsilon.Value) {
                return TaskStatus.Success;
            }
            
            // We haven't reached the target yet so keep rotating towards it
            head.Value.rotation = Quaternion.RotateTowards(head.Value.rotation,
                rotation,
                m_MaxLookAtRotationDelta.Value * Time.deltaTime);
            return TaskStatus.Running;
        }

        // Return targetPosition if targetTransform is null
        private Quaternion Target()
        {
            if (target == null || target.Value == null) {
                return Quaternion.Euler(m_TargetRotation.Value);
            }
            var position = target.Value.transform.position - head.Value.position;
            if (m_OnlyY.Value) {
                position.y = 0;
            }
            
            return Quaternion.LookRotation(position);
        }

        // Reset the public variables
        public override void OnReset()
        {
            m_RotationEpsilon = 0.5f;
            m_MaxLookAtRotationDelta = 1f;
            m_OnlyY = false;
            target = null;
            m_TargetRotation = Vector3.zero;
        }
    
}