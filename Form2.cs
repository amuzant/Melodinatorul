using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace proiect
{
    public partial class Form2 : Form
    {
        public int okmare = 0;
        public static int nrpiese,songs_id,exista;
        public static string yt;
            public string genre;
            public string an;
            public string piesa;
            public string des;
            public string ink;
            public string timp;
        public int ct1, ct2;
        public byte[] img;


        public Form2()
        {
            InitializeComponent();
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Region = new Region(path);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (Form1.img != null)
                pictureBox1.Image = ConvertByteArrayToImage(Form1.img);
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
          
            label2.Text = Form1.nme;
            pictureBox2.Hide();
            pictureBox3.Hide();
            pictureBox4.Hide();
            label1.Hide();
            label3.Hide();

            radioButton1.Appearance = Appearance.Button;
            radioButton2.Appearance = Appearance.Button;
            radioButton3.Appearance = Appearance.Button;
            radioButton4.Appearance = Appearance.Button;
            radioButton5.Appearance = Appearance.Button;
            radioButton6.Appearance = Appearance.Button;
            radioButton7.Appearance = Appearance.Button;
            radioButton8.Appearance = Appearance.Button;
            radioButton9.Appearance = Appearance.Button;
            radioButton10.Appearance = Appearance.Button;
            radioButton13.Appearance = Appearance.Button;
            radioButton14.Appearance = Appearance.Button;
            radioButton15.Appearance = Appearance.Button;
            radioButton16.Appearance = Appearance.Button;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form3 Formtrei;
            Formtrei = new Form3();
            this.Close();
            Formtrei.Show();
        }

        private void rand(string a, string g)
        {
            using (SqlConnection cn = new SqlConnection(Form1.cns))
            {
                if (cn.State == ConnectionState.Closed) cn.Open();

                using (SqlCommand cmd = new SqlCommand("select TOP 1 s.Id,s.piesa,s.des,s.link,s.img from songs s,userisongs us where genre = @genre and an = @an order by newid()", cn))
                {
                    if (cn.State == ConnectionState.Closed) cn.Open();
                    cmd.Parameters.AddWithValue("@genre", g);
                    cmd.Parameters.AddWithValue("@an", a);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                        {
                            exista = 0;
                            MessageBox.Show("nu exista lol", "eroare!");
                        }
                        else
                        {
                            exista = 1;
                            dr.Read();
                            songs_id = dr.GetInt32(0);
                            piesa = dr.GetString(1);
                            des = dr.GetString(2);
                            ink = dr.GetString(3);
                            if (!Convert.IsDBNull(dr["img"]))
                                img = (byte[])dr["img"];
                        }
                    }
                }
                // piesa random (dupa criterii), retine in songs_id id-ul piesei

                using (SqlCommand cmd2 = new SqlCommand("select us.useri_id,us.songs_id from userisongs us,useri u,songs s where us.useri_id=@useri_id and us.songs_id=@songs_id", cn))
                {
                    cmd2.Parameters.AddWithValue("@useri_id", Form1.useri_id);
                    cmd2.Parameters.AddWithValue("@songs_id", songs_id);
                    using (SqlDataReader sql = cmd2.ExecuteReader())
                    {   //verifica daca deja a ascultat piesa respectiva

                        if (sql.HasRows)
                        {
                            sql.Close();
                            using (SqlCommand cmd3 = new SqlCommand("select count(us.songs_id) from userisongs us join songs s on us.songs_id=s.id where s.genre=@genre and us.useri_id=@useri_id and s.an=@an", cn))
                            {
                                cmd3.Parameters.AddWithValue("@useri_id", Form1.useri_id);
                                cmd3.Parameters.AddWithValue("@genre", g);
                                cmd3.Parameters.AddWithValue("@an", a);
                                using (var dr = cmd3.ExecuteReader())
                                {
                                    dr.Read();
                                    ct1 = dr.GetInt32(0);
                                }
                            }
                            using (SqlCommand cmd3 = new SqlCommand("select count(*) from songs s where s.genre=@genre and s.an=@an", cn))
                            {
                                cmd3.Parameters.AddWithValue("@genre", g);
                                cmd3.Parameters.AddWithValue("@an", a);
                                using (var dr = cmd3.ExecuteReader())
                                {
                                    dr.Read();
                                    ct2 = dr.GetInt32(0);
                                }
                            }
                            if (ct1 == ct2)
                            {
                                label1.Text = "am ramas fara melodii :(";
                                label3.Text = "am ramas fara melodii cu anul si genul muzical selectat. incearcati alte optiuni!";
                                pictureBox2.Hide();
                                pictureBox3.Hide();
                                pictureBox4.Hide();
                            }
                            else rand(a, g);
                        }
                        else
                        {
                            sql.Close();
                            using (SqlCommand cmd3 = new SqlCommand("insert into userisongs(useri_id, songs_id, data) values (@useri_id, @songs_id, @data)", cn))
                            {
                                cmd3.Parameters.AddWithValue("@useri_id", Form1.useri_id);
                                cmd3.Parameters.AddWithValue("@songs_id", songs_id);
                                cmd3.Parameters.AddWithValue("@data", DateTime.UtcNow.Date);
                                cmd3.ExecuteNonQuery();
                                label1.Text = piesa;
                                label3.Text = des;
                                yt = ink;
                                if (exista==1)
                                {
                                    pictureBox2.Image = ConvertByteArrayToImage(img);
                                    pictureBox2.Show();
                                    pictureBox3.Show();
                                    pictureBox4.Show();
                                }
                                else
                                {
                                    pictureBox2.Hide();
                                    pictureBox3.Hide();
                                    pictureBox4.Hide();
                                }
                            }

                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string a = "";
            string g = "";
            //anii
            if (radioButton1.Checked)
            {
                a = radioButton1.Text;
            }
            else if (radioButton2.Checked)
            {
                a = radioButton2.Text;
            }
            else if (radioButton3.Checked)
            {
                a = radioButton3.Text;
            }
            else if (radioButton4.Checked)
            {
                a = radioButton4.Text;
            }
            else if (radioButton5.Checked)
            {
                a = radioButton5.Text;
            }
            else if (radioButton6.Checked)
            {
                a = radioButton6.Text;
            }
            else if (radioButton7.Checked)
            {
                a = radioButton7.Text;
            }
            else if (radioButton8.Checked)
            {
                a = radioButton8.Text;
            }

            // genreurile
            if (radioButton9.Checked)
            {
                g = radioButton9.Text;
            }
            else if (radioButton10.Checked)
            {
                g = radioButton10.Text;
            }
            //rip muzica clasica nu mai exista si rip electronic si rip jazz/blues si rip nu conteaza
            else if (radioButton13.Checked)
            {
                g = radioButton13.Text;
            }
            else if (radioButton14.Checked)
            {
                g = radioButton14.Text;
            }
            else if (radioButton15.Checked)
            {
                g = radioButton15.Text;
            }
            else if (radioButton16.Checked)
            {
                g = radioButton16.Text;
            }
            if (a == "" || g == "") MessageBox.Show("te rog selecteaza anul si genul muzical", "eroare!");
            else
            {
                rand(a, g);
                label3.Show();
                label1.Show();
                
            }
        }


        private void link(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(yt);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            form1.Show();
            this.Close();
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("hei! uite ce melodie am gasit pe melodinator! " + yt + " \n" + "#melodinatorul");
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
