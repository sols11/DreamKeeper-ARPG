using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek目标，并且控制朝向")]
    [TaskCategory("Movement")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=3")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class AiSeek : Action
    {
        [Tooltip("The speed of the agent")]
        public SharedFloat speed;
        [Tooltip("Angular speed of the agent")]
        public SharedFloat angularSpeed;
        [Tooltip("The agent has arrived when the magnitude is less than this value")]
        public SharedFloat arriveDistance = 0.1f;
        [Tooltip("The transform that the agent is moving towards")]
        public SharedTransform targetTransform;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        // True if the target is a transform
        private bool dynamicTarget;
        // A cache of the NavMeshAgent
        private UnityEngine.AI.NavMeshAgent navMeshAgent;
        private Vector3 direction;  // 目标方向

        public override void OnAwake()
        {
            // cache for quick lookup
            navMeshAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public override void OnStart()
        {
            // the target is dynamic if the target transform is not null and has a valid
            dynamicTarget = (targetTransform != null && targetTransform.Value != null);

            // set the speed, angular speed, and destination then enable the agent
            navMeshAgent.speed = speed.Value;
            navMeshAgent.angularSpeed = angularSpeed.Value;
            navMeshAgent.enabled = true;
            navMeshAgent.destination = Target();
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < arriveDistance.Value&&
                Vector3.Distance(transform.forward,direction)<0.3) //设置要转向
            {
                return TaskStatus.Success;
            }

            // Update the destination if the target is a transform because that agent could move
            if (dynamicTarget)
            {
                navMeshAgent.destination = Target();
            }
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            // Disable the nav mesh
            navMeshAgent.enabled = false;
        }

        // Return targetPosition if targetTransform is null
        private Vector3 Target()
        {
            if (dynamicTarget)
            {
                direction = (targetTransform.Value.position - transform.position).normalized; //计算
                return targetTransform.Value.position;
            }
            return targetPosition.Value;
        }

        // Reset the public variables
        public override void OnReset()
        {
            arriveDistance = 0.1f;
        }
    }
}