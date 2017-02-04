using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace test1
{
    public partial class Form1 : Form
    {

        String DbFile;
        String strConn;
        SQLiteConnection cn = new SQLiteConnection();
        SQLiteCommand cmd = new SQLiteCommand();
        SQLiteDataReader dr;
        SQLiteParameter picture;
        String deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); //바탕화면 경로 가져오기
        
        public Form1()
        {
            InitializeComponent();
            createtable();
        }
        private void open()
        {
            try
            {
                OpenFileDialog f = new OpenFileDialog();
                f.InitialDirectory = deskPath;
                f.Filter = "All Files|*.*|JPEG|*.jpg|Bitmaps|*.bmp|GIF|*.gif";
                f.FilterIndex = 2;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(f.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.BorderStyle = BorderStyle.Fixed3D;
                    label1.Text = f.SafeFileName.ToString();
                }
            }
            catch { }
        }
        private void createtable()
        {
            deskPath = deskPath.Replace("\\", "/"); //\\글자 /로 바꾸기
            DbFile = deskPath + @"/test1.db";
            if (!System.IO.File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);
            }
            label2.Text = DbFile;
            strConn = "Data Source="+DbFile+"; Version=3;";
            cn.ConnectionString = strConn;
            //cn.ConnectionString = @"Data Source=C:/Users/HUN/Desktop/test.db;Version=3;";
            cn.Open();
            string query = "Create table if not exists pictures (id INTEGER  PRIMARY KEY, name varchar(20), picture image)";
            SQLiteCommand cmd = new SQLiteCommand(query, cn);
            cmd.ExecuteNonQuery();
            cn.Close();
            
        }
        private void loaddata()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            cmd.CommandText = "select id, name from pictures";
            cn.Open();
            dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    listBox1.Items.Add(dr[0].ToString());
                    listBox2.Items.Add(dr[1].ToString());
                }
            }
            dr.Close();
            cn.Close();

        }
        private void savepicture()
        {
            if (pictureBox1.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                byte[] a = ms.GetBuffer();
                ms.Close();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@picture", a);
                cmd.CommandText = "insert into pictures (name, picture) values ('" + label1.Text.ToString() + "',@picture)";
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                
                label1.Text = "";
                pictureBox1.Image = null;
                MessageBox.Show("Image saved", "Programming At hun");
            }
            
        }
        private int count_table_column(string tablename){
            cmd.CommandText = "pragma table_info("+tablename+")";  //해당 테이블 칼럼 이름 가져오는 query
            cn.Open();
            int count = 0;
            dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    count++;
                }
            }
            label3.Text = count.ToString();
            dr.Close();
            cn.Close();
            return count;
        }
        private void Form1_Load(object sender, EventArgs e)
        {            
            cmd.Connection = cn;
            label1.Text = "";
            picture = new SQLiteParameter("@picture", SqlDbType.Image);
            loaddata();
            count_table_column("pictures");
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            open();   
        }

        private void button2_Click(object sender, EventArgs e)
        {
            savepicture();
            loaddata();
        }
        
        
        private void loadpicture()
        {
            
            cn.Open();
            cmd.CommandText = "select picture from pictures where id='" + listBox1.Text.ToString() + "'";
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            SQLiteCommandBuilder cbd = new SQLiteCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cn.Close();
            byte[] ap = (byte[])(ds.Tables[0].Rows[0]["picture"]);
            MemoryStream ms = new MemoryStream(ap);
            pictureBox1.Image = Image.FromStream(ms);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            label1.Text = listBox1.Text.ToString();
            ms.Close();
            
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox l = sender as ListBox;
            if (l.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = l.SelectedIndex;
                listBox2.SelectedIndex = l.SelectedIndex;
                loadpicture();
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox l = sender as ListBox;
            if (l.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = l.SelectedIndex;
                listBox2.SelectedIndex = l.SelectedIndex;
                loadpicture();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                OpenFileDialog find = new OpenFileDialog();
                find.InitialDirectory = deskPath;
                find.Filter = "DB|*.db";
                if (find.ShowDialog() == DialogResult.OK)
                {
                    DbFile = find.FileName;
                    DbFile = DbFile.Replace("\\", "/");
                    label2.Text = DbFile;
                    strConn = "Data Source=" + DbFile + "; Version=3;";
                    cn.ConnectionString = strConn;
                    cn.Open();
                    string query = "Create table if not exists pictures (id INTEGER  PRIMARY KEY, name varchar(20), picture image)";
                    SQLiteCommand cmd = new SQLiteCommand(query, cn);
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
                loaddata();
                
            }
            catch { }
        }
        private void addcolumn(string tablename)
        {
            int count = count_table_column(tablename);
            count++;
            cmd.CommandText = "alter table "+tablename+" add column the" + count + "th char(1);";  //칼럼 추가
            cn.Open();
            dr = cmd.ExecuteReader();
            dr.Close();
            cn.Close();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            addcolumn("pictures");
            count_table_column("pictures");

        }

    }
}
