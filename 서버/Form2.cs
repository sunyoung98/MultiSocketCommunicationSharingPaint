using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary1;

namespace WindowsFormsApp4
{
    public partial class Form2 : Form
    {

        private string ip;
        private int port;
        private NetworkStream m_networkstream;
        private TcpListener m_listener;
        private bool m_bClientOn = false;
        private Thread m_thread;
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
        int nchild = 0;
        private byte[] sendBuffer = new byte[1024 * 4];
        private byte[] readBuffer = new byte[1024 * 4];
        private List<TcpClient> clients;
        private List<string> clientsname;
        public List<image> images;

        public Form2(string s1, string s2)
        {

            ip = s1;
            port = Convert.ToInt32(s2);
            InitializeComponent();
            this.m_thread = new Thread(new ThreadStart(RUN));
            this.m_thread.Start();
            this.clients = new List<TcpClient>();
            this.clientsname = new List<string>();
            this.images = new List<image>();
            SetupVar();
            FileStream stream = File.Open("data.txt", FileMode.OpenOrCreate);
            if (stream.Length > 0)
            {
                    BinaryFormatter formatter = new BinaryFormatter();
                    images = (List<image>)formatter.Deserialize(stream);
                    for (int i = 0; i < images.Count; i++)
                    {
                        if (images[i].mode == 1)
                        {
                            mylines[nline].setPoint(images[i].point[0], images[i].point[1], images[i].thick, images[i].isSolid, images[i].pen, images[i].n);
                            SendLine(images[i].mode, images[i].point[0], images[i].point[1], images[i].thick, images[i].isSolid, images[i].pen, images[i].n);
                            nline++;
                        }
                        if (images[i].mode == 2)
                        {
                            myrect[nrect].setRect(images[i].point[0], images[i].point[1], images[i].thick, images[i].isSolid, images[i].pen, images[i].brush, images[i].fill, images[i].n);
                            nrect++;
                            SendRC(images[i].mode, images[i].point[0], images[i].point[1], images[i].thick, images[i].isSolid, images[i].pen, images[i].brush, images[i].fill, images[i].n);
                        }
                        if (images[i].mode == 3)
                        {
                            mycircle[ncircle].setRectc(images[i].point[0], images[i].point[1], images[i].thick, images[i].isSolid, images[i].pen, images[i].brush, images[i].fill, images[i].n);
                            ncircle++;
                            SendRC(images[i].mode, images[i].point[0], images[i].point[1], images[i].thick, images[i].isSolid, images[i].pen, images[i].brush, images[i].fill, images[i].n);
                        }
                        n++;
                    }
                }

            stream.Close();
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (images.Count > 0)
            {
                Stream stm = File.Open("Data.txt", FileMode.Create, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stm, images);
                stm.Close();
            }
            this.m_listener.Stop();
            this.m_networkstream.Close();
            this.m_thread.Abort();
            
        }

        public void Send()
        {

            this.m_networkstream.Write(this.sendBuffer, 0, this.sendBuffer.Length);
            this.m_networkstream.Flush();
            for (int i = 0; i < this.sendBuffer.Length; i++)
            {
                this.sendBuffer[i] = 0;
            }

        }
        private void SendFirst(TcpClient client)
        {
            m_networkstream = client.GetStream();
            for (int i=0; i<nline; i++)
            {
                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = 1;
                img.point[0] = mylines[i].getPoint1();
                img.point[1] = mylines[i].getPoint2();
                img.thick = mylines[i].getThick();
                img.isSolid = mylines[i].getSolid();
                img.pen = mylines[i].getPen();
                img.n = mylines[i].getn();
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();
            }
            for (int i = 0; i < ncircle; i++)
            {
                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = 3;
                img.point[0] = mycircle[i].getPoint1();
                img.point[1] = mycircle[i].getPoint2();
                img.thick = mycircle[i].getThick();
                img.isSolid = mycircle[i].getSolid();
                img.pen = mycircle[i].getPen();
                img.brush = mycircle[i].getBrush();
                img.fill = mycircle[i].getfill();
                img.n = mycircle[i].getn();
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();
            }
            for (int i = 0; i < nrect; i++)
            {
                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = 2;
                img.point[0] = myrect[i].getPoint1();
                img.point[1] = myrect[i].getPoint2();
                img.thick = myrect[i].getThick();
                img.isSolid = myrect[i].getSolid();
                img.pen = myrect[i].getPen();
                img.brush = myrect[i].getBrush();
                img.fill = myrect[i].getfill();
                img.n = myrect[i].getn();
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();
            }
        }
        public void SendAll(string str,string id)
        {
            foreach(var client in clients)
            {
                m_networkstream = client.GetStream();

                Text text = new Text();
                text.Type = (int)PacketType.문자열;
                text.str = str;
                text.id = id;
                Packet.Serialize(text).CopyTo(this.sendBuffer, 0);
                this.Send();
            }
        }
        private void SendRC(int mode, Point point1, Point point2, int thick, bool isSolid, Color pen, Color brush, bool fill, int n)
        {
            foreach (var client in clients)
            {
                m_networkstream = client.GetStream();

                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = mode;
                img.point[0] = point1;
                img.point[1] = point2;
                img.thick = thick;
                img.isSolid = isSolid;
                img.pen = pen;
                img.brush = brush;
                img.fill = fill;
                img.n = n;
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();
            }
        }

