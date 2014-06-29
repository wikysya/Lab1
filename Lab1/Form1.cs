using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace Lab1
{
    public partial class Form1 : Form
    {
        private GraphPane myPane;
        private double a = double.NaN, b = double.NaN, ε, l;
        public Form1()
        {
            InitializeComponent();
            myPane = zedGraphControl1.GraphPane;
        }

        public void dihotomiya(object sender, EventArgs e)
        {
            #region если это первый шаг то считываем заданные значения
            if (double.IsNaN(a) || double.IsNaN(b))
                button2_Click(sender, e);
            #endregion

            if (Math.Abs(b - a) >= l)
            {
                #region удаляем из памяти все линии кроме графика
                int i = myPane.CurveList.Count - 1;
                while (i >= 1)
                {
                    myPane.CurveList.RemoveAt(i);
                    i--;
                }
                #endregion
                #region добавляем границы отрезка a и b
                PointPairList listA = new PointPairList();
                PointPairList listB = new PointPairList();
                listA.Add(a, -20);
                listA.Add(a, 80);
                listB.Add(b, -20);
                listB.Add(b, 80);
                LineItem myCurve = myPane.AddCurve("a", listA, Color.Magenta, SymbolType.None);
                myCurve = myPane.AddCurve("b", listB, Color.DeepSkyBlue, SymbolType.None);
                #endregion

                double yk, zk, fyk, fzk;
                yk = (a + b - ε) / 2;
                zk = (a + b + ε) / 2;
                fyk = yk * yk + 3 * yk - 4;  //ФОРМУЛА!!!!!!!!!!!
                fzk = zk * zk + 3 * zk - 4;  //ФОРМУЛА!!!!!!!!!!!
                #region добавляем линии Yk и Zk
                PointPairList listYk = new PointPairList();
                PointPairList listZk = new PointPairList();
                listYk.Add(yk, -20);
                listYk.Add(yk, 80);
                listZk.Add(zk, -20);
                listZk.Add(zk, 80);
                myCurve = myPane.AddCurve("Yk", listYk, Color.ForestGreen, SymbolType.None);
                myCurve = myPane.AddCurve("Zk", listZk, Color.Lime, SymbolType.None);
                #endregion

                if (fyk > fzk)
                {
                    #region добавляем линии вычеркивающие область
                    PointPairList list1 = new PointPairList();
                    PointPairList list2 = new PointPairList();
                    list1.Add(a, -20);
                    list1.Add(yk, 80);
                    list2.Add(a, 80);
                    list2.Add(yk, -20);
                    myCurve = myPane.AddCurve("", list1, Color.OrangeRed, SymbolType.None);
                    myCurve.Line.Width = 2.0F;
                    myCurve.Label.IsVisible = false;
                    myCurve = myPane.AddCurve("", list2, Color.OrangeRed, SymbolType.None);
                    myCurve.Line.Width = 2.0F;
                    myCurve.Label.IsVisible = false;
                    #endregion
                    a = yk;
                }
                else
                {
                    #region добавляем линии вычеркивающие область
                    PointPairList list1 = new PointPairList();
                    PointPairList list2 = new PointPairList();
                    list1.Add(b, -20);
                    list1.Add(zk, 80);
                    list2.Add(b, 80);
                    list2.Add(zk, -20);
                    myCurve = myPane.AddCurve("", list1, Color.OrangeRed, SymbolType.None);
                    myCurve.Line.Width = 2.0F;
                    myCurve.Label.IsVisible = false;
                    myCurve = myPane.AddCurve("", list2, Color.OrangeRed, SymbolType.None);
                    myCurve.Line.Width = 2.0F;
                    myCurve.Label.IsVisible = false;
                    #endregion
                    b = zk;
                }

                #region перерисовываем график
                zedGraphControl1.AxisChange();
                zedGraphControl1.Refresh();
                zedGraphControl1.Visible = true;
                #endregion
            }
            else
            {
                double xmin = a + (b - a) / 2;
                double fmin = xmin * xmin + 3 * xmin - 4;   //ФОРМУЛА!!!!!!!!!!!
                #region округление значений до заданной точности
                int c; 
                for (c = 0; l < 1; c++) //узнаем заданную точность, количество знаков после запятой
                    l*= 10;
                
                fmin =Math.Round(fmin, c);
                xmin=Math.Round(xmin, c);
                #endregion

                MessageBox.Show("x="+Convert.ToString(xmin)+"\nFmin="+Convert.ToString(fmin),"Ответ");
                button3.Enabled = false; //кнопка Шаг недоступна
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateGraph();
            SetSize();
        }

        /// <summary>
        /// Рисует график функции
        /// </summary>
        private void CreateGraph()
        {
            myPane.Title.Text = "График функции";
            myPane.XAxis.Title.Text = "Ось Ox";
            myPane.YAxis.Title.Text = "Ось Oy";
            myPane.XAxis.Scale.Min = -10;
            myPane.XAxis.Scale.Max = 7;
            myPane.YAxis.Scale.Min = -20;
            myPane.YAxis.Scale.Max = 80;
            myPane.Legend.Position = ZedGraph.LegendPos.Right; //расположение легенды

            #region добавляем линии сетки и делаем их серыми
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = Color.Gray;
            myPane.YAxis.MajorGrid.Color = Color.Gray;
            #endregion

            PointPairList list1 = new PointPairList();
            for (double x = -10; x <= 7; x += 0.0001)
            {
                list1.Add(x, x * x + 3 * x - 4);   //ФОРМУЛА!!!!!!!!!!!
            }
            LineItem myCurve = myPane.AddCurve("y = x^2 + 3x - 4", list1, Color.Blue, SymbolType.None);
            myCurve.Line.Width = 2.0F;
            zedGraphControl1.AxisChange();
        }

        private void SetSize()
        {
            zedGraphControl1.Location = new Point(10, 10);
            zedGraphControl1.Size = new Size(ClientRectangle.Width - 180, ClientRectangle.Height - 20);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            a = Convert.ToDouble(this.numericUpDown1.Value);
            b = Convert.ToDouble(this.numericUpDown2.Value);
            ε = Convert.ToDouble(this.numericUpDown3.Value);
            l = Convert.ToDouble(this.numericUpDown4.Value);
            button3.Enabled = true;
        }

        /// <summary>
        /// Перерисовывает линии при зуммировании
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        private void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            zedGraphControl1.Visible = true;
        }
    }
}
