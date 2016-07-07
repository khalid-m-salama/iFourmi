using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.IO;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACONeuralNets;
using iFourmi.ACONeuralNets.Algorithms;
using iFourmi.ACONeuralNets.ProblemSpecifics.ComponentInvalidators;
using iFourmi.ACONeuralNets.ProblemSpecifics.LocalSearch;
using iFourmi.ACONeuralNets.ProblemSpecifics.HeuristicCalculators;
using iFourmi.ACONeuralNets.ProblemSpecifics.QualityEvaluators;
using iFourmi.NeuralNetworks.Model;
using iFourmi.NeuralNetworks.LearningMethods;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.NeuralNetworks.ActivationFunctions;

namespace iFourmi.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Dataset dataset= ArffHelper.LoadDatasetFromArff(@"D:\Academics\Others\Datasets\Datasets  - All\iris\TR0_iris.arff");
            //DataMining.Utilities.RandomUtility.Initialize(0);
            //BatchTest.DatasetFolderPath = GetFilePath();            
            
            //BatchTest.RunBackPropagation();
            //BatchTest.RunANNMiner();
            //BatchTest.RunANNMiner_NoWis();


            //BatchTest.RunGHCNN();
            //BatchTest.RunACORNN();
            //BatchTest.RunANNMiner_ACOR();

           // ResultsParser parser = new ResultsParser(@"D:\Academics\0 - Academic Papers\2014.10 - ANN-Miner - Extended\Results\Set 2");
           // parser.ParseResults();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static string GetFilePath()
        {
            return System.IO.File.ReadAllLines("Config.txt")[0];
        }

        static void TestANNMiner(Dataset dataset)
        {

            IClassificationMeasure measure = new AccuracyMeasure();
            ILearningMethod learningMethod = new BackPropagation(0.1,10,0.9,false);

            int hiddenUnitCount = dataset.Metadata.Attributes.Length * dataset.Metadata.Target.Length;
                        
            IActivationFunction activationFunction = new SigmoidActivationFunction();

            ISolutionQualityEvaluator<ConnectionDC> evaluator = new NNClassificationQualityEvaluator(measure, learningMethod, hiddenUnitCount, activationFunction);
            IHeuristicsCalculator<ConnectionDC> calculator = new DefaultHeuristicCalculator<ConnectionDC>();
            ILocalSearch<ConnectionDC> localSearch = new DefaultRemovalLocalSearch<ConnectionDC>(evaluator);
            IComponentInvalidator<ConnectionDC> invalidator = new NNConnectorInvalidator();

            Problem<ConnectionDC> problem = new Problem<ConnectionDC>(invalidator, calculator, evaluator, localSearch);

            NeuralNetwork network_before = null;
            NeuralNetwork network_final = SingleTest.CreateNeuralNet_ANNMiner(problem,hiddenUnitCount,true,false,dataset,ref network_before);

            double quilty_before = SingleTest.TestClassifier(network_before, dataset, measure);
            double quilty_final = SingleTest.TestClassifier(network_final, dataset, measure);

            Console.WriteLine("ANN -"+quilty_before);
            Console.WriteLine("ANN -"+quilty_final);

        }


        static void TestBackProbagation(Dataset dataset)
        {
            IActivationFunction activationFunction = new SigmoidActivationFunction();
            //int hiddenUnitCount = dataset.Metadata.Attributes.Length * dataset.Metadata.Target.Length;

            int hiddenUnitCount = (dataset.Metadata.Attributes.Length + dataset.Metadata.Target.Length) / 2;

            NeuralNetwork network = SingleTest.CreateNeuralNet_BP(dataset, hiddenUnitCount, 0.9,0.01, 1000, activationFunction);

            AccuracyMeasure measure=new AccuracyMeasure();

            double quality = SingleTest.TestClassifier(network, dataset, measure);
            Console.WriteLine(measure.ToString()+":"+ Math.Round( quality*100,2));
            Console.WriteLine("Size:"+network.Size);
 
        }
    }
}
