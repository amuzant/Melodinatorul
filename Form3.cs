using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using System.Data.SqlClient;
using System.Collections;

namespace proiect
{
    public partial class Form3 : Form
    {
        public int okvlad=0;
        public Form3()
        {
            InitializeComponent();
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Region = new Region(path);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            StyleDatagridview();
            ArrayList row = new ArrayList();
            int k = 0;
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "#";
            dataGridView1.Columns[1].Name = "Piese";
            dataGridView1.Columns[2].Name = "Data adaugarii";
            using (SqlConnection cn = new SqlConnection(Form1.cns))
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("select s.piesa, us.data from userisongs us join songs s on us.songs_id=s.id where us.useri_id=@useri_id", cn))
                {
                    cmd.Parameters.AddWithValue("@useri_id", Form1.useri_id);

                    using (var dr = cmd.ExecuteReader())
                        if (!dr.HasRows)
                        {
                            dataGridView1.Hide();
                            label6.Text = "inca nu aveti melodii!";
                        }
                        else
                        {
                            dataGridView1.Show();
                            label6.Hide();
                            while (dr.Read())
                            {
                                row = new ArrayList();
                                row.Add(++k);
                                row.Add(dr["piesa"]);
                                row.Add(((DateTime)dr["data"]).ToString("dd MMM yyyy"));
                                dataGridView1.Rows.Add(row.ToArray());
                            }

                        }
                }
            }
            checkBox1.Hide();
            textBox1.Hide();
            textBox2.Hide();
            label5.Hide();
            label4.Hide();
            button2.Hide();
            panel1.Hide();
            label1.Text = Form1.nme;
            if (Form1.img != null)
                pictureBox1.Image = ConvertByteArrayToImage(Form1.img);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form2 Formdoi;
            Formdoi = new Form2();
            this.Close();
            Formdoi.okmare = 1;
            Formdoi.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 Formdoi;
            Formdoi = new Form2();
            this.Close();
            Formdoi.okmare = 1;
            Formdoi.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (okvlad % 2 == 0)
            {
                textBox1.Text = Form1.nme;
                textBox2.Text = Form1.pw;
                textBox1.Show();
                textBox2.Show();
                label5.Show();
                label4.Show();
                button2.Show();
                checkBox1.Show();
                panel1.Show();
            }
            if (okvlad % 2 == 1)
            {
                checkBox1.Checked = false;
                checkBox1.Hide();
                textBox1.Hide();
                textBox2.Hide();
                label5.Hide();
                label4.Hide();
                button2.Hide();
                panel1.Hide();
            }
            okvlad++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = new SqlConnection(Form1.cns))
            {

                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("select username from useri where username=@username", cn))
                {
                    cmd.Parameters.AddWithValue("@username", textBox1.Text);
                    using (SqlDataReader sql = cmd.ExecuteReader())
                    {
                        if (sql.HasRows && textBox1.Text != Form1.nme) { MessageBox.Show("eroare! acest username este deja folosit de un alt utilizator!", "eroare!"); }
                        else
                        {
                            sql.Close();
                            //fix profile update not working
                            using (SqlCommand cmd2 = new SqlCommand("update useri set username=@username,parola=@parola,img=@img where id=@id", cn))
                            {
                                cmd2.Parameters.AddWithValue("@username", textBox1.Text);
                                cmd2.Parameters.AddWithValue("@parola", textBox2.Text);
                                cmd2.Parameters.AddWithValue("@img", ConvertImageToBytes(pictureBox1.Image));
                                cmd2.Parameters.AddWithValue("@id", Form1.useri_id);
                                cmd2.ExecuteNonQuery();
                                if (cn.State == ConnectionState.Closed) cn.Open();
                                //cmd.Parameters.AddWithValue("@id", Form1.useri_id);
                                Form1.nme = textBox1.Text;
                                Form1.pw = textBox2.Text;
                                Form1.img = ConvertImageToBytes(pictureBox1.Image);
                                label1.Text = Form1.nme;
                                MessageBox.Show("updatat cu success!");
                            }
                        }
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
                textBox2.PasswordChar = '\0';
            else textBox2.PasswordChar = '*';
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image files(*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png|All files (*.*)|*.*";
            if (od.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(od.FileName);
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
        void StyleDatagridview()
        {
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(153, 155, 132);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.BackgroundColor = Color.FromArgb(171, 128, 119);
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;//optional
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 15);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(153, 155, 132);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
            
        }
        public Image ConvertByteArrayToImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
