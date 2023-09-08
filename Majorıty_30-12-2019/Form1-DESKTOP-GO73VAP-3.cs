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
            public const int NOV = 1000;
            public const int iter = 10000;
            public const int sample = 50;
        }

        System.Random random = new System.Random();
        int number_of_posotive;
        int number_of_negative;
        int number_of_zealots;
        Int16[,] network_array_opinions = new Int16[Constants.NOV, 2]; // network array and opinions-- [i,0]== opinions -- [i,1] == zealot flag
        double[,] sample_array = new double[Constants.iter, 2];
        
        //random ER graph
        int[,] link = new int[Constants.NOV, Constants.NOV];
        double p_ER_graph = 0.1;
        int[,] neighbourhood = new int[Constants.NOV, Constants.NOV]; // neighbours array

        //Scale-Free Network
        int[,] link_SF = new int[Constants.NOV, Constants.NOV];
        int[,] neighbourhood_SF = new int[Constants.NOV, Constants.NOV]; // neighbours array
        double[,] SF_nodes_probability = new double[Constants.NOV, 1]; // an array to keep SFN nodes probability

        void ER_Network_generate()
        {
            //empty_network_generate();
            for (int i = 0; i < Constants.NOV; i++)             // generate random network
                for (int j = i + 1; j < Constants.NOV; j++)
                {
                    link[i, j] = 0;
                    link[j, i] = 0;
                    double rnd_val = random.NextDouble();
                    if (p_ER_graph > rnd_val)
                    {
                        link[i, j] = 1;
                        link[j, i] = 1;
                    }
                }
        }

        
        void empty_network_generate()
        {
            for (int i = 0; i < Constants.NOV; i++) //make all matrix zero
                for (int j = i + 1; j < Constants.NOV; j++)
                {
                    link[i, j] = 0;
                    link[j, i] = 0;
                }
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
                        // textBox2.Text += i + "," + k + "> " + j;
                       //  textBox2.Text += Environment.NewLine;
                    }
                    neighbourhood[i, 0] = k; // the index 0 is the nodes link (ki)
                }
                // textBox2.Text += "Links " + i + "= " + k;
               //  textBox2.Text += Environment.NewLine;
               //  textBox2.Text += Environment.NewLine;
            }
        }
        void find_neighbours_of_an_agent(int agent)
        {
            int rnd_neighbour = random.Next(1, neighbourhood[agent, 0]+1);
            network_array_opinions[agent,0] = network_array_opinions[neighbourhood[agent, rnd_neighbour], 0]; 

        }
        void add_opinion(double p)
        {
            for (int i = 0; i < Constants.NOV; i++) //make all opinion 0
            {
                network_array_opinions[i, 0] = 0;
                //network_array_opinions[i, 1] = 0;
            }
            for (int i = 0; i < Constants.NOV; i++)
            {
                if (p >= random.NextDouble())
                    network_array_opinions[i, 0] = 1;
                else
                    network_array_opinions[i, 0] = -1;
            }
        }
        void make_zealot()
        {
            for (int i = 0; i < Constants.NOV; i++)
                    network_array_opinions[i, 1] = 0; // to make all nodes non-Zealot 
            int count32=0;
                for (int j = 0; j < Constants.NOV; j++)
                   // if (neighbourhood[j, 0] == numericUpDown_Zealots_Degree.Value)  // the opinion that zealots are in
                    {
                        if (network_array_opinions[j,0]==1) // only positive agents must be zealot
                        {
                            network_array_opinions[j, 1] = 1; // make the agent zealot
                            count32++;
                        }
                       
                        if (count32 == numericUpDown_Zealot_Quantity.Value)
                        {
                            break;
                        }
                    }
            /*
            for (int i = 0; i < numericUpDown_Zealot_Quantity.Value; i++ )
                for (int j = 0; j < Constants.NOV; j++)
                    if (neighbourhood[j, 0] == numericUpDown_Zealots_Degree.Value) // the opinion that zealots are in
                       // if (network_array_opinions[j,0]==1)
                            network_array_opinions[j, 1] = 1; // make the agent zealot
              */
        }


        void count_zealots()
        {
            number_of_zealots = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (network_array_opinions[i, 1] == 1)
                    number_of_zealots++;
        }
        void change_opinion(int agent)
        {
            if (network_array_opinions[agent, 0] == -1)
                network_array_opinions[agent, 0] = 1;
            else
                network_array_opinions[agent, 0] = 1;

        }

        int sigma(int agent)// this function should be run after  find_neighbours() function.
        {
            int sigma_s_count=0; // to hold the agent neighbours sign
            for (int i = 1; i <= neighbourhood[agent, 0]; i++)
                sigma_s_count = network_array_opinions[neighbourhood[agent, i], 0];
            return sigma_s_count;
        }
        void voter_update(int agent)
        {
            find_neighbours();
            double sigma_val= sigma(agent);
            double k=neighbourhood[agent, 0];
            double s= network_array_opinions[agent,0];
            double p_voter = (Math.Abs((double)1 / 2 * (1 - (s / k) * sigma_val)));
            double p_sel= random.NextDouble();
            if (p_sel > p_voter)
                change_opinion(agent);

        }
        void voter_update_2(int agent)
        {
            find_neighbours();
            int rnd_neighbour_index = random.Next(neighbourhood[agent,0]);
            network_array_opinions[agent, 0] = network_array_opinions[rnd_neighbour_index, 0];

        }
        void count_opinion()
        {
            number_of_posotive = 0;
            number_of_negative = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (network_array_opinions[i, 0] == 1)
                    number_of_posotive++;
                else
                    number_of_negative++;

        }
        public Form1()
        {
            InitializeComponent();
        }
        private void btn_reset_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

        }
        private void button1_Click(object sender, EventArgs e)
        {
                ER_Network_generate();
                add_opinion(0.5); //The pecent of 1s
                make_zealot();
               // find_neighbours();
               // using (var writer = new StreamWriter("vahiddddd.csv"))


                for (int it = 0; it < Constants.iter; it++)
                {
                    number_of_negative = 0;
                    number_of_posotive = 0;

                    int agent = random.Next(Constants.NOV);
                    if (network_array_opinions[agent, 1] == 0) // to check the agent to be non-zealot
                        voter_update_2(agent);

                    count_opinion();
                    //writer.WriteLine(it + "    " + number_of_posotive + "    " + number_of_negative);
                    textBox1.Text += it + "    " + number_of_posotive + "    " + number_of_negative; 
                    textBox1.Text += Environment.NewLine;
                    progressBar1.Value = it / 100;

                    {
                        //timer1 = new System.Windows.Forms.Timer();
                        //timer1.Tick += new EventHandler(timer1_Tick);
                        
                    }

                 
                }


        }


        private void button2_Click(object sender, EventArgs e)
        {
            for (int it = 0; it < Constants.iter; it++)
            {
                sample_array[it, 0] = 0;
                sample_array[it, 1] = 0;
            }


            for (int s = 0; s < Constants.sample; s++)
            {
                ER_Network_generate();
                add_opinion(0.5); //The pecent of 1s
                make_zealot(); // The percent of zealots

                for (int it = 0; it < Constants.iter; it++)
                {
                    number_of_negative = 0;
                    number_of_posotive = 0;

                    int agent = random.Next(Constants.NOV);
                        if (network_array_opinions[agent, 1] == 0)
                            voter_update_2(agent);

                     count_opinion();
                     sample_array[it, 0] += number_of_posotive;
                     sample_array[it, 1] += number_of_negative;
                }
                
            }

            for (int it = 0; it < Constants.iter; it++)
            {
                sample_array[it, 0] = sample_array[it, 0] / (Constants.sample * Constants.NOV); // number_of_posotiıve
                sample_array[it, 1] = sample_array[it, 1] / (Constants.sample * Constants.NOV); // number_of_negative

            }
            for (int it = 0; it < Constants.iter; it++)
            {
                textBox1.Text += it + "    " + sample_array[it, 0] + "    " + sample_array[it, 1]; // it    ones     zeros
                textBox1.Text += Environment.NewLine;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ER_Network_generate();
            add_opinion(0.5);
            find_neighbours();
            make_zealot();
           

            for (int i = 0; i < Constants.NOV; i++)
            {
                if (network_array_opinions[i, 1] == 1)
                {
                    textBox1.Text += i + "    " + network_array_opinions[i, 0] + "    " + network_array_opinions[i, 1];
                    textBox1.Text += Environment.NewLine;
                }
                    
            }
                

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SF_Network_generate();

        }


        /*
            
        public static void ListResize<T> (this List<T> list, int size, T element = default(T))
            {
                int count = list.Count;

                if (size < count)
                {
                    list.RemoveRange(size, count - size);
                }
                else if (size > count)
                {
                    if (size > list.Capacity)   // Optimization
                        list.Capacity = size;

                    list.AddRange(Enumerable.Repeat(element, size - count));
                }
            }
        

        private void SF_LCD(List< List<int>> graph, int N, int d)
        {
	        if (d < 1 || d > N - 1)
	        {
                textBox1.Text += "Error: SF_LCD: k_min is out of bounds: "+d;
 	        }

	        List<int> M = new List<int>();
            ListResize (M ,2 * N * d);
	        // M.resize(2 * N * d);

	        int r = -1;
	        //Use Batagelj's implementation of the LCD model
	        for (int v = 0; v < N; v++)
	        {
		        for (int i = 0; i < d; i++)
		        {
			        M[2 * (v * d + i)] = v;
			         r = random.Next(2 * (v * d + i));
			        M[2 * (v * d + i) + 1] = M[r];
		        }
	        }

	        //create the adjacency list
	        graph.Resize(N);
	        bool exists = false;
	        for (int v = 0; v < M.Count; v += 2)
	        {
		        int m = M[v];
		        int n = M[v + 1];

		        graph[m].Add(n);
		        graph[n].Add(m);
	        }
}
        */

        #region Scale-Free network
        void SF_Network_generate()
        {
            double a = 2.5;
            SF_initial_State();
            int link_flag = 0; // a flag to check ıf new_node connected to a new node
            int new_node = 3;

            for (int i=0; i < Constants.NOV; i++)
            {
                double p_SFN = 0;

                do
                {
                    int rnd_node = random.Next(0, new_node);
                    find_neighbours_SF(new_node);
                    int k = neighbourhood_SF[rnd_node, 0];
                    int sigma_k = Calculate_Sigma_k(rnd_node, new_node);
                    p_SFN = 1; //Convert.ToDouble (k / sigma_k);
                    
                    double rnd_select = random.NextDouble();
                    if (p_SFN > rnd_select)
                    {
                        link_SF[new_node, i] = 1;
                        link_SF[i, new_node] = 1;
                        link_flag++;
                    }
                    new_node++;

                } while (link_flag < 2);
            }
        }
        void SF_initial_State()
        {
            link_SF[0, 1] = 1;
            link_SF[1, 0] = 1;

            link_SF[0, 2] = 1;
            link_SF[2, 0] = 1;
        }
        
        int Calculate_Sigma_k(int agent, int graph_size)
        {
            int sigma_k=0;
            for (int i = 0; i < graph_size; i++)
                sigma_k += neighbourhood_SF[i, 0];
            return sigma_k;
        }
        void find_neighbours_SF(int graph_size)
        {
            int k;
            for (int i = 0; i < graph_size; i++)
            {
                k = 0;
                for (int j = 0 ; j < graph_size; j++)
                {
                    if (link_SF[i, j] == 1)
                    {
                        neighbourhood_SF[i, ++k] = j;
                       // textBox2.Text += i + "," + k + "> " + j;
                       // textBox2.Text += Environment.NewLine;
                    }
                    neighbourhood_SF[i, 0] = k; // the index 0 is the nodes link (ki)
                }
               // textBox2.Text += "Links " + i + "= " + k;
               //  textBox2.Text += Environment.NewLine;
                // textBox2.Text += Environment.NewLine;
            }
        }
        #endregion

        private void button5_Click(object sender, EventArgs e)
        {
            SF_Network_generate();
            add_opinion(0.5); //The pecent of 1s
            make_zealot();
            //find_neighbours_SF();

            for (int it = 0; it < Constants.iter; it++)
            {
                number_of_negative = 0;
                number_of_posotive = 0;

                int agent = random.Next(Constants.NOV);
                if (network_array_opinions[agent, 1] == 0) // to check the agent to be non-zealot
                    voter_update(agent);

                count_opinion();
                textBox1.Text += it + "    " + number_of_posotive + "    " + number_of_negative;
                textBox1.Text += Environment.NewLine;


            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i=0; i<Constants.NOV; i++)
            {
                textBox1.Text += i + "    " + neighbourhood_SF[i, 0];
                textBox1.Text += Environment.NewLine;
            }
        }


    }
}

