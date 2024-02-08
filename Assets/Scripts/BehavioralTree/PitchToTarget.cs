using BehaviorDesigner.Runtime;
using UnityEngine;
using Tooltip = BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Turbo Towers")]
[TaskDescription("Pitch agent's turret to aim at the target")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}RotateTowardsIcon.png")]
public class PitchToTarget : Action
{
    public SharedBodyPart target;
    //public float angleThreshold = 5f;
    public SharedTransform turretPivot;
    
    [Tooltip.Tooltip("The agent is done rotating when the angle is less than this value")]
    public SharedFloat m_PitchRotationEpsilon = 0.5f;
    [Tooltip.Tooltip("The maximum number of angles the agent can rotate in a single tick")]
    public SharedFloat m_MaxTurretPitchRotationDelta = 1;
    [Tooltip.Tooltip("If target is null then use the target rotation")]
    public SharedVector3 m_TargetPitchRotation;

    public override TaskStatus OnUpdate()
        {
            var rotation = Target();
            // Return a task status of success once we are done rotating
            if (Quaternion.Angle(turretPivot.Value.rotation, rotation) < m_PitchRotationEpsilon.Value) {
                return TaskStatus.Success;
            }
            
            // We haven't reached the target yet so keep rotating towards it
            turretPivot.Value.rotation = Quaternion.RotateTowards(turretPivot.Value.rotation,
                rotation,
                m_MaxTurretPitchRotationDelta.Value * Time.deltaTime);
            return TaskStatus.Running;
        }

        // Return targetPosition if targetTransform is null
        private Quaternion Target()
        {
            if (target == null || target.Value == null) {
                return Quaternion.Euler(m_TargetPitchRotation.Value);
            }
            var position = target.Value.transform.position - turretPivot.Value.position;
            
            return Quaternion.LookRotation(position, Vector3.forward);
        }

        // Reset the public variables
        public override void OnReset()
        {
            m_PitchRotationEpsilon = 0.5f;
            m_MaxTurretPitchRotationDelta = 1f;
            target = null;
            m_TargetPitchRotation = Vector3.zero;
        }
        
        public override void OnDrawGizmos()
        {
            if (target.Value == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(turretPivot.Value.transform.position, target.Value.transform.position);
        }
}