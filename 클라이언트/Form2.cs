using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Data;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Threading;
using ClassLibrary1;
using System.Runtime.Serialization;


namespace WindowsFormsApp5
{

    public partial class Form2 : System.Windows.Forms.Form
    {
        private string ip;
        private int port;
        private bool line;
        private bool rect;
        private bool circle;
        private bool fill;
        private Point start;
        private Point finish;
        private Pen pen;
        private SolidBrush brush;
        private int nline;
        private int nrect;
        private int ncircle;
        private int n;
        private int i;
        private int thick;
        private bool isSolid;
        private MyLines[] mylines;
        private MyRect[] myrect;
        private MyCircle[] mycircle;
        private bool m_bConnect = false;
        private NetworkStream m_networkstream;
        private TcpClient m_client;
        private Thread m_thread;
        private byte[] sendBuffer = new byte[1024 * 4];
        private byte[] readBuffer = new byte[1024 * 4];
        private string id;
        private int mode;
        private float ratio;
        private bool mousing;
        private bool handbuttonon;

        private void mousewheel(object sender, MouseEventArgs e)
        {
            int lines = e.Delta * SystemInformation.MouseWheelScrollLines;
            if (lines > 0)
            {
                ratio *= 1.1f;
                if (ratio > 10.0) ratio = 10.0f;
            }
            if (lines < 0)
            {
                ratio *= 0.9f;
                if (ratio <1) ratio = 1.0f;
            }
            panel1.Height =(int)(panel1.Height* ratio);
            panel1.Width = (int)(panel1.Width * ratio);

            for (int j = 0; j < n; j++)
            {
                for(int i=0; i<nline; i++)
                {
                    if(j==mylines[i].getn())
                    mylines[i].setPoint(mylines[i].getPoint1(), mylines[i].getPoint2(), mylines[i].getThick(),mylines[i].getSolid(), mylines[i].getPen(), mylines[i].getn());
                }
                for (int i = 0; i < ncircle; i++)
                {
                    if (j == mycircle[i].getn())
                     mycircle[i].setRectc(mycircle[i].getPoint1(), mycircle[i].getPoint2(), mycircle[i].getThick(), mycircle[i].getSolid(), mycircle[i].getPen(), mycircle[i].getBrush(), mycircle[i].getfill(), mycircle[i].getn());
                }
                for (int i = 0; i < nrect; i++)
                {
                    if (j == myrect[i].getn())
                    myrect[i].setRect(myrect[i].getPoint1(), myrect[i].getPoint2(), myrect[i].getThick(), myrect[i].getSolid(), myrect[i].getPen(), myrect[i].getBrush(), myrect[i].getfill(), myrect[i].getn());
                }
            }
            panel1.Invalidate();
            panel1.Update();
        }
        public Form2(string s1, string s2, string s3)
        {
            ip = s1;
            port = Convert.ToInt32(s2);
            id = s3;
            InitializeComponent();
            SetupVar();
            panel1.MouseWheel += new MouseEventHandler(mousewheel);
            this.m_client = new TcpClient();
            try
            {
                this.m_client.Connect(ip, port);
            }
            catch
            {
                MessageBox.Show("접속 에러");
                return;
            }
            this.m_bConnect = true;
            this.m_networkstream = this.m_client.GetStream();

            m_thread = new Thread(new ThreadStart(Receive));
            m_thread.Start();
            Text text = new Text();
            text.Type = (int)PacketType.문자열;
            text.mode = 2;
            text.id = id;
            Packet.Serialize(text).CopyTo(this.sendBuffer, 0);
            this.Send();
        }

