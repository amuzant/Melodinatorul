using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proiect
{
    public partial class Form5 : Form
    {
        int index,idd;
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadData();

        }

        public void Insert(byte[] image)
        {
            using (SqlConnection cn = new SqlConnection(Form1.cns))
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("insert into Songs(piesa,an,genre,des,link,img) values (@piesa, @an, @genre, @des, @link, @img)", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@piesa", textBox1.Text);
                    cmd.Parameters.AddWithValue("@des", textBox2.Text);
                    cmd.Parameters.AddWithValue("@link", textBox3.Text);
                    cmd.Parameters.AddWithValue("@an", comboBox1.SelectedItem);
                    cmd.Parameters.AddWithValue("@genre", comboBox2.SelectedItem);
                    cmd.Parameters.AddWithValue("@img", image);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LoadData()
        {
            using (SqlConnection cn = new SqlConnection(Form1.cns))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                using (DataTable dt = new DataTable("Songs"))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("select * from songs order by id", cn);
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.MultiSelect = true;
                }
            }
            dataGridView1.AllowUserToAddRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image files(*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png|All files (*.*)|*.*";
            if (od.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(od.FileName);
                pictureBox1.Load(od.FileName);
                Insert(ConvertImageToBytes(pictureBox1.Image));
                LoadData();
            }
        }

        byte[] ConvertImageToBytes(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public Image ConvertByteArrayToImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        private void dataGridview_Cellclick(object sender, DataGridViewCellEventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt != null)
            {
                index = e.RowIndex;
                DataGridViewRow row = dataGridView1.Rows[index];
                idd = (int)row.Cells[0].Value;
                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[4].Value.ToString();
                textBox3.Text = row.Cells[5].Value.ToString();
                comboBox1.SelectedIndex = comboBox1.FindStringExact(row.Cells[2].Value.ToString());
                comboBox2.SelectedIndex = comboBox2.FindStringExact(row.Cells[3].Value.ToString());
                pictureBox1.Image = ConvertByteArrayToImage((byte[])row.Cells[6].Value);
            }

        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            form1.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = new SqlConnection(Form1.cns))
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("delete from songs where id=@id", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", idd);
                    cmd.ExecuteNonQuery();
                    dataGridView1.Rows.RemoveAt(index);
                    MessageBox.Show("melodia selectata a fost stersa cu success", "success!");
                }
            }
        }
    }
}
