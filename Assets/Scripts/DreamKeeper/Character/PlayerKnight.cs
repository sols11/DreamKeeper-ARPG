using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    public class PlayerKnight : IPlayer
    {
        private string aniWalk = "Walk";
        private string aniRun = "Run";
        private string aniHurt = "Hurt";
        private string aniDead = "Dead";
        private string aniDefend = "Defend";
        private string aniAttack1 = "Attack1";
        private string aniAttack12 = "Attack12";
        private string aniAttack13 = "Attack13";


        //自身GameObject相关的初始化
        public PlayerKnight(GameObject gameObject):base(gameObject)
        {

        }

    }
}