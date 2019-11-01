using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imagelab
{
    public partial class Form1 : Form
    {
        private PictureBox currentpic;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
        {

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] name;
            PictureBox pb = new PictureBox();// 新增图片容器
            TabPage page = new TabPage();// 新增选项卡

            OpenFileDialog opfiledlg = new OpenFileDialog();// 显示选择文件对话框
            if (opfiledlg.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(opfiledlg.FileName);
                int w = img.Width;
                int h = img.Height;
                pb.Image = img;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;// 图片为strench模式
                pb.Size = new Size(w,h);// picturebox的大小设置为图片的初始大小
            }

            page.Controls.Add(pb);// 图片显示到选项卡上
            name = opfiledlg.FileName.Split('\\');// 选项卡的标签
            page.Text = name[name.Length - 1];
            this.tabControl1.TabPages.Add(page);// 选项卡添加到选项卡控制容器
            page.AutoScroll = true;
            currentpic = pb;// 记录当前选项卡图片
            
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            int wd = (int)currentpic.Width*1000 / currentpic.Height;// 储存长宽比
            currentpic.Height += 10;// 放大10
            currentpic.Width = (int)currentpic.Height * wd/1000;
        }

        private void drawHistogramImage(int color)
        {
            // 创建bitmap等，并获得参数
            long maxr = 0;
            long maxg = 0;
            long maxb = 0;
            long maxgray =0;

            Rectangle rec = new Rectangle(0,0, currentpic.Width, currentpic.Height);
            Bitmap bp = new Bitmap(currentpic.Image);
            System.Drawing.Imaging.BitmapData bmdata = bp.LockBits(rec,System.Drawing.Imaging.ImageLockMode.ReadOnly, bp.PixelFormat);
            IntPtr ptr = bmdata.Scan0;// bitmap内存起始位置
            // 获得数据存储数组
            byte[] pixeldata = new Byte[bmdata.Stride*bmdata.Height];
            System.Runtime.InteropServices.Marshal.Copy(ptr,pixeldata,0,bmdata.Stride*bmdata.Height);
            bp.UnlockBits(bmdata);
            // 绘制好red、green、blue、gray的色阶图
            long[] Rhistodata = new long[256];
            long[] Ghistodata = new long[256];
            long[] Bhistodata = new long[256];
            long[] Grayhistodata = new long[256];

            // 得到rgb和灰色色阶图的数据
            for (int i = 0; i < bp.Width; i++)
            {
                for (int j = 0; j < bp.Height; j++)
                {
                    Point p = new Point(i,j);
                    if (null == Region || Region.IsVisible(p))
                    {
                        Color c = bp.GetPixel(i,j);
                        Rhistodata[c.R]++;
                        Ghistodata[c.G]++;
                        Bhistodata[c.B]++;
                        Grayhistodata[(int)(c.R + c.G + c.B) / 3]++;
                        if (maxr < Rhistodata[c.R]) maxr = Rhistodata[c.R];
                        if (maxg < Ghistodata[c.G]) maxg = Ghistodata[c.G];
                        if (maxb < Bhistodata[c.B]) maxb = Bhistodata[c.B];
                        if (maxgray < Grayhistodata[(int)(c.R + c.G + c.B) / 3]) maxgray = Grayhistodata[(int)(c.R + c.G + c.B) / 3];
                    }
                }
            }

            // 根据不同模式绘图
            Pen curPen = new Pen(Brushes.Black, 1);
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
            if (color == 0)
            {
                for (int k = 0; k < 256; k++)
                {
                    g.DrawLine(curPen, 20 + k, 180, 20 + k, 180 - Rhistodata[k] * 180 / maxr);
                }
            }
            else if (color == 1)
            {
                for (int k = 0; k < 256; k++)
                {
                    g.DrawLine(curPen, 20 + k, 180, 20 + k, 180 - Ghistodata[k] * 180 / maxg);
                }
            }
            else if (color == 2)
            {
                for (int k = 0; k < 256; k++)
                {
                    g.DrawLine(curPen, 20 + k, 180, 20 + k, 180 - Bhistodata[k] * 180 / maxb);
                }
            }
            else if (color == 3)
            {
                for (int k = 0; k < 256; k++)
                {
                    g.DrawLine(curPen, 20 + k, 180, 20 + k, 180 - Grayhistodata[k] * 180 / maxgray);
                }
            }

            g.Flush();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            int wd = (int)1000*currentpic.Width / currentpic.Height;// 锁定长宽比
            if (currentpic.Height > 50)// 最小大小（太小会失真）
            { 
                currentpic.Height -= 10;
                currentpic.Width = (int)currentpic.Height * wd / 1000;
            }
            else {
                MessageBox.Show("已是最小");// 超出最小大小时，弹出消息框
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawHistogramImage(this.comboBox1.SelectedIndex);
        }

        private void tabControl1_MouseMove(object sender, MouseEventArgs e)
        {
        
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point();
            p.X = e.X;
            p.Y = e.Y;
            //if (currentpic.Bounds.Contains(p))
            {
                toolStripStatusLabel3.Text = "(" + e.X.ToString() + ", " + e.Y.ToString() + ")"; ;
            }
        }

        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string[] name;
            PictureBox pb = new PictureBox();// 新增图片容器
            TabPage page = new TabPage();// 新增选项卡

            OpenFileDialog opfiledlg = new OpenFileDialog();// 显示选择文件对话框
            if (opfiledlg.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(opfiledlg.FileName);
                int w = img.Width;
                int h = img.Height;
                pb.Image = img;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;// 图片为strench模式
                pb.Size = new Size(w, h);// picturebox的大小设置为图片的初始大小
            }

            page.Controls.Add(pb);// 图片显示到选项卡上
            name = opfiledlg.FileName.Split('\\');// 选项卡的标签
            page.Text = name[name.Length - 1];
            this.tabControl1.TabPages.Add(page);// 选项卡添加到选项卡控制容器
            page.AutoScroll = true;
            currentpic = pb;// 记录当前选项卡图片
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdlg = new SaveFileDialog();
            sfdlg.OverwritePrompt = true;
            if (sfdlg.ShowDialog() == DialogResult.OK)
            {
                Bitmap bp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

                pictureBox1.DrawToBitmap(bp,new Rectangle(0 ,0, bp.Width, bp.Height));
                bp.Save(sfdlg.FileName+".jpg");
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdlg = new SaveFileDialog();
            sfdlg.OverwritePrompt = true;
            if (sfdlg.ShowDialog() == DialogResult.OK)
            {
                Bitmap bp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bp, new Rectangle(0, 0, bp.Width, bp.Height));
                bp.Save(sfdlg.FileName+".jpg");
            }
        }
    }
}
