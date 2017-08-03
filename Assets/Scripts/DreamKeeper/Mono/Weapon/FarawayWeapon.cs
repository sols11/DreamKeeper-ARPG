using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    /// <summary>
    /// 远程武器，释放time时间后关闭collider，自动setFalse，再次使用时setTrue
    /// </summary>
    public class FarawayWeapon : IEnemyWeapon
    {
        public bool autoCloseCollider = true;
        public float time = 0.1f;

        public override void Initialize()
        {
            base.Initialize();
            IsOnlyPlayer = false;
            EnemyMedi = null;
            OnlyPlayerMono = null;
            gameObject.SetActive(false);
        }
        public override void Release()
        {
            base.Release();
            StopAllCoroutines();
        }

        public void UseWeapon(Vector3 _pos)
        {
            transform.position = _pos;
            gameObject.SetActive(true);
            WeaponCollider.enabled = true;
            if (autoCloseCollider)
                StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);
            WeaponCollider.enabled = false;
        }

        protected override void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == (int)ObjectLayer.Player)
            {
                RealAttack = BasicAttack * AttackFactor;
                //传参给Player告知受伤
                PlayerHurtAttr.ModifyAttr((int)RealAttack, VelocityForward, VelocityVertical, TransformForward, canDefeatedFly);
                if (IsOnlyPlayer)
                {
                    if (OnlyPlayerMono == null)
                        OnlyPlayerMono = col.GetComponent<IPlayerMono>();
                    OnlyPlayerMono.Hurt(PlayerHurtAttr);
                }
                else
                {
                    col.GetComponent<IPlayerMono>().Hurt(PlayerHurtAttr);
                }
            }
        }  // end_function

    }
}