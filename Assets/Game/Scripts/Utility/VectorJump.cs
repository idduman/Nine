using DG.Tweening;
using UnityEngine;

public static class VectorJump
{
    public static Tween DOVectorJump(this Transform transform, Vector3 endValue, Vector3 upVector, float jumpheight, float duration)
    {
        Vector3 from = transform.position;
        return DOTween.To(
            () => 0f,
            t => transform.position = CalcJumpVector(from, endValue, t, upVector, jumpheight),
            1f, duration
        );
    }

    private static Vector3 CalcJumpVector(Vector3 from, Vector3 to, float time, Vector3 upVector, float jumpheight)
    {
        float jumpVal = 4 * time - 4 * time * time;
        Vector3 moveVec = Vector3.LerpUnclamped(from, to, time);
        return moveVec + jumpVal * jumpheight * upVector.normalized;
    }
}
