using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollisionNotifier : MonoBehaviour
{
    // 定義一個靜態事件，用於通知碰撞
    public static event Action<GameObject, GameObject> OnPlayerCollision;

    // 定義一個靜態事件，用於通知離開碰撞
    public static event Action<GameObject, GameObject> OnPlayerCollisionExit;

    // 當玩家進行 2D 碰撞時觸發
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 檢查是否有訂閱者，然後觸發事件，傳遞玩家和被碰撞的物件
        OnPlayerCollision?.Invoke(this.gameObject, collision.gameObject);
    }

    // 當玩家離開 2D 碰撞時觸發
    void OnCollisionExit2D(Collision2D collision)
    {
        // 檢查是否有訂閱者，然後觸發事件，傳遞玩家和離開碰撞的物件
        OnPlayerCollisionExit?.Invoke(this.gameObject, collision.gameObject);
    }
}
