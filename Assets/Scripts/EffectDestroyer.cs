using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    // 在动画结束时被 Animator Event 调用
    public void DestroyEffect()
    {
        Destroy(gameObject);
    }
}