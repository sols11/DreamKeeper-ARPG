using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
	/// <summary>
	/// 传参给Enemy受伤的属性类
	/// </summary>
	public class EnemyHurtAttr
	{
		public int Attack { get; set; }
		public float VelocityForward { get; set; }
		public float VelocityVertical { get; set; }
        public Vector3 TransformForward { get; set; }

        public EnemyHurtAttr()
		{	}

		public void ModifyAttr(int _attack, float _velocityForward, float _velocityVertical,Vector3 _transformForward)
		{
			Attack = _attack;
            VelocityForward = _velocityForward;
            VelocityVertical = _velocityVertical;
            TransformForward = _transformForward;
		}
	}
}