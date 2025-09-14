
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSizeController : MonoBehaviour
{
    void Awake()
    {
        // 1. 获取粒子系统组件
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // 2. 获取 Size over Lifetime 模块
        var sizeOverLifetime = ps.sizeOverLifetime;

        // 3. 启用该模块
        sizeOverLifetime.enabled = true;

        // 4. 创建一个新的动画曲线
        AnimationCurve curve = new AnimationCurve();

        // 5. 为曲线添加关键帧 (Keyframe)
        //    Keyframe(时间, 值, 入切线, 出切线)
        //    切线(tangent)控制了曲线在关键帧附近的斜率，从而创造缓动效果
        //    一个负值的出切线会让曲线一开始下降得很快

        // 创建第一个关键帧：在时间0时，值为1
        Keyframe startKey = new Keyframe(0.0f, 1.0f);
        startKey.outTangent = -2.0f; // 关键：设置一个负的出切线，让曲线开始时非常陡峭（快速缩小）

        // 创建第二个关键帧：在时间1时，值为0
        Keyframe endKey = new Keyframe(1.0f, 0.0f);
        endKey.inTangent = 0.0f; // 关键：设置一个平缓的入切线，让曲线结束时变得平缓（缓慢缩小）

        // 将关键帧添加到曲线中
        curve.AddKey(startKey);
        curve.AddKey(endKey);

        // (可选) 为了让曲线更平滑，Unity提供了方法来自动平滑切线
        // 你也可以用下面的代码来代替手动设置切线，看看效果
        // for (int i = 0; i < curve.keys.Length; i++)
        // {
        //     curve.SmoothTangents(i, 0.0f);
        // }

        // 6. 将我们创建的曲线应用到 Size over Lifetime 模块
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);

        Debug.Log("粒子系统的大小曲线已通过代码更新为 '先快后慢' 的效果！");
    }
}