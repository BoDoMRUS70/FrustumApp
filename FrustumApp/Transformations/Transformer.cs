using System.Windows.Media.Media3D;

namespace FrustumApp.Transformations
{
    public static class Transformer
    {
        public static Transform3DGroup ApplyTransformations(
            double translateX, double translateY, double translateZ,
            double rotateX, double rotateY, double rotateZ,
            double scaleX, double scaleY, double scaleZ,
            bool reflectX, bool reflectY, bool reflectZ)
        {
            var transformGroup = new Transform3DGroup();

            // 1. Масштабирование и отражение (применяем первым)
            transformGroup.Children.Add(new ScaleTransform3D(
                reflectX ? -scaleX : scaleX,
                reflectY ? -scaleY : scaleY,
                reflectZ ? -scaleZ : scaleZ));

            // 2. Поворот (в порядке Z -> Y -> X)
            var rotationGroup = new Transform3DGroup();
            rotationGroup.Children.Add(new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 0, 1), rotateZ)));
            rotationGroup.Children.Add(new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 1, 0), rotateY)));
            rotationGroup.Children.Add(new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(1, 0, 0), rotateX)));
            transformGroup.Children.Add(rotationGroup);

            // 3. Перенос (применяем последним)
            transformGroup.Children.Add(new TranslateTransform3D(
                translateX, translateY, translateZ));

            return transformGroup;
        }

        public static Matrix3D GetTransformationMatrix(
            double tx, double ty, double tz,
            double rx, double ry, double rz,
            double sx, double sy, double sz)
        {
            // Создаём единичную матрицу
            var matrix = Matrix3D.Identity;

            // Масштабирование
            matrix.Scale(new Vector3D(sx, sy, sz));

            // Поворот (в радианах)
            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), rx));
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), ry));
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), rz));

            // Перенос
            matrix.Translate(new Vector3D(tx, ty, tz));

            return matrix;
        }
    }
}