        public void Receive()
        {
            while (true)
            {
                try
                {
                    m_networkstream.Read(readBuffer, 0, 1024 * 4);
                    Packet packet = (Packet)Packet.Desserialize(this.readBuffer);
                    switch ((int)packet.Type)
                    {
                        case (int)PacketType.문자열:
                            {
                                Text text = (Text)Packet.Desserialize(this.readBuffer);
                                if (text.Type == 0)
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        this.textBox1.AppendText(text.id+":"+text.str + "\n");
                                    }));

                                break;
                            }
                        case (int)PacketType.그림:
                            {
                                image img = (image)Packet.Desserialize(this.readBuffer);
                                if (img.mode == 1)
                                {
                                    mylines[nline].setPoint(img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.n);
                                    nline++;
                                }
                                if (img.mode == 2)
                                {
                                    myrect[nrect].setRect(img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.brush, img.fill, img.n);
                                    nrect++;
                                }
                                if (img.mode == 3)
                                {
                                    mycircle[ncircle].setRectc(img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.brush, img.fill, img.n);
                                    ncircle++;
                                }
                                n++;
                                panel1.Invalidate();
                                break;
                            }
                    }
                }
                catch
                {
                    textBox1.AppendText("데이터를 읽는 과정에서 오류가 발생");
                }
            }
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Text text = new Text();
            text.Type = (int)PacketType.문자열;
            text.mode = 1;
            text.id = id;
            Packet.Serialize(text).CopyTo(this.sendBuffer, 0);
            Send();
            this.m_client.Close();
            this.m_networkstream.Close();
            this.m_thread.Abort();
            this.m_bConnect = false;
            
        }
        private void SetupVar()
        {
            i = 0;
            mousing = false;
            ratio = 1.0f;
            thick = 1;
            isSolid = true;
            line = false;
            rect = false;
            circle = false;
            handbuttonon = false;
            start = new Point(0, 0);
            finish = new Point(0, 0);
            pen = new Pen(Color.Black);
            mylines = new MyLines[100];
            myrect = new MyRect[100];
            mycircle = new MyCircle[100];
            nline = 0;
            nrect = 0;
            ncircle = 0;
            n = 0;
            fill = false;
            brush = new SolidBrush(Color.White);
            colorDialog1.Color = toolStripButton1.BackColor;
            colorDialog2.Color = toolStripButton2.BackColor;
            SetupMine();
        }
        private void SetupMine()
        {
            for (i = 0; i < 100; i++)
                mylines[i] = new MyLines();
            for (i = 0; i < 100; i++)
                myrect[i] = new MyRect();
            for (i = 0; i < 100; i++)
                mycircle[i] = new MyCircle();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            toolStripButton1.BackColor = colorDialog1.Color;
            pen.Color = toolStripButton1.BackColor;

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            toolStripButton2.BackColor = colorDialog2.Color;
            brush.Color = toolStripButton2.BackColor;
        }

        private void rectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Image = rectToolStripMenuItem.Image;
            line = false;
            rect = true;
            circle = false;
            handbuttonon = false;
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Image = circleToolStripMenuItem.Image;
            line = false;
            rect = false;
            circle = true;
            handbuttonon = false;
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Image = lineToolStripMenuItem.Image;
            line = true;
            rect = false;
            circle = false;
            handbuttonon = false;
        }

        private void line1Button_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton2.Image = line1Button.Image;
            thick = 1;
        }

        private void line2Button_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton2.Image = line2Button.Image;
            thick = 2;
        }

        private void line3Button_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton2.Image = line3Button.Image;
            thick = 3;
        }

        private void line4Button_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton2.Image = line4Button.Image;
            thick = 4;
        }

        private void line5Button_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton2.Image = line5Button.Image;
            thick = 5;
        }
        private void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            start.X = (int)((e.X + panel1.Left) / ratio);
            start.Y = (int)((e.Y + panel1.Top) / ratio);
            mousing = true;
        }
        private void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((start.X == 0) && (start.Y == 0))
                return;
            finish.X = (int)((e.X + panel1.Left)/ratio) ;
            finish.Y = (int)((e.Y + panel1.Top)/ratio);
            /*if (mousing && handbuttonon)
            {

                int changeX = (int)((e.X - start.X)/ratio);
                int changeY = (int)((e.Y - start.Y) / ratio);
                panel1.Left = changeX;
                panel1.Top = changeY;
                
            }*/

            if (line == true)
            {
                mylines[nline].setPoint(start, finish, thick, isSolid,pen.Color,n);
            }
            if (rect == true)
            {
                myrect[nrect].setRect(start, finish, thick, isSolid,pen.Color,brush.Color,fill,n);
            }
            if (circle == true)
            {
                mycircle[ncircle].setRectc(start, finish, thick, isSolid,pen.Color,brush.Color,fill,n);
            }
            panel1.Invalidate();
            panel1.Update();
        }
        private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.ScaleTransform(ratio,ratio);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            for (int j = 0; j <= n; j++)
            {
                for (i = 0; i <= nline; i++)
                {
                    if (mylines[i].getn() == j)
                    {
                        if (!mylines[i].getSolid())
                        {
                            pen.Width = 1;
                            pen.DashStyle = DashStyle.Dot;
                            pen.Color = mylines[i].getPen();
                        }
                        else
                        {
                            pen.Color = mylines[i].getPen();
                            pen.Width = mylines[i].getThick()*ratio;
                            pen.DashStyle = DashStyle.Solid;
                        }
                        Point p1 = mylines[i].getPoint1();
                        Point p2 = mylines[i].getPoint2();
                        e.Graphics.DrawLine(pen, p1,p2);

                    }
                }
                for (i = 0; i <= nrect; i++)
                {
                    if (myrect[i].getn() == j)
                    {
                        if (!myrect[i].getSolid())
                        {
                            pen.Width = 1;
                            pen.Color = myrect[i].getPen();
                            brush.Color = myrect[i].getBrush();
                            pen.DashStyle = DashStyle.Dot;
                        }
                        else
                        {
                            pen.Color = myrect[i].getPen();
                            pen.Width = myrect[i].getThick();
                            brush.Color = myrect[i].getBrush();
                            pen.DashStyle = DashStyle.Solid;
                        }
                        Rectangle r1 = myrect[i].getRect();
                        e.Graphics.DrawRectangle(pen, r1);
                        if (myrect[i].getfill())
                        {
                            e.Graphics.FillRectangle(brush, r1);
                        }

                    }
                }
                for (i = 0; i <= ncircle; i++)
                {
                    if (mycircle[i].getn() == j)
                    {
                        if (!mycircle[i].getSolid())
                        {
                            pen.Width = 1*ratio;
                            pen.DashStyle = DashStyle.Dot;
                            brush.Color = mycircle[i].getBrush();
                            pen.Color = mycircle[i].getPen();
                        }
                        else
                        {
                            brush.Color = mycircle[i].getBrush();
                            pen.Color = mycircle[i].getPen();
                            pen.Width = mycircle[i].getThick();
                            pen.DashStyle = DashStyle.Solid;
                        }
                        Rectangle r1 = mycircle[i].getRectC();
                        e.Graphics.DrawEllipse(pen, r1);
                        if (mycircle[i].getfill())
                        {
                            e.Graphics.FillEllipse(brush, r1);
                        }
                    }
                }
            }
            pen.Width = thick;
            pen.DashStyle = DashStyle.Solid;
            pen.Color = colorDialog1.Color;
            if (fill == true)
            {
                brush.Color = colorDialog2.Color;
            }

        }
        private void panel1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mousing = false;
            if (line == true)
            {
                mode = 1;
                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = mode;
                img.n=mylines[nline].getn();
                img.pen=mylines[nline].getPen();
                img.point[0]= mylines[nline].getPoint1();
                img.point[1] = mylines[nline].getPoint2();
                img.isSolid=mylines[nline].getSolid();
                img.thick=mylines[nline].getThick();
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();

                
            }
            if (rect == true)
            {
                mode = 2;
                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = mode;
                img.point[0] = myrect[nrect].getPoint1();
                img.point[1] = myrect[nrect].getPoint2();
                img.rect = myrect[nrect].getRect();
                img.brush = myrect[nrect].getBrush();
                img.fill = myrect[nrect].getfill();
                img.n = myrect[nrect].getn();
                img.pen = myrect[nrect].getPen();
                img.isSolid = myrect[nrect].getSolid();
                img.thick = myrect[nrect].getThick();
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();


            }
            if (circle == true)
            {
                mode = 3;
                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = mode;
                img.point[0] = mycircle[ncircle].getPoint1();
                img.point[1] = mycircle[ncircle].getPoint2();
                img.rectC = mycircle[ncircle].getRectC();
                img.brush = mycircle[ncircle].getBrush();
                img.fill = mycircle[ncircle].getfill();
                img.n = mycircle[ncircle].getn();
                img.pen = mycircle[ncircle].getPen();
                img.isSolid = mycircle[ncircle].getSolid();
                img.thick = mycircle[ncircle].getThick();
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();

            }
            if (!handbuttonon)
            {
                start.X = 0;
                start.Y = 0;
                finish.X = 0;
                finish.Y = 0;
                fill = false;
            }

        }

        private void tool_Click(object sender, EventArgs e)
        {
            fill = true;
        }
        public void Send()
        {
            this.m_networkstream.Write(this.sendBuffer, 0, this.sendBuffer.Length);
            this.m_networkstream.Flush();
            for(int i=0; i<this.sendBuffer.Length; i++)
            {
                this.sendBuffer[i] = 0;
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Text text = new Text();
            text.Type = (int)PacketType.문자열;
            text.mode = 0;
            text.str = textBox2.Text;
            text.id = id;
            Packet.Serialize(text).CopyTo(this.sendBuffer, 0);
            this.Send();
            textBox2.Text = "";
        }

        private void handToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Image = handToolStripMenuItem.Image;
            handbuttonon = true;
            line = false;
            rect = false;
            circle = false;
        }

        private void handToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Image = handToolStripMenuItem.Image;
            handbuttonon = true;
            line = false;
            rect = false;
            circle = false;
        }
    }
    
}
