using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtQuaternion
{
    public enum TurnType
    {
        X,
        Y,
        Z,
        ALL,
    }

    /// <summary>
    /// formula behind smoothDamp
    /// </summary>
    public static float OwnSmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        float num4 = current - target;
        float num5 = target;
        float num6 = maxSpeed * smoothTime;
        num4 = Mathf.Clamp(num4, -num6, num6);
        target = current - num4;
        float num7 = (currentVelocity + num * num4) * deltaTime;
        currentVelocity = (currentVelocity - num * num7) * num3;
        float num8 = target + (num4 + num7) * num3;
        if (num5 - current > 0f == num8 > num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }

    /// <summary>
    /// Is a Quaternion.Slerp is over ?
    /// </summary>
    public static bool IsQuaternionSlerpOver(Quaternion objectInRotation, Quaternion targetRotation, float marginInDegree = 1)
    {
        if (!IsAngleCloseToOtherByAmount(objectInRotation.x, targetRotation.x, marginInDegree))
            return (false);
        if (!IsAngleCloseToOtherByAmount(objectInRotation.y, targetRotation.y, marginInDegree))
            return (false);
        if (!IsAngleCloseToOtherByAmount(objectInRotation.z, targetRotation.z, marginInDegree))
            return (false);
        return (true);
    }

    /// <summary>
    /// return if we are right or left from a vector. 1: right, -1: left, 0: forward
    /// </summary>
    /// <param name="forwardDir"></param>
    /// <param name="upDir">up reference of the forward dir</param>
    /// <param name="toGoDir">target direction to test</param>
    public static int IsRightOrLeft(Vector3 forwardDir, Vector3 upDir, Vector3 toGoDir, Vector3 debugPos, ref float dotLeft, ref float dotRight)
    {
        Vector3 left = CrossProduct(forwardDir, upDir);
        Vector3 right = -left;

        //Debug.DrawRay(debugPos, left, Color.magenta, 5f);
        //Debug.DrawRay(debugPos, right, Color.magenta, 5f);


        dotRight = DotProduct(right, toGoDir);
        dotLeft = DotProduct(left, toGoDir);
        //Debug.Log("left: " + dotLeft + ", right: " + dotRight);
        if (dotRight > 0)
        {
            //Debug.Log("go right");
            return (1);
        }
        else if (dotLeft > 0)
        {
            //Debug.Log("go left");
            return (-1);
        }
        //Debug.Log("go pls");
        return (0);
    }

    /// <summary>
    /// clamp a quaternion around one axis
    /// </summary>
    /// <param name="q"></param>
    /// <param name="minX"></param>
    /// <param name="maxX"></param>
    /// <returns></returns>
    public static Quaternion ClampRotationAroundXAxis(Quaternion q, float minX, float maxX)
    {
        if (q.w == 0)
            return (q);

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
    
    public static Quaternion ClampRotationAroundYAxis(Quaternion q, float minY, float maxY)
    {
        if (q.w == 0)
            return (q);

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

        angleY = Mathf.Clamp(angleY, minY, maxY);

        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        return q;
    }

    public static Quaternion ClampRotationAroundZAxis(Quaternion q, float minZ, float maxZ)
    {
        if (q.w == 0)
            return (q);

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

        angleZ = Mathf.Clamp(angleZ, minZ, maxZ);

        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }

    /// <summary>
    /// https://gamedev.stackexchange.com/questions/167389/unity-smooth-local-rotation-around-one-axis-oriented-toward-a-target/167395#167395
    /// 
    /// Vector3 relativeDirection = mainReferenceObjectDirection.right * dirInput.x + mainReferenceObjectDirection.forward * dirInput.y;
    /// Vector3 up = objectToRotate.up;
    /// Quaternion desiredOrientation = TurretLookRotation(relativeDirection, up);
    ///objectToRotate.rotation = Quaternion.RotateTowards(
    ///                         objectToRotate.rotation,
    ///                         desiredOrientation,
    ///                         turnRate* Time.deltaTime
    ///                        );
    /// </summary>
    public static Quaternion TurretLookRotation(Vector3 approximateForward, Vector3 exactUp)
    {
        Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -approximateForward);
        Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

        return rotateZToUp * rotateYToZ;
    }
    public static Quaternion SmoothTurretLookRotation(Vector3 approximateForward, Vector3 exactUp,
        Quaternion objCurrentRotation, float maxDegreesPerSecond)
    {
        Quaternion desiredOrientation = TurretLookRotation(approximateForward, exactUp);
        Quaternion smoothOrientation = Quaternion.RotateTowards(
                                    objCurrentRotation,
                                    desiredOrientation,
                                    maxDegreesPerSecond * Time.deltaTime
                                 );
        return (smoothOrientation);
    }

    /*
    public static Quaternion ClampAroundXAxis(Quaternion q, float minY, float maxY)
    {
        Vector3 eulerAngles = q.eulerAngles;

        eulerAngles.x = Mathf.Clamp(eulerAngles.x, minY, maxY);

        return Quaternion.Euler(eulerAngles);
    }
    public static Quaternion ClampAroundYAxis(Quaternion q, float minX, float maxX)
    {
        Vector3 eulerAngles = q.eulerAngles;

        eulerAngles.y = Mathf.Clamp(eulerAngles.y, minX, maxX);

        return Quaternion.Euler(eulerAngles);
    }
    */

    public static bool IsCloseYToClampAmount(Quaternion q, float minY, float maxY, float margin = 2)
    {
        if (q.w == 0)
            return (true);

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

        if (ExtUtilityFunction.IsClose(angleY, minY, margin)
            || ExtUtilityFunction.IsClose(angleY, maxY, margin))
        {
            return (true);
        }
        return (false);
    }
    public static bool IsCloseZToClampAmount(Quaternion q, float minZ, float maxZ, float margin = 2)
    {
        if (q.w == 0)
            return (true);

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

        if (ExtUtilityFunction.IsClose(angleZ, minZ, margin)
            || ExtUtilityFunction.IsClose(angleZ, maxZ, margin))
        {
            return (true);
        }
        return (false);
    }
    public static bool IsCloseXToClampAmount(Quaternion q, float minX, float maxX, float margin = 2)
    {
        if (q.w == 0)
            return (true);

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        if (ExtUtilityFunction.IsClose(angleX, minX, margin)
            || ExtUtilityFunction.IsClose(angleX, maxX, margin))
        {
            return (true);
        }
        return (false);
    }

    public static Quaternion DirObject(this Quaternion rotation, Vector3 dir, float turnRate, TurnType typeRotation = TurnType.Z)
    {
        float heading = Mathf.Atan2(-dir.x * turnRate * Time.deltaTime, dir.y * turnRate * Time.deltaTime);

        Quaternion _targetRotation = Quaternion.identity;

        float x = (typeRotation == TurnType.X) ? heading * 1 * Mathf.Rad2Deg : 0;
        float y = (typeRotation == TurnType.Y) ? heading * -1 * Mathf.Rad2Deg : 0;
        float z = (typeRotation == TurnType.Z) ? heading * -1 * Mathf.Rad2Deg : 0;

        _targetRotation = Quaternion.Euler(x, y, z);
        rotation = Quaternion.RotateTowards(rotation, _targetRotation, turnRate * Time.deltaTime);
        return (rotation);
    }


    /// <summary>
    /// rotate smoothly selon 2 axe
    /// </summary>
	public static Quaternion DirObject(this Quaternion rotation, float horizMove, float vertiMove, float turnRate, out Quaternion _targetRotation, TurnType typeRotation = TurnType.Z)
    {
        float heading = Mathf.Atan2(horizMove * turnRate * Time.deltaTime, -vertiMove * turnRate * Time.deltaTime);

        //Quaternion _targetRotation = Quaternion.identity;

        float x = (typeRotation == TurnType.X) ? heading * 1 * Mathf.Rad2Deg : 0;
        float y = (typeRotation == TurnType.Y) ? heading * -1 * Mathf.Rad2Deg : 0;
        float z = (typeRotation == TurnType.Z) ? heading * -1 * Mathf.Rad2Deg : 0;

        _targetRotation = Quaternion.Euler(x, y, z);
        rotation = Quaternion.RotateTowards(rotation, _targetRotation, turnRate * Time.deltaTime);
        return (rotation);
    }

    public static Vector3 DirLocalObject(Vector3 rotation, Vector3 dirToGo, float turnRate, TurnType typeRotation = TurnType.Z)
    {
        Vector3 returnRotation = rotation;
        float x = returnRotation.x;
        float y = returnRotation.y;
        float z = returnRotation.z;

        //Debug.Log("Y current: " + y + ", y to go: " + dirToGo.y);



        x = (typeRotation == TurnType.X || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.x, dirToGo.x, Time.deltaTime * turnRate) : x;
        y = (typeRotation == TurnType.Y || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.y, dirToGo.y, Time.deltaTime * turnRate) : y;
        z = (typeRotation == TurnType.Z || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.z, dirToGo.z, Time.deltaTime * turnRate) : z;

        //= Vector3.Lerp(rotation, dirToGo, Time.deltaTime * turnRate);
        return (new Vector3(x, y, z));
    }

    /// <summary>
    /// rotate un quaternion selon un vectir directeur
    /// use: transform.rotation.LookAtDir((transform.position - target.transform.position) * -1);
    /// </summary>
    public static Quaternion LookAtDir(Vector3 dir)
    {
        Quaternion rotation = Quaternion.LookRotation(dir * -1);
        return (rotation);
    }

    /// <summary>
    /// dirWithRightLeft: vecteur ou il faut tester le vecteur droite/gauche
    /// dirReference: vecteur qui doit être plus proche de l'un ou de l'autre..
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetTheGoodRightAngleClosest(Vector3 dirLeftRight, Vector3 dirReference, float angleMargin = 10f)
    {
        if (dirReference == Vector3.zero)
        {
            Debug.Log("bug zero ?");
            return (Vector3.zero);
        }
        Vector3 dirRight = CrossProduct(dirLeftRight, Vector3.forward);
        Vector3 dirLeft = -CrossProduct(dirLeftRight, Vector3.forward);

        float angleRef = GetAngleFromVector2(dirReference);
        float angleLeftRight = GetAngleFromVector2(dirLeftRight);
        float angleRight = GetAngleFromVector2(dirRight);
        float angleLeft = GetAngleFromVector2(dirLeft);

        float diffAngleLRRef;
        bool isCLose = IsAngleCloseToOtherByAmount(angleRef, angleLeftRight, angleMargin, out diffAngleLRRef);
        float diffAngleRight;
        IsAngleCloseToOtherByAmount(angleRef, angleRight, angleMargin, out diffAngleRight);
        float diffAngleLeft;
        IsAngleCloseToOtherByAmount(angleRef, angleLeft, angleMargin, out diffAngleLeft);

        if (isCLose)
        {
            return (dirLeftRight);
        }
        else if (diffAngleRight < diffAngleLeft)
        {
            return (-dirLeft);
        }
        else if (diffAngleRight > diffAngleLeft)
        {
            return (-dirRight);
        }
        Debug.LogWarning("on a pas trouvé ??");
        return (Vector3.zero);
    }

    public static Vector3 GetTheGoodRightAngleClosestNoClose(Vector3 dirLeftRight, Vector3 dirReference, float angleMargin, out int right)
    {
        if (dirReference == Vector3.zero)
        {
            
            right = 0;
            return (Vector3.zero);
        }
        Vector3 dirRight = CrossProduct(dirLeftRight, Vector3.forward);
        Vector3 dirLeft = -CrossProduct(dirLeftRight, Vector3.forward);

        float angleRef = GetAngleFromVector2(dirReference);

        float angleRight = GetAngleFromVector2(dirRight);
        float angleLeft = GetAngleFromVector2(dirLeft);

        float diffAngleRight;
        IsAngleCloseToOtherByAmount(angleRef, angleRight, angleMargin, out diffAngleRight);
        float diffAngleLeft;
        IsAngleCloseToOtherByAmount(angleRef, angleLeft, angleMargin, out diffAngleLeft);

        if (diffAngleRight < diffAngleLeft)
        {
            right = -1;
            return (-dirLeft);
        }
        else
        {
            right = 1;
            return (-dirRight);
        }
    }

    //Gets an XY direction of magnitude from a radian angle relative to the x axis
    //Simple version
    public static Vector3 GetXYDirection(float angle, float magnitude)
    {
        return (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * magnitude);
    }

    /// <summary>
    /// get mirror of a vector, according to a normal
    /// </summary>
    /// <param name="point">Vector 1</param>
    /// <param name="normal">normal</param>
    /// <returns>vector mirror to 1 (reference= normal)</returns>
    public static Vector3 ReflectionOverPlane(Vector3 point, Vector3 normal)
    {
        return point - 2 * normal * Vector3.Dot(point, normal) / Vector3.Dot(normal, normal);
    }

    /// <summary>
    /// prend un quaternion en parametre, et retourn une direction selon un repère
    /// </summary>
    /// <param name="quat">rotation d'un transform</param>
    /// <param name="up">Vector3.up</param>
    /// <returns>direction du quaternion</returns>
    public static Vector3 QuaternionToDir(Quaternion quat, Vector3 up)
    {
        return ((quat * up).normalized);
    }

    /// <summary>
    /// Dot product de 2 vecteur, retourne négatif si l'angle > 90°, 0 si angle = 90, positif si moin de 90
    /// </summary>
    /// <param name="a">vecteur A</param>
    /// <param name="b">vecteur B</param>
    /// <returns>retourne négatif si l'angle > 90°</returns>
    public static float DotProduct(Vector3 a, Vector3 b)
    {
        return (Vector3.Dot(a, b));
    }

    /// <summary>
    /// retourne le vecteur de droite au vecteur A, selon l'axe Z
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 CrossProduct(Vector3 a, Vector3 z)
    {
        return (Vector3.Cross(a, z));
    }

    public static Vector3 GetMiddleOf2Vector(Vector3 a, Vector3 b)
    {
        return ((a + b).normalized);
    }

    public static Vector3 GetMiddleOfXVector(ContactPoint[] arrayVect)
    {
        Vector3[] arrayTmp = new Vector3[arrayVect.Length];

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < arrayVect.Length; i++)
        {
            arrayTmp[i] = arrayVect[i].normal;
        }
        return (GetMiddleOfXVector(arrayTmp));
    }

    public static Vector3 GetMiddleOfXVector(Vector3[] arrayVect)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < arrayVect.Length; i++)
        {
            if (ExtUtilityFunction.IsNullVector(arrayVect[i]))
                continue;

            sum += arrayVect[i];
        }
        return ((sum).normalized);
    }

    /// <summary>
    /// return the middle of X points
    /// </summary>
    public static Vector3 GetMiddleOfXPoint(Vector3[] arrayVect, bool barycenter = true)
    {
        if (arrayVect.Length == 0)
            return (ExtUtilityFunction.GetNullVector());

        if (!barycenter)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < arrayVect.Length; i++)
            {
                sum += arrayVect[i];
            }
            return (sum / arrayVect.Length);
        }
        else
        {
            if (arrayVect.Length == 1)
                return (arrayVect[0]);
            
            float xMin = arrayVect[0].x;
            float yMin = arrayVect[0].y;
            float zMin = arrayVect[0].z;
            float xMax = arrayVect[0].x;
            float yMax = arrayVect[0].y;
            float zMax = arrayVect[0].z;

            for (int i = 1; i < arrayVect.Length; i++)
            {
                if (arrayVect[i].x < xMin)
                    xMin = arrayVect[i].x;
                if (arrayVect[i].x > xMax)
                    xMax = arrayVect[i].x;

                if (arrayVect[i].y < yMin)
                    yMin = arrayVect[i].y;
                if (arrayVect[i].y > yMax)
                    yMax = arrayVect[i].y;

                if (arrayVect[i].z < zMin)
                    zMin = arrayVect[i].z;
                if (arrayVect[i].z > zMax)
                    zMax = arrayVect[i].z;
            }
            Vector3 lastMiddle = new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2, (zMin + zMax) / 2);
            return (lastMiddle);
        }
    }
    /// <summary>
    /// get la bisection de 2 vecteur
    /// </summary>
    public static Vector3 GetbisectionOf2Vector(Vector3 a, Vector3 b)
    {
        return ((a + b) * 0.5f);
    }

    /// <summary>
    /// prend un vecteur2 et retourne l'angle x, y en degré
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static float GetAngleFromVector2(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
        //float signed_angle = angle * sign;                                  // angle in [-179,180]
        float angle360 = (angle + 360) % 360;                       // angle in [0,360]
        return (angle360);

        //return (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    public static Vector3 ProjectVectorIntoPlane(Vector3 relativeDirection, Vector3 normalPlane)
    {
        //Projection of a vector on a plane and matrix of the projection.
        //http://users.telenet.be/jci/math/rmk.htm

        Vector3 Pprime = Vector3.Project(relativeDirection, normalPlane);
        Vector3 relativeProjeted = relativeDirection - Pprime;
        return (relativeProjeted);
    }

    /// <summary>
    /// return an angle from a vector
    /// </summary>
    public static float GetAngleFromVector3(Vector3 dir, Vector3 reference)
    {
        float angle = Vector3.Angle(dir, reference);
        return (angle);
    }

    /// <summary>
    /// check la différence d'angle entre les 2 vecteurs
    /// </summary>
    public static float GetDiffAngleBetween2Vectors(Vector2 dir1, Vector2 dir2)
    {
        float angle1 = GetAngleFromVector2(dir1);
        float angle2 = GetAngleFromVector2(dir2);

        float diffAngle;
        IsAngleCloseToOtherByAmount(angle1, angle2, 10f, out diffAngle);
        return (diffAngle);
    }

    /// <summary>
    /// prend un angle A, B, en 360 format, et test si les 2 angles sont inférieur à différence (180, 190, 20 -> true, 180, 210, 20 -> false)
    /// </summary>
    /// <param name="angleReference">angle A</param>
    /// <param name="angleToTest">angle B</param>
    /// <param name="differenceAngle">différence d'angle accepté</param>
    /// <returns></returns>
    public static bool IsAngleCloseToOtherByAmount(float angleReference, float angleToTest, float differenceAngle, out float diff)
    {
        if (angleReference < 0 || angleReference > 360 ||
            angleToTest < 0 || angleToTest > 360)
        {
            Debug.LogError("angle non valide: " + angleReference + ", " + angleToTest);
        }

        diff = 180 - Mathf.Abs(Mathf.Abs(angleReference - angleToTest) - 180);

        //diff = Mathf.Abs(angleReference - angleToTest);        

        if (diff <= differenceAngle)
            return (true);
        return (false);
    }
    public static bool IsAngleCloseToOtherByAmount(float angleReference, float angleToTest, float differenceAngle)
    {
        if (angleReference < 0 || angleReference > 360 ||
            angleToTest < 0 || angleToTest > 360)
        {
            Debug.LogError("angle non valide: " + angleReference + ", " + angleToTest);
        }

        float diff = 180 - Mathf.Abs(Mathf.Abs(angleReference - angleToTest) - 180);

        //diff = Mathf.Abs(angleReference - angleToTest);        

        if (diff <= differenceAngle)
            return (true);
        return (false);
    }

    /// <summary>
    /// retourne un vecteur2 par rapport à un angle
    /// </summary>
    /// <param name="angle"></param>
    public static Vector3 GetVectorFromAngle(float angle)
    {
        return (new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0));
    }

    /// <summary>
    /// renvoi l'angle entre deux vecteur, avec le 3eme vecteur de référence
    /// </summary>
    /// <param name="a">vecteur A</param>
    /// <param name="b">vecteur B</param>
    /// <param name="n">reference</param>
    /// <returns>Retourne un angle en degré</returns>
    public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        float angle = Vector3.Angle(a, b);                                  // angle in [0,180]
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
        float signed_angle = angle * sign;                                  // angle in [-179,180]
        float angle360 = (signed_angle + 360) % 360;                       // angle in [0,360]
        return (angle360);
    }

    /// <summary>
    /// Return the projection of A on B (with the good magnitude), based on ref (ex: Vector3.up)
    /// </summary>
    public static Vector3 GetProjectionOfAOnB(Vector3 A, Vector3 B, Vector3 refVector)
    {
        float angleDegre = SignedAngleBetween(A, B, refVector); //get angle A-B
        angleDegre *= Mathf.Deg2Rad;                            //convert to rad
        float magnitudeX = Mathf.Cos(angleDegre) * A.magnitude; //get magnitude
        Vector3 realDir = B.normalized * magnitudeX;            //set magnitude of new Vector
        return (realDir);   //vector A with magnitude based on B
    }

    /// <summary>
    /// Return the projection of A on B (with the good magnitude), based on ref (ex: Vector3.up)
    /// </summary>
    public static Vector3 GetProjectionOfAOnB(Vector3 A, Vector3 B, Vector3 refVector, float minMagnitude, float maxMagnitude)
    {
        float angleDegre = SignedAngleBetween(A, B, refVector); //get angle A-B
        angleDegre *= Mathf.Deg2Rad;                            //convert to rad
        float magnitudeX = Mathf.Cos(angleDegre) * A.magnitude; //get magnitude
        //set magnitude of new Vector
        Vector3 realDir = B.normalized * Mathf.Clamp(Mathf.Abs(magnitudeX), minMagnitude, maxMagnitude) * Mathf.Sign(magnitudeX);
        return (realDir);   //vector A with magnitude based on B
    }
}
