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
            public const int iter = 1000;
            public const int sample = 100;
        }
        struct Nodes
        {
            public int nodeID;
            public int degree;
            public int opinion;
            public int zealot;
            public double closeness;
        };
        struct Links
        {
            public int source;  // linkin başlangıç düğümü
            public int target;  // hedef düğümü
        };

        const int step = Constants.NOV - 2;  // toplam adım sayımız
        const int initialNodes = 2;     // başlangıç düğüm sayımız
        const int linksPerStep = 8;     // her adımda oluşturulacak link sayısı

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
        int number_of_zealots;
        double sum_of_deg;
        double edge_density;

        double[] sample_array = new double[Constants.iter];
        double[] sample_array_pos = new double[Constants.iter];
        double[] sample_array_neg = new double[Constants.iter];

        double[] sample_array_edge_density = new double[Constants.sample];
        double[] sample_array_ze = new double[7]; // for ze=0,2,4,8,16,32,64

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

        public void generte_network_from_dataset()
        {
            empty_Network();

            for (int i = 0; i <1005 ; i++) ///////???????  50443
               N[i].nodeID = i; 

            for (int i = 0; i < 1005; i++) 
            {
               // L[i].source = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
              //  L[i].target = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value);
                link[0, 1] = 1;
                link[1, 0] = 1;

                link[Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value), Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value)] = 1;
                link[Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value), Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value)] = 1;

                N[i].degree++;
               // N[y].degree++;
            }
               
        }
       
        void make_zealot_degreeBased(int zealot_number, int zealot_degree)
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].zealot = 0;

           // int count = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].opinion == 1 && N[i].degree == zealot_degree) // to check only poitive agents
                {
                    N[i].zealot = 1; // to make the randmly selected agent a zealot
                         break;
                }

            //// test zealots /////
            //  count_zealots();
            // richTextBox4.Text += "total_number_of_zealots: " + number_of_zealots + "\n";

        }
        void make_zealot_degreeBased_neg(int zealot_number, int zealot_degree)
        {

            int count = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].opinion == -1 && N[i].degree == zealot_degree) // to check only poitive agents
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
        void make_zealot_closenesBased(int zealot_number, double zealot_min_closeness, double zealot_max_closeness)
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].zealot = 0;

            int count = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].opinion == 1 && N[i].closeness >= zealot_min_closeness && N[i].closeness < zealot_max_closeness) // to check only poitive agents
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
        void make_zealot_eigenvector(int zealot_number, int zealot_degree)
        {
            ///// find hubs  //////
            int hub = 0;
            for (int i = 1; i < Constants.NOV;)
            {
                if (N[i].degree > N[hub].degree)
                    hub = N[i].nodeID;
                i++;

            }
            richTextBox1.Text = hub.ToString();

            List<int> hubs_neigh = new List<int>(); // list to add hubs neighbours
            for (int i = 1; i <= N[hub].degree; i++)
                hubs_neigh.Add(neighbourhood[hub, i]);

            int count = 0;
            foreach (int el in hubs_neigh)
                if (N[el].opinion == 1 && N[el].degree == zealot_degree) // to check only poitive agents
                {
                    N[el].zealot = 1; // to make the randmly selected agent a zealot
                    count++;
                    if (count == zealot_number)
                        break;
                }
            //// test zealots /////
            //  count_zealots();
            // richTextBox4.Text += "total_number_of_zealots: " + number_of_zealots + "\n";

        }
        void make_zealot_eigenvector_rev(int zealot_number, int zealot_degree)
        {
            ///// find hubs  //////
            int hub = 0;
            for (int i = 1; i < Constants.NOV;)
            {
                if (N[i].degree > N[hub].degree)
                    hub = N[i].nodeID;
                i++;
            }
            // richTextBox1.Text = hub.ToString();
            List<int> hubs_neigh = new List<int>(); // list to add hubs neighbours
            for (int i = 1; i <= N[hub].degree; i++)
                hubs_neigh.Add(neighbourhood[hub, i]);

            int count = 0;

            for (int i = 0; i < 99999; i++)
            {
                int rnd = random.Next(Constants.NOV);
                foreach (int el in hubs_neigh)
                    if (rnd == el) // to check only poitive agents
                        break;

                if (N[rnd].opinion == 1 && N[rnd].degree == zealot_degree) // to check only poitive agents
                {
                    N[rnd].zealot = 1; // to make the randmly selected agent a zealot
                    count++;
                    if (count == zealot_number)
                        break;
                }
            }

        }
        void make_hobs_zealot(int zealot_degree)
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].zealot = 0;

            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].opinion == 1 && N[i].degree >= zealot_degree) // to check only poitive agents
                    N[i].zealot = 1; // to make the randmly selected agent a zealot

            //// test zealots /////
            //  count_zealots();
            // richTextBox4.Text += "total_number_of_zealots: " + number_of_zealots + "\n";
        }
        public void make_zealot_quantityBased(int zealot_number)
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].zealot = 0;

            int count = 0;
            for (int i = 0; i < 999999; i++)
            {
                int rnd = random.Next(Constants.NOV);
                if (N[rnd].opinion == 1) // to check only poitive agents
                {
                    N[rnd].zealot = 1; // to make the randmly selected agent a zealot
                    count++;
                    //richTextBox3.Text += N[rnd].degree +"    "+ N[rnd].zealot + "    " +N[rnd].opinion + "\n";
                }
                if (count == zealot_number)
                    break;
            }

            //// test zealots /////
            //  count_zealots();
            // richTextBox4.Text += "total_number_of_zealots: " + number_of_zealots + "\n";
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

            double s_k = (double)s / k;
            double p_voter = 0.5 * (double)(1 - s_k * sigma_val);
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

            for (int i = 0; i < Constants.NOV; i++)
                if (N[i].opinion == 1)
                    number_of_posotive++;
                else
                    number_of_negative++;
            count_zealots();
            // sigma_pos = (double) number_of_posotive / Constants.NOV;
            // sigma_neg = (double) number_of_negative / Constants.NOV;
            // mag = Math.Abs(sigma_pos - sigma_neg);
            //m = Math.Abs(((double)(number_of_posotive - number_of_negative) / Constants.NOV));
        }
        void calculate_edge_density() // This function need to pre-process of calculate_sum_of_deg(). calculate_sum_of_deg() is constraint so one time running this function is enough
        {
            edge_density = 0;
            double sum = 0;
            for (int i = 0; i < Constants.NOV; i++)
                for (int j = 0; j < N[i].degree; j++)
                    sum = (double)(1 - N[i].opinion * N[j].opinion) / 2;

            calculate_sum_of_deg();
            edge_density = (double)sum / sum_of_deg;
        }
        public double calculate_network_density()
        {
            int AC=0; //AC(Actual Connection)
            for (int i=0; i<Constants.NOV; i++)
                AC+=N[i].degree;
            AC =AC/2;
            richTextBox1.Text += "AC=" + AC.ToString() + "/n";

            double PC = Constants.NOV * (Constants.NOV - 1) / 2; // PC(Potential Connection) 
            double ND = AC / PC;  // ND(Network Density)
            richTextBox1.Text += "PC=" + PC.ToString() + "/n" + "ND=" + ND.ToString() + "/n";

            return ND;

        }
        void calculate_sum_of_deg() //Pre-function of calculate_edge_density()
        {
            sum_of_deg = 0;
            for (int i = 0; i < Constants.NOV; i++)
                sum_of_deg += neighbourhood[i, 0];
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
                       // richTextBox4.Text += i + "," + k + "> " + j+ "\n";
                    }
                    neighbourhood[i, 0] = k; // the index 0 is the nodes link (ki)
                }
               // richTextBox4.Text += "Links= " + k + "\n\n";
            }
        }
        public double calculate_p_m(int z_pos, int z_neg, double m)
        {
            double pm = 0;
            int susc = Constants.NOV - (z_pos + z_neg);
            int si = z_pos - z_neg;
            double r = Math.Sqrt(Math.Pow(si, 2) + 4 * susc);
            pm = Math.Pow((1 - m * (si + m * susc)), (double)(z_pos + z_neg - 2) / 2) * Math.Pow((1 + ((double)r / (m * susc - (double)(r - si) / 2))), (((double)si / 2 * r) * (2 * Constants.NOV - z_pos - z_neg)));
            //pm = (((1 - m*(si + m*susc))) ^ (((z_pos + z_neg - 2) / (2))))(((1 + (((r) / (m*susc - ((r - si) / (2)))))))^((((si) / (2 *r))) (2*Constants.NOV - z_pos - z_neg))));

            return pm;
        }
        public int find_edge_no()
        {
            int count = 0;
            for (int i = 0; i < Constants.NOV; i++)
                for (int j = 0; j < Constants.NOV; j++)
                    if (link[i, j] == 1)
                        count++;
            return count / 2;
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
        void generate_sample_network()
        {
            for (int i = 0; i < 6; i++)
                N[i].nodeID = i;

            link[0, 1] = 1;
            link[1, 0] = 1;
            N[0].degree++;
            N[1].degree++;

            link[0, 2] = 1;
            link[2, 0] = 1;
            N[0].degree++;
            N[2].degree++;

            link[0, 3] = 1;
            link[3, 0] = 1;
            N[0].degree++;
            N[3].degree++;

            link[1, 5] = 1;
            link[5, 1] = 1;
            N[1].degree++;
            N[5].degree++;

            link[2, 3] = 1;
            link[3, 2] = 1;
            N[2].degree++;
            N[3].degree++;

            link[3, 4] = 1;
            link[4, 3] = 1;
            N[3].degree++;
            N[4].degree++;

            link[4, 5] = 1;
            link[5, 4] = 1;
            N[4].degree++;
            N[5].degree++;

        }
        void DegreeDistribution_SF()   // derece dağılımını oluşturacak
        {
            for (int j = 0; j < Nmax; j++)     // bütün düğümler kontrol
                Degrees[N[j].degree]++;     // düğümün derecesi kaç ise o dereceden olan düğüm sayısını 1 artır

            //  for (int j = 0; j <= maxDegree; j++)
            // richTextBox2.Text += j.ToString() + "    " + Degrees[j].ToString() + "\n";
            // richTextBox2.Text += "\n\n";
        }
        void find_closeness()
        {
            for (int i = 0; i < Constants.NOV; i++)
                N[i].closeness = 0;

            for (int i = 0; i < Constants.NOV; i++)
            {
                int sum = dijkstra(link, i);
                N[i].closeness = (double)(Constants.NOV - 1) / sum;
            }
        }
        int minDistance(int[] dist, bool[] sptSet)
        {
            // Initialize min value 
            int min = int.MaxValue, min_index = -1;

            for (int v = 0; v < Constants.NOV; v++)
                if (sptSet[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    min_index = v;
                }

            return min_index;
        }
        public int dijkstra(int[,] graph, int src)
        {
            int sum_of_dist = 0;
            int[] dist = new int[Constants.NOV]; // The output array. dist[i] 
            // will hold the shortest 
            // distance from src to i 

            // sptSet[i] will true if vertex 
            // i is included in shortest path 
            // tree or shortest distance from 
            // src to i is finalized 
            bool[] sptSet = new bool[Constants.NOV];

            // Initialize all distances as 
            // INFINITE and stpSet[] as false 
            for (int i = 0; i < Constants.NOV; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
            }

            // Distance of source vertex 
            // from itself is always 0 
            dist[src] = 0;

            // Find shortest path for all vertices 
            for (int count = 0; count < Constants.NOV - 1; count++)
            {
                // Pick the minimum distance vertex 
                // from the set of vertices not yet 
                // processed. u is always equal to 
                // src in first iteration. 
                int u = minDistance(dist, sptSet);

                // Mark the picked vertex as processed 
                sptSet[u] = true;

                // Update dist value of the adjacent 
                // vertices of the picked vertex. 
                for (int v = 0; v < Constants.NOV; v++)

                    // Update dist[v] only if is not in 
                    // sptSet, there is an edge from u 
                    // to v, and total weight of path 
                    // from src to v through u is smaller 
                    // than current value of dist[v] 
                    if (!sptSet[v] && graph[u, v] != 0 &&
                        dist[u] != int.MaxValue && dist[u] + graph[u, v] < dist[v])
                        dist[v] = dist[u] + graph[u, v];
            }
            // print
            for (int i = 0; i < Constants.NOV; i++)
            {
                sum_of_dist += dist[i];
                //richTextBox1.Text += i + "    " + dist[i] + "\n";
            }
            return sum_of_dist;
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
            //generate_sample_network();
            find_neighbours();
            double network_density = calculate_network_density();
            richTextBox2.Text += network_density.ToString();

        }
        private void button5_Click(object sender, EventArgs e)
        {
            SF_Network_generate();
            find_neighbours();
            find_closeness();
            for (int i = 0; i < Constants.NOV; i++)
                richTextBox4.Text += N[i].closeness.ToString() + "     " + N[i].degree + "\n";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SF_Network_generate();
            find_neighbours();
            add_opinion(0.5);
            make_zealot_degreeBased(0, 4);
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
            generte_network_from_dataset();
            
            find_neighbours();

         //  Status_txtBox.Text = dataGridView1.Rows[1].Cells[0].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = System.IO.File.ReadAllText(@"C:\Users\user\Documents\facebook_combined.txt");
            richTextBox1.Text += text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
           // int ze =4;
           // for (int ze =2; ze <= 64; ze = ze *2)
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
                    make_hobs_zealot(100); //min hob_degree
                    //make_zealot_degreeBased(23, 11); //int zealot_number, int zealot_degree
                    make_zealot_degreeBased_neg(32, 8); //int zealot_number, int zealot_degree
                    //make_zealot_eigenvector(4, 8);
                    //make_zealot_eigenvector_rev(2,8);
                    //make_zealot_quantityBased(8);

                    count_zealots();
                    richTextBox4.Text += number_of_zealots + "\n"; //  "Total_number_of_zealots: "
                    // mag = 0;

                    for (int it = 0; it < Constants.iter;)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int agent = random.Next(Constants.NOV);
                            if (N[agent].zealot == 0)
                            {
                                voter_update(agent);
                                //richTextBox5.Text += "The agent " + N[agent].nodeID + " opinion has been changed to: " + N[agent].opinion + " Zealot: " + N[agent].zealot + "\n\n";
                            }
                        }
                        calculate_m();
                        //sample_array[it] += mag;
                        sample_array_pos[it] += number_of_posotive;
                        sample_array_neg[it] += number_of_negative;
                        it++;
                    }
                    progressBar1.Value = sa;
                }


                for (int it = 0; it < Constants.iter; it++)
                {
                    // sample_array[it] = sample_array[it] / (Constants.sample);
                    sample_array_pos[it] = sample_array_pos[it] / (Constants.sample);
                    sample_array_neg[it] = sample_array_neg[it] / (Constants.sample);
                }
                using (var writer = new StreamWriter("D:\\comp3.txt"))
                    for (int it = 0; it < Constants.iter; it++)
                    {
                        double magnization = Math.Abs((double)(sample_array_pos[it] - sample_array_neg[it]) / (sample_array_pos[it] + sample_array_neg[it]));
                        writer.WriteLine(it + "    " + magnization);
                        // writer.WriteLine(it + "    " + sample_array_pos[it] + "    " + sample_array_neg[it] + "    " + magnization);
                    }

                
            }



            progressBar1.Visible = false;
            Console.Beep();
        }
        private void button8_Click_1(object sender, EventArgs e)
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
                find_closeness();
                make_zealot_closenesBased(4, 0.30, 0.35); //int zealot_number, double zealot min closeness value, double zealot max closeness value

                count_zealots();
                richTextBox4.Text += number_of_zealots + "\n"; //  "Total_number_of_zealots: "
                                                               // mag = 0;

                for (int it = 0; it < Constants.iter;)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int agent = random.Next(Constants.NOV);
                        if (N[agent].zealot == 0)
                        {
                            voter_update(agent);
                            //richTextBox5.Text += "The agent " + N[agent].nodeID + " opinion has been changed to: " + N[agent].opinion + " Zealot: " + N[agent].zealot + "\n\n";
                        }
                    }
                    calculate_m();
                    //sample_array[it] += mag;
                    sample_array_pos[it] += number_of_posotive;
                    sample_array_neg[it] += number_of_negative;
                    it++;

                }
                progressBar1.Value = sa;
            }

            for (int it = 0; it < Constants.iter; it++)
            {
                // sample_array[it] = sample_array[it] / (Constants.sample);
                sample_array_pos[it] = sample_array_pos[it] / (Constants.sample);
                sample_array_neg[it] = sample_array_neg[it] / (Constants.sample);
            }
            using (var writer = new StreamWriter("C:\\Users\\user\\Documents\\z=4_30.txt"))
                for (int it = 0; it < Constants.iter; it++)
                {
                    double magnization = Math.Abs((double)(sample_array_pos[it] - sample_array_neg[it]) / (sample_array_pos[it] + sample_array_neg[it]));
                    writer.WriteLine(it + "    " + magnization);
                }

            progressBar1.Visible = false;
            Console.Beep();
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            //int ze =0;
            using (var writer = new StreamWriter("C:\\Users\\user\\Documents\\z_vs_r.txt"))

                for (int ze = 2; ze <= 64; ze = ze * 2)
                {
                    progressBar1.Maximum = ze;
                    double sum_of_edge_density = 0;
                    for (int sa = 0; sa < Constants.sample; sa++)
                    {
                        empty_Network();
                        SF_Network_generate();
                        find_neighbours();
                        add_opinion(0.5);
                        //make_hobs_zealot(100); //min hob_degree
                        // make_zealot_degreeBased(ze, 8); //int zealot_number, int zealot_degree

                        for (int it = 0; it < Constants.iter;)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                int agent = random.Next(Constants.NOV);
                                // if (N[agent].zealot == 0)
                                {
                                    voter_update(agent);
                                    //richTextBox5.Text += "The agent " + N[agent].nodeID + " opinion has been changed to: " + N[agent].opinion + " Zealot: " + N[agent].zealot + "\n\n";
                                    it++;
                                }
                            }
                        }
                        //calculate_sum_of_deg();
                        calculate_edge_density(); // to calculate r
                        sum_of_edge_density += edge_density;
                    }
                    sum_of_edge_density = (double)sum_of_edge_density / Constants.sample;

                    richTextBox1.Text += ze + "    " + sum_of_edge_density + "\n";
                    //writer.WriteLine(ze + "    " + sum_of_edge_density + "\n");

                    progressBar1.Value = ze;
                }
            progressBar1.Visible = false;
            Console.Beep();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            empty_Network();
            SF_Network_generate();
            find_neighbours();
            add_opinion(0.5);
            //make_zealot_eigenvector(4, 8);
            make_zealot_eigenvector_rev(2, 8);

            count_zealots();
            richTextBox2.Text += number_of_zealots + "\n"; //  "Total_number_of_zealots: "

            for (int i = 0; i < Constants.NOV; i++)
            {
                if (N[i].zealot == 1)
                    richTextBox2.Text += N[i].nodeID + "\n";
                richTextBox3.Text += N[i].nodeID.ToString() + "     " + N[i].degree + "    " + N[i].zealot + "     " + N[i].opinion + "\n";

            }
            Console.Beep();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            int Z_POS = 64;
            int Z_NEG = 8;
            double P_M;
            double magnetization = Convert.ToDouble(Status_txtBox.Text);
            P_M = (calculate_p_m(Z_POS, Z_NEG, magnetization));
            richTextBox4.Text += P_M + "    " + magnetization + "\n";

                


        }

        private void button12_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Open dateset",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt file (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true,
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                helper.file = textBox1.Text;

                dataGridView1.DataSource = helper.DataTableFromTextFile(textBox1.Text);

            }
              
        }


    }

}

        
    

