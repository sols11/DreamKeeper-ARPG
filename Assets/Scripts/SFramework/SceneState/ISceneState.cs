using System.Collections;
using UnityEngine;

namespace SFramework
{
	/// <summary>
	/// 场景状态"接口"类
	/// </summary>
	public class ISceneState
	{
		private string m_StateName = "ISceneState";
		public string StateName
		{
			get { return m_StateName; }
			set { m_StateName = value; }
		}

		// 控制者
		protected SceneStateController m_Controller = null;

		// 建構者
		public ISceneState(SceneStateController Controller)
		{
			m_Controller = Controller;
		}

		public virtual void StateBegin(){ }
		public virtual void StateEnd(){ }
		public virtual void StateUpdate(){ }
		public virtual void FixedUpdate(){ }

		public override string ToString()
		{
			return string.Format("[I_SceneState: StateName={0}]", StateName);
		}


	}
}
