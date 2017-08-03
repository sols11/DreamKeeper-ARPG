using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// 当选中物体时，可以通过快捷键“Ctrl + Shift + H”激活或者关闭该物体。
/// </summary>
public class GameObjectActive : ScriptableObject
{
    //这个字符串是在菜单栏里新建了一个Editor，点开后DisableSelectGameObect,%#h 代表的是ctrl+shift+h 快捷键组合
    public const string KeyName = "Editor/DisableSelectGameObect %#h";

    //根据当前有没有选中物体来判断可否用快捷键(这个是设置这个命令什么时候可用)  
    [MenuItem(KeyName, true)]
    static bool ValidateSelectEnableDisable()
    {
        GameObject[] go = GetSelectedGameObjects() as GameObject[];

        if (go == null || go.Length == 0)
            return false;
        return true;
    }
    //这个就是执行命令的函数了，找到选中的物体，及其所有子物体，然后设置他们的激活状态
    [MenuItem(KeyName)]
    static void SeletEnable()
    {
        bool enable = false;
        GameObject[] gos = GetSelectedGameObjects() as GameObject[];

        foreach (GameObject go in gos)
        {
            enable = !go.activeInHierarchy;
            EnableGameObject(go, enable);
        }
    }

    //获得选中的物体  
    static GameObject[] GetSelectedGameObjects()
    {
        return Selection.gameObjects;
    }

    //激活或关闭当前选中物体  
    public static void EnableGameObject(GameObject parent, bool enable)
    {
        parent.gameObject.SetActive(enable);
    }
}