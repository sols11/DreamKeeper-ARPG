using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskCategory("Custom")]//此Task将显示在Custom栏目下  
	[TaskDescription("寻找标签目标物体,通过引用IEnemyMono")]
	public class AiSearchTarget : Action
	{
		[SerializeField]
		private SharedTransform target;//SharedTransform是BehaviorDesigner的类型，对应Transform类型。顾名思义，就是将此变量共享，以便其它Task访问，需要在Variables面板中定义出一个同类型变量以接收此变量  
        private IEnemyMono iEnemyMono;

		public override void OnStart()
		{
            iEnemyMono = GetComponent<IEnemyMono>();
			target.Value = iEnemyMono.Target; // 传值的一步
		}

		public override TaskStatus OnUpdate()
		{
			if (target != null)
				return TaskStatus.Success;
			return TaskStatus.Failure;
		}

	}
}