
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSizeController : MonoBehaviour
{
    void Awake()
    {
        // 1. ��ȡ����ϵͳ���
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // 2. ��ȡ Size over Lifetime ģ��
        var sizeOverLifetime = ps.sizeOverLifetime;

        // 3. ���ø�ģ��
        sizeOverLifetime.enabled = true;

        // 4. ����һ���µĶ�������
        AnimationCurve curve = new AnimationCurve();

        // 5. Ϊ������ӹؼ�֡ (Keyframe)
        //    Keyframe(ʱ��, ֵ, ������, ������)
        //    ����(tangent)�����������ڹؼ�֡������б�ʣ��Ӷ����컺��Ч��
        //    һ����ֵ�ĳ����߻�������һ��ʼ�½��úܿ�

        // ������һ���ؼ�֡����ʱ��0ʱ��ֵΪ1
        Keyframe startKey = new Keyframe(0.0f, 1.0f);
        startKey.outTangent = -2.0f; // �ؼ�������һ�����ĳ����ߣ������߿�ʼʱ�ǳ����ͣ�������С��

        // �����ڶ����ؼ�֡����ʱ��1ʱ��ֵΪ0
        Keyframe endKey = new Keyframe(1.0f, 0.0f);
        endKey.inTangent = 0.0f; // �ؼ�������һ��ƽ���������ߣ������߽���ʱ���ƽ����������С��

        // ���ؼ�֡��ӵ�������
        curve.AddKey(startKey);
        curve.AddKey(endKey);

        // (��ѡ) Ϊ�������߸�ƽ����Unity�ṩ�˷������Զ�ƽ������
        // ��Ҳ����������Ĵ����������ֶ��������ߣ�����Ч��
        // for (int i = 0; i < curve.keys.Length; i++)
        // {
        //     curve.SmoothTangents(i, 0.0f);
        // }

        // 6. �����Ǵ���������Ӧ�õ� Size over Lifetime ģ��
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);

        Debug.Log("����ϵͳ�Ĵ�С������ͨ���������Ϊ '�ȿ����' ��Ч����");
    }
}