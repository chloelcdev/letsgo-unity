using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Utilities
{
    // thank god to https://forum.unity.com/threads/1-15-1-assetreference-not-allow-loadassetasync-twice.959910/#post-6338337 for making this not break
    public static AsyncOperationHandle<TObject> LoadAssetAsyncIfValid<TObject>(this AssetReference assetReference, Action<AsyncOperationHandle<TObject>> OnLoaded = null)
    {
        AsyncOperationHandle op = assetReference.OperationHandle;
        AsyncOperationHandle<TObject> handle = default(AsyncOperationHandle<TObject>);

        if (assetReference.IsValid() && op.IsValid())
        {
            // Increase the usage counter & Convert.
            Addressables.ResourceManager.Acquire(op);
            handle = op.Convert<TObject>();
        }
        else
        {
            handle = assetReference.LoadAssetAsync<TObject>();
        }

        return handle;
    }

    public static void SetXVelocity(this Rigidbody2D rb, float newValue)
    {
        Vector2 currentVelocity = rb.velocity;

        currentVelocity.x = newValue;

        rb.velocity = currentVelocity;
    }

    public static void SetYVelocity(this Rigidbody2D rb, float newValue)
    {
        Vector2 currentVelocity = rb.velocity;

        currentVelocity.y = newValue;

        rb.velocity = currentVelocity;
    }

    public static void SetAlpha(this SpriteRenderer renderer, float newValue)
    {
        Color currentColor = renderer.color;
        currentColor.a = newValue;
        renderer.color = currentColor;
    }
    
    public static void SetAlpha(this UnityEngine.UI.Image image, float newValue)
    {
        Color c = image.color;
        c.a = newValue;
        image.color = c;
    }
    
    public static void AddCappedVelocity(this Rigidbody2D rb, Vector2 force, float maxPotential)
    {
        float prevSpeed = rb.velocity.magnitude;
        Vector2 newVel = rb.velocity + force;
        float newSpeed = newVel.magnitude;

        if (newSpeed > maxPotential && newSpeed > prevSpeed)
            newVel = rb.velocity * (prevSpeed / newSpeed);

        rb.velocity = newVel;
    }

    #region Hitbox Checking

    public static Collider2D CheckHitbox(this Transform hitbox, LayerMask mask)
    {
        return Physics2D.OverlapBox(hitbox.position, hitbox.localScale, 0, mask);
    }

    public static T CheckHitbox<T>(this Transform hitbox, LayerMask mask) where T : UnityEngine.Object
    {
        Collider2D areaCheck = hitbox.CheckHitbox(mask);

        if (areaCheck == null || areaCheck.attachedRigidbody == null)
            return null;

        return areaCheck.attachedRigidbody.GetComponent<T>();
    }

    public static T CheckHitbox<T>(this Transform hitbox, params string[] layerNames) where T : UnityEngine.Object
    {
        return hitbox.CheckHitbox<T>(LayerMask.GetMask(layerNames));
    }

    public static Collider2D CheckHitbox(this Transform hitbox, params string[] layerNames)
    {
        return hitbox.CheckHitbox( LayerMask.GetMask(layerNames) );
    }


    public static Collider2D[] CheckHitboxAll(this Transform hitbox, LayerMask mask)
    {
        return Physics2D.OverlapBoxAll(hitbox.position, hitbox.localScale, 0, mask);
    }

    public static T[] CheckHitboxAll<T>(this Transform hitbox, LayerMask mask) where T : UnityEngine.Object
    {
        Collider2D[] areaCheck = hitbox.CheckHitboxAll(mask);

        if (areaCheck == null)
            return null;

        List<T> validHits = new List<T>();
        foreach (var collider in areaCheck)
        {
            if (collider.attachedRigidbody != null)
            {
                T comp = collider.attachedRigidbody.GetComponent<T>();
                if (comp != null) validHits.Add(comp);
            }
        }

        return validHits.ToArray();
    }

    public static T[] CheckHitboxAll<T>(this Transform hitbox, params string[] layerNames) where T : UnityEngine.Object
    {
        return hitbox.CheckHitboxAll<T>(LayerMask.GetMask(layerNames));
    }

    public static Collider2D[] CheckHitboxAll(this Transform hitbox, params string[] layerNames)
    {
        return hitbox.CheckHitboxAll(LayerMask.GetMask(layerNames));
    }

    #endregion

    #region Range Checking

    public static Collider2D CheckRange(this Vector3 position, float radius, LayerMask mask)
    {
        return Physics2D.OverlapCircle(position, radius, mask);
    }

    public static T CheckRange<T>(this Vector3 position, float radius, LayerMask mask) where T : UnityEngine.Object
    {
        Collider2D areaCheck = CheckRange(position, radius, mask);

        if (areaCheck == null || areaCheck.attachedRigidbody == null)
            return null;

        return areaCheck.attachedRigidbody.GetComponent<T>();
    }

    public static Collider2D CheckRange(this Vector3 position, float radius, params string[] layerNames)
    {
        return CheckRange(position, radius, LayerMask.GetMask(layerNames) );
    }

    public static T CheckRange<T>(this Vector3 position, float radius, params string[] layerNames) where T : UnityEngine.Object
    {
        return CheckRange<T>(position, radius, LayerMask.GetMask(layerNames) );
    }

    #endregion

    public static T GetRandom<T>(this List<T> values)
    {
        if (values.Count > 0)
            return values[UnityEngine.Random.Range(0, values.Count)];

        return default(T);
    }
    public static void SetSelectOnDown(this UnityEngine.UI.Button button, UnityEngine.UI.Selectable selectable)
    {
        var nav = button.navigation;
        nav.selectOnDown = selectable;
        button.navigation = nav;
    }

    public static void SetSelectOnRight(this UnityEngine.UI.Button button, UnityEngine.UI.Selectable selectable)
    {
        var nav = button.navigation;
        nav.selectOnRight = selectable;
        button.navigation = nav;
    }

    public static void SetSelectOnUp(this UnityEngine.UI.Button button, UnityEngine.UI.Selectable selectable)
    {
        var nav = button.navigation;
        nav.selectOnUp = selectable;
        button.navigation = nav;
    }

    public static void SetSelectOnLeft(this UnityEngine.UI.Button button, UnityEngine.UI.Selectable selectable)
    {
        var nav = button.navigation;
        nav.selectOnLeft = selectable;
        button.navigation = nav;
    }

    public static void AccountForButtonRemoval(this UnityEngine.UI.Button button, params UnityEngine.UI.Button[] buttons)
    {
        foreach (var thisBtn in buttons)
        {
            if (thisBtn.navigation.selectOnDown == button)
                thisBtn.SetSelectOnDown(button.navigation.selectOnDown);

            if (thisBtn.navigation.selectOnRight == button)
                thisBtn.SetSelectOnRight(button.navigation.selectOnRight);

            if (thisBtn.navigation.selectOnUp == button)
                thisBtn.SetSelectOnUp(button.navigation.selectOnUp);

            if (thisBtn.navigation.selectOnLeft == button)
                thisBtn.SetSelectOnLeft(button.navigation.selectOnLeft);
        }
    }

    public static bool HasFinished(this Animancer.AnimancerState state)
    {
        if (state.NormalizedTime < 1)
            return false;

        return true;
    }

    public static bool IsStillPlaying(this Animancer.AnimancerComponent animancer, AnimationClip clip)
    {
        if (animancer.IsPlaying(clip) && !animancer.States.Current.HasFinished())
            return true;

        return false;
    }

    public static Vector3 Round(this Vector3 vec, int decimalPlaces = 0) 
    {
        Vector3 newVector = vec;
        newVector.x = (float)System.Math.Round(vec.x, decimalPlaces);
        newVector.y = (float)System.Math.Round(vec.y, decimalPlaces);
        newVector.z = (float)System.Math.Round(vec.z, decimalPlaces);

        return newVector;
    }

    public static void DrawX(Vector2 point, Color color = default(Color), float xSize = 0.2f)
    {
        Debug.DrawLine(point + Vector2.up * xSize + Vector2.right * xSize, point - Vector2.up * xSize - Vector2.right * xSize, color, 2);
        Debug.DrawLine(point + Vector2.up * xSize - Vector2.right * xSize, point - Vector2.up * xSize + Vector2.right * xSize, color, 2);
    }

    public static void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }

    public static Vector2 RotateAround(this Vector2 pos, Vector2 pivotSpot, float angle)
    {
        Vector2 rotatedPoint = pos - pivotSpot;

        float cosAng = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sinAng = Mathf.Sin(Mathf.Deg2Rad * angle);

        float newX = (rotatedPoint.x * cosAng) - (rotatedPoint.y * sinAng);
        float newY = (rotatedPoint.y * cosAng) + (rotatedPoint.x * sinAng);

        rotatedPoint = new Vector2(newX, newY);

        return rotatedPoint + pivotSpot;
    }

    public static Vector2 RealSize(this CapsuleCollider2D col)
    {
        return new Vector2(col.size.x, col.size.y + col.size.x);
    }

    public static bool MatchesLayerMask(this Collider2D collider, LayerMask layerMask)
    {
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static bool IsComplete_Safe(this Tween tween)
    {
        return tween == null || !tween.IsActive() || tween.IsComplete();
    }

    #region Basic Draws

    public static void DrawCircle(Vector2 pos, float radius, int segments, Color c, float duration = 1)
    {
        float angle = 0f;
        Vector2 lastPoint = Vector2.zero;
        Vector2 thisPoint = Vector2.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            if (i > 0)
            {
                Debug.DrawLine(lastPoint + pos, thisPoint + pos, c, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }

    public static void DrawBox(Vector2 pos, float angle, Vector2 scale, Color c, float duration = 1)
    {
        var point1 = pos + scale / 2;
        var point3 = pos - scale / 2;

        var point2 = point1;
        point2.x = point3.x;

        var point4 = point3;
        point4.x = point1.x;

        point1 = point1.RotateAround(pos, angle);
        point2 = point2.RotateAround(pos, angle);
        point3 = point3.RotateAround(pos, angle);
        point4 = point4.RotateAround(pos, angle);

        Debug.DrawLine(point1, point2, c, duration);
        Debug.DrawLine(point2, point3, c, duration);
        Debug.DrawLine(point3, point4, c, duration);
        Debug.DrawLine(point4, point1, c, duration);

        // optional axis display
        //Debug.DrawRay(m.GetPosition(), m.GetForward(), Color.magenta);
        //Debug.DrawRay(m.GetPosition(), m.GetUp(), Color.yellow);
        //Debug.DrawRay(m.GetPosition(), m.GetRight(), Color.red);
    }

    #endregion
}

public class Arc
{
    Vector2 origin;
    Vector2 arcMax;

    public Arc(Vector3 _origin, Vector3 _arcMax)
    {
        origin = _origin;
        arcMax = _arcMax;
    }

    public float GetY(float currentX)
    {
        float a = (origin.y - arcMax.y) / Mathf.Pow(origin.x - arcMax.x, 2);
        return a * Mathf.Pow(currentX - arcMax.x, 2) + arcMax.y;
    }

    public Vector2 GetPoint(float lerp)
    {
        float x = Mathf.LerpUnclamped(origin.x, arcMax.x, 2 * lerp);
        float y = GetY(x);
        return new Vector2(x, y);
    }

    public RaycastHit2D Raycast(float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal_single(detail, PhysicsCastType.Ray, fromLerp, toLerp, new ExtraCastParams(_mask: layerMask));
    }

    public RaycastHit2D[] RaycastAll(float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal(false, detail, PhysicsCastType.Ray, fromLerp, toLerp, new ExtraCastParams(_mask: layerMask));
    }


    public RaycastHit2D BoxCast(Vector2 size, float angle, float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal_single(detail, PhysicsCastType.Box, fromLerp, toLerp, new ExtraCastParams(
            _boxOrCapsuleCastSize: size,
            _boxOrCapsuleCastAngle: angle,
            _mask: layerMask
            ));
    }

    public RaycastHit2D[] BoxCastAll(Vector2 size, float angle, float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal(false, detail, PhysicsCastType.Box, fromLerp, toLerp, new ExtraCastParams(
            _boxOrCapsuleCastSize: size,
            _boxOrCapsuleCastAngle: angle,
            _mask: layerMask
            ));
    }


    public RaycastHit2D CapsuleCast(Vector2 size, float angle, CapsuleDirection2D capsuleDirection, float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal_single(detail, PhysicsCastType.Capsule, fromLerp, toLerp, new ExtraCastParams(
            _boxOrCapsuleCastSize: size,
            _boxOrCapsuleCastAngle: angle,
            _capsuleDirection: capsuleDirection,
            _mask: layerMask
            ));
    }

    public RaycastHit2D[] CapsuleCastAll(Vector2 size, float angle, CapsuleDirection2D capsuleDirection, float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal(false, detail, PhysicsCastType.Capsule, fromLerp, toLerp, new ExtraCastParams(
            _boxOrCapsuleCastSize: size,
            _boxOrCapsuleCastAngle: angle,
            _capsuleDirection: capsuleDirection,
            _mask: layerMask
            ));
    }


    public RaycastHit2D CircleCast(float radius, float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal_single(detail, PhysicsCastType.Circle, fromLerp, toLerp, new ExtraCastParams(
            _circleCastRadius: radius,
            _mask: layerMask
            ));
    }

    public RaycastHit2D[] CircleCastAll(float radius, float detail, float fromLerp = 0, float toLerp = 1, int layerMask = Physics2D.AllLayers)
    {
        return Raycast_internal(false, detail, PhysicsCastType.Circle, fromLerp, toLerp, new ExtraCastParams(
            _circleCastRadius: radius,
            _mask: layerMask
            ));
    }


    public void Draw(float detail, Color color, float duration, PhysicsCastType castType = PhysicsCastType.Ray, ExtraCastParams extraParams = default(ExtraCastParams))
    {
        switch (castType)
        {
            case PhysicsCastType.Box:
                DrawBoxes(detail, color, duration, extraParams);
                break;
            case PhysicsCastType.Capsule:
                DrawCapsules(detail, color, duration, extraParams);
                break;
            case PhysicsCastType.Circle:
                DrawCircles(detail, color, duration, extraParams);
                break;
            case PhysicsCastType.Ray:
                DrawRays(detail, color, duration);
                break;
            default:
                break;
        }
    }

    void DrawBoxes(float detail, Color color, float duration, ExtraCastParams extraParams)
    {
        float step = 1 / detail;

        for (float i = 0; i < 1; i += step)
        {
            Vector2 point1 = GetPoint(i);
            Vector2 point2 = GetPoint(i + step);

            float dist = Vector2.Distance(point2, point1);

            //Debug.Log("a");
            Utilities.DrawBox(point1, extraParams.castAngle, extraParams.castSize, color, duration);
            //GizmosExtended.DrawBoxCastBox(point1, extraParams.castSize / 2, Quaternion.Euler(0, 0, extraParams.castAngle), point2-point1, dist, color);
            //Debug.DrawRay(point1, point2 - point1, color, duration);
        }
    }

    void DrawCapsules(float detail, Color color, float duration, ExtraCastParams extraParams)
    {
        float step = 1 / detail;

        for (float i = 0; i < 1; i += step)
        {
            Vector2 point1 = GetPoint(i);
            Vector2 point2 = GetPoint(i + step);

            float radius = extraParams.capsuleDirection == CapsuleDirection2D.Vertical ? extraParams.castSize.x/2 : extraParams.castSize.y/2;

            Utilities.DrawBox(point1, extraParams.castAngle, extraParams.castSize, color, duration);

            Vector2 circlePosMod = Vector2.up * (extraParams.castSize.y / 2);

            Vector2 circle1Loc = point1 + circlePosMod;
            circle1Loc = circle1Loc.RotateAround(point1, extraParams.castAngle);
            Vector2 circle2Loc = point1 - circlePosMod;
            circle2Loc = circle2Loc.RotateAround(point1, extraParams.castAngle);

            Utilities.DrawCircle(circle1Loc, radius, 17, color, duration);
            Utilities.DrawCircle(circle2Loc, radius, 17, color, duration);
        }
    }

    void DrawCircles(float detail, Color color, float duration, ExtraCastParams extraParams)
    {
        float step = 1 / detail;

        for (float i = 0; i < 1; i += step)
        {
            Vector2 point1 = GetPoint(i);
            Vector2 point2 = GetPoint(i + step);

            Utilities.DrawCircle(point1, extraParams.castRadius, 8, color, duration);
        }
    }

    void DrawRays(float detail, Color color, float duration)
    {
        float step = 1 / detail;

        for (float i = 0; i < 1; i += step)
        {
            Vector2 point1 = GetPoint(i);
            Vector2 point2 = GetPoint(i + step);

            Debug.DrawRay(point1, point2 - point1, color, duration);
        }
    }

    #region Internal Raycasting

    RaycastHit2D Raycast_internal_single(float detail, PhysicsCastType castType, float fromLerp = 0, float toLerp = 1, ExtraCastParams extraParams = default(ExtraCastParams))
    {
        RaycastHit2D[] hits = Raycast_internal(true, detail, castType, fromLerp, toLerp, extraParams);

        if (hits.Length > 0)
            return hits[0];

        return default(RaycastHit2D);
    }

    RaycastHit2D[] Raycast_internal(bool returnFirst, float detail, PhysicsCastType castType, float fromLerp = 0, float toLerp = 1, ExtraCastParams extraParams = default(ExtraCastParams))
    {
        float step = (toLerp - fromLerp) / detail;

        RaycastHit2D hit = default(RaycastHit2D);

        float traveledDistance = 0;

        int stepsTaken = 1;
        int totalSteps = Mathf.RoundToInt(Mathf.Abs(toLerp - fromLerp) / step);

        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        for (float i = fromLerp; i < toLerp; i += step)
        {
            Vector2 point1 = GetPoint(i);
            Vector2 point2 = GetPoint(i + step);

            float dist = Vector2.Distance(point1, point2);
            traveledDistance += dist;

            Vector2 dir = (point2 - point1);

            switch (castType)
            {
                case PhysicsCastType.Box:
                        hit = Physics2D.BoxCast(point1, extraParams.castSize, extraParams.castAngle, dir, dist, extraParams.mask);
                    break;
                case PhysicsCastType.Capsule:
                        hit = Physics2D.CapsuleCast(point1, extraParams.castSize, extraParams.capsuleDirection, extraParams.castAngle, dir, dist, extraParams.mask);
                    break;
                case PhysicsCastType.Circle:
                        hit = Physics2D.CircleCast(point1, extraParams.castRadius, dir, dist, extraParams.mask);
                    break;
                case PhysicsCastType.Ray:
                        hit = Physics2D.Raycast(point1, dir, dist, extraParams.mask);
                    break;
                default:
                    break;
            }

            hit.distance = traveledDistance;
            hit.fraction = (stepsTaken + hit.fraction - 1) / totalSteps;

            if (hit.collider != null)
            {
                hits.Add(hit);

                if (returnFirst)
                    return hits.ToArray();
            }

            stepsTaken++;
        }

        Debug.Log("no hit ");
        return hits.ToArray();
    }

    public enum PhysicsCastType
    {
        Box,
        Capsule,
        Circle,
        Ray
    }

    [System.Serializable]
    public class ExtraCastParams
    {
        public float castRadius = 0;
        public float castAngle = 0;
        public Vector2 castSize = default(Vector2);
        public CapsuleDirection2D capsuleDirection = default(CapsuleDirection2D);
        public LayerMask mask = default(LayerMask);

        public ExtraCastParams(float _circleCastRadius = 0, 
            float _boxOrCapsuleCastAngle = 0, 
            Vector2 _boxOrCapsuleCastSize = default(Vector2), 
            CapsuleDirection2D _capsuleDirection = default(CapsuleDirection2D), 
            LayerMask _mask = default(LayerMask))
        {
            castRadius = _circleCastRadius;
            castAngle = _boxOrCapsuleCastAngle;
            castSize = _boxOrCapsuleCastSize;
            capsuleDirection = _capsuleDirection;
            mask = _mask;
        }
    }

    #endregion
}
