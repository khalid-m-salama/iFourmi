using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.IO;
using iFourmi.NeuralNetworks.Model;
using iFourmi.NeuralNetworks.ActivationFunctions;
using iFourmi.NeuralNetworks.LearningMethods;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACONeuralNets;
using iFourmi.ACONeuralNets.Algorithms;
using iFourmi.ACONeuralNets.ProblemSpecifics.ComponentInvalidators;
using iFourmi.ACONeuralNets.ProblemSpecifics.HeuristicCalculators;
using iFourmi.ACONeuralNets.ProblemSpecifics.LocalSearch;
using iFourmi.ACONeuralNets.ProblemSpecifics.QualityEvaluators;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.Model;



namespace iFourmi.Main
{
    public static class SingleTest
    {
        static int counter = 0;

        static int maxIterations = 100;
        static int colonySize = 10;
        static int convergence = 10;
        static int archive = 25;
        static double q = 0.25;
        static double segma = 0.85;


        public static NeuralNetwork CreateNeuralNet_ANNMiner(Problem<ConnectionDC> problem, int hiddenUnitCount, bool wisdom, bool performLocalSearch, Dataset trainingset, ref NeuralNetwork network_before)
        {
            ANNMiner miner = new ANNMiner(maxIterations, colonySize, convergence, problem, hiddenUnitCount, wisdom, performLocalSearch, trainingset);
            miner.OnPostAntSolutionContruction+=new EventHandler(OnPostAntSolutionContruction);
            miner.OnPostColonyIteration+=new EventHandler(OnPostColonyIteration);

            NeuralNetwork network = miner.CreateClassifier() as NeuralNetwork;
            network_before = miner.NetworkBeforePostProcessing;
            return network;
 
        }

        public static NeuralNetwork CreateNeuralNet_ANNMiner_ACOR(Problem<ConnectionDC> problem, int hiddenUnitCount, bool wisdom, bool performLocalSearch, Dataset trainingset, ref NeuralNetwork network_before)
        {
            ANNMiner miner = new ANNMiner(maxIterations, colonySize, convergence, problem, hiddenUnitCount, wisdom, performLocalSearch, trainingset);
            miner.OnPostAntSolutionContruction += new EventHandler(OnPostAntSolutionContruction);
            miner.OnPostColonyIteration += new EventHandler(OnPostColonyIteration);

            NeuralNetwork network = miner.CreateClassifier2() as NeuralNetwork;
            network_before = miner.NetworkBeforePostProcessing;
            return network;

        }

        public static NeuralNetwork CreateNeuralNet_GHCNN(Problem<ConnectionDC> problem, int hiddenUnitCount, bool wisdom, bool performLocalSearch, Dataset trainingset, ref NeuralNetwork network_before)
        {
            GHCNN miner = new GHCNN(maxIterations, colonySize, convergence, problem, hiddenUnitCount, wisdom, performLocalSearch, trainingset);
            miner.OnPostEvaluation += new EventHandler(OnPostColonyIteration);

            NeuralNetwork network = miner.CreateClassifier() as NeuralNetwork;
            network_before = miner.NetworkBeforePostProcessing;
            return network;

        }

        public static NeuralNetwork CreateNeuralNet_BP(Dataset trainingset,int hiddenUnitCount, double positiveClassValue,double learningRate, int epochCount, IActivationFunction activationFunction)
        {
            counter = 0;

            //MSError measure = new MSError();
            //AccuracyMeasure measure = new AccuracyMeasure();

            Connection[] connections = NeuralNetwork.Create3LayerConnectedConnections(trainingset.Metadata, hiddenUnitCount);
            NeuralNetwork network = new NeuralNetwork(trainingset.Metadata, hiddenUnitCount, activationFunction,connections);            
            BackPropagation BP = new BackPropagation(learningRate, epochCount, positiveClassValue, true);

            BP.OnPostEpoch+=new EventHandler(OnPostEpoch);

            BP.TrainNetwork(network, trainingset);            
            return network;
            
        }

        public static NeuralNetwork CreateNeuralNet_ACOR(Dataset trainingset, int hiddenUnitCount, double positiveClassValue, IActivationFunction activationFunction)
        {
            
            Connection[] connections = NeuralNetwork.Create3LayerConnectedConnections(trainingset.Metadata, hiddenUnitCount);
            NeuralNetwork network = new NeuralNetwork(trainingset.Metadata, hiddenUnitCount, activationFunction, connections);

            int problemSize = network.Size;
            QLFunction measure = new QLFunction();

            NNClassificationQualityEvaluator2 evaluator = new NNClassificationQualityEvaluator2(-5,5,measure);
            evaluator.LearningSet = trainingset;
            evaluator.ValidationSet = trainingset;
            evaluator.NeuralNetwork = network;

            Problem<double> problem = new Problem<double>(null, null, evaluator, null);


            ACO_RNN acornn = new ACO_RNN(maxIterations, colonySize, convergence, problem, problemSize, archive, q, segma);
            acornn.OnPostColonyIteration += OnPostColonyIteration;

            acornn.TrainNetwork(network, trainingset);
            return network;

        }

        public static double TestClassifier(IClassifier classifier, Dataset testingSet, IClassificationMeasure measure)
        {
           return measure.CalculateMeasure(classifier, testingSet);
           
        }

        static void OnPostColonyIteration(object sender, EventArgs e)
        {

            if (sender is AntColony<ConnectionDC>)
            {
                AntColony<ConnectionDC> colony = sender as AntColony<ConnectionDC>;
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration [" + colony.CurrentIteration.ToString() + "]");
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration Best: " + Math.Round(colony.IterationBestAnt.Solution.Quality, 2).ToString());
                Console.WriteLine("Global Best: " + Math.Round(colony.BestAnt.Solution.Quality, 2).ToString());
                Console.WriteLine("------------------------");
            }
            if (sender is AntColony<double>)
            {
                ContinuousACO.Algorithms.ACO_R acor = sender as ContinuousACO.Algorithms.ACO_R;
                Console.WriteLine("Iteration [" + acor.CurrentIteration.ToString() + "] "+Math.Round(acor.GlobalBest.Quality, 2).ToString());
              
            }

        }

        static void OnPostAntSolutionContruction(object sender, EventArgs e)
        {
            if (sender is Ant<ConnectionDC>)
            {
                Ant<ConnectionDC> ant = sender as Ant<ConnectionDC>;
                Console.WriteLine("[" + ant.Index.ToString() + "]: " + Math.Round(ant.Solution.Quality, 2).ToString());
            }
            if (sender is Ant<double>)
            {
                Ant<double> ant = sender as Ant<double>;
                Console.WriteLine("[" + ant.Index.ToString() + "]: " + Math.Round(ant.Solution.Quality, 2).ToString());
            }
        }

        static void OnPostEpoch(object sender, EventArgs e)
        {

            double quality=0;

            if(e!=null)
                quality = ((BPEventArgs)e).Quality;
            

            Console.WriteLine(counter+" - Quality:"+Math.Round(quality,5));
            counter++;
        }


     
    }
}
