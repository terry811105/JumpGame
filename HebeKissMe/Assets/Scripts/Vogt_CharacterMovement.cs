using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;       // 移動速度
    public float jumpForce = 10f;      // 跳躍力度
    private bool isGrounded = false;   // 判斷是否接觸地板

   

    void Start()
    {
        
    }

    void Update()
    {
        // 左右移動
        if (Input.GetKey(KeyCode.A)){
            this.gameObject.transform.position += new Vector3(-moveSpeed, 0f, 0f);
        }

        // 左右移動
        if (Input.GetKey(KeyCode.D)){
            this.gameObject.transform.position += new Vector3(moveSpeed, 0f, 0f);
        }


    }
}