        private void SendLine(int mode, Point point1, Point point2, int thick, bool isSolid, Color pen, int n)
        {
            foreach (var client in clients)
            {
                m_networkstream = client.GetStream();

                image img = new image();
                img.Type = (int)PacketType.그림;
                img.mode = mode;
                img.point[0] = point1;
                img.point[1] = point2;
                img.thick = thick;
                img.isSolid = isSolid;
                img.pen = pen;
                img.n = n;
                Packet.Serialize(img).CopyTo(this.sendBuffer, 0);
                this.Send();
            }

        }

        public void Receive(object networkStream)
        {
            NetworkStream stream = (NetworkStream)networkStream;
            while (true)
            {
                try
                {
                    stream.Read(readBuffer, 0, 1024 * 4);
                    Packet packet = (Packet)Packet.Desserialize(this.readBuffer);
                    switch ((int)packet.Type)
                    {
                        case (int)PacketType.문자열:
                            {
                                Text text = (Text)Packet.Desserialize(this.readBuffer);
                                if (text.mode == 0)
                                {
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        this.textBox1.AppendText(text.id + ":" + text.str + "\n");
                                    }));
                                    SendAll(text.str, text.id);
                                }
                                if (text.mode == 2)
                                {
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        textBox1.AppendText(text.id + "\n");
                                        clientsname.Add(text.id);
                                    }));
                                }
                                if (text.mode == 1)
                                {                                 
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        textBox1.AppendText("클라이언트 접속 종료..." + nchild.ToString()+"\n");                                        
                                    }));

                                    nchild--;
                                    int index = clientsname.IndexOf(text.id);
                                    clients[index].Close();
                                    clients.RemoveAt(index);
                                    clientsname.RemoveAt(index);
                                    stream.Close();

                                    return;
                                }
                            
                                break;
                            }
                        case (int)PacketType.그림:
                            {
                                image img = (image)Packet.Desserialize(this.readBuffer);
                                images.Add(img); 
                                if (img.mode == 1)
                                {
                                    mylines[nline].setPoint(img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.n);
                                    SendLine(img.mode, img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.n);
                                    nline++;
                                }
                                if (img.mode == 2)
                                {
                                    myrect[nrect].setRect(img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.brush, img.fill, img.n);
                                    nrect++;
                                    SendRC(img.mode, img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.brush, img.fill, img.n);
                                }
                                if (img.mode == 3)
                                {
                                    mycircle[ncircle].setRectc(img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.brush, img.fill, img.n);
                                    ncircle++;
                                    SendRC(img.mode, img.point[0], img.point[1], img.thick, img.isSolid, img.pen, img.brush, img.fill, img.n);
                                }
                                n++;
                                panel1.Invalidate();
                                break;
                            }
                    }
                }
                catch(Exception e)
                {
                    textBox1.AppendText("데이터를 읽는 과정에서 오류가 발생\n");
                    textBox1.AppendText(e.Message);
                }
            }
        }



        public void RUN()
        {
            this.m_listener = new TcpListener(port);
            this.m_listener.Start();
            if (!this.m_bClientOn)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    textBox1.AppendText("클라이언트 접속 대기중.... \n");
                }));
            }
            while (true)
            {
                TcpClient client = this.m_listener.AcceptTcpClient();

                if (client.Connected)
                {

                    this.m_bClientOn = true;
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        textBox1.AppendText("클라이언트 접속.... " + nchild.ToString()+"\n");
                    }));
                    m_networkstream = client.GetStream();
                    Thread Receivethread = new Thread(new ParameterizedThreadStart(Receive));
                    Receivethread.Start(m_networkstream);
                    nchild++;
                    clients.Add(client);
                    SendFirst(client);


                }

            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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
                                pen.Width = mylines[i].getThick();
                                pen.DashStyle = DashStyle.Solid;
                            }
                            e.Graphics.DrawLine(pen, mylines[i].getPoint1(), mylines[i].getPoint2());

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
                            e.Graphics.DrawRectangle(pen, myrect[i].getRect());
                            if (myrect[i].getfill())
                            {
                                e.Graphics.FillRectangle(brush, myrect[i].getRect());
                            }

                        }
                    }
                    for (i = 0; i <= ncircle; i++)
                    {
                        if (mycircle[i].getn() == j)
                        {
                            if (!mycircle[i].getSolid())
                            {
                                pen.Width = 1;
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
                            e.Graphics.DrawEllipse(pen, mycircle[i].getRectC());
                            if (mycircle[i].getfill())
                            {
                                e.Graphics.FillEllipse(brush, mycircle[i].getRectC());
                            }
                        }
                    }
                }
                pen.Width = thick;
                pen.DashStyle = DashStyle.Solid;

        }

        private void SetupVar()
        {
            i = 0;
            thick = 1;
            isSolid = true;    
            pen = new Pen(Color.Black);
            mylines = new MyLines[100];
            myrect = new MyRect[100];
            mycircle = new MyCircle[100];
            nline = 0;
            nrect = 0;
            ncircle = 0;
            n = 0;
            brush = new SolidBrush(Color.White);
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
    }
        
}
