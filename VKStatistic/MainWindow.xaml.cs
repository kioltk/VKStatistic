using System;
using System.Collections.Generic;
using System.Linq;
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
using application.Models;
using System.ComponentModel;
namespace VKStatistic
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Graphs.DataContext = gs;
        }
        
        public class GraphsSettings : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;
            //// Create the OnPropertyChanged method to raise the event
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
            double _zoom = 0.5;
            public double zoom
            {
                get
                {
                    return _zoom;
                }
                set
                {
                    _zoom = value;
                    OnPropertyChanged("zoom");
                }
            }
            double _zoom_x = 0;
            double _zoom_y = 0;
            public double zoom_x
            {
                get
                {
                    return _zoom_x;
                }
                set
                {
                    _zoom_x = value;
                    OnPropertyChanged("zoom_x");
                }
            }
            public double zoom_y
            {
                get
                {
                    return _zoom_y;
                }
                set
                {
                    _zoom_y = value;
                    OnPropertyChanged("zoom_y");
                }
            }
            double _vertical = -500;
            public double vertical
            {
                get
                {
                    return _vertical;
                }
                set
                {
                    _vertical = value;
                    OnPropertyChanged("vertical");
                }


            }
            double _horizontal = 0;
            public double horizontal
            {
                get
                {
                   return _horizontal;
                }
                set
                {
                    _horizontal = value;
                    OnPropertyChanged("horizontal");
                }
            }
            public GraphsSettings()
            {
 
            }
            public void standart()
            {
                zoom = 1;
                zoom_x = 0;
                zoom_y = 0;
                vertical = 0;
                horizontal = 0;
            }
        }
        public GraphsSettings gs = new GraphsSettings();
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            g = new Graph();

            foreach (Dialog selected in ((ListView)sender).SelectedItems)
            {
                
                var o = Server.GetMessages(selected.user_id);
                this.DataContext = o;
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            LoadingState.Text = "Загрузка Диалогов..";
            Dialogs o = Server.GetDialogs();
            if (o != null)
            {
                LoadingState.Text = "Диалоги загружены!";
                Server.DialogsUserInfo(o);
                foreach (Dialog d in o.items)
                {
                    DialogsList.Items.Add(d);
                }
            }
            else
            {
                LoadingState.Text = "Ошибка загрузки диалогов";
                
            }

        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (Dialog selected in ((ListView)DialogsList).SelectedItems)
            {
                var o = this.DataContext;
                Server.DownloadAllMessages(o as Messages, selected.user_id);
            }

            LoadingState.Text = "загружено";
        }
        Graph g;
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            gs.standart();
            g.brushcolor = System.Windows.Media.Brushes.LightSteelBlue;
            g.Insert(this.DataContext as Messages, this.Graphs, this.LoadingState);

        }

        public class Graph
        {
            int offsetm;
            int offsetd;
            Messages _m;
            public List<Line> items;
            public Line Get()
            {
                var myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.X1 = 1;
                myLine.X2 = 50;
                myLine.Y1 = 1;
                myLine.Y2 = 50;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 1;
                return myLine;
            }
            public System.Windows.Media.SolidColorBrush brushcolor;
            public Line New(int x1, int x2, int y1, int y2)
            {
                var myLine = new Line();
                myLine.Stroke = brushcolor;
                myLine.X1 = 1100 - x1;
                myLine.X2 = 1100 - x2;
                myLine.Y1 = 650-y1;
                myLine.Y2 = 650-y2;
                myLine.StrokeThickness = 1;
                return myLine;
            }
            public Graph()
            {
                items = new List<Line>();
                offsetd = 1;
            }
            public async void Insert(Messages ms, Canvas canv, TextBlock loading)
            {
                items.Clear();
                _m = ms; var messgs = ms.items.Skip(offsetm);

                if (messgs.Count() != 0)
                {
                    List<Message> day = new List<Message>();

                    int now = Time.UNIXNow();
                    int ld = Time.HowManyDaysFromThe(messgs.Last().date);
                    int current = now - 0 * 86400;
                    int ycurrent = now - (0 + 1) * 86400;
                    var pr = messgs.Where(x => x.date < current && x.date > ycurrent).Count();
                    var n = pr;

                    for (int i = offsetd; i < ld; i++)
                    {

                        current = now - i * 86400;
                        ycurrent = now - (i + 1) * 86400;
                        n = messgs.Where(x => x.date < current && x.date > ycurrent).Count();
                        canv.Children.Add(New(i, i + 1, pr, n));
                        pr = n;
                    }
                    offsetm = ms.items.Count();
                    offsetd = Time.HowManyDaysFromThe(messgs.Last().date);
                   
                }

                
            }
            
        }

        public int graphs_count;
        public Point p;
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
               var y= e.GetPosition(Graphs).Y;
                gs.vertical -= (p.Y - y) * 0.1;

                gs.horizontal-=( p.X - e.GetPosition(Graphs).X) * 0.1;
            }
            
        }

        private void Graphs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            p = e.GetPosition(Graphs);
        }

        private void Graphs_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var s=  e.GetPosition(Graphs);
            double a = e.Delta;
            a = a / 1200;
            
            gs.zoom += a ;
            gs.zoom_x = s.X;
            gs.zoom_y = s.Y;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            dialogs auth = new dialogs();
                auth.Owner = this;
            do
            {
                LoadingState.Text = "Проходит авторизация...";
                
            }
            while ( auth.ShowDialog().Value);
            if (string.IsNullOrEmpty(Server.access_token))
            {
                LoadingState.Text = "Авторизация неудачна(";

            }
            else
            {
                LoadingState.Text = "Походу авторизация прошла успешна. можно загружать диалоги..";
                
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Graphs.Children.Clear();
            graphs_count = 0;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            graphs_count++;
            gs.standart();
            switch (graphs_count)
            {
                case 1:
                    {
                        g.brushcolor = System.Windows.Media.Brushes.LightSalmon;
                    }
                    break;
                case 2:
                    {
                        g.brushcolor = System.Windows.Media.Brushes.YellowGreen;
                    }
                    break;

                case 3:
                    {
                        g.brushcolor = System.Windows.Media.Brushes.LightSlateGray;
                    }
                    break;

                case 4:
                    {
                        g.brushcolor = System.Windows.Media.Brushes.HotPink;
                    }
                    break;
                case 5:
                    {
                        g.brushcolor = System.Windows.Media.Brushes.Khaki;
                    }
                    break;
                case 6:
                    {
                        g.brushcolor = System.Windows.Media.Brushes.Lavender;
                    }
                    break;
                case 7:
                    {
                        g.brushcolor = System.Windows.Media.Brushes.LawnGreen;
                    }
                    break;
            }
            g.Insert(this.DataContext as Messages, this.Graphs, this.LoadingState);

        }

        
    }
}
