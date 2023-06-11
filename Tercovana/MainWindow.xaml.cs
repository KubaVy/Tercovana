using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TerceApp
{
    public partial class MainWindow : Window
    {
        private Random random;
        private List<Terč> terce;
        private int skore;
        private int naboje;
        private readonly int maxNaboje = 10;

        public MainWindow()
        {
            InitializeComponent();
            random = new Random();
            terce = new List<Terč>();
            skore = 0;
            naboje = maxNaboje;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartGeneratorCasovac();
            UpdateSkore();
            UpdateNaboje();
        }

        private void StartGeneratorCasovac()
        {
            var generatorCasovac = new DispatcherTimer();
            generatorCasovac.Tick += GeneratorCasovac_Tick;
            generatorCasovac.Interval = TimeSpan.FromSeconds(2);
            generatorCasovac.Start();
        }

        private void GeneratorCasovac_Tick(object sender, EventArgs e)
        {
            var velikost = random.Next(20, 100);
            var umisteni = GetRandomUmisteni(velikost);
            var terc = new Terč(velikost, umisteni);
            terce.Add(terc);
            canvas.Children.Add(terc.GetGrafika());

            terc.Zavre += Terce_Zavre;
            terc.Zasah += Terce_Zasah;

            var zaviraciCasovac = new DispatcherTimer();
            zaviraciCasovac.Tick += (s, args) =>
            {
                terc.Kliknuto();
            };
            zaviraciCasovac.Interval = TimeSpan.FromSeconds(2);
            zaviraciCasovac.Start();
        }

        private void Terce_Zavre(object sender, EventArgs e)
        {
            var terc = (Terč)sender;
            terce.Remove(terc);
            canvas.Children.Remove(terc.GetGrafika());
            UpdateSkore();
        }

        private void Terce_Zasah(object sender, EventArgs e)
        {
            skore++;
            UpdateSkore();
        }

        private void UpdateSkore()
        {
            skoreTextBlock.Text = $"Skóre: {skore}";
        }

        private void UpdateNaboje()
        {
            nabojeTextBlock.Text = $"Náboje: {naboje}";

            if (naboje == 0)
            {
                canvas.IsEnabled = false;
            }
            else
            {
                canvas.IsEnabled = true;
            }
        }

        private Point GetRandomUmisteni(double velikost)
        {
            var maxX = canvas.ActualWidth - velikost;
            var maxY = canvas.ActualHeight - velikost;
            var x = random.NextDouble() * maxX;
            var y = random.NextDouble() * maxY;
            return new Point(x, y);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (naboje > 0)
            {
                var bod = e.GetPosition(canvas);
                bool trefa = false;

                foreach (var terc in terce)
                {
                    if (terc.Zasah2(bod))
                    {
                        terc.Kliknuto();
                        trefa = true;
                        break;
                    }
                }

                if (!trefa)
                {
                    skore--;
                    UpdateSkore();
                }

                naboje--;
                UpdateNaboje();
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            naboje = maxNaboje;
            UpdateNaboje();
        }
    }
}
