using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// + 道具数目与上限
    /// 道具基类
    /// </summary>
    public class IProp : IItem
    {
        //不需考虑计算时超出范围可能性的属性就不写具体的set了
        //展示属性
        public PropType Type { get; protected set; }
        public int Num { get; set; }  //当前数目
        public int MaxNum { get; protected set; }  //上限数目

        public IProp():base()
        {
            Type = PropType.Medicine;
            Name = "Prop";
            Num = 0;
            MaxNum = 10;
        }

    }
}
