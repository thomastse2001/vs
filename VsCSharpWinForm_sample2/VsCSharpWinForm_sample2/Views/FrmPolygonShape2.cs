using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsCSharpWinForm_sample2.Views
{
    public partial class FrmPolygonShape2 : Form
    {
        /// <summary>
        /// This is a .Net program used to generate the polygon shape of target area.
        /// You CAN skip this program and directy execute "WindowsFormsApp3.sln" to generate the result; But this is recommand
        /// to generate your polygon shape by this program and verify againts the shape in GIS, to ensure you didnt pot a wrong graph.
        /// 
        /// How to use
        /// 1. First of all, you should garthered list of points with their x and y corrdinate. open "WindowsFormsApp2.sln", put those points in the class section, like:
        /// PointF[] pt = {
        /// new PointF(834618, 820853),
        /// new PointF(834916, 821208),
        /// new PointF(834556, 820791)
        /// };
        /// 
        /// 2. By execute the program with configuration in (1); The program will generate a polygon with those points you inputted. 
        /// To verify againt the original shape in GIS, you have to "Face-Down" the printed result
        /// </summary>

        public static Helpers.TLog Logger = null;

        private float _shiftX = 0f;
        private float _shiftY = 0f;
        private float _zoom = 1.0f;
        private PointF ptCheck = new PointF(834594, 821023);//IN
        private PointF[] pt = {
            /* AREA A
                new PointF(834618, 820853),
                new PointF(834916, 821208),
                new PointF(834904, 821212),
                new PointF(834607, 821416),
                new PointF(834595, 821412),
                new PointF(834429, 821158),
                new PointF(834290, 821247),
                new PointF(834278, 821185),
                new PointF(834202, 821142),
                new PointF(834214, 821080),
                new PointF(834556, 820791)
                */

                /* AREA B
                new PointF(834208, 822245),
                new PointF(834191, 822281),
                new PointF(833971, 822458),
                new PointF(833624, 822333),
                new PointF(833644, 822293),
                new PointF(833687, 822194),
                new PointF(833794, 822125),
                new PointF(833875, 822203),
                new PointF(833951, 822147),
                new PointF(834000, 822193),
                new PointF(834097,822118)
                */

                // AREA C
                /*
                new PointF(834898, 820787),
                new PointF(834867, 820793),
                new PointF(834612, 820793),
                new PointF(834612, 820740),
                new PointF(834724, 820680),
                new PointF(834715, 820568),
                new PointF(834622, 820568),
                new PointF(834533, 820446),
                new PointF(834533, 820066),
                new PointF(834623, 820051),
                new PointF(834651, 820010),
                new PointF(834739, 820010),
                new PointF(834739, 820082),
                new PointF(834897, 819961),
                new PointF(835081, 819961),
                new PointF(835145, 820028),
                new PointF(835016, 820702)
                */

                //AREA D
                /*
                new PointF(829619,826066),
                new PointF(829550,826098),
                new PointF(829469,825979),
                new PointF(829418,825869),
                new PointF(829433,825858),
                new PointF(829607,825759),
                new PointF(829622,825706),
                new PointF(829725,825602),
                new PointF(829846,825526),
                new PointF(830088,825382),
                new PointF(830167,825400),
                new PointF(830351,825517),
                new PointF(830336,825528),
                new PointF(830162,825637),
                new PointF(830190,825773),
                new PointF(830405,825646),
                new PointF(830440,825592),
                new PointF(830455,825603),
                new PointF(830431,825688),
                new PointF(830189,825939),
                new PointF(829935,826075),
                new PointF(829935,826020),
                new PointF(829933,825856),
                new PointF(829855,825751),
                new PointF(829583,825917),
                new PointF(829636,826034)
                */        
        
                //E
                /*
                new PointF(832366,816331),
                new PointF(832545,816309),
                new PointF(832545,816267),
                new PointF(832572,816276),
                new PointF(832701,816250),
                new PointF(832833,816246),
                new PointF(832847,816310),
                new PointF(832851,816350),
                new PointF(832847,816396),
                new PointF(832890,816396),
                new PointF(832925,816418),
                new PointF(832955,816432),
                new PointF(833018,816380),
                new PointF(833052,816345),
                new PointF(833083,816336),
                new PointF(833128,816319),
                new PointF(833172,816306),
                new PointF(833228,816269),
                new PointF(833144,816176),
                new PointF(833179,816127),
                new PointF(833260,816092),
                new PointF(833323,816077),
                new PointF(833365,816077),
                new PointF(833400,816040),
                new PointF(833437,816078),
                new PointF(833513,816040),
                new PointF(833586,816001),
                new PointF(833561,815985),
                new PointF(833664,815946),
                new PointF(833701,816000),
                new PointF(833697,815970),
                new PointF(833786,816059),
                new PointF(833824,815986),
                new PointF(833883,815976),
                new PointF(833931,815985),
                new PointF(834005,816064),
                new PointF(834027,816128),
                new PointF(834060,816154),
                new PointF(834093,816190),
                new PointF(834068,816215),
                new PointF(834037,816255),
                new PointF(833934,816318),
                new PointF(833911,816334),
                new PointF(833750,816363),
                new PointF(833589,816405),
                new PointF(833564,816415),
                new PointF(833549,816430),
                new PointF(833544,816529),
                new PointF(833511,816545),
                new PointF(833450,816561),
                new PointF(833344,816590),
                new PointF(833250,816612),
                new PointF(833151,816618),
                new PointF(833008,816631),
                new PointF(832866,816636),
                new PointF(832568,816645),
                new PointF(832558,816620),
                new PointF(832552,816458),
                new PointF(832498,816453),
                new PointF(832386,816419),
                new PointF(832376,816409),
                new PointF(832374,816360)
                */

                //F
                new PointF(835348,815299),
                new PointF(835359,815287),
                new PointF(835343,815255),
                new PointF(835331,815234),
                new PointF(835342,815222),
                new PointF(835378,815211),
                new PointF(835389,815223),
                new PointF(835418,815226),
                new PointF(835486,815207),
                new PointF(835517,815173),
                new PointF(835535,815172),
                new PointF(835553,815134),
                new PointF(835564,815146),
                new PointF(835601,815182),
                new PointF(835679,815125),
                new PointF(835738,815108),
                new PointF(835786,815114),
                new PointF(835876,815072),
                new PointF(835884,815046),
                new PointF(835940,815028),
                new PointF(835948,814988),
                new PointF(835925,814934),
                new PointF(835907,814870),
                new PointF(835969,814873),
                new PointF(836062,814953),
                new PointF(836089,815050),
                new PointF(836078,815062),
                new PointF(835993,815151),
                new PointF(836020,815213),
                new PointF(836083,815298),
                new PointF(836201,815293),
                new PointF(836300,815287),
                new PointF(836341,815324),
                new PointF(836407,815335),
                new PointF(836418,815347),
                new PointF(836415,815414),
                new PointF(836467,815417),
                new PointF(836537,815418),
                new PointF(836557,815425),
                new PointF(836568,815437),
                new PointF(836560,815484),
                new PointF(836546,815549),
                new PointF(836557,815561),
                new PointF(836510,815674),
                new PointF(836490,815694),
                new PointF(836398,815676),
                new PointF(836307,815653),
                new PointF(836217,815615),
                new PointF(836151,815625),
                new PointF(835949,815621),
                new PointF(835898,815616),
                new PointF(835827,815615),
                new PointF(835617,815621),
                new PointF(835605,815551),
                new PointF(835606,815470),
                new PointF(835603,815404),
                new PointF(835400,815432),
                new PointF(835356,815435),
                new PointF(835356,815411),
                new PointF(835337,815354),
                new PointF(835414,815371),
                new PointF(835364,815330)


                //G
                /*
                new PointF(841384,819755),
                new PointF(841434,819727),
                new PointF(841391,819902),
                new PointF(841063,819941),
                new PointF(840952,819801),
                new PointF(840823,819716),
                new PointF(840784,819661),
                new PointF(840887,819538),
                new PointF(840861,819502),
                new PointF(840980,819452),
                new PointF(841557,819232),
                new PointF(841607,819292),
                new PointF(841664,819370),
                new PointF(841647,819523),
                new PointF(841533,819509),
                new PointF(841490,819655)
                */

                //H
                /*
                new PointF(836791,818723),
                new PointF(836806,818681),
                new PointF(836997,818540),
                new PointF(837017,818510),
                new PointF(837136,818562),
                new PointF(837231,818534),
                new PointF(837310,818570),
                new PointF(837342,818656),
                new PointF(837332,818686),
                new PointF(837323,818726),
                new PointF(837377,818831),
                new PointF(837371,819068),
                new PointF(837480,819068),
                new PointF(837565,819188),
                new PointF(838070,819188),
                new PointF(838090,819934),
                new PointF(838141,820061),
                new PointF(838141,820391),
                new PointF(837676,820528),
                new PointF(837463,820642),
                new PointF(837433,820622),
                new PointF(837326,820434),
                new PointF(837449,820398),
                new PointF(837372,820360),
                new PointF(837367,820179),
                new PointF(837247,820170),
                new PointF(837247,819605),
                new PointF(837300,819612),
                new PointF(837278,819469),
                new PointF(837184,819299),
                new PointF(837105,819202),
                new PointF(836912,818974),
                new PointF(836920,818844)
                */

                //I
                /*
                new PointF(835332,818976),
                new PointF(835447,818889),
                new PointF(835672,818838),
                new PointF(835692,818858),
                new PointF(835626,819306),
                new PointF(835631,819340),
                new PointF(835700,819350),
                new PointF(835681,819451),
                new PointF(835654,819496),
                new PointF(835620,819596),
                new PointF(835895,819649),
                new PointF(835945,819669),
                new PointF(835945,819714),
                new PointF(836009,819782),
                new PointF(835890,820118),
                new PointF(835815,820118),
                new PointF(835754,820118),
                new PointF(835728,820106),
                new PointF(835675,820253),
                new PointF(835158,820144),
                new PointF(835128,820114)
                */
        };

        public FrmPolygonShape2()
        {
            InitializeComponent();
        }

        private void FrmPolygonShape2_Load(object sender, EventArgs e)
        {
        }

        private bool IsPointInsideTrapezoid(float minX, float minY, float maxX, float maxY, PointF pointToTest)
        {
            if (pointToTest.X < 0.0f || pointToTest.Y < 0.0f)
                return false;

            float slopeAB = minY / minX;
            float slopeAToPoint = pointToTest.Y / (pointToTest.X - minX);
            if (Math.Abs(slopeAToPoint) < Math.Abs(slopeAB) && slopeAToPoint < 0.0f)
                return false;

            float slopeCD = maxY / maxX;
            float slopeCToPoint = pointToTest.Y / (pointToTest.X - maxX);
            if (Math.Abs(slopeCToPoint) > Math.Abs(slopeCD) || slopeCToPoint > 0.0f)
                return false;

            return true;
        }

        private void FrmPolygonShape2_MouseClick(object sender, MouseEventArgs e)
        {
            ptCheck = new PointF(e.X / _zoom + _shiftX, e.Y / _zoom + _shiftY);
            this.Invalidate();
        }

        private void FrmPolygonShape2_MouseMove(object sender, MouseEventArgs e)
        {
            /// get current MousPos relative to the minima of the trapez and zoom
            PointF ptCur = new PointF(e.X / _zoom + _shiftX, e.Y / _zoom + _shiftY);
            this.Text = this.Text.Split(new string[] { " ### " }, StringSplitOptions.RemoveEmptyEntries)[0] + " ### " + ptCur.ToString();
        }

        private void FrmPolygonShape2_Paint(object sender, PaintEventArgs e)
        {
            /// zoom and shift the output
            Form f = (Form)sender;
            double d = f.ClientSize.Width / (double)f.ClientSize.Height;

            _shiftX = pt.Min(a => a.X);
            _shiftY = pt.Min(a => a.Y);
            float distX = Math.Abs(pt.Max(a => a.X) - _shiftX);
            float distY = Math.Abs(pt.Max(a => a.Y) - _shiftY);
            double factor = distX / distY;

            _zoom = 1.0f;
            _zoom = factor >= d ? (float)(f.ClientSize.Width / distX) : (float)(f.ClientSize.Height / distY);

            e.Graphics.ScaleTransform(_zoom, _zoom);
            e.Graphics.TranslateTransform(-_shiftX * _zoom, -_shiftY * _zoom, System.Drawing.Drawing2D.MatrixOrder.Append);

            /// make nicer look
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            /// setup the path and test for being inside
            using (System.Drawing.Drawing2D.GraphicsPath myPath = new System.Drawing.Drawing2D.GraphicsPath())
            {
                myPath.AddLines(pt);
                myPath.CloseFigure();

                using (Pen pen = new Pen(Color.Blue, 4))
                    e.Graphics.DrawPath(pen, myPath);

                using (Pen pen = new Pen(Color.Blue))
                {
                    /// IsoutlineVisible is not needed normally
                    if (myPath.IsVisible(ptCheck) || myPath.IsOutlineVisible(ptCheck, pen))
                        this.Text = "inside - Origin at " + new PointF(_shiftX, _shiftY).ToString() + "; ZoomFacotr: ";// + _zoom.ToString();
                    else
                        this.Text = "outside - Origin at " + new PointF(_shiftX, _shiftY).ToString() + "; ZoomFacotr: ";// + _zoom.ToString();
                }
            }

            /// draw the testpoint as a small cross
            using (Pen pen = new Pen(Color.Red, 4))
            {
                e.Graphics.DrawLine(pen, ptCheck.X - 5 / _zoom, ptCheck.Y, ptCheck.X + 5 / _zoom, ptCheck.Y);
                e.Graphics.DrawLine(pen, ptCheck.X, ptCheck.Y - 5 / _zoom, ptCheck.X, ptCheck.Y + 5 / _zoom);
            }
        }
    }
}
