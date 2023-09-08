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
            public const int NOV = 10;
            public const int iter = 5000;
            public const int sample = 50;
        }

        System.Random random = new System.Random();
        int number_of_posotiıve;
        int number_of_negative;
        int number_of_zeologs;
        Int32[,] network_array = new Int32[Constants.NOV, 2]; // network array and opinions-- [i,0]== opinions -- [i,1] == zealot flag
        double[,] sample_array = new double[Constants.iter, 2];
        
        //random ER graph
        int[,] link = new int[Constants.NOV, Constants.NOV];
        double p_ER_graph = 0.4;
        int[,] neighbourhood = new int[Constants.NOV, Constants.NOV]; // neighbours array


        void ER_Network_generate()
        {
            empty_network_generate();
            for (int i = 0; i < Constants.NOV; i++)             // generate random network
                for (int j = i + 1; j < Constants.NOV; j++)
                {
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
               //  textBox2.Text += "Links " + i + "= " + k;
               //  textBox2.Text += Environment.NewLine;
                // textBox2.Text += Environment.NewLine;
            }
        }
        void find_neighbours_of_an_agent(int agent)
        {
            int rnd_neighbour = random.Next(1, neighbourhood[agent, 0]+1);
            network_array[agent,0] = network_array[neighbourhood[agent, rnd_neighbour], 0]; 

        }
        void add_opinion(double p)
        {
            for (int i = 0; i < Constants.NOV; i++) //make all opinion 0
            {
                network_array[i, 0] = 0;
                //network_array[i, 1] = 0;
            }
            for (int i = 0; i < Constants.NOV; i++)
            {
                if (p >= random.NextDouble())
                    network_array[i, 0] = 1;
                else
                    network_array[i, 0] = 0;
            }
        }
        void make_zoalot()
        {
            for (int i = 0; i < Constants.NOV; i++)
                    network_array[i, 1] = 0; // alpha =1 means zealot
            for (int i = 1; i <= numericUpDown_Number.Value; i++)
                for (int j = 0; j < Constants.NOV; j++ )
                    if (neighbourhood[j,0] == numericUpDown_Degree.Value) // the opinion that zealots are in
                            network_array[j, 1] = 1; // to make node j zealot
        }
        void count_zeologs()
        {
            number_of_zeologs = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (network_array[i, 1] == 1)
                    number_of_zeologs++;
        }
        void change_opinion(int agent)
        {
            if (network_array[agent, 0] == 0)
                network_array[agent, 0] = 1;
            else
                network_array[agent, 0] = 0;
        }
    
        int sigma(int agent)// this function should be run after  find_neighbours() function.
        {
            int sigma_s_count=0; // to hold the agent neighbours sign
            for (int i = 1; i <= neighbourhood[agent, 0]; i++)
                sigma_s_count = network_array[neighbourhood[agent, i], 0];
            return sigma_s_count;
        }
        void voter_update(int agent)
        {
            find_neighbours();
            int sigma_val= sigma(agent);
            int k=neighbourhood[agent, 0];
            int s= network_array[agent,0];
            double p_voter = 1/2 * (1- (s/k)*sigma_val);
            if (random.NextDouble() > p_voter)
                change_opinion(agent);
        }
        void count_opinion()
        {
            number_of_posotiıve = 0;
            number_of_negative = 0;
            for (int i = 0; i < Constants.NOV; i++)
                if (network_array[i, 0] == 1)
                    number_of_posotiıve++;
                else
                    number_of_negative++;

        }
        public Form1()
        {
            InitializeComponent();
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
                make_zoalot(); // The percent of zealots

                for (int it = 0; it < Constants.iter; it++)
                {
                    number_of_negative = 0;
                    number_of_posotiıve = 0;

                    int agent = random.Next(Constants.NOV);
                       // if (network_array[agent, 1] == 0) //checking the zealot flag
                            voter_update(agent);

                     count_opinion();
                     sample_array[it, 0] += number_of_posotiıve;
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

        private void btn_reset_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ER_Network_generate();
            add_opinion(0.5); //The pecent of 1s
            make_zoalot(); // The percent of zealots
            find_neighbours(); // to test the neighbourhood delete all // in find_neighbours() function

        }

      

    }
}
