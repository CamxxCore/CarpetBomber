using GTA.Math;

public static class Utils
    {
        public static Vector3 RightVector(this Vector3 position, Vector3 up)
        {
            position.Normalize();
            up.Normalize();
            return Vector3.Cross(position, up);
        }

        public static Vector3 LeftVector(this Vector3 position, Vector3 up)
        {
            position.Normalize();
            up.Normalize();
            return -(Vector3.Cross(position, up));
        }
    }
