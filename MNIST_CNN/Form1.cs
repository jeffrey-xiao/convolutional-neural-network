using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MNIST_CNN {
    public partial class Form1 : Form {
        private static string TRAIN_PATH;
        private static string TEST_PATH;
        private static string WEIGHTS_PATH;

        private const int TRAIN_SIZE = 60000;
        private const int TEST_SIZE = 10000;

        private const int CONVOLUTION_SIZE = 5;
        private const int FEATURE_SIZE = 8;
        private const int POOL_SIZE = 2;

        private const int WIDTH = 28;
        private const int HEIGHT = 28;

        private const int CONV_WIDTH_1 = WIDTH - CONVOLUTION_SIZE + 1;
        private const int CONV_HEIGHT_1 = HEIGHT - CONVOLUTION_SIZE + 1;

        private const int POOLED_WIDTH_1 = CONV_WIDTH_1 / 2;
        private const int POOLED_HEIGHT_1 = CONV_HEIGHT_1 / 2;

        private const int CONV_WIDTH_2 = POOLED_WIDTH_1 - CONVOLUTION_SIZE + 1;
        private const int CONV_HEIGHT_2 = POOLED_HEIGHT_1 - CONVOLUTION_SIZE + 1;

        private const int POOLED_WIDTH_2 = CONV_WIDTH_2 / 2;
        private const int POOLED_HEIGHT_2 = CONV_HEIGHT_2 / 2;

        private const int HIDDEN_SIZE = 15;
        private const int OUTPUT_SIZE = 10;

        private Random rand = new Random();
        private double[,,] cw1 = new double[FEATURE_SIZE, CONVOLUTION_SIZE, CONVOLUTION_SIZE];
        private double[,,,] cw2 = new double[FEATURE_SIZE, FEATURE_SIZE, CONVOLUTION_SIZE, CONVOLUTION_SIZE];
        private double[] cb1 = new double[FEATURE_SIZE];
        private double[] cb2 = new double[FEATURE_SIZE];

        private double[,,,] w1 = new double[FEATURE_SIZE, POOLED_WIDTH_2, POOLED_HEIGHT_2, HIDDEN_SIZE];
        private double[,] w2 = new double[HIDDEN_SIZE, OUTPUT_SIZE];

        private double[] b1 = new double[HIDDEN_SIZE];
        private double[] b2 = new double[OUTPUT_SIZE];

        private double[][,] train;
        private double[][,] test;

        private int[] trainAnswer;
        private int[] testAnswer;

        private double correct = 0;
        private double total = 0;
        private double cost;
        private double iterations = 0;
        private Queue<double> costList = new Queue<double>();
        private Queue<bool> correctList = new Queue<bool>();

        private bool training = false;


        public Form1 () {
            InitializeComponent();
        }

        private void Initialize_Weights_Click (object sender, EventArgs e) {
            double range1 = 1 / Math.Sqrt(CONVOLUTION_SIZE * CONVOLUTION_SIZE);
            double range2 = 1 / Math.Sqrt(FEATURE_SIZE * CONVOLUTION_SIZE * CONVOLUTION_SIZE);
            double range3 = 1 / Math.Sqrt(FEATURE_SIZE * POOLED_WIDTH_2 * POOLED_HEIGHT_2);
            double range4 = 1 / Math.Sqrt(HIDDEN_SIZE);

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < CONVOLUTION_SIZE; j++)
                    for (int k = 0; k < CONVOLUTION_SIZE; k++)
                    {
                        cw1[i, j, k] = GetRandomGaussian(range1);
                    }
            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < FEATURE_SIZE; j++)
                    for (int k = 0; k < CONVOLUTION_SIZE; k++)
                        for (int l = 0; l < CONVOLUTION_SIZE; l++)
                            cw2[i, j, k, l] = GetRandomGaussian(range2);

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < POOLED_WIDTH_2; j++)
                    for (int k = 0; k < POOLED_HEIGHT_2; k++)
                        for (int l = 0; l < HIDDEN_SIZE; l++)
                            w1[i, j, k, l] = GetRandomGaussian(range3);

            for (int i = 0; i < HIDDEN_SIZE; i++)
                for (int j = 0; j < OUTPUT_SIZE; j++)
                    w2[i, j] = GetRandomGaussian(range4);

            for (int i = 0; i < HIDDEN_SIZE; i++)
                b1[i] = 0;

            for (int i = 0; i < OUTPUT_SIZE; i++)
                b2[i] = 0;

            for (int i = 0; i < FEATURE_SIZE; i++)
                cb1[i] = cb2[i] = 0;

            this.Text = "Finished initializing";
        }

        private void LoadData_Click (object sender, EventArgs e) {
            TRAIN_PATH = TrainingData.Text;
            TEST_PATH = TestingData.Text;

            train = new double[TRAIN_SIZE][,];
            trainAnswer = new int[TRAIN_SIZE];

            test = new double[TEST_SIZE][,];
            testAnswer = new int[TEST_SIZE];

            using (StreamReader reader = new StreamReader(TRAIN_PATH)) {
                for (int i = 0; i < TRAIN_SIZE; i++) {
                    string inputData = reader.ReadLine();
                    string[] data = inputData.Split(',');
                    trainAnswer[i] = int.Parse(data[0]);
                    train[i] = new double[WIDTH, HEIGHT];
                    for (int j = 1; j <= WIDTH * HEIGHT; j++)
                        train[i][(j - 1) / HEIGHT, (j - 1) % HEIGHT] = double.Parse(data[j]) / 255.0;
                }
            }

            using (StreamReader reader = new StreamReader(TEST_PATH)) {
                for (int i = 0; i < TEST_SIZE; i++) {
                    string inputData = reader.ReadLine();
                    string[] data = inputData.Split(',');
                    testAnswer[i] = int.Parse(data[0]);
                    test[i] = new double[WIDTH, HEIGHT];
                    for (int j = 1; j <= WIDTH * HEIGHT; j++)
                        test[i][(j - 1) / HEIGHT, (j - 1) % HEIGHT] = double.Parse(data[j]) / 255.0;
                }
            }
            this.Text = "Finished loading data!";
        }

        private void Stop_Click (object sender, EventArgs e) {
            training = false;
        }

        private void Train_Click (object sender, EventArgs e) {
            total = 0;
            correct = 0;
            cost = 0;
            costList.Clear();
            correctList.Clear();
            training = true;

            while (training) {
                ShuffleTraining();
                for (int j = 0; j < TRAIN_SIZE && training; j++) {
                    Application.DoEvents();
                    double alpha = 1000.0 / (iterations + 20000);
                    
                    if (Predict(RunNetwork(train[j]), trainAnswer[j])) {
                        correct++;
                        correctList.Enqueue(true);
                    } else {
                        correctList.Enqueue(false);
                    }

                    double currCost = BackPropagate(train[j], trainAnswer[j], alpha);

                    cost += currCost;
                    costList.Enqueue(currCost);

                    total++;
                    iterations++;

                    if (total > 60000) {
                        total--;
                        if (correctList.Dequeue())
                            correct--;
                        cost -= costList.Dequeue();
                    }

                    this.Text = correct + "/" + total + " " + String.Format("{0:0.0000} {1:0.0000} {2}", correct / total * 100.0, cost / total, iterations);
                }
            }
        }

        private void TestButton_Click (object sender, EventArgs e) {
            total = 0;
            correct = 0;
            correctList.Clear();
            for (int i = 0; i < TEST_SIZE; i++) {
                if (Predict(RunNetwork(test[i]), testAnswer[i]))
                    correct++;
                total++;
                this.Text = correct + "/" + total + " " + String.Format("{0:0.0000}", correct / total * 100.0);
            }
        }

        private void ShuffleTraining () {
            for (int i = 0; i < TRAIN_SIZE; i++) {
                int swapIndex = rand.Next(TRAIN_SIZE - i) + i;
                double[,] tempArray = train[i];
                train[i] = train[swapIndex];
                train[swapIndex] = tempArray;

                int temp = trainAnswer[i];
                trainAnswer[i] = trainAnswer[swapIndex];
                trainAnswer[swapIndex] = temp;
            }
        }

        private bool Predict (double[] output, int ans) {
            int max = 0;
            for (int i = 1; i < OUTPUT_SIZE; i++)
                if (output[i] > output[max])
                    max = i;

            return max == ans;
        }

        private double[] RunNetwork (double[,] input) {
            double[,,] a1 = new double[FEATURE_SIZE, CONV_WIDTH_1, CONV_HEIGHT_1];
            double[,,] p1 = new double[FEATURE_SIZE, POOLED_WIDTH_1, POOLED_HEIGHT_1];

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < CONV_WIDTH_1; j++)
                    for (int k = 0; k < CONV_HEIGHT_1; k++)
                        a1[i, j, k] = cb1[i];

            for (int i = 0; i < FEATURE_SIZE; i++) {
                for (int m = 0; m < CONV_WIDTH_1; m++)
                    for (int n = 0; n < CONV_HEIGHT_1; n++) {
                        for (int j = 0; j < CONVOLUTION_SIZE; j++)
                            for (int k = 0; k < CONVOLUTION_SIZE; k++)
                                a1[i, m, n] += cw1[i, j, k] * input[m + j, n + k];
                        a1[i, m, n] = Math.Tanh(a1[i, m, n]);
                    }

                for (int m = 0; m < POOLED_WIDTH_1; m++)
                    for (int n = 0; n < POOLED_HEIGHT_1; n++) {
                        p1[i, m, n] = Math.Max(Math.Max(a1[i, m * 2, n * 2], a1[i, m * 2, n * 2 + 1]),
                                               Math.Max(a1[i, m * 2 + 1, n * 2], a1[i, m * 2 + 1, n * 2 + 1]));

                    }

            }
            double[,,] a2 = new double[FEATURE_SIZE, CONV_WIDTH_2, CONV_HEIGHT_2];
            double[,,] p2 = new double[FEATURE_SIZE, POOLED_WIDTH_2, POOLED_HEIGHT_2];

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < CONV_WIDTH_2; j++)
                    for (int k = 0; k < CONV_HEIGHT_2; k++)
                        a2[i, j, k] = cb2[i];

            for (int map = 0; map < FEATURE_SIZE; map++) {
                for (int i = 0; i < CONV_WIDTH_2; i++) {
                    for (int j = 0; j < CONV_HEIGHT_2; j++) {
                        for (int pmap = 0; pmap < FEATURE_SIZE; pmap++) {
                            for (int m = 0; m < CONVOLUTION_SIZE; m++) {
                                for (int n = 0; n < CONVOLUTION_SIZE; n++) {
                                    a2[map, i, j] += cw2[map, pmap, m, n] * p1[pmap, i + m, j + n];
                                }
                            }
                        }
                        a2[map, i, j] = Math.Tanh(a2[map, i, j]);
                    }
                }
            }

            for (int map = 0; map < FEATURE_SIZE; map++)
                for (int i = 0; i < POOLED_WIDTH_2; i++)
                    for (int j = 0; j < POOLED_HEIGHT_2; j++)
                        p2[map, i, j] = Math.Max(Math.Max(a2[map, i * 2, j * 2], a2[map, i * 2, j * 2 + 1]),
                                                 Math.Max(a2[map, i * 2 + 1, j * 2], a2[map, i * 2 + 1, j * 2 + 1]));

            double[] a3 = new double[HIDDEN_SIZE];

            for (int i = 0; i < HIDDEN_SIZE; i++)
                a3[i] = b1[i];

            for (int pmap = 0; pmap < FEATURE_SIZE; pmap++)
                for (int i = 0; i < POOLED_WIDTH_2; i++)
                    for (int j = 0; j < POOLED_HEIGHT_2; j++)
                        for (int k = 0; k < HIDDEN_SIZE; k++)
                            a3[k] += p2[pmap, i, j] * w1[pmap, i, j, k];

            for (int i = 0; i < HIDDEN_SIZE; i++)
                a3[i] = Math.Tanh(a3[i]);

            double[] a4 = new double[OUTPUT_SIZE];

            for (int i = 0; i < OUTPUT_SIZE; i++)
                a4[i] = b2[i];

            for (int i = 0; i < HIDDEN_SIZE; i++)
                for (int j = 0; j < OUTPUT_SIZE; j++)
                    a4[j] += w2[i, j] * a3[i];

            for (int i = 0; i < OUTPUT_SIZE; i++)
                a4[i] = Math.Tanh(a4[i]);

            return a4;
        }

        // returns true if correct and false if incorrect
        private double BackPropagate (double[,] input, int answer, double learningRate) {

            double[,,] a1 = new double[FEATURE_SIZE, CONV_WIDTH_1, CONV_HEIGHT_1];
            double[,,] p1 = new double[FEATURE_SIZE, POOLED_WIDTH_1, POOLED_HEIGHT_1];

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < CONV_WIDTH_1; j++)
                    for (int k = 0; k < CONV_HEIGHT_1; k++)
                        a1[i, j, k] = cb1[i];

            for (int i = 0; i < FEATURE_SIZE; i++) {
                for (int m = 0; m < CONV_WIDTH_1; m++)
                    for (int n = 0; n < CONV_HEIGHT_1; n++) {
                        for (int j = 0; j < CONVOLUTION_SIZE; j++)
                            for (int k = 0; k < CONVOLUTION_SIZE; k++)
                                a1[i, m, n] += cw1[i, j, k] * input[m + j, n + k];
                        a1[i, m, n] = Math.Tanh(a1[i, m, n]);
                    }

                for (int m = 0; m < POOLED_WIDTH_1; m++)
                    for (int n = 0; n < POOLED_HEIGHT_1; n++) {
                        p1[i, m, n] = Math.Max(Math.Max(a1[i, m * 2, n * 2], a1[i, m * 2, n * 2 + 1]),
                                               Math.Max(a1[i, m * 2 + 1, n * 2], a1[i, m * 2 + 1, n * 2 + 1]));

                    }
            }

            double[,,] a2 = new double[FEATURE_SIZE, CONV_WIDTH_2, CONV_HEIGHT_2];
            double[,,] p2 = new double[FEATURE_SIZE, POOLED_WIDTH_2, POOLED_HEIGHT_2];

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < CONV_WIDTH_2; j++)
                    for (int k = 0; k < CONV_HEIGHT_2; k++)
                        a2[i, j, k] = cb2[i];

            for (int map = 0; map < FEATURE_SIZE; map++) {
                for (int i = 0; i < CONV_WIDTH_2; i++) {
                    for (int j = 0; j < CONV_HEIGHT_2; j++) {
                        for (int pmap = 0; pmap < FEATURE_SIZE; pmap++) {
                            for (int m = 0; m < CONVOLUTION_SIZE; m++) {
                                for (int n = 0; n < CONVOLUTION_SIZE; n++) {
                                    a2[map, i, j] += cw2[map, pmap, m, n] * p1[pmap, i + m, j + n];
                                }
                            }
                        }
                        a2[map, i, j] = Math.Tanh(a2[map, i, j]);
                    }
                }
            }

            for (int map = 0; map < FEATURE_SIZE; map++)
                for (int i = 0; i < POOLED_WIDTH_2; i++)
                    for (int j = 0; j < POOLED_HEIGHT_2; j++)
                        p2[map, i, j] = Math.Max(Math.Max(a2[map, i * 2, j * 2], a2[map, i * 2, j * 2 + 1]),
                                                 Math.Max(a2[map, i * 2 + 1, j * 2], a2[map, i * 2 + 1, j * 2 + 1]));

            double[] a3 = new double[HIDDEN_SIZE];

            for (int i = 0; i < HIDDEN_SIZE; i++)
                a3[i] = b1[i];

            for (int pmap = 0; pmap < FEATURE_SIZE; pmap++)
                for (int i = 0; i < POOLED_WIDTH_2; i++)
                    for (int j = 0; j < POOLED_HEIGHT_2; j++)
                        for (int k = 0; k < HIDDEN_SIZE; k++)
                            a3[k] += p2[pmap, i, j] * w1[pmap, i, j, k];

            for (int i = 0; i < HIDDEN_SIZE; i++)
                a3[i] = Math.Tanh(a3[i]);

            double[] a4 = new double[OUTPUT_SIZE];

            for (int i = 0; i < OUTPUT_SIZE; i++)
                a4[i] = b2[i];

            for (int i = 0; i < HIDDEN_SIZE; i++)
                for (int j = 0; j < OUTPUT_SIZE; j++)
                    a4[j] += w2[i, j] * a3[i];

            for (int i = 0; i < OUTPUT_SIZE; i++)
                a4[i] = Math.Tanh(a4[i]);


            // CALCULATING ERROR

            double[] e4 = new double[OUTPUT_SIZE];

            double cost = 0;

            for (int i = 0; i < OUTPUT_SIZE; i++) {
                e4[i] = (a4[i] - (i == answer ? 1 : -1)) * (1 - a4[i] * a4[i]);
                cost += (a4[i] - (i == answer ? 1 : -1)) * (a4[i] - (i == answer ? 1 : -1));
            }


            double[] e3 = new double[HIDDEN_SIZE];
            for (int i = 0; i < HIDDEN_SIZE; i++)
                for (int j = 0; j < OUTPUT_SIZE; j++)
                    e3[i] += e4[j] * w2[i, j] * (1 - a3[i] * a3[i]);

            double[,,] pe2 = new double[FEATURE_SIZE, POOLED_WIDTH_2, POOLED_HEIGHT_2];
            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < POOLED_WIDTH_2; j++)
                    for (int k = 0; k < POOLED_HEIGHT_2; k++)
                        for (int l = 0; l < HIDDEN_SIZE; l++)
                            pe2[i, j, k] += e3[l] * w1[i, j, k, l];

            double[,,] ce2 = new double[FEATURE_SIZE, CONV_WIDTH_2, CONV_HEIGHT_2];
            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < POOLED_WIDTH_2; j++)
                    for (int k = 0; k < POOLED_HEIGHT_2; k++) {

                        if (a2[i, j * 2, k * 2] == p2[i, j, k])
                            ce2[i, j * 2, k * 2] = pe2[i, j, k];

                        else if (a2[i, j * 2 + 1, k * 2] == p2[i, j, k])
                            ce2[i, j * 2 + 1, k * 2] = pe2[i, j, k];

                        else if (a2[i, j * 2, k * 2 + 1] == p2[i, j, k])
                            ce2[i, j * 2, k * 2 + 1] = pe2[i, j, k];

                        else if (a2[i, j * 2 + 1, k * 2 + 1] == p2[i, j, k])
                            ce2[i, j * 2 + 1, k * 2 + 1] = pe2[i, j, k];
                    }

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < CONV_WIDTH_2; j++)
                    for (int k = 0; k < CONV_HEIGHT_2; k++)
                        ce2[i, j, k] *= (1 - a2[i, j, k] * a2[i, j, k]);

            double[,,] pe1 = new double[FEATURE_SIZE, POOLED_WIDTH_1, POOLED_HEIGHT_1];

            for (int map = 0; map < FEATURE_SIZE; map++)
                for (int i = 0; i < CONV_WIDTH_2; i++)
                    for (int j = 0; j < CONV_HEIGHT_2; j++) {
                        for (int pmap = 0; pmap < FEATURE_SIZE; pmap++) {
                            for (int m = 0; m < CONVOLUTION_SIZE; m++)
                                for (int n = 0; n < CONVOLUTION_SIZE; n++)
                                    pe1[pmap, i + m, j + n] += ce2[map, i, j] * cw2[map, pmap, m, n];
                        }
                    }

            double[,,] ce1 = new double[FEATURE_SIZE, CONV_WIDTH_1, CONV_HEIGHT_1];
            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < POOLED_WIDTH_1; j++)
                    for (int k = 0; k < POOLED_HEIGHT_1; k++) {

                        if (a1[i, j * 2, k * 2] == p1[i, j, k])
                            ce1[i, j * 2, k * 2] = pe1[i, j, k];

                        else if (a1[i, j * 2 + 1, k * 2] == p1[i, j, k])
                            ce1[i, j * 2 + 1, k * 2] = pe1[i, j, k];

                        else if (a1[i, j * 2, k * 2 + 1] == p1[i, j, k])
                            ce1[i, j * 2, k * 2 + 1] = pe1[i, j, k];

                        else if (a1[i, j * 2 + 1, k * 2 + 1] == p1[i, j, k])
                            ce1[i, j * 2 + 1, k * 2 + 1] = pe1[i, j, k];
                    }

            for (int i = 0; i < FEATURE_SIZE; i++)
                for (int j = 0; j < CONV_WIDTH_1; j++)
                    for (int k = 0; k < CONV_HEIGHT_1; k++)
                        ce1[i, j, k] *= (1 - a1[i, j, k] * a1[i, j, k]);


            // GRADIENT DESCENT
            for (int i = 0; i < HIDDEN_SIZE; i++)
                for (int j = 0; j < OUTPUT_SIZE; j++)
                    w2[i, j] -= a3[i] * e4[j] * learningRate;

            for (int map = 0; map < FEATURE_SIZE; map++)
                for (int i = 0; i < POOLED_WIDTH_2; i++)
                    for (int j = 0; j < POOLED_HEIGHT_2; j++)
                        for (int k = 0; k < HIDDEN_SIZE; k++)
                            w1[map, i, j, k] -= p2[map, i, j] * e3[k] * learningRate;

            for (int map = 0; map < FEATURE_SIZE; map++)
                for (int i = 0; i < CONV_WIDTH_2; i++)
                    for (int j = 0; j < CONV_HEIGHT_2; j++)
                        for (int pmap = 0; pmap < FEATURE_SIZE; pmap++)
                            for (int m = 0; m < CONVOLUTION_SIZE; m++)
                                for (int n = 0; n < CONVOLUTION_SIZE; n++)
                                    cw2[map, pmap, m, n] -= p1[pmap, i + m, j + n] * ce2[map, i, j] * learningRate;

            for (int map = 0; map < FEATURE_SIZE; map++)
                for (int i = 0; i < CONV_WIDTH_1; i++)
                    for (int j = 0; j < CONV_HEIGHT_1; j++)
                        for (int m = 0; m < CONVOLUTION_SIZE; m++)
                            for (int n = 0; n < CONVOLUTION_SIZE; n++)
                                cw1[map, m, n] -= input[i + m, j + n] * ce1[map, i, j] * learningRate;

            for (int i = 0; i < FEATURE_SIZE; i++) {
                for (int j = 0; j < CONV_WIDTH_2; j++) {
                    for (int k = 0; k < CONV_HEIGHT_2; k++) {
                        cb2[i] -= ce2[i, j, k] * learningRate;
                    }
                }

                for (int j = 0; j < CONV_WIDTH_1; j++) {
                    for (int k = 0; k < CONV_HEIGHT_1; k++) {
                        cb1[i] -= ce1[i, j, k] * learningRate;
                    }
                }
            }

            for (int i = 0; i < HIDDEN_SIZE; i++)
                b1[i] -= e3[i] * learningRate;

            for (int i = 0; i < OUTPUT_SIZE; i++)
                b2[i] -= e4[i] * learningRate;

            return cost;
        }

        private double GetRandomGaussian (double stdDev) {
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return stdDev * randStdNormal;
        }

        private void SaveWeights_Click (object sender, EventArgs e) {
            WEIGHTS_PATH = WeightsPath.Text;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(WEIGHTS_PATH)) {
                file.WriteLine(iterations);
                for (int i = 0; i < FEATURE_SIZE; i++)
                    for (int j = 0; j < CONVOLUTION_SIZE; j++)
                        for (int k = 0; k < CONVOLUTION_SIZE; k++)
                            file.WriteLine(cw1[i, j, k]);

                for (int i = 0; i < FEATURE_SIZE; i++)
                    for (int j = 0; j < FEATURE_SIZE; j++)
                        for (int k = 0; k < CONVOLUTION_SIZE; k++)
                            for (int l = 0; l < CONVOLUTION_SIZE; l++)
                                file.WriteLine(cw2[i, j, k, l]);

                for (int i = 0; i < FEATURE_SIZE; i++)
                    for (int j = 0; j < POOLED_WIDTH_2; j++)
                        for (int k = 0; k < POOLED_HEIGHT_2; k++)
                            for (int l = 0; l < HIDDEN_SIZE; l++)
                                file.WriteLine(w1[i, j, k, l]);

                for (int i = 0; i < HIDDEN_SIZE; i++)
                    for (int j = 0; j < OUTPUT_SIZE; j++)
                        file.WriteLine(w2[i, j]);

                for (int i = 0; i < HIDDEN_SIZE; i++)
                    file.WriteLine(b1[i]);

                for (int i = 0; i < OUTPUT_SIZE; i++)
                    file.WriteLine(b2[i]);

                for (int i = 0; i < FEATURE_SIZE; i++)
                    file.WriteLine(cb1[i]);
                for (int i = 0; i < FEATURE_SIZE; i++)
                    file.WriteLine(cb2[i]);
            }
            this.Text = "Finished saving weights";
        }

        private void LoadWeights_Click (object sender, EventArgs e) {
            WEIGHTS_PATH = WeightsPath.Text;
            total = 0;
            correct = 0;
            correctList.Clear();
            using (StreamReader reader = new StreamReader(WEIGHTS_PATH)) {
                iterations = double.Parse(reader.ReadLine());
                for (int i = 0; i < FEATURE_SIZE; i++)
                    for (int j = 0; j < CONVOLUTION_SIZE; j++)
                        for (int k = 0; k < CONVOLUTION_SIZE; k++)
                            cw1[i, j, k] = double.Parse(reader.ReadLine());

                for (int i = 0; i < FEATURE_SIZE; i++)
                    for (int j = 0; j < FEATURE_SIZE; j++)
                        for (int k = 0; k < CONVOLUTION_SIZE; k++)
                            for (int l = 0; l < CONVOLUTION_SIZE; l++)
                                cw2[i, j, k, l] = double.Parse(reader.ReadLine());

                for (int i = 0; i < FEATURE_SIZE; i++)
                    for (int j = 0; j < POOLED_WIDTH_2; j++)
                        for (int k = 0; k < POOLED_HEIGHT_2; k++)
                            for (int l = 0; l < HIDDEN_SIZE; l++)
                                w1[i, j, k, l] = double.Parse(reader.ReadLine());

                for (int i = 0; i < HIDDEN_SIZE; i++)
                    for (int j = 0; j < OUTPUT_SIZE; j++)
                        w2[i, j] = double.Parse(reader.ReadLine());

                for (int i = 0; i < HIDDEN_SIZE; i++) {
                    b1[i] = double.Parse(reader.ReadLine());
                }

                for (int i = 0; i < OUTPUT_SIZE; i++)
                    b2[i] = double.Parse(reader.ReadLine());

                for (int i = 0; i < FEATURE_SIZE; i++)
                    cb1[i] = double.Parse(reader.ReadLine());
                for (int i = 0; i < FEATURE_SIZE; i++)
                    cb2[i] = double.Parse(reader.ReadLine());
            }
            this.Text = "Finished loading weights";
        }
    }
}
