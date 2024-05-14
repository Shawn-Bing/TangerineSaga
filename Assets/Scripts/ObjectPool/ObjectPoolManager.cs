using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    // 创建prefabs List
    // TODO：引擎中赋值
    public List<GameObject> poolPrefabs;

    // 创建对象池List
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    private void Start()
    {
        CreatePool();
    }

    /// <summary>
    /// 生成对象池
    /// </summary>
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            // 生成父物体管理生成对象
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                // 对象池包含 生成、获取、释放、摧毁等步骤
                () => Instantiate(item, parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );

            poolEffectList.Add(newPool);
        }
    }

    #region 注册对象池事件
    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
    }

    private void OnParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        //FIXME:补全特效类型
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticleEffectType.Leaves_1 => poolEffectList[0],
            ParticleEffectType.Leaves_2 => poolEffectList[1],
            _ => null,
        };

        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(Settings.pariticalPoolObjectReleaseTime);
        pool.Release(obj);
    }

    #endregion
}
