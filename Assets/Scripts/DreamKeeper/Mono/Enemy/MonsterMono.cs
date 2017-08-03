using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    public class MonsterMono : IEnemyMono
    {
        private float roarDistance;
        private EnemyMonster enemyMonster;
        private IPlayer player;
        private string monsterAttack1Path= @"EnemySkills\MonsterAttack1"; // 远程技能
        private FarawayWeapon skillWeapon;  // Enemy的第二武器,利用缓存技术反复使用（不使用resourcesMgr的缓冲池）
        private Vector3 skillPosition; // 存储技能目标位置

        public override void Initialize()
        {
            base.Initialize();
            roarDistance = 10;
            enemyMonster = EnemyMedi.Enemy as EnemyMonster;

        }
        /// <summary>
        /// 因为要在装备切换后初始化，所以需要由IEnemy显式调用
        /// </summary>
        public void SkillWeaponInitialize()
        {
            // skillWeapon 初始化，设定属性值
            skillWeapon = GameMainProgram.Instance.resourcesMgr.LoadAsset(monsterAttack1Path, false, skillPosition, Quaternion.identity).GetComponent<FarawayWeapon>();
            skillWeapon.Initialize();
            EnemyMedi.UpdateEnemyWeapon(skillWeapon);
        }

        /// <summary>
        /// 动画事件
        /// </summary>
        public void Roar()
        {
            ShakeCamera();
            float _dis = Vector3.Distance(Target.position, transform.position);
            if (_dis > roarDistance)
                return;

            if (player == null)
            {
                player = Target.GetComponent<IPlayerMono>().PlayerMedi.Player;
            }
            if (player != null)
                player.Roared();
        }
        public void ShakeCameraWalk()
        {
            CameraCtrl.Instance.ShakeMainCamera(new Vector3(0, 0.5f, 0), 0, 3);
        }

        /// <summary>
        /// 在Target位置生成skill
        /// </summary>
        public void MonsterSkill1()
        {
            skillPosition = Target.position;
            Invoke("ShakeCamera", 0.1f);
            Invoke("SkillLiberate", 0.2f);
        }
        private void ShakeCamera()
        {
            CameraCtrl.Instance.ShakeMainCamera(new Vector3(0, 1, 0), 0, 2); // shake
        }
        private void SkillLiberate()
        {
            skillWeapon.UseWeapon(skillPosition);
        }

    }
}