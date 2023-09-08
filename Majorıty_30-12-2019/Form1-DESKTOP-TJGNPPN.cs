using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Majorıty_30_12_2019
{
    public partial class Form1 : Form
    {
        static class Constants
        {
            public const int NOV = 1024;
            public const int iter = 100000;
            public const int sample = 500;
        }
        struct Nodes
        {
            public int nodeID;
            public int degree;
            public int opinion;
            public int zealot;
        };
        struct Links
        {
            public int source;  // linkin başlangıç düğümü
            public int target;  // hedef düğümü
        };

        const int step = Constants.NOV -2;  // toplam adım sayımız
        const int initialNodes = 2;     // başlangıç düğüm sayımız
        const int linksPerStep = 1;     // her adımda oluşturulacak link sayısı

        const int Nmax = (int)(initialNodes + step);    // ulaşacağımız düğüm sayısı
        const int Lnum = (int)(1 + linksPerStep * step);     // ulaşacağımız link sayımız

        Nodes[] N = new Nodes[Nmax]; // Nmax elemanlı düğüm dizisi
        Links[] L = new Links[Lnum];

        int maxDegree = 0;
        int[] Degrees = new int[step];    //Her dereceden kaç düğüm olduğunu tutacak
        int[] DegreesValue = new int[step];
        int[,] neighbourhood = new int[Constants.NOV, Constants.NOV];
        int[,] link = new int[Constants.NOV, Constants.NOV];

        System.Random random = new System.Random();
        int number_of_posotive;
        int number_of_negative;
        double mag;
        int number_of_zealots;

        double[] sample_array = new double[Constants.iter];
        double[] sample_array_pos = new double[Constants.iter];
        double[] sample_array_neg = new double[Constants.iter];

        
        void add_opinion(double p)
        {
            for (int i = 0; i < Constants.NOV; i++) //make all opinion 0
                N[i].opinion = 0;

            for (int i = 0; i < Constants.NOV; i++)
            {
                if (p >= random.NextDouble())
                    N[i].opinion = 1;
                else
                    N[i].opinion = -1;
            }
        }
        public void empty_Network()
        {
            for (int i = 0; i < Constants.NOV; i++)
            {
                N[i].degree = 0;
                N[i].nodeID = 0;
                N[i].opinion = 0;
                N[i].zealot = 0;
            }

            for (int j = 0; j < step; j++)
                Degrees[j] = 0;

            for (int i = 0; i < Constants.NOV; i++)
                for (int j = 0; j < Constants.NOV; j++)
                {
                    neighbourhood[i, j] = 0;
                    link[i, j] = 0;
                }
        }
        void make_zealot(int zealot_number, int zealot_degree)
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].zealot = 0;

            int count = 0;

            for (int i=0; i< Constants.NOV; i++) 
            {
                int rnd_agent = random.Next(Constants.NOV);
                if (N[rnd_agent].opinion == 1 && N[rnd_agent].degree == zealot_degree ) // to check only poitive agents
                    {
                        if (count >= zealot_number)
                            break;
                        N[rnd_agent].zealot = 1; // to make the randmly selected agent a zealot
                        count++;
                        
                    }
            }
            richTextBox4.Text += "Zealots = " + count + "\n";
        }
        void make_zealot2(int zealot_number, int zealot_degree)
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].zealot = 0;

            int count = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].opinion == 1 && N[i].degree == zealot_degree) // to check only poitive agents
                {
                    N[i].zealot = 1; // to make the randmly selected agent a zealot
                    count++;
                    if (count == zealot_number)
                        break;
                }

            //// test zealots /////
            //  count_zealots();
            // richTextBox4.Text += "total_number_of_zealots: " + number_of_zealots + "\n";

        }
        void make_zealot3(int zealot_number, int zealot_degree)
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].zealot = 0;

            int count = 0;
            do
            {
                int rnd_agent = random.Next(Constants.NOV);
                if (N[rnd_agent].opinion == 1 && N[rnd_agent].degree == zealot_degree)
                {
                    count++;
                    N[rnd_agent].zealot = 1;
                }

            } while (count <= zealot_number);
            //// test zealots /////
            count_zealots();
            richTextBox4.Text += "Total_number_of_zealots: " + number_of_zealots + " with degree " + zealot_degree + "\n";

        }
        void count_zealots()
        {
            number_of_zealots = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].zealot == 1)
                    number_of_zealots++;
        }
        void change_opinion(int agent)
        {
            if (N[agent].opinion == 1)
                N[agent].opinion = -1;
            else if (N[agent].opinion == -1)
                N[agent].opinion = 1;
        }
        int sigma(int agent)// this function should be run after  distrubution function.
        {
            int sigma_s_count = 0; // to hold the agent neighbours sign
            for (int i = 1; i <= neighbourhood[agent, 0]; i++)
                sigma_s_count += N[neighbourhood[agent, i]].opinion;

            return sigma_s_count;
        }
        void voter_update_Suchecki(int agent)
        {
            //richTextBox5.Text += "ID: " + N[agent].nodeID + " Opinion: " + N[agent].opinion + " Zealot: " + N[agent].zealot + "\n";
            int sigma_val = sigma(agent);
            int k = N[agent].degree; // neighbourhood[agent, 0];
            int s = N[agent].opinion;

            double s_k = (double) s/k;
            double p_voter = 0.5 * (double) (1 - s_k *  sigma_val);
            double p_sel = random.NextDouble();
            if (p_voter > p_sel)
                change_opinion(agent);
        }
        void voter_update(int agent)
        {
            int rnd_neighbour_index = random.Next(1, neighbourhood[agent, 0] + 1);
            N[agent].opinion = N[neighbourhood[agent, rnd_neighbour_index]].opinion;
        }
        
        void calculate_m()
        {
            number_of_posotive = 0;
            number_of_negative = 0;
            //mag = 0;
            double sigma_pos;
            double sigma_neg;

            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].opinion == 1)
                    number_of_posotive++;
                else
                    number_of_negative++;
            count_zealots();
            sigma_pos = (double) number_of_posotive / Constants.NOV;
            sigma_neg = (double) number_of_negative / Constants.NOV;
            mag = Math.Abs(sigma_pos - sigma_neg);
            //m = Math.Abs(((double)(number_of_posotive - number_of_negative) / Constants.NOV));
        }
        void find_neighbours()
        {
              
            int k;
            for (int i = 0; i < Constants.NOV; i++)
            {
                k = 0;
                for (int j = 0; j < Constants.NOV; j++)
                {
                    if (link[i, j] == 1)
                    {
                        neighbourhood[i, ++k] = j;
                        //richTextBox4.Text += i + "," + k + "> " + j+ "\n";
                    }
                    neighbourhood[i, 0] = k; // the index 0 is the nodes link (ki)
                }
                //richTextBox4.Text += " Links= " + k + "="+ N[i].degree+ "\n\n";
            }
        }
        #region Scale-Free network
        void SF_Network_generate()
        {
            Random Rn = new Random();
            int x, y, xN, yN;   // her link bağlantısı için 2 tane random sayı (düğüm ID'si) oluşturacağız
            int li = 0;     // link sayısını dinamik olarak takip edebilmek için

            for (int i = 0; i < initialNodes; i++)
                N[i].nodeID = i;    // başlangıçta mevcut olan düğümlere ID atadık

            // başlangıç koşullarını oluşturalım:
            L[0].source = 0;
            L[0].target = 1;

            link[0, 1] = 1;
            link[1, 0] = 1;

            li++;   // 1 tane linkimiz oldu
            N[0].degree++;
            N[1].degree++;
            ///////////////////////////////////

            for (int i = initialNodes; i < Nmax; i++)
            {
                N[i].nodeID = i;
                int linkBefore = -1;

                for (int j = 0; j < linksPerStep; j++)
                {
                    do
                    {
                        x = Rn.Next(0, li);     // link dizimizden herhangi bir eleman
                        y = Rn.Next(0, 2);      // 0 ise source tarafı, 1 ise target tarafı olsun

                        if (y == 0)
                        {
                            yN = L[x].source;
                        }
                        else
                        {
                            yN = L[x].target;
                        }

                        xN = i;     // xN, ağa yeni katılan düğüm, yN de eski düğümlerden bağlanmak için seçtiğimiz düğüm
                    } while (linkBefore == yN);    // linksPerStep 1'den büyük olursa yeni katılan düğümün, eski düğümlerden AYNI düğüme tekrar bağlanmasını engellemek için

                    L[li].source = yN;
                    L[li].target = xN;    // Uygun (kullanılmamış) x-y ikilisini bulduk, L[j]'ye source ve target olarak atadık

                   link[yN, xN] = 1;
                    link[xN, yN] = 1;

                    /////////////////////////////////////neighbourhood[yN, li] = xN;
                    N[xN].degree++;
                    N[yN].degree++;  // Link ile bağladığımız düğümlerin derecelerini 1 artırdık              
                    li++;
                    linkBefore = yN;        // bu aşamada bağlandığımız eski düğümü saklayalım
                }
            }


            for (int i = 0; i < Nmax; i++) // bütün düğümlerimizin derecerini yazdıralım
            {
               //richTextBox1.Text += i.ToString() + "    " + N[i].degree.ToString() + '\n';
                if (N[i].degree > maxDegree)
                    maxDegree = N[i].degree;    // maximum dereceyi tutacak
                neighbourhood[i, 0] = N[i].degree;
            }
            DegreeDistribution_SF();
        }
        void DegreeDistribution_SF()   // derece dağılımını oluşturacak
        {
            for (int j = 0; j < Nmax; j++)     // bütün düğümler kontrol
                Degrees[N[j].degree]++;     // düğümün derecesi kaç ise o dereceden olan düğüm sayısını 1 artır

            //for (int j = 0; j <= maxDegree; j++)
               // richTextBox2.Text += j.ToString() + "    " + Degrees[j].ToString() + "\n";
           // richTextBox2.Text += "\n\n";
        }

        
        #endregion
        public Form1()
        {
            InitializeComponent();
        }
        private void btn_reset_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            SF_Network_generate();
            add_opinion(0.5);

            for (int i = 0; i < Constants.NOV; i++)
                richTextBox4.Text += i + "    " + N[i].opinion + "\n";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            empty_Network();
            SF_Network_generate();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            empty_Network();
            SF_Network_generate();
            find_neighbours();
            add_opinion(0.5);
            //make_zealot3(0,8);
            calculate_m();
            using (var writer = new StreamWriter("C:\\Users\\user\\Documents\\data.txt"))

            for (int it = 0; it < Constants.iter; it++)
            {
                int agent = random.Next(Constants.NOV);
                //if (N[agent].zealot == 0) // to check the agent to be non-zealot
                {
                    voter_update_Suchecki(agent);
                    calculate_m();
                }
                writer.WriteLine(it + "    " + number_of_posotive + "    "+ number_of_negative);
                //richTextBox4.Text += it + "    " + number_of_posotive + "    " + number_of_negative + "\n";
            }
        }
       
        private void button7_Click(object sender, EventArgs e)
        {
            SF_Network_generate();
            find_neighbours();
            add_opinion(0.5);
            make_zealot(0, 4);
            //using (var writer = new StreamWriter("result.txt"))

            for (int i = 0; i < Constants.NOV; i++)
            {
                richTextBox3.Text += i + "    " + N[i].opinion + "\n";
                if (N[i].zealot == 1)
                {
                    richTextBox3.Text += i + "    " + N[i].opinion + "    " + N[i].zealot + "    Z" + "\n";
                    i++;
                }
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            SF_Network_generate();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SF_Network_generate();
            find_neighbours();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = System.IO.File.ReadAllText(@"C:\Users\user\Documents\facebook_combined.txt");
            richTextBox1.Text += text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int ze = 0;
            //for (int ze =2; ze <= 128; ze = ze *2)
            {
                progressBar1.Maximum = Constants.sample;

                for (int it = 0; it < Constants.iter; it++)
                    sample_array[it] = 0;

                for (int sa = 0; sa < Constants.sample; sa++)
                {
                    empty_Network();
                    SF_Network_generate();
                    find_neighbours();
                    add_opinion(0.5);
                   // make_zealot2(ze, 2); //int zealot_number, int zealot_degree

                    count_zealots();
                    richTextBox4.Text += "Total_number_of_zealots: " + number_of_zealots + "\n";
                    mag = 0;

                    for (int it = 0; it < Constants.iter; it++)
                    {
                        int agent = random.Next(Constants.NOV);
                       // if (N[agent].zealot == 0)
                        {
                            voter_update(agent);
                            //richTextBox5.Text += "The agent " + N[agent].nodeID + " opinion has been changed to: " + N[agent].opinion + " Zealot: " + N[agent].zealot + "\n\n";
                        }
                        calculate_m();
                        //sample_array[it] += mag;
                        sample_array_pos[it] += number_of_posotive;
                        sample_array_neg[it] += number_of_negative;

                    }
                    progressBar1.Value = sa;
                }

                for (int it = 0; it < Constants.iter; it++)
                {
                    // sample_array[it] = sample_array[it] / (Constants.sample);
                    sample_array_pos[it] = sample_array_pos[it] / (Constants.sample);
                    sample_array_neg[it] = sample_array_neg[it] / (Constants.sample);
                }

                using (var writer = new StreamWriter("C:\\Users\\user\\Documents\\z=" + ze + ".txt"))
                    for (int it = 0; it < Constants.iter; it++)
                    {
                        double magnization = Math.Abs((double)(sample_array_pos[it] - sample_array_neg[it]) / (sample_array_pos[it] + sample_array_neg[it]));
                        writer.WriteLine(it + "    " + magnization);
                    }
            }
            
                

            progressBar1.Visible = false;
            Console.Beep();
        }
    }
}
