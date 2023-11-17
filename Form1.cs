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
    public partial class Form1 : Form
    {
        int okand = 0;
        public static string nme,pw;
        public static int useri_id;
        public static byte[] img;
        public static string cns= @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|\melodinatorul.mdf; Integrated Security = True";

        public Form1()
        {
            InitializeComponent();
            textBox2.PasswordChar = '\0';
            textBox2.ForeColor = Color.Gray;
            textBox2.Text = "parola";
            nume.ForeColor = Color.Gray;
            nume.Text = "username";
       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = new SqlConnection(cns))
            {
                using (SqlCommand cmd = new SqlCommand("select parola,id,img from useri where username=@username", cn))
                {
                    if (cn.State == ConnectionState.Closed) cn.Open();

                    cmd.Parameters.AddWithValue("@username", nume.Text);

                    var dr = cmd.ExecuteReader();
                    string parola = "";
                    if(dr.HasRows)
                    {

                        dr.Read();
                        parola = dr.GetString(0);
                        useri_id = dr.GetInt32(1);
                        if (!Convert.IsDBNull(dr["img"]))
                            img = (byte[])dr["img"];
                       
                    }
                     
                    if (String.IsNullOrWhiteSpace(textBox2.Text) || String.IsNullOrWhiteSpace(nume.Text))
                    {
                        MessageBox.Show("completati campurile goale si incercati din nou!", "eroare!");
                    }
                    else if (parola!=textBox2.Text)
                    {
                        MessageBox.Show("utilizatorul nu exista sau parola este gresita!", "eroare!");
                    }
                    else
                    {
                        nme = nume.Text;
                        pw = textBox2.Text;
                        Form2 Formdoi;
                        Formdoi = new Form2();
                        this.Hide();
                        Formdoi.Show();
                    }
                }
            }
        }

        public void Insert(byte[] image)
        {
            using (SqlConnection cn = new SqlConnection(cns))
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                if(image!=null)
                using (SqlCommand cmd = new SqlCommand("insert into useri(username, parola, img) values (@username, @parola, @img)", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", nume.Text);
                    cmd.Parameters.AddWithValue("@parola", textBox2.Text);
                    cmd.Parameters.AddWithValue("@img", image);
                    cmd.ExecuteNonQuery();
                }

                else using(SqlCommand cmd = new SqlCommand("insert into useri(username, parola) values (@username, @parola)", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", nume.Text);
                    cmd.Parameters.AddWithValue("@parola", textBox2.Text);
                    cmd.ExecuteNonQuery();
                }
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

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = new SqlConnection(cns))
            {
                 
                if (cn.State == ConnectionState.Closed) cn.Open();
                SqlCommand cmd = new SqlCommand("select username from useri where username=@username", cn);
                cmd.Parameters.AddWithValue("@username", nume.Text);
                SqlDataReader sql = cmd.ExecuteReader();

                if (String.IsNullOrWhiteSpace(textBox2.Text) || String.IsNullOrWhiteSpace(nume.Text))
                {
                    MessageBox.Show("inserati un username si incercati din nou!", "eroare!");
                }

                else if (sql.HasRows == true)
                {
                    MessageBox.Show("eroare! acest username este deja folosit de un alt utilizator!", "eroare!");
                }

                else
                {
                    if (pictureBox1.Image != null)
                        Insert(ConvertImageToBytes(pictureBox1.Image));
                    else Insert(null);

                    MessageBox.Show("inregistrat cu success!", "success!");
                    button2.Hide();
                    button1.Show();
                    okand = 1;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
            label3.Hide();
            textBox2.Hide();
            nume.Hide();
            button1.Hide();
            button2.Hide();
            button6.Hide();
            pictureBox1.Hide();
            checkBox1.Hide();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            textBox2.Show();
            nume.Show();
            button1.Show();
            button4.Hide();
            button5.Hide();
            button6.Show();
            checkBox1.Show();
            label5.Hide();
            label6.Hide();
            okand = 1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
           
            label3.Show();
            textBox2.Show();
            nume.Show();
            button2.Show();
            button4.Hide();
            button5.Hide();
            button6.Show();
            pictureBox1.Show();
            label5.Hide();
            label6.Hide();
            checkBox1.Show();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && okand == 1) button1.PerformClick();
            else if (e.KeyCode == Keys.Enter && okand == 0) button2.PerformClick();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image files(*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png|All files (*.*)|*.*";
            if (od.ShowDialog()==DialogResult.OK)
            {
                pictureBox1.Load(od.FileName);
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
                if (checkBox1.Checked)
                    textBox2.PasswordChar = '\0';
                else textBox2.PasswordChar = '*';
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            label3.Hide();
            textBox2.Hide();
            nume.Hide();
            button1.Hide();
            button2.Hide();
            pictureBox1.Hide();
            checkBox1.Hide();
            button4.Show();
            button5.Show();
            button6.Hide();
            label5.Show();
            label6.Show();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void nume_TextChanged(object sender, EventArgs e)
        {

        }

        private void nume_Enter(object sender, EventArgs e)
        {
            if (nume.Text == "username")
            {
                nume.Text = "";
                nume.ForeColor = Color.Black;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "parola")
            {
                textBox2.PasswordChar = '•';
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.PasswordChar = '\0';
                textBox2.ForeColor = Color.Gray;
                textBox2.Text = "parola";

            }
        }

        private void nume_Leave(object sender, EventArgs e)
        {
            if (nume.Text == "")
            {
                nume.ForeColor = Color.Gray;
                nume.Text = "username";

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form5 Formcinci;
            Formcinci = new Form5();
            this.Hide();
            Formcinci.Show();
        }
    }
}
