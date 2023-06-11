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
        private Random random; // Instance třídy Random pro generování náhodných čísel
        private List<Terč> terce; // Seznam terčů
        private int skore; // Skóre hráče
        private int naboje; // Počet nábojů
        private readonly int maxNaboje = 10; // Maximální počet nábojů

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
            StartGeneratorCasovac(); // Spustíme generátor terčů pomocí časovače
            UpdateSkore(); // Aktualizujeme zobrazení skóre
            UpdateNaboje(); // Aktualizujeme zobrazení počtu nábojů
        }

        private void StartGeneratorCasovac()
        {
            var generatorCasovac = new DispatcherTimer();
            generatorCasovac.Tick += GeneratorCasovac_Tick; // Při každém tiknutí časovače se spustí metoda GeneratorCasovac_Tick
            generatorCasovac.Interval = TimeSpan.FromSeconds(2); // Interval mezi generováním terčů je 2 sekundy
            generatorCasovac.Start(); // Spustíme časovač
        }

        private void GeneratorCasovac_Tick(object sender, EventArgs e)
        {
            var velikost = random.Next(20, 100); // Náhodně vygenerujeme velikost terče mezi 20 a 100
            var umisteni = GetRandomUmisteni(velikost); // Vygenerujeme náhodné umístění terče
            var terc = new Terč(velikost, umisteni); // Vytvoříme novou instanci terče
            terce.Add(terc); // Přidáme terč do seznamu
            canvas.Children.Add(terc.GetGrafika()); // Přidáme terč do Canvasu

            terc.Zavre += Terce_Zavre; // Přiřadíme obslužnou metodu pro událost Zavre terče
            terc.Zasah += Terce_Zasah; // Přiřadíme obslužnou metodu pro událost Zasah terče

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
            var terc = (Terč)sender; // Získáme terč, který vyvolal událost
            terce.Remove(terc); // Odebereme terč ze seznamu
            canvas.Children.Remove(terc.GetGrafika()); // Odebereme terč z Canvasu
            UpdateSkore(); // Aktualizujeme zobrazení skóre
        }

        private void Terce_Zasah(object sender, EventArgs e)
        {
            skore++; // Zvýšíme skóre o 1
            UpdateSkore(); // Aktualizujeme zobrazení skóre
        }

        private void UpdateSkore()
        {
            skoreTextBlock.Text = $"Skóre: {skore}"; // Zobrazíme aktuální skóre v textovém bloku
        }

        private void UpdateNaboje()
        {
            nabojeTextBlock.Text = $"Náboje: {naboje}"; // Zobrazíme aktuální počet nábojů v textovém bloku

            if (naboje == 0)
            {
                canvas.IsEnabled = false; // Pokud došly náboje, zakážeme střelbu (Canvas je neaktivní)
            }
            else
            {
                canvas.IsEnabled = true; // Pokud jsou náboje k dispozici, povolíme střelbu (Canvas je aktivní)
            }
        }

        private Point GetRandomUmisteni(double velikost)
        {
            var maxX = canvas.ActualWidth - velikost; // Maximální X-ová souřadnice umístění terče
            var maxY = canvas.ActualHeight - velikost; // Maximální Y-ová souřadnice umístění terče
            var x = random.NextDouble() * maxX; // Náhodná X-ová souřadnice umístění terče
            var y = random.NextDouble() * maxY; // Náhodná Y-ová souřadnice umístění terče
            return new Point(x, y); // Vytvoříme nový bod s náhodnými souřadnicemi
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (naboje > 0)
            {
                var bod = e.GetPosition(canvas); // Získáme souřadnice kliknutí
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
            naboje = maxNaboje; // Znovu naplníme náboje na maximální počet
            UpdateNaboje(); // Aktualizujeme zobrazení počtu nábojů
        }
    }
}
