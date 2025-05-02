using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public static class SCR_ScreenShake
{
    //Screenshake base functionality from Thomas friday
    //https://www.youtube.com/watch?v=BQGTdRhGmE4

    /// <summary>
    /// A basic screenshake function that shakes the given GO around its starting point
    /// </summary>
    /// <param name="CameraOBJ">the GO that will be shaken</param>
    /// <param name="Duration">How long the Shaking will happen for</param>
    /// <returns></returns>
    public static IEnumerator ScreenShake(GameObject CameraOBJ, float Duration)
    {
        Vector3 startPos = CameraOBJ.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < Duration) 
        { 
            elapsedTime += Time.deltaTime;
            CameraOBJ.transform.position = startPos + Random.insideUnitSphere;
            yield return null;
            Debug.Log("SCREENSHAKING");
        }

        CameraOBJ.transform.position = startPos;
    }

    /// <summary>
    /// Screenshake that vibrates the CameraOBJ in local position only
    /// </summary>
    /// <param name="CameraOBJ">The GO that will be shaken</param>
    /// <param name="Duration">How long the Shaking will happen for</param>
    /// <param name="stregnth"> a strength multiplier that affects how Strong or violent the screen shake is</param>
    /// <returns></returns>
    public static IEnumerator ScreenshakeLocal(GameObject CameraOBJ, float Duration, float Strength)
    {
        Transform OriginalTransform = CameraOBJ.transform;
        Vector3 pos = CameraOBJ.transform.localPosition;
        Quaternion rot = CameraOBJ.transform.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < Duration) 
        {
            elapsedTime += Time.deltaTime;
            CameraOBJ.transform.localPosition = pos;

            CameraOBJ.transform.localPosition = CameraOBJ.transform.localPosition + Random.insideUnitSphere * Strength;

            yield return null;
        }
        Debug.Log(pos);

        CameraOBJ.transform.localPosition = pos;
    }
    /// <summary>
    /// Screenshake that vibrates the CameraOBJ in local position only
    /// </summary>
    /// <param name="CameraOBJ">The GO that will be shaken</param>
    /// <param name="Duration">How long the Shaking will happen for</param>
    /// <param name="curve"> A animation curve to modify the natural feel of the shake</param>
    /// <returns></returns>
    public static IEnumerator ScreenshakeLocalCurve(GameObject CameraOBJ, float Duration, AnimationCurve curve)
    {
        Transform OriginalTransform = CameraOBJ.transform;
        Vector3 pos = CameraOBJ.transform.localPosition;
        Quaternion rot = CameraOBJ.transform.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;
            CameraOBJ.transform.localPosition = pos;

            float multiplier = curve.Evaluate(elapsedTime/Duration);
            CameraOBJ.transform.localPosition = CameraOBJ.transform.localPosition + Random.insideUnitSphere * multiplier;

            yield return null;
        }
        Debug.Log(pos);

        CameraOBJ.transform.localPosition = pos;
    }

    /// <summary>
    /// Screenshake that vibrates the CameraOBJ in local position only
    /// </summary>
    /// <param name="CameraOBJ">The GO that will be shaken</param>
    /// <param name="Duration">How long the Shaking will happen for</param>
    /// <param name="curve"> A animation curve to modify the natural feel of the shake</param>
    /// <param name="stregnth"> a strength multiplier that affects how Strong or violent the screen shake is</param>
    /// <returns></returns>
    public static IEnumerator ScreenshakeLocalCurve(GameObject CameraOBJ, float Duration, AnimationCurve curve, float stregnth)
    {
        Transform OriginalTransform = CameraOBJ.transform;
        Vector3 pos = CameraOBJ.transform.localPosition;
        Quaternion rot = CameraOBJ.transform.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;
            CameraOBJ.transform.localPosition = pos;

            float multiplier = curve.Evaluate(elapsedTime / Duration);
            CameraOBJ.transform.localPosition = CameraOBJ.transform.localPosition + Random.insideUnitSphere * multiplier * stregnth;

            yield return null;
        }
        Debug.Log(pos);

        CameraOBJ.transform.localPosition = pos;
    }
}
