using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace ClassLibrary1
{
    public enum PacketType
    {
        문자열,
        그림
    }
    public enum PacketSendERROR
    {
        정상 = 0,
        에러
    }
    [Serializable]
    public class Packet
    {
        public int Length;
        public int Type;
        public Packet()
        {
            this.Length = 0;
            this.Type = 0;
        }
        public static byte[] Serialize(Object o)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
        public static Object Desserialize(byte[] bt)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            foreach (byte b in bt)
            {
                ms.WriteByte(b);
            }
            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            Object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }

    }
    [Serializable]
    public class Text : Packet
    {
        public string str;
        public string id;
        public int mode;
        public Text()
        {
            this.str = null;
            this.id = null;
        }
    }
    [Serializable]
    public class image : Packet
    {
        public int mode;
        public Point[] point = new Point[2];
        public int thick;
        public bool isSolid;
        public Color pen;
        public int n;
        public Rectangle rectC;
        public Rectangle rect;
        public Color brush;
        public bool fill;
        public image()
        {

            point[0] = new Point();
            point[1] = new Point();
            thick = 1;
            isSolid = true;
            rectC = new Rectangle();
            rect = new Rectangle();
        }

    }


}
