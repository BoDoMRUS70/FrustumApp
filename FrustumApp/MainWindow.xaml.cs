using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace FrustumApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeFrustum();
            SetupEventHandlers();
        }

        private void InitializeFrustum()
        {
            UpdateFrustumGeometry();
            UpdateProjection();
        }

        private void SetupEventHandlers()
        {
            sliderBottom.ValueChanged += (s, e) => UpdateFrustumGeometry();
            sliderTop.ValueChanged += (s, e) => UpdateFrustumGeometry();
            sliderHeight.ValueChanged += (s, e) => UpdateFrustumGeometry();
            sliderSides.ValueChanged += (s, e) => UpdateFrustumGeometry();

            sliderTranslateX.ValueChanged += (s, e) => UpdateTransform();
            sliderRotateY.ValueChanged += (s, e) => UpdateTransform();

            rbFront.Checked += (s, e) => UpdateProjection();
            rbPerspective.Checked += (s, e) => UpdateProjection();
            rbOblique.Checked += (s, e) => UpdateProjection();
        }

        private void UpdateFrustumGeometry()
        {
            var mesh = new MeshGeometry3D();
            double bottom = sliderBottom.Value;
            double top = sliderTop.Value;
            double height = sliderHeight.Value;
            int sides = (int)sliderSides.Value;

            // Генерация вершин
            for (int i = 0; i <= sides; i++)
            {
                double angle = 2 * Math.PI * i / sides;

                // Нижнее основание
                mesh.Positions.Add(new Point3D(
                    bottom * Math.Cos(angle),
                    -height / 2,
                    bottom * Math.Sin(angle)));

                // Верхнее основание
                mesh.Positions.Add(new Point3D(
                    top * Math.Cos(angle),
                    height / 2,
                    top * Math.Sin(angle)));
            }

            // Генерация треугольников для боковых граней
            for (int i = 0; i < sides; i++)
            {
                int baseIdx = i * 2;

                // Боковые грани (два треугольника на грань)
                mesh.TriangleIndices.Add(baseIdx);
                mesh.TriangleIndices.Add(baseIdx + 1);
                mesh.TriangleIndices.Add((baseIdx + 2) % (sides * 2));

                mesh.TriangleIndices.Add((baseIdx + 2) % (sides * 2));
                mesh.TriangleIndices.Add(baseIdx + 1);
                mesh.TriangleIndices.Add((baseIdx + 3) % (sides * 2));
            }

            // Генерация оснований
            for (int i = 2; i < sides; i++)
            {
                // Нижнее основание
                mesh.TriangleIndices.Add(0);
                mesh.TriangleIndices.Add(i * 2);
                mesh.TriangleIndices.Add((i - 1) * 2);

                // Верхнее основание
                mesh.TriangleIndices.Add(1);
                mesh.TriangleIndices.Add((i - 1) * 2 + 1);
                mesh.TriangleIndices.Add(i * 2 + 1);
            }

            modelContainer.Content = new GeometryModel3D
            {
                Geometry = mesh,
                Material = new DiffuseMaterial(Brushes.LightBlue),
                BackMaterial = new DiffuseMaterial(Brushes.LightCoral)
            };

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (modelContainer.Content is GeometryModel3D model)
            {
                var transform = new Transform3DGroup();
                transform.Children.Add(new TranslateTransform3D(sliderTranslateX.Value, 0, 0));
                transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), sliderRotateY.Value)));
                model.Transform = transform;
            }
        }

        private void UpdateProjection()
        {
            if (rbFront.IsChecked == true)
            {
                viewport.Camera = new OrthographicCamera
                {
                    Position = new Point3D(0, 0, 5),
                    LookDirection = new Vector3D(0, 0, -1),
                    UpDirection = new Vector3D(0, 1, 0),
                    Width = 5
                };
            }
            else if (rbPerspective.IsChecked == true)
            {
                viewport.Camera = new PerspectiveCamera
                {
                    Position = new Point3D(0, 0, 5),
                    LookDirection = new Vector3D(0, 0, -1),
                    UpDirection = new Vector3D(0, 1, 0),
                    FieldOfView = 45
                };
            }
            else if (rbOblique.IsChecked == true)
            {
                var camera = new OrthographicCamera
                {
                    Position = new Point3D(3, 3, 3),
                    LookDirection = new Vector3D(-1, -1, -1),
                    UpDirection = new Vector3D(0, 1, 0),
                    Width = 6
                };
                viewport.Camera = camera;
            }
        }
    }
}