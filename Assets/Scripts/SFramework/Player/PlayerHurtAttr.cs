using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// 传参给Player受伤的属性类
    /// </summary>
    public class PlayerHurtAttr
    {
        public int Attack { get; set; }
        public float VelocityForward { get; set; }
        public float VelocityVertical { get; set; }
        public Vector3 TransformForward { get; set; }
        public bool CanDefeatedFly { get; set; }

        public PlayerHurtAttr()
        { }

        public void ModifyAttr(int _Attack, float _velocityForward, float _velocityVertical, Vector3 _transformForward,
            bool _canDefeatedFly=false)
        {
            Attack = _Attack;
            VelocityForward = _velocityForward;
            VelocityVertical = _velocityVertical;
            TransformForward = _transformForward;
            CanDefeatedFly = _canDefeatedFly;
        }
    }
}