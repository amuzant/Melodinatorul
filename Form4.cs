using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces; 

namespace proiect
{
    public partial class Form4 : Form
    {
        public static string filename;

        public Form4()
        {
            InitializeComponent();
        }

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "9UuScZMRIaNf6tShalfbBQLHFAyoP7yEsvGDnnAX",
            BasePath = "https://melodinatorul-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient ifcl;

        private void button1_Click(object sender, EventArgs e)
        {
            
            FirebaseResponse res = ifcl.Get(@"Useri/" + nume.Text);
            Useri rezUser = res.ResultAs<Useri>();
            Useri CurUser = new Useri()
            {
                username = nume.Text,
                parola = textBox2.Text
            };
            if (Useri.IsEqual(rezUser, CurUser))
            {
                Form1.nme = nume.Text;
                Form2 Formdoi;
                Formdoi = new Form2();
                this.Hide();
                Formdoi.Show();
            }
            else
            {
                Useri.ShowError();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(textBox2.Text) || String.IsNullOrWhiteSpace(nume.Text))
            {
                MessageBox.Show("inserati un username si incercati din nou!", "eroare!");
            }
            else if (System.IO.File.Exists("C:/Users/Andrei/Desktop/proiect atestat/" + nume.Text + ".txt"))
            {
                MessageBox.Show("eroare! acest username este deja folosit de un alt utilizator!", "eroare!");
            }
            else
            {
                Useri user = new Useri()
                {
                    username = nume.Text,
                    parola = textBox2.Text,
                };

                SetResponse set = ifcl.Set(@"Useri/" + nume.Text, user);

                filename = "C:/Users/Andrei/Desktop/proiect atestat/" + nume.Text.ToString() + ".txt";

                var f = System.IO.File.Create(filename);
                f.Close();

                MessageBox.Show("inregistrat cu success!", "success!");
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            try
            {
                ifcl = new FireSharp.FirebaseClient(ifc);
            }
            catch
            {
                MessageBox.Show("problem?");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
