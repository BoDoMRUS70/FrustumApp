using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace FrustumApp.Transformations
{
    public static class Projections
    {
        public static void SetPerspectiveProjection(Viewport3D viewport,
            double cameraDistance = 5, double fieldOfView = 60)
        {
            viewport.Camera = new PerspectiveCamera
            {
                Position = new Point3D(0, 0, cameraDistance),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = fieldOfView
            };
        }

        public static void SetOrthographicProjection(Viewport3D viewport,
            double cameraDistance = 5, double width = 5)
        {
            viewport.Camera = new OrthographicCamera
            {
                Position = new Point3D(0, 0, cameraDistance),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                Width = width
            };
        }

        public static void SetIsometricProjection(Viewport3D viewport,
            double distance = 5)
        {
            const double isometricAngle = 35.264; // 45° по XZ и 30° по Y

            viewport.Camera = new OrthographicCamera
            {
                Position = new Point3D(
                    distance * Math.Cos(isometricAngle * Math.PI / 180),
                    distance * Math.Sin(isometricAngle * Math.PI / 180),
                    distance * Math.Cos(isometricAngle * Math.PI / 180)),
                LookDirection = new Vector3D(-1, -1, -1),
                UpDirection = new Vector3D(0, 1, 0),
                Width = distance * 2
            };
        }

        public static Matrix3D GetOrthographicProjectionMatrix(
            double left, double right,
            double bottom, double top,
            double near, double far)
        {
            return new Matrix3D(
                2 / (right - left), 0, 0, 0,
                0, 2 / (top - bottom), 0, 0,
                0, 0, -2 / (far - near), 0,
                -(right + left) / (right - left),
                -(top + bottom) / (top - bottom),
                -(far + near) / (far - near),
                1);
        }
    }
}