using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace li_algorithm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool[,] Map;
        TextBlock[,] MapMarks;
        int[,] marks;
        Grid[,] Grids;
        int blockSize = 30;
        Rectangle[,] MapBlocks;
        bool isStarted = false;
        bool isFinished = false;
        int startI;
        int startJ;
        int stopI;
        int stopJ;
        int d;

        private void SetStart(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                int i = (int)Math.Floor(Mouse.GetPosition(MapBlocks[0, 0]).Y / blockSize);
                int j = (int)Math.Floor(Mouse.GetPosition(MapBlocks[0, 0]).X / blockSize);
                if (!Map[i, j] && !isStarted)
                {
                    MapBlocks[i, j].Fill = Brushes.Blue;
                    MapMarks[i, j].Text = d.ToString();
                    isStarted = true;
                    marks[i, j] = d;
                    startI = i;
                    startJ = j;
                }
                else if (!Map[i, j] && !isFinished)
                {
                    MapBlocks[i, j].Fill = Brushes.Red;
                    MapMarks[i, j].Text = "!";
                    isFinished = true;
                    stopI = i;
                    stopJ = j;
                }
            }
        }

        private void setD(int z, int i, int j, int H, int W)
        {
            if (i >= 0 && j >= 0 && i < H && j < W)
            {
                if (marks[i,j] == -1)
                {
                    marks[i, j] = z;
                    MapBlocks[i, j].Fill = Brushes.Blue;
                    MapBlocks[i, j].Opacity = MapBlocks[i, j].Opacity+0.05;
                    MapMarks[i, j].Text = z.ToString();
                }
            }
        }
        

        private void Logic(object sender, RoutedEventArgs e)
        {
            int H = Convert.ToInt32(CanvasMap.Height / blockSize);
            int W = Convert.ToInt32(CanvasMap.Width / blockSize);
            bool stop = false;
            if (!stop && MapBlocks[stopI, stopJ].Fill == Brushes.Red)
            {
                label.Content = (d+1).ToString();
                stop = true;
                for (int x = 0; x < H; x++)
                {
                    for (int y = 0; y < W; y++)
                    {
                        if (marks[x, y] == d)          // ячейка (x, y) помечена числом d
                        {
                            stop = false;              // найдены непомеченные клетки
                            setD(d + 1, x + 1, y, H, W);
                            setD(d + 1, x - 1, y, H, W);
                            setD(d + 1, x, y + 1, H, W);
                            setD(d + 1, x, y - 1, H, W);
                        }
                    }
                }
                d++;
            }
            
            if (MapBlocks[stopI, stopJ].Fill == Brushes.Green)
            {
                if (chooseD(d, stopI + 1, stopJ, H, W)) stopI++;
                else if (chooseD(d, stopI - 1, stopJ, H, W)) stopI--;
                else if (chooseD(d, stopI, stopJ + 1, H, W)) stopJ++;
                else if (chooseD(d, stopI, stopJ - 1, H, W)) stopJ--;
                d--;
            }
            else if (MapBlocks[stopI, stopJ].Fill == Brushes.Blue)
            {
                MapBlocks[stopI, stopJ].Fill = Brushes.Green;
                MapMarks[stopI, stopJ].Text = "*";
            }
        }
        
        private bool chooseD(int d, int i, int j, int H, int W)
        {
            if (i >= 0 && j >= 0 && i < H && j < W)
            {
                if (marks[i, j] == d - 1)
                {
                    MapBlocks[i, j].Fill = Brushes.Green;
                    MapMarks[i, j].Text = "*";
                    return true;
                }
                else return false;
            }
            else
            {
                return false;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Print();
            btn.Click += Logic;
        }
       

        private void Print()
        {
            d = 0;
            var rand = new Random(new DateTime().Millisecond.GetHashCode());
            int aWidth = Convert.ToInt32(CanvasMap.Width / blockSize); 
            int aHeight = Convert.ToInt32(CanvasMap.Height / blockSize); 
            CanvasMap.Children.Clear();
            Map = new bool[aHeight, aWidth];
            marks = new int[aHeight, aWidth];
            MapMarks = new TextBlock[aWidth, aHeight];
            MapBlocks = new Rectangle[aHeight, aWidth];
            Grids = new Grid[aHeight, aWidth];
            for (int i = 0; i < aHeight; i++)
            {
                for (int j = 0; j < aWidth; j++)
                {
                    Grids[i, j] = new Grid();
                    MapMarks[i, j] = new TextBlock
                    {
                        Width = blockSize / 2,
                        Height = blockSize - 1.0
                    };
                    Grids[i, j].Children.Add(MapMarks[i, j]);
                    Map[i, j] = (rand.Next(4) == 0);
                    marks[i, j] = !Map[i, j] ? -1 : -2;
                    Rectangle rectangle = new Rectangle
                    {
                        Width = blockSize - 1.0,
                        Height = blockSize - 1.0
                    };
                    rectangle.MouseDown += SetStart;
                    rectangle.Opacity = 0.4;
                    Canvas.SetLeft(Grids[i, j], j * blockSize);
                    Canvas.SetTop(Grids[i, j], i * blockSize);
                    MapBlocks[i, j] = rectangle;
                    if (Map[i, j])
                    {
                        rectangle.Opacity = 1;
                    }
                    rectangle.Fill = Brushes.Black;
                    Grids[i, j].Children.Add(rectangle);
                    CanvasMap.Children.Add(Grids[i, j]);
                }
            }
        }

    }
}
