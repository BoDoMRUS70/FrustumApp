using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace FrustumApp
{
    public partial class MainWindow : Window
    {
        private class SavedState
        {
            public double BottomRadius { get; set; } = 1.0;
            public double TopRadius { get; set; } = 0.5;
            public double Height { get; set; } = 1.0;
            public int Sides { get; set; } = 4;
            public double TranslateX { get; set; } = 0;
            public double RotateY { get; set; } = 0;
            public bool IsFrontView { get; set; } = true;
            public bool IsPerspective { get; set; } = false;
            public bool IsOblique { get; set; } = false;
        }

        private SavedState? _savedState;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded; // Подписываемся на событие загрузки окна
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Установка ортографической камеры по умолчанию
            viewport.Camera = new OrthographicCamera
            {
                Position = new Point3D(0, 0, 10),  // Отодвинута дальше
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                Width = 10  // Увеличена область видимости
            };

            CreateFrustumGeometry();
            SetupEventHandlers();
            UpdateStatus("Пирамида создана");
        }

        private void CreateFrustumGeometry()
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

                mesh.Positions.Add(new Point3D(
                    bottom * Math.Cos(angle),
                    -height / 2,
                    bottom * Math.Sin(angle)));

                mesh.Positions.Add(new Point3D(
                    top * Math.Cos(angle),
                    height / 2,
                    top * Math.Sin(angle)));
            }

            // Генерация треугольников
            for (int i = 0; i < sides; i++)
            {
                int baseIdx = i * 2;
                mesh.TriangleIndices.Add(baseIdx);
                mesh.TriangleIndices.Add(baseIdx + 1);
                mesh.TriangleIndices.Add((baseIdx + 2) % (sides * 2));

                mesh.TriangleIndices.Add((baseIdx + 2) % (sides * 2));
                mesh.TriangleIndices.Add(baseIdx + 1);
                mesh.TriangleIndices.Add((baseIdx + 3) % (sides * 2));
            }

            // Основания
            for (int i = 2; i < sides; i++)
            {
                mesh.TriangleIndices.Add(0);
                mesh.TriangleIndices.Add(i * 2);
                mesh.TriangleIndices.Add((i - 1) * 2);

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

        private void SetupEventHandlers()
        {
            sliderBottom.ValueChanged += (s, e) => { CreateFrustumGeometry(); UpdateStatus("Изменен нижний радиус"); };
            sliderTop.ValueChanged += (s, e) => { CreateFrustumGeometry(); UpdateStatus("Изменен верхний радиус"); };
            sliderHeight.ValueChanged += (s, e) => { CreateFrustumGeometry(); UpdateStatus("Изменена высота"); };
            sliderSides.ValueChanged += (s, e) => { CreateFrustumGeometry(); UpdateStatus("Изменено количество граней"); };

            sliderTranslateX.ValueChanged += (s, e) => { UpdateTransform(); UpdateStatus("Изменен перенос по X"); };
            sliderRotateY.ValueChanged += (s, e) => { UpdateTransform(); UpdateStatus("Изменен поворот по Y"); };

            rbFront.Checked += (s, e) => { UpdateProjection(); UpdateStatus("Установлен вид спереди"); };
            rbPerspective.Checked += (s, e) => { UpdateProjection(); UpdateStatus("Установлена перспективная проекция"); };
            rbOblique.Checked += (s, e) => { UpdateProjection(); UpdateStatus("Установлена косоугольная проекция"); };
        }

        private void UpdateStatus(string message)
        {
            statusText.Text = $"{DateTime.Now:T} - {message}";
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

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            sliderBottom.Value = 1;
            sliderTop.Value = 0.5;
            sliderHeight.Value = 1;
            sliderSides.Value = 4;

            sliderTranslateX.Value = 0;
            sliderRotateY.Value = 0;

            rbFront.IsChecked = true;

            UpdateStatus("Состояние сброшено");
        }

        private void SaveState_Click(object sender, RoutedEventArgs e)
        {
            _savedState = new SavedState
            {
                BottomRadius = sliderBottom.Value,
                TopRadius = sliderTop.Value,
                Height = sliderHeight.Value,
                Sides = (int)sliderSides.Value,
                TranslateX = sliderTranslateX.Value,
                RotateY = sliderRotateY.Value,
                IsFrontView = rbFront.IsChecked == true,
                IsPerspective = rbPerspective.IsChecked == true,
                IsOblique = rbOblique.IsChecked == true
            };

            UpdateStatus("Состояние сохранено");
        }

        private void RestoreState_Click(object sender, RoutedEventArgs e)
        {
            if (_savedState == null)
            {
                UpdateStatus("Нет сохраненного состояния");
                return;
            }

            sliderBottom.Value = _savedState.BottomRadius;
            sliderTop.Value = _savedState.TopRadius;
            sliderHeight.Value = _savedState.Height;
            sliderSides.Value = _savedState.Sides;

            sliderTranslateX.Value = _savedState.TranslateX;
            sliderRotateY.Value = _savedState.RotateY;

            rbFront.IsChecked = _savedState.IsFrontView;
            rbPerspective.IsChecked = _savedState.IsPerspective;
            rbOblique.IsChecked = _savedState.IsOblique;

            UpdateStatus("Состояние восстановлено");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Обработчики для меню
        private void MenuFront_Click(object sender, RoutedEventArgs e)
        {
            rbFront.IsChecked = true;
        }

        private void MenuPerspective_Click(object sender, RoutedEventArgs e)
        {
            rbPerspective.IsChecked = true;
        }

        private void MenuOblique_Click(object sender, RoutedEventArgs e)
        {
            rbOblique.IsChecked = true;
        }
    }
}