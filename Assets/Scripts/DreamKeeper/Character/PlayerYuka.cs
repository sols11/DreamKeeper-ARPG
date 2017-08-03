using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
	public class PlayerYuka:IPlayer
	{
		//动画参数名
		private string aniMainIdle = "MainIdle";
		private string aniSpeed = "Speed";
		private string aniAvoid = "Avoid";
        private string aniParried = "Parried";
        private string aniRoared = "Roared";
        private string aniDush = "Dush";
        private string aniDrink = "Drink";
        private string aniHurt = "Hurt";
        private string aniDefeated = "Defeated";
        private string aniDead = "Dead";
        private string aniLAttack = "LightAttack";
		private string aniLAttack1 = "LightAttack1";//第一击是trigger
		private string aniHAttack = "HeavyAttack";
		private string aniHAttack1 = "HeavyAttack1";//第一击是trigger
		//计算需要的字段
		private int lightAttack = 0;
		private int heavyAttack = 0;
        private float recoveryTime = 0.3f; // SP自动回复所需时间
        private float recoveryTimer = 0.3f; // 使用计时器而不使用协程实现
        private float recoveryWait = 1; // 要实现使用SP1秒后才能开始回复的效果
		private Vector3 targetDirection;//输入的方向
		private Vector3 forwardDirection;//存储输入后的朝向
        //获取Mono的引用
		private KatanaAnimEvent katanaAnimEvent;

        /// <summary>
        /// 创建时的初始化
        /// </summary>
        /// <param name="gameObject"></param>
		public PlayerYuka(GameObject gameObject):base(gameObject)
		{
            katanaAnimEvent = PlayerMedi.PlayerMono as KatanaAnimEvent;
            // 存档无关的属性初始化
            MaxHP = 200;
            MaxSP = 100;
            CurrentSP = MaxSP;
            MoveSpeed = 6;
            RotSpeed = 12;
            // 以下是其他属性
            m_CanMove = true;
            CanRotate = true;
            recoveryTime = 0.05f;
            recoveryTimer = recoveryTime;

            LoadData();
        }

        /// <summary>
        /// 每次切换场景时执行
        /// </summary>
		public override void Initialize() {
            // 更新数量与背包中相同
            if (Fit[(int)FitType.Medicine] < PropPack.Length)
                MedicineNum = PropPack[Fit[(int)FitType.Medicine]].Num;
            else
                MedicineNum = 0;    // 没装备就是0
        }

        public override void Update() {
			stateInfo = animator.GetCurrentAnimatorStateInfo(0);

			MoveInput();
			Attack();

            //自动回复
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0)
            {
                recoveryTimer = recoveryTime;
                CurrentSP+=1;
            }
        }
		public override void FixedUpdate() {
			if (CanMove)
				if (stateInfo.IsName("Idle") || stateInfo.IsName("Run"))//使用MovePosition调整速度的状态
					GroundMove(h, v);
		}

		/// <summary>
		/// 触发动画，伤害计算，死亡判断
		/// </summary>
		/// <param name="damage"></param>
		public override void Hurt(PlayerHurtAttr _playerHurtAttr)
		{
            if (stateInfo.IsName("Dead") || stateInfo.IsName("Dush"))
                return;
            Rgbd.velocity = _playerHurtAttr.TransformForward * _playerHurtAttr.VelocityForward;
            if (_playerHurtAttr.CanDefeatedFly) // 击飞
            {
                if (_playerHurtAttr.TransformForward != Vector3.zero)
                    GameObjectInScene.transform.forward = -_playerHurtAttr.TransformForward;
                animator.SetTrigger(aniDefeated);
            }
            else
            {
                animator.SetTrigger(aniHurt);
            }
			CurrentHP -= _playerHurtAttr.Attack;
            Debug.Log("PlayerHurt:" + _playerHurtAttr.Attack);
            katanaAnimEvent.TrailSwitch(0); // close Trail
		}
        public override void Dead()
        {
            base.Dead();
            animator.SetTrigger(aniDead);

        }
        /// <summary>
        /// 被弹开，提供给AnimEvent调用
        /// </summary>
        public void Parried()
        {
            animator.SetTrigger(aniParried);
        }
        /// <summary>
        /// 被吼叫，提供给Enemy触发
        /// </summary>
        public override void Roared()
        {
            animator.SetTrigger(aniRoared);
        }
        /// <summary>
        /// 喝药，提供给AnimEvent调用
        /// </summary>
        public void DrinkMedicine()
        {
            CurrentHP += PropPack[Fit[(int)FitType.Medicine]].HP;
        }
        /// <summary>
        /// 处理方向键、闪避输入、冲刺攻击，进行移动和旋转
        /// </summary>
        void MoveInput()
		{
            h = Input.GetAxisRaw("Horizontal");
			v = Input.GetAxisRaw("Vertical");
			targetDirection = new Vector3(h, 0, v);

			//移动状态时使用平滑旋转
			if (stateInfo.IsName("Idle") || stateInfo.IsName("Run"))
			{
				if (CanRotate)
					Rotating();
				//用于回到了Idle或Run状态则恢复输入，但是如果仍在Transition未过渡到attack等状态时不要打开开关（刚刚在attack关闭开关）
				if (!animator.IsInTransition(0))
				{
					m_CanMove = true;//仅打开开关
				}
                DrinkInput();
			}
            //注意下面这段代码要放在canRotate=true下面(执行顺序)
            AvoidInput();
            DushInput();

            //是否切换为Run
            if (Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1)
				Speed = 1;
			else
				Speed = 0;
			animator.SetFloat(aniSpeed, Speed);
		}
		/// <summary>
		/// 平滑旋转
		/// </summary>
		/// <param name="h"></param>
		/// <param name="v"></param>
		void Rotating()
		{
			//计算出旋转
			if (targetDirection != Vector3.zero)
			{
				//目标方向的旋转角度
				Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
				//平滑插值
				Quaternion newRotation = Quaternion.Slerp(Rgbd.rotation, targetRotation, RotSpeed * Time.deltaTime);
				Rgbd.MoveRotation(newRotation);
			}
		}
		/// <summary>
		/// 地面移动
		/// </summary>
		/// <param name="h"></param>
		/// <param name="v"></param>
		void GroundMove(float h, float v)
		{
			if (Speed != 0)
				Rgbd.MovePosition(GameObjectInScene.transform.position + new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);
		}

        void DrinkInput()
        {
            // 只要Num>=1，就说明装备了可以使用的道具
            if(Input.GetButtonDown("Drink")&& MedicineNum >= 1)
            {
                MedicineNum -= 1;
                // 每次使用时让背包中对应道具数目也减少
                PropPack[Fit[(int)FitType.Medicine]].Num -= 1;
                animator.SetTrigger(aniDrink);
            }
        }
        void AvoidInput()
        {
            //如果是Transition状态（如avoid->run,run->avoid）或正在avoid则不允许输入
            if (!animator.IsInTransition(0) && (stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Att1") ||
                stateInfo.IsName("Att2") || stateInfo.IsName("Att3") || stateInfo.IsName("Att4") || stateInfo.IsName("3Att3") || stateInfo.IsName("3Att4")))
                if (Input.GetButtonDown("Avoid") && CurrentSP >= 25)//闪避
                {
                    CurrentSP -= 25;
                    recoveryTimer = recoveryWait;
                    animator.SetTrigger(aniAvoid);
                    if (targetDirection != Vector3.zero)//如果不是向前方闪避，那么先转向
                    {
                        Rgbd.MoveRotation(Quaternion.LookRotation(targetDirection, Vector3.up));
                        katanaAnimEvent.TrailSwitch(0);//关闭col和trail

                    }
                }
        }
        void DushInput()
        {
            if (Input.GetButtonDown("Dush") && CurrentSP >= 35)//闪避
            {
                if (animator.IsInTransition(0) || stateInfo.IsName("Hurt") || stateInfo.IsName("Avoid") || stateInfo.IsName("Dush") ||
stateInfo.IsName("YukaRoaredStart") || stateInfo.IsName("YukaRoaredLoop") || stateInfo.IsName("Parried") ||
stateInfo.IsName("Defeated") || stateInfo.IsName("Dead") || stateInfo.IsName("Drink"))
                    return;
                CurrentSP -= 35;
                recoveryTimer = recoveryWait;
                animator.SetTrigger(aniDush);
                if (targetDirection != Vector3.zero)//如果不是向前方闪避，那么先转向
                {
                    Rgbd.MoveRotation(Quaternion.LookRotation(targetDirection, Vector3.up));
                    katanaAnimEvent.TrailSwitch(0);//关闭col和trail
                                                   //Rgbd.velocity = targetDirection * 10;
                }
                // 切换layer并自动切换回来
                GameObjectInScene.layer = (int)ObjectLayer.Without;
                CoroutineMgr.Instance.StartCoroutine(ComebackLayer());
            }
        }

        IEnumerator ComebackLayer()
        {
            yield return new WaitForSeconds(0.9f);
            GameObjectInScene.layer = (int)ObjectLayer.Player;
        }

        void Attack()
		{
			if ((stateInfo.IsName("Idle") || stateInfo.IsName("Run")))
			{
				IdleOrRun();
			}
			else if (stateInfo.IsName("Avoid"))
			{
				Avoid();
			}
			else if (stateInfo.IsName("Att1"))
			{
				Att1();
			}
			else if (stateInfo.IsName("2Att1"))
			{
				_2Att1();
			}
			else if (stateInfo.IsName("Att2"))
			{
				Att2();
			}
			else if (stateInfo.IsName("2Att2"))
			{
				_2Att2();
			}
			else if (stateInfo.IsName("3Att2"))
			{
				_3Att2();
			}
			else if (stateInfo.IsName("Att3"))
			{
				Att3();
			}
			else if (stateInfo.IsName("2Att3"))
			{
				_2Att3();
			}
			else if (stateInfo.IsName("3Att3"))
			{
				_3Att3();
			}
		}
		void IdleOrRun()
		{
			if (!animator.IsInTransition(0))
			{
				if (Input.GetButtonDown("Attack1"))
				{
					lightAttack = 1;
					heavyAttack = 0;//清除
					animator.SetTrigger(aniLAttack1);
					animator.SetInteger(aniLAttack, lightAttack);//清除之前记录的lightAttack
					animator.SetInteger(aniHAttack, heavyAttack);//清除之前记录的heavyAttack
					CanMove = false;
				}
				else if (Input.GetButtonDown("Attack2"))
				{
					lightAttack = 0;//清除
					heavyAttack = 1;//第一击重击设为0以区别开
					animator.SetTrigger(aniHAttack1);
					animator.SetInteger(aniLAttack, lightAttack);//清除之前记录的lightAttack
					animator.SetInteger(aniHAttack, heavyAttack);//清除之前记录的lightAttack
					CanMove = false;
				}
			}
			else
			{
				//idle->Att1
				if (lightAttack == 1)
				{
					if (Input.GetButtonDown("Attack1"))
					{
						lightAttack = 2;
					}
					else if (Input.GetButtonDown("Attack2"))
					{
						heavyAttack = 1;
					}
				}
				else if (heavyAttack == 1)
				{
					if (Input.GetButtonDown("Attack1"))
					{
						lightAttack = 2;
					}
					else if (Input.GetButtonDown("Attack2"))
					{
						heavyAttack = 2;
					}
				}
			}
		}
		void Avoid()
		{
			if(stateInfo.normalizedTime<0.7f)
				Rgbd.velocity = GameObjectInScene.transform.forward*MoveSpeed*2;
		}
		void Att1()
		{
			if (Input.GetButtonDown("Attack1"))
			{
				lightAttack = 2;
			}
			else if (Input.GetButtonDown("Attack2"))
			{
				heavyAttack = 1;
			}
			//过早的输入也只能在0.7的time时进行transition，其他输入在输入时tansition
			//如果在Att1状态且未发生状态转换
			if (!animator.IsInTransition(0))
			{
				if (stateInfo.normalizedTime >= 0.6f)
				{
					//朝目标方向的旋转
					if (targetDirection != Vector3.zero)
						Rgbd.MoveRotation(Quaternion.LookRotation(targetDirection, Vector3.up));
					//转完方向后再移动
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);
				}
			}
			else//如果发生到其他状态的转换，那么更新输入
			{
				if (lightAttack == 2)//Att1->Att2
				{
					if (Input.GetButtonDown("Attack1"))
					{
						lightAttack = 3;
					}
					else if (Input.GetButtonDown("Attack2"))
					{
						heavyAttack = 2;
					}
				}
				// Att1->2Att1
				else if (heavyAttack == 1)
					if (Input.GetButtonDown("Attack1"))
					{
						lightAttack = 2;
					}
					else if (Input.GetButtonDown("Attack2"))
					{
						heavyAttack = 2;
					}
			}
		}
		void _2Att1()
		{
			if (Input.GetButtonDown("Attack1"))
			{
				lightAttack = 2;
			}
			else if (Input.GetButtonDown("Attack2"))
			{
				heavyAttack = 2;
			}
			//过早的输入也只能在0.7的time时进行transition，其他输入在输入时tansition
			if (!animator.IsInTransition(0))//一旦已经向atk2过渡就不要执行了
			{
				if (stateInfo.normalizedTime >= 0.75f)
				{
					//攻击前平滑转向
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);
				}
			}
			else
			{
				if (CanRotate)
					Rotating();
				if (lightAttack == 2)
				{
					if (Input.GetButtonDown("Attack1"))
					{
						lightAttack = 3;
					}
					else if (Input.GetButtonDown("Attack2"))
					{
						heavyAttack = 2;
					}
				}
				else if (heavyAttack == 2)
				{
					if (Input.GetButtonDown("Attack1"))
						lightAttack = 3;
					else if (Input.GetButtonDown("Attack2"))
						heavyAttack = 3;
				}
			}
		}
		void Att2()
		{
			if (Input.GetButtonDown("Attack1"))
			{
				lightAttack = 3;
			}
			else if (Input.GetButtonDown("Attack2"))
			{
				heavyAttack = 2;
			}
			if (!animator.IsInTransition(0))//一旦已经向atk3过渡就不要执行了
			{
				if (stateInfo.normalizedTime >= 0.6f)
				{
					//朝目标方向的旋转
					if (targetDirection != Vector3.zero)
						Rgbd.MoveRotation(Quaternion.LookRotation(targetDirection, Vector3.up));
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);
				}
			}
			else
			{
				if (Input.GetButtonDown("Attack1"))
					lightAttack = 4;
				else if (Input.GetButtonDown("Attack1"))
					heavyAttack = 4;
			}
		}
		void _2Att2()
		{
			if (Input.GetButtonDown("Attack1"))
				lightAttack = 3;
			else if (Input.GetButtonDown("Attack2"))
				heavyAttack = 3;
			//过早的输入也只能在0.7的time时进行transition，其他输入在输入时tansition
			if (!animator.IsInTransition(0))//一旦已经向atk2过渡就不要执行了
			{
				if (stateInfo.normalizedTime >= 0.75f)
				{
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);
				}
				
			}
			else
			{
				if (CanRotate)
					Rotating();
				if (Input.GetButtonDown("Attack1"))
					lightAttack = 4;
				else if (Input.GetButtonDown("Attack2"))
					heavyAttack = 4;
			}
		}
		void _3Att2()
		{
			if (Input.GetButtonDown("Attack1"))
				lightAttack = 3;
			else if (Input.GetButtonDown("Attack2"))
				heavyAttack = 3;

			if (!animator.IsInTransition(0))//一旦已经向atk3过渡就不要执行了
			{
				if (stateInfo.normalizedTime >= 0.7f)
				{
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);
					if (lightAttack == 3)
						Rgbd.velocity = GameObjectInScene.transform.forward * 5;
				}
			}
			else
			{
				if (CanRotate)
					Rotating();
				if (Input.GetButtonDown("Attack1"))
					lightAttack = 4;
				else if (Input.GetButtonDown("Attack1"))
					heavyAttack = 4;
			}
		}
		void Att3()
		{
			if (Input.GetButtonDown("Attack1"))
				lightAttack = 4;
			else if (Input.GetButtonDown("Attack2"))
				heavyAttack = 4;

			if (!animator.IsInTransition(0))//一旦已经向atk4过渡就不要执行了
			{
				if (stateInfo.normalizedTime >= 0.7f)
				{
					//朝目标方向的旋转
					if (targetDirection != Vector3.zero)
						Rgbd.MoveRotation(Quaternion.LookRotation(targetDirection, Vector3.up));
					//转完方向后再移动
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);

					if (lightAttack == 4)
						Rgbd.velocity += new Vector3(0, 3, 0);
				}
			}
		}
		void _2Att3()
		{
			if (Input.GetButtonDown("Attack1"))
				lightAttack = 4;
			else if (Input.GetButtonDown("Attack2"))
				heavyAttack = 4;

			if (!animator.IsInTransition(0))//一旦已经向atk4过渡就不要执行了
			{
				if (stateInfo.normalizedTime >= 0.75f)
				{
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);
				}
			}
			else if (CanRotate)
				Rotating();
		}
		void _3Att3()
		{
			if (Input.GetButtonDown("Attack1"))
				lightAttack = 4;
			else if (Input.GetButtonDown("Attack2"))
				heavyAttack = 4;

			if (!animator.IsInTransition(0))//一旦已经向atk4过渡就不要执行了
				if (stateInfo.normalizedTime >= 0.7f)
				{
					//朝目标方向的旋转
					if (targetDirection != Vector3.zero)
						Rgbd.MoveRotation(Quaternion.LookRotation(targetDirection, Vector3.up));
					//转完方向后再移动
					animator.SetInteger(aniLAttack, lightAttack);
					animator.SetInteger(aniHAttack, heavyAttack);
				}
		}

        // MainScene
        public void MainIdle()
        {
            animator.SetBool(aniMainIdle,true);
            Rgbd.MoveRotation(Quaternion.Euler(0, 160, 4));
        }
        public void BackIdle()
        {
            animator.SetBool(aniMainIdle, false);
            Rgbd.MoveRotation(Quaternion.identity);
        }
    }
}