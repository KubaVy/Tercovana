using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TerceApp
{
    public class Terč
    {
        private double velikost; // Velikost terče
        private Point umisteni; // Umístění terče
        private Canvas skupinaElips; // Skupina prvků pro grafické zobrazení terče

        public event EventHandler Zavre; // Událost, která se vyvolá při zavření terče
        public event EventHandler Zasah; // Událost, která se vyvolá při zásahu terče

        public Terč(double velikost, Point umisteni)
        {
            this.velikost = velikost;
            this.umisteni = umisteni;
            skupinaElips = new Canvas();

            // Vytvoření jednotlivých elips terče
            var elips1 = CreateElips(velikost, Brushes.Red);
            skupinaElips.Children.Add(elips1);

            var elips2 = CreateElips(velikost * 0.75, Brushes.White);
            var posun = (velikost - elips2.Width) / 2;
            Canvas.SetLeft(elips2, posun);
            Canvas.SetTop(elips2, posun);
            skupinaElips.Children.Add(elips2);

            var elips3 = CreateElips(velikost * 0.5, Brushes.Red);
            posun = (velikost - elips3.Width) / 2;
            Canvas.SetLeft(elips3, posun);
            Canvas.SetTop(elips3, posun);
            skupinaElips.Children.Add(elips3);

            // Nastavení umístění terče na plátno
            Canvas.SetLeft(skupinaElips, umisteni.X);
            Canvas.SetTop(skupinaElips, umisteni.Y);

            skupinaElips.MouseLeftButtonDown += SkupinaElips_MouseLeftButtonDown;
        }

        private Ellipse CreateElips(double velikost, Brush barva)
        {
            return new Ellipse
            {
                Width = velikost,
                Height = velikost,
                Fill = barva,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
        }

        private void SkupinaElips_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Kliknuto();
            Zasah?.Invoke(this, EventArgs.Empty); // Vyvoláme událost Zasah
        }

        public UIElement GetGrafika()
        {
            return skupinaElips;
        }

        public bool Zasah2(Point bod)
        {
            var tercBod = new Point(umisteni.X + velikost / 2, umisteni.Y + velikost / 2);
            var vzdalenost = Point.Subtract(bod, tercBod).Length;
            return vzdalenost <= velikost / 2;
        }

        public void Kliknuto()
        {
            Zavre?.Invoke(this, EventArgs.Empty); // Vyvoláme událost Zavre
        }

        public void StartZaviraciCasovac()
        {
            var zaviraciCasovac = new DispatcherTimer();
            zaviraciCasovac.Tick += ZaviraciCasovac_Tick;
            zaviraciCasovac.Interval = TimeSpan.FromSeconds(8);
            zaviraciCasovac.Start();
        }

        private void ZaviraciCasovac_Tick(object sender, EventArgs e)
        {
            Kliknuto();
        }
    }
}
