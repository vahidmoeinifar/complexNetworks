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
            public const int iter = 5000;
            public const int sample = 50;
        }

        System.Random random = new System.Random();
        int number_of_posotiıve;
        int number_of_negative;
        int number_of_zeologs;
        Int32[,] network_array = new Int32[Constants.NOV, 2]; // network array and opinions-- [i,0]== opinions -- [i,1] == alphas
        double[,] sample_array = new double[Constants.iter, 2];
        
        //random ER graph
        int[,] link = new int[Constants.NOV, Constants.NOV];
        double p_ER_graph = 0.1;
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
                        // textBox2.Text += Environment.NewLine;
                    }
                    neighbourhood[i, 0] = k;
                }
                // textBox2.Text += "Links " + i + "= " + k;
                // textBox2.Text += Environment.NewLine;
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
        void make_zoalot(double p)
        {
            for (int i = 0; i < Constants.NOV; i++)
                    network_array[i, 1] = 0; // alpha =1 means zealot

            for (int i = 0; i < Constants.NOV; i++)
                if (network_array[i, 0] == 1) // the opinion that zealots are in
                    if (p >= random.NextDouble())
                        network_array[i, 1] = 1; // alpha =1 means zealot
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

        void majority_update(int agent)
        {
            /////////////////////////////////////// Model Description //////////////////////////////////////////////
            //    In this model, a random agent will be select and then we look at two of the opinions            //
            //    of the neighborhood.The majority opinion will be confirmed to all three agents. Each            //
            //    agent that is zealot, don't change its opinion.                                                 //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////

            int r = Convert.ToInt32(numericUpDown_r_value.Value);
            List<int> r_List = new List<int>(); // list to input r agent in
            int count = 0;
            r_List.Clear();

            for (int i = 0; i < r; i++) // to fill r_list with r agents
            {
                int rnd_agent = random.Next(Constants.NOV);
                r_List.Add(rnd_agent);
                count += network_array[rnd_agent, 0];
            }
            if (count > r/2) // posotive
                 for (int j = 0; j < r; j++) // to set all agents 1
                      network_array[r_List[j],0] = 1;
            else
                for (int j = 0; j < r; j++) // to set all agents 0
                    network_array[r_List[j], 0] = 0;


        }
        void majority_update_majoity_zealots(int agent)
        {
            /////////////////////////////////////// Model Description //////////////////////////////////////////////
            //    In this model, a random agent will be select and then we look at r number of            //
            //    it`s neighborhoods.The majority opinion will be confirmed to all three agents. Each            //
            //    agent that is zealot, don't change their opinion.                                               //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////

            int rnd_agent2 = random.Next(Constants.NOV);
            int rnd_agent3 = random.Next(Constants.NOV);
            int count = 0;
            count += (network_array[agent, 0]);
            count += (network_array[rnd_agent2, 0]);
            count += (network_array[rnd_agent3, 0]);

            if (count >= 2)
            {
                if (network_array[agent, 1] == 0)
                    network_array[agent, 0] = 1;
                if (network_array[rnd_agent2, 1] == 0)
                    network_array[rnd_agent2, 0] = 1;
                if (network_array[rnd_agent3, 1] == 0)
                    network_array[rnd_agent3, 0] = 1;

            }
            else
            {
                if (network_array[agent, 1] == 0)
                    network_array[agent, 0] = 0;
                if (network_array[rnd_agent2, 1] == 0)
                    network_array[rnd_agent2, 0] = 0;
                if (network_array[rnd_agent3, 1] == 0)
                    network_array[rnd_agent3, 0] = 0;
            }

        }
        void majority_update_the_power_of_zealots(int agent)
        {
            /////////////////////////////////////// Model Description //////////////////////////////////////////////
            //    In this model, a random agent will be select and then we look at r number of            //
            //    it`s neighborhoods.The majority opinion will be confirmed to all three agents. Each            //
            //    agent that is zealot, don't change their opinion.                                               //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            int rnd_agent2 = random.Next(Constants.NOV);
            int rnd_agent3 = random.Next(Constants.NOV);

            if (network_array[agent, 1] == 1)
            {
                if (network_array[rnd_agent2, 1] == 0)
                    network_array[rnd_agent2, 0] = network_array[agent, 0];
                if (network_array[rnd_agent3, 1] == 0)
                    network_array[rnd_agent3, 0] = network_array[agent, 0];
            }
                
        }
        void voter_update(int agent)
        {
            find_neighbours();
            int rnd_neighbour = random.Next(1, neighbourhood[agent, 0] + 1);
            network_array[agent, 0] = network_array[neighbourhood[agent, rnd_neighbour], 0]; 

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

        private void btn_model1_Click(object sender, EventArgs e)
        {
            for (int it = 0; it < Constants.iter; it++)
            {
                sample_array[it, 0] = 0;
                sample_array[it, 1] = 0;
            }


            for (int s = 0; s < Constants.sample; s++ )
            {
                add_opinion(0.3); //The pecent of 1s
                make_zoalot(0.1); // The percent of zealots

                for (int it = 0; it < Constants.iter; it++)
                {
                    number_of_negative = 0;
                    number_of_posotiıve = 0;
                    majority_update(random.Next(Constants.NOV));

                    count_opinion();
                    sample_array[it,0] += number_of_posotiıve ;
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
            textBox1.Text = "";

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = @"d:\";
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.FileName = "*.dat";
            saveFileDialog1.DefaultExt = "dat";
            saveFileDialog1.Filter = "Data files (*.dat)|*.dat|All files (*.*)|*.*";


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream filestream = saveFileDialog1.OpenFile();
                StreamWriter sw = new StreamWriter(filestream);
                sw.Write(textBox1.Text);
                sw.Close();
                filestream.Close();
            }
        }

        private void betn_model3_Click(object sender, EventArgs e)
        {
            add_opinion(0.5); // The pecent of 1s
            make_zoalot(0.9);

            count_opinion();
            textBox1.Text += 0 + "    " + number_of_posotiıve + "    " + number_of_negative;
            textBox1.Text += Environment.NewLine;

            for (int i = 1; i < Constants.iter; i++)
            {
                for (int j = 0; j < Constants.NOV; j++)
                {
                    majority_update_the_power_of_zealots(j);
                    count_opinion();
                }
                textBox1.Text += i + "    " + number_of_posotiıve + "    " + number_of_negative;
                textBox1.Text += Environment.NewLine;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btn_model2_Click(object sender, EventArgs e)
        {
            for (int it = 0; it < Constants.iter; it++)
            {
                sample_array[it, 0] = 0;
                sample_array[it, 1] = 0;
            }


            for (int s = 0; s < Constants.sample; s++)
            {
                add_opinion(0.3); //The pecent of 1s
                make_zoalot(0.1); // The percent of zealots

                for (int it = 0; it < Constants.iter; it++)
                {
                    number_of_negative = 0;
                    number_of_posotiıve = 0;
                    int agent = random.Next(Constants.NOV);
                    if (network_array[agent,1]==0)
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
                make_zoalot(0.0); // The percent of zealots

                for (int it = 0; it < Constants.iter; it++)
                {
                    number_of_negative = 0;
                    number_of_posotiıve = 0;

                    int agent = random.Next(Constants.NOV);
                        if (network_array[agent, 1] == 0)
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

       

       
    }
}
