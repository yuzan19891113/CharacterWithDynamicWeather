
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accumulateSnow : MonoBehaviour
{//脚本挂在粒子对象上
    public List<ParticleCollisionEvent> partcollision;  //储存一帧内的所有碰撞信息
    public ParticleSystem _part;  //这个粒子系统
    public List<GameObject> Snow = new List<GameObject>();  //储存所有生成的对象
    public GameObject Snows;//雪的预制体
                            // Use this for initialization
    void Start()
    {
        partcollision = new List<ParticleCollisionEvent>();
        _part = transform.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnParticleCollision(GameObject other)
    {
        _part.GetCollisionEvents(other, partcollision);  //把数据存入这个碰撞信息集合里，自动就添加完了//下一帧清空
        Quaternion q = Quaternion.FromToRotation(Snows.transform.forward, -partcollision[0].normal);//计算这个生成物体的旋转。
        GameObject s = GameObject.Instantiate(Snows, partcollision[0].intersection, q);  //生成创建
        Snow.Add(s);
        // Debug.Log(partcollision[0].intersection);
    }
}
