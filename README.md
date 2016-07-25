# Convolutional Neural Network in C&#x23;

Convolutional neural network trained on the MNIST data set. The structure of the convolution is as follows:
<code>
1x28x28 Input -> 8x5x5 Convolution -> 2x2 Max Pooling -> 8x5x5 Convolution -> 2x2 Max Pooling -> 15 Hidden -> 10 Output
</code>
In the data folder, there are three weights files of various number of iterations.

# More Details

| File Name                             | "weights1.txt" | "weights2.txt" | "weights3.txt" |
|---------------------------------------|----------------|----------------|----------------|
| Number of Gradient Descent Iterations | 749529         | 604315         | 2644380        |
| Test Case Accuracy                    | 96.31%         | 96.36%         | 96.39%         |
