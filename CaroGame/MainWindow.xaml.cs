using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
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
using System.Data;
using System.Drawing;
using System.Windows.Threading;

namespace CaroGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer tmCooldown = new DispatcherTimer();
        private Image[,] Map;
        private static int columns, rows;
        private DispatcherTimer timer;
        private int player;
        private bool gameover;
        private bool vsComputer;
        private int[,] vtMap;
        private Stack<Chess> chesses;
        private Chess chess;
        public MainWindow()
        {
            columns = 21;
            rows = 21;
            vsComputer = false;
            gameover = false;
            player = 1;
            Map = new Image[rows + 2, columns + 2];
            vtMap = new int[rows + 2, columns + 2];
            chesses = new Stack<Chess>();
            InitializeComponent();
            tmCooldown.Tick += new EventHandler(tmCooldown_Tick);
            tmCooldown.Interval = TimeSpan.FromMilliseconds(50); // Điều chỉnh khoảng thời gian tại đây
            tmCooldown.Start(); // Bắt đầu đếm thời gian
            BuildTable();
        }
        private void BuildTable()
        {
            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < columns; j++)
                {
                    Map[i, j] = new Image();
                    pnTableChess.Children.Add(Map[i, j]);
                    Grid.SetRow(Map[i, j], i); // Đặt hàng cho ô cờ
                    Grid.SetColumn(Map[i, j], j); // Đặt cột cho ô cờ
                    Map[i, j].Margin = new Thickness(0);
                    Map[i, j].Width = 30;
                    Map[i, j].Height = 30;
                    BitmapImage myBitmapImage = new BitmapImage();
                    myBitmapImage.BeginInit();
                    myBitmapImage.UriSource = new Uri(@"E:\CUSC\C#\CaroGame\CaroGame\Resources\bg.png");
                    myBitmapImage.EndInit();
                    Map[i, j].Source = myBitmapImage;
                    Map[i, j].MouseLeave += Form1_MouseLeave;
                    Map[i, j].MouseEnter += Form1_MouseEnter;
                    Map[i, j].Tag = new System.Drawing.Point(i, j);
                    Map[i, j].MouseDown += Form1_Click;
                }
            }
        }
        private void Form1_Click(object sender, EventArgs e)
        {
           
            if (gameover)
                return;
            Image lb = (Image)sender;
            System.Drawing.Point coordinates = (System.Drawing.Point)lb.Tag;
            int x = (int)coordinates.X;
            int y = (int)coordinates.Y;
            if (vtMap[x, y] != 0)
                return;
            if (vsComputer)
            {
                player = 1;
                psbCooldownTime.Value = 0;
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(@"E:\CUSC\C#\CaroGame\CaroGame\Resources\o.png");
                myBitmapImage.EndInit();
                lb.Source = myBitmapImage;
                vtMap[x, y] = 1;
                Check(x, y);
                CptFindChess();
            }
            else
            {
                if (player == 1)
                { 
                    psbCooldownTime.Value = 0;
                    BitmapImage myBitmapImage = new BitmapImage();
                    myBitmapImage.BeginInit();
                    myBitmapImage.UriSource = new Uri(@"E:\CUSC\C#\CaroGame\CaroGame\Resources\o.png");
                    myBitmapImage.EndInit();
                    lb.Source = myBitmapImage;
                    vtMap[x, y] = 1;
                    //MessageBox.Show($"x: {x} y: {y}");
                    Check(x, y);
                    player = 2;
                }
                else
                {
                    psbCooldownTime.Value = 0;
                    BitmapImage myBitmapImage = new BitmapImage();
                    myBitmapImage.BeginInit();
                    myBitmapImage.UriSource = new Uri(@"E:\CUSC\C#\CaroGame\CaroGame\Resources\x.png");
                    myBitmapImage.EndInit();
                    lb.Source = myBitmapImage;
                    vtMap[x, y] = 2;
                    Check(x, y);
                    player = 1;
                }
            }
            chess = new Chess(lb, x, y);
            chesses.Push(chess);
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            if (gameover)
                return;
            Image lb = (Image)sender;

        }
        private void tmCooldown_Tick(object sender, EventArgs e)
        {
            psbCooldownTime.Value += 1; // Tùy chỉnh giá trị tại đây
            if (psbCooldownTime.Value >= psbCooldownTime.Maximum)
            {
                tmCooldown.Stop();
                Gameover();
                if (vsComputer)
                {
                    if (player != 1)
                        MessageBox.Show("You win!!");
                    else
                        MessageBox.Show("You lost!!");
                }
                else
                {
                    if (player != 1)
                        MessageBox.Show("Player1 Win");
                    else
                        MessageBox.Show("Player2 Win");
                }
            }
        }
        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            if (gameover)
                return;
            Image lb = (Image)sender;
            
        }
        private void Check(int x, int y)
        {
            int i = x - 1, j = y;
            int column = 1, row = 1, mdiagonal = 1, ediagonal = 1;
            while (vtMap[x, y] == vtMap[i, j] && i >= 0)
            {
                column++;
                i--;
            }
            i = x + 1;
            while (vtMap[x, y] == vtMap[i, j] && i <= rows)
            {
                column++;
                i++;
            }
            i = x; j = y - 1;
            while (vtMap[x, y] == vtMap[i, j] && j >= 0)
            {
                row++;
                j--;
            }
            j = y + 1;
            while (vtMap[x, y] == vtMap[i, j] && j <= columns)
            {
                row++;
                j++;
            }
            i = x - 1; j = y - 1;
            while (vtMap[x, y] == vtMap[i, j] && i >= 0 && j >= 0)
            {
                mdiagonal++;
                i--;
                j--;
            }
            i = x + 1; j = y + 1;
            while (vtMap[x, y] == vtMap[i, j] && i <= rows && j <= columns)
            {
                mdiagonal++;
                i++;
                j++;
            }
            i = x - 1; j = y + 1;
            while (vtMap[x, y] == vtMap[i, j] && i >= 0 && j <= columns)
            {
                ediagonal++;
                i--;
                j++;
            }
            i = x + 1; j = y - 1;
            while (vtMap[x, y] == vtMap[i, j] && i <= rows && j >= 0)
            {
                ediagonal++;
                i++;
                j--;
            }
            if (row >= 5 || column >= 5 || mdiagonal >= 5 || ediagonal >= 5)
            {
                Gameover();
                if (vsComputer)
                {
                    if (player == 1)
                        MessageBox.Show("You win!!");
                    else
                        MessageBox.Show("You lost!!");
                }
                else
                {
                    if (player == 1)
                        MessageBox.Show("Player1 Win");
                    else
                        MessageBox.Show("Player2 Win");
                }
            }

        }
        private void Gameover()
        {  
            gameover = true;
        }
       
        #region AI

        private int[] Attack = new int[7] { 0, 9, 54, 162, 1458, 13112, 118008 };
        private int[] Defense = new int[7] { 0, 3, 27, 99, 729, 6561, 59049 };

        private void PutChess(int x, int y)
        {
            player = 0;
            psbCooldownTime.Value = 0;
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"E:\CUSC\C#\CaroGame\CaroGame\Resources\x.png");
            myBitmapImage.EndInit();
            Map[x, y].Source = myBitmapImage;
            vtMap[x, y] = 2;
            Check(x, y);
            chess = new Chess(Map[x, y], x, y);
            chesses.Push(chess);
        }

        
        private void CptFindChess()
        {
            if (gameover) return;
            long max = 0;
            int imax = 1, jmax = 1;
            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < columns; j++)
                    if (vtMap[i, j] == 0)
                    {
                        long temp = Caculate(i, j);
                        if (temp > max)
                        {
                            max = temp;
                            imax = i; jmax = j;
                        }
                    }
            }
            //MessageBox.Show($"x: {imax} y: {jmax}");
            PutChess(imax, jmax);
        }
        private long Caculate(int x, int y)
        {
            return EnemyChesses(x, y) + ComputerChesses(x, y);
        }
        private long ComputerChesses(int x, int y)
        {
            int i = x - 1, j = y;
            int column = 0, row = 0, mdiagonal = 0, ediagonal = 0;
            int sc_ = 0, sc = 0, sr_ = 0, sr = 0, sm_ = 0, sm = 0, se_ = 0, se = 0;
            while (vtMap[i, j] == 2 && i >= 0)
            {
                column++;
                i--;
            }
            if (vtMap[i, j] == 0) sc_ = 1;
            i = x + 1;
            while (vtMap[i, j] == 2 && i <= rows)
            {
                column++;
                i++;
            }
            if (vtMap[i, j] == 0) sc = 1;
            i = x; j = y - 1;
            while (vtMap[i, j] == 2 && j >= 0)
            {
                row++;
                j--;
            }
            if (vtMap[i, j] == 0) sr_ = 1;
            j = y + 1;
            while (vtMap[i, j] == 2 && j <= columns)
            {
                row++;
                j++;
            }
            if (vtMap[i, j] == 0) sr = 1;
            i = x - 1; j = y - 1;
            while (vtMap[i, j] == 2 && i >= 0 && j >= 0)
            {
                mdiagonal++;
                i--;
                j--;
            }
            if (vtMap[i, j] == 0) sm_ = 1;
            i = x + 1; j = y + 1;
            while (vtMap[i, j] == 2 && i <= rows && j <= columns)
            {
                mdiagonal++;
                i++;
                j++;
            }
            if (vtMap[i, j] == 0) sm = 1;
            i = x - 1; j = y + 1;
            while (vtMap[i, j] == 2 && i >= 0 && j <= columns)
            {
                ediagonal++;
                i--;
                j++;
            }
            if (vtMap[i, j] == 0) se_ = 1;
            i = x + 1; j = y - 1;
            while (vtMap[i, j] == 2 && i <= rows && j >= 0)
            {
                ediagonal++;
                i++;
                j--;
            }
            if (vtMap[i, j] == 0) se = 1;

            if (column == 4) column = 5;
            if (row == 4) row = 5;
            if (mdiagonal == 4) mdiagonal = 5;
            if (ediagonal == 4) ediagonal = 5;

            if (column == 3 && sc == 1 && sc_ == 1) column = 4;
            if (row == 3 && sr == 1 && sr_ == 1) row = 4;
            if (mdiagonal == 3 && sm == 1 && sm_ == 1) mdiagonal = 4;
            if (ediagonal == 3 && se == 1 && se_ == 1) ediagonal = 4;

            if (column == 2 && row == 2 && sc == 1 && sc_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (column == 2 && mdiagonal == 2 && sc == 1 && sc_ == 1 && sm == 1 && sm_ == 1) column = 3;
            if (column == 2 && ediagonal == 2 && sc == 1 && sc_ == 1 && se == 1 && se_ == 1) column = 3;
            if (row == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (row == 2 && ediagonal == 2 && se == 1 && se_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (ediagonal == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && se == 1 && se_ == 1) column = 3;
            long Sum = Attack[row] + Attack[column] + Attack[mdiagonal] + Attack[ediagonal];
            return Sum;

        }

        private void PlaybWithAI(object sender, RoutedEventArgs e)
        {
            vsComputer = true;
            gameover = false;
            psbCooldownTime.Value = 0;

            pnTableChess.Children.Clear();
            player = 1;
            Map = new Image[rows + 2, columns + 2];
            vtMap = new int[rows + 2, columns + 2];
            chesses = new Stack<Chess>();

            BuildTable();
        }

 
        private long EnemyChesses(int x, int y)
        {
            int i = x - 1, j = y;
            int sc_ = 0, sc = 0, sr_ = 0, sr = 0, sm_ = 0, sm = 0, se_ = 0, se = 0;
            int column = 0, row = 0, mdiagonal = 0, ediagonal = 0;
            while (vtMap[i, j] == 1 && i >= 0)
            {
                column++;
                i--;
            }
            if (vtMap[i, j] == 0) sc_ = 1;
            i = x + 1;
            while (vtMap[i, j] == 1 && i <= rows)
            {
                column++;
                i++;
            }
            if (vtMap[i, j] == 0) sc = 1;
            i = x; j = y - 1;
            while (vtMap[i, j] == 1 && j >= 0)
            {
                row++;
                j--;
            }
            if (vtMap[i, j] == 0) sr_ = 1;
            j = y + 1;
            while (vtMap[i, j] == 1 && j <= columns)
            {
                row++;
                j++;
            }
            if (vtMap[i, j] == 0) sr = 1;
            i = x - 1; j = y - 1;
            while (vtMap[i, j] == 1 && i >= 0 && j >= 0)
            {
                mdiagonal++;
                i--;
                j--;
            }
            if (vtMap[i, j] == 0) sm_ = 1;
            i = x + 1; j = y + 1;
            while (vtMap[i, j] == 1 && i <= rows && j <= columns)
            {
                mdiagonal++;
                i++;
                j++;
            }
            if (vtMap[i, j] == 0) sm = 1;
            i = x - 1; j = y + 1;
            while (vtMap[i, j] == 1 && i >= 0 && j <= columns)
            {
                ediagonal++;
                i--;
                j++;
            }
            if (vtMap[i, j] == 0) se_ = 1;
            i = x + 1; j = y - 1;
            while (vtMap[i, j] == 1 && i <= rows && j >= 0)
            {
                ediagonal++;
                i++;
                j--;
            }
            if (vtMap[i, j] == 0) se = 1;

            if (column == 4) column = 5;
            if (row == 4) row = 5;
            if (mdiagonal == 4) mdiagonal = 5;
            if (ediagonal == 4) ediagonal = 5;

            if (column == 3 && sc == 1 && sc_ == 1) column = 4;
            if (row == 3 && sr == 1 && sr_ == 1) row = 4;
            if (mdiagonal == 3 && sm == 1 && sm_ == 1) mdiagonal = 4;
            if (ediagonal == 3 && se == 1 && se_ == 1) ediagonal = 4;

            if (column == 2 && row == 2 && sc == 1 && sc_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (column == 2 && mdiagonal == 2 && sc == 1 && sc_ == 1 && sm == 1 && sm_ == 1) column = 3;
            if (column == 2 && ediagonal == 2 && sc == 1 && sc_ == 1 && se == 1 && se_ == 1) column = 3;
            if (row == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (row == 2 && ediagonal == 2 && se == 1 && se_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (ediagonal == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && se == 1 && se_ == 1) column = 3;
            long Sum = Defense[row] + Defense[column] + Defense[mdiagonal] + Defense[ediagonal];

            return Sum;
        }
        #endregion
    }

    public class Chess
    {
        public Image lb;
        public int X;
        public int Y;
        public Chess()
        {
            lb = new Image();
        }
        public Chess(Image _lb, int x, int y)
        {
            lb = new Image();
            lb = _lb;
            X = x;
            Y = y;
        }
    }
}
