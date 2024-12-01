using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float distance = 500;
    public float speed = 500;
    private int _direction;
    private float _start;
  
    public void SetDirection(int dir) { _direction = dir; _start = this.transform.position.x; }

    void Update()
    {
        this.transform.position += new Vector3(speed * Time.deltaTime * _direction, 0, 0);
        if (_start - this.transform.position.x > distance)
            Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(gameObject);
        //if (collision.gameObject.CompareTag("Breakable")) 
        //{
        //    Destroy(collision.gameObject);
        //}
    }
}
