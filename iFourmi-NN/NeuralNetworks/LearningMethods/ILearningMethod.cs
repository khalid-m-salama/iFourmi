using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.NeuralNetworks.Model;
using iFourmi.DataMining.Data;

namespace iFourmi.NeuralNetworks.LearningMethods
{
    public interface ILearningMethod
    {
        void TrainNetwork(NeuralNetwork network, Dataset trainingSet);
    }
}
