namespace MNIST_CNN
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.Initialize_Weights = new System.Windows.Forms.Button();
            this.Train = new System.Windows.Forms.Button();
            this.TrainingData = new System.Windows.Forms.TextBox();
            this.TestingData = new System.Windows.Forms.TextBox();
            this.SaveWeights = new System.Windows.Forms.Button();
            this.LoadWeights = new System.Windows.Forms.Button();
            this.LoadData = new System.Windows.Forms.Button();
            this.WeightsPath = new System.Windows.Forms.TextBox();
            this.TestButton = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Initialize_Weights
            // 
            this.Initialize_Weights.Location = new System.Drawing.Point(556, 12);
            this.Initialize_Weights.Name = "Initialize_Weights";
            this.Initialize_Weights.Size = new System.Drawing.Size(126, 32);
            this.Initialize_Weights.TabIndex = 0;
            this.Initialize_Weights.Text = "Initialize Weights";
            this.Initialize_Weights.UseVisualStyleBackColor = true;
            this.Initialize_Weights.Click += new System.EventHandler(this.Initialize_Weights_Click);
            // 
            // Train
            // 
            this.Train.Location = new System.Drawing.Point(556, 50);
            this.Train.Name = "Train";
            this.Train.Size = new System.Drawing.Size(126, 28);
            this.Train.TabIndex = 1;
            this.Train.Text = "Train";
            this.Train.UseVisualStyleBackColor = true;
            this.Train.Click += new System.EventHandler(this.Train_Click);
            // 
            // TrainingData
            // 
            this.TrainingData.Location = new System.Drawing.Point(413, 149);
            this.TrainingData.Name = "TrainingData";
            this.TrainingData.Size = new System.Drawing.Size(269, 22);
            this.TrainingData.TabIndex = 2;
            this.TrainingData.Text = "C:\\Users\\Jeffrey\\Documents\\Visual Studio 2015\\Projects\\MNIST_CNN\\MNIST_CNN\\data\\m" +
    "nist_train.csv";
            // 
            // TestingData
            // 
            this.TestingData.Location = new System.Drawing.Point(413, 177);
            this.TestingData.Name = "TestingData";
            this.TestingData.Size = new System.Drawing.Size(269, 22);
            this.TestingData.TabIndex = 3;
            this.TestingData.Text = "C:\\Users\\Jeffrey\\Documents\\Visual Studio 2015\\Projects\\MNIST_CNN\\MNIST_CNN\\data\\m" +
    "nist_test.csv";
            // 
            // SaveWeights
            // 
            this.SaveWeights.Location = new System.Drawing.Point(556, 262);
            this.SaveWeights.Name = "SaveWeights";
            this.SaveWeights.Size = new System.Drawing.Size(126, 28);
            this.SaveWeights.TabIndex = 4;
            this.SaveWeights.Text = "Save Weights";
            this.SaveWeights.UseVisualStyleBackColor = true;
            this.SaveWeights.Click += new System.EventHandler(this.SaveWeights_Click);
            // 
            // LoadWeights
            // 
            this.LoadWeights.Location = new System.Drawing.Point(556, 296);
            this.LoadWeights.Name = "LoadWeights";
            this.LoadWeights.Size = new System.Drawing.Size(126, 28);
            this.LoadWeights.TabIndex = 5;
            this.LoadWeights.Text = "Load Weights";
            this.LoadWeights.UseVisualStyleBackColor = true;
            this.LoadWeights.Click += new System.EventHandler(this.LoadWeights_Click);
            // 
            // LoadData
            // 
            this.LoadData.Location = new System.Drawing.Point(556, 205);
            this.LoadData.Name = "LoadData";
            this.LoadData.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.LoadData.Size = new System.Drawing.Size(126, 23);
            this.LoadData.TabIndex = 6;
            this.LoadData.Text = "Load Data";
            this.LoadData.UseVisualStyleBackColor = true;
            this.LoadData.Click += new System.EventHandler(this.LoadData_Click);
            // 
            // WeightsPath
            // 
            this.WeightsPath.Location = new System.Drawing.Point(413, 234);
            this.WeightsPath.Name = "WeightsPath";
            this.WeightsPath.Size = new System.Drawing.Size(269, 22);
            this.WeightsPath.TabIndex = 7;
            this.WeightsPath.Text = "C:\\Users\\Jeffrey\\Documents\\Visual Studio 2015\\Projects\\MNIST_CNN\\MNIST_CNN\\data\\w" +
    "eights.txt";
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(556, 115);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(126, 28);
            this.TestButton.TabIndex = 8;
            this.TestButton.Text = "Test";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(556, 81);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(126, 28);
            this.Stop.TabIndex = 9;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 419);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.WeightsPath);
            this.Controls.Add(this.LoadData);
            this.Controls.Add(this.LoadWeights);
            this.Controls.Add(this.SaveWeights);
            this.Controls.Add(this.TestingData);
            this.Controls.Add(this.TrainingData);
            this.Controls.Add(this.Train);
            this.Controls.Add(this.Initialize_Weights);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Initialize_Weights;
        private System.Windows.Forms.Button Train;
        private System.Windows.Forms.TextBox TrainingData;
        private System.Windows.Forms.TextBox TestingData;
        private System.Windows.Forms.Button SaveWeights;
        private System.Windows.Forms.Button LoadWeights;
        private System.Windows.Forms.Button LoadData;
        private System.Windows.Forms.TextBox WeightsPath;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.Button Stop;
    }
}

