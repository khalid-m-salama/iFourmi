using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.Model;
using iFourmi.NeuralNetworks.Model;
using iFourmi.NeuralNetworks.ActivationFunctions;
using iFourmi.NeuralNetworks.LearningMethods;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACONeuralNets;
using iFourmi.ACONeuralNets.ProblemSpecifics.ComponentInvalidators;
using iFourmi.ACONeuralNets.ProblemSpecifics.HeuristicCalculators;
using iFourmi.ACONeuralNets.ProblemSpecifics.QualityEvaluators;
using iFourmi.ACONeuralNets.ProblemSpecifics.LocalSearch;


namespace iFourmi.Main
{
    public static class BatchTest
    {
        public static string DatasetFolderPath
        {
            get;
            set;
        }

        static int _folds = 10;
        static int _currentFold = 0;

        static double _bpLearningRate = 0.01;
        static int _bpEpochs = 1000;

        static double _acoLearningRateW = 0.05;
        static int _acoEpochsW = 10;

        static double _acoLearningRateNW = 0.1;
        static int _acoEpochsNW = 20;

        static Stopwatch stopWatch = new Stopwatch();

        public static void RunBackPropagation()
        {

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                double avgQuality = 0;
                double avgSize = 0;

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality = 0.0;
                    double size = 0.0;

                    AccuracyMeasure testMeasure = new AccuracyMeasure();
                    IActivationFunction activationFunction = new SigmoidActivationFunction();

                    //int hiddenUnitCount = trainingSet.Metadata.Attributes.Length * trainingSet.Metadata.Target.Length;
                    int hiddenUnitCount = (trainingSet.Metadata.Attributes.Length + trainingSet.Metadata.Target.Length);

                    try
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        NeuralNetwork network = SingleTest.CreateNeuralNet_BP(trainingSet, hiddenUnitCount, 0.9, _bpLearningRate, _bpEpochs, activationFunction);
                        stopWatch.Stop();

                        quality = SingleTest.TestClassifier(network, testingSet, testMeasure);
                        quality = Math.Round(quality * 100, 2);
                        size = network.Size;

                        avgQuality += quality;
                        avgSize += size;

                        //----------------------------------------
                        Console.WriteLine("Backprop:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality.ToString());
                        Console.WriteLine("---------------------------------------------------");
                        //----------------------------------------
                       
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        break;

                    }

                  
                }

                avgQuality /= _folds;
                avgSize /= _folds;
                SaveResults(dataset, "BackProp", avgQuality.ToString(), avgSize.ToString(), stopWatch.ElapsedMilliseconds.ToString());


                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");


            }

        }

        public static void RunACORNN()
        {

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                double avgQuality = 0;
                double avgSize = 0;


                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality = 0.0;
                    double size = 0.0;

                    AccuracyMeasure testMeasure = new AccuracyMeasure();
                    IActivationFunction activationFunction = new SigmoidActivationFunction();

                    //int hiddenUnitCount = trainingSet.Metadata.Attributes.Length * trainingSet.Metadata.Target.Length;
                    int hiddenUnitCount = (trainingSet.Metadata.Attributes.Length + trainingSet.Metadata.Target.Length);

                    try
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        NeuralNetwork network = SingleTest.CreateNeuralNet_ACOR(trainingSet,hiddenUnitCount,0.9,activationFunction);
                        stopWatch.Stop();

                        quality = SingleTest.TestClassifier(network, testingSet, testMeasure);
                        quality = Math.Round(quality * 100, 2);
                        size = network.Size;

                        avgQuality += quality;
                        avgSize += size;

                        //----------------------------------------
                        Console.WriteLine("ACOR-NN:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality.ToString());
                        Console.WriteLine("---------------------------------------------------");
                        //----------------------------------------
                    
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        break;
                    }

                }

                avgSize /= _folds;
                SaveResults(dataset, "ACOR-NN", avgQuality.ToString(), avgSize.ToString(), stopWatch.ElapsedMilliseconds.ToString());


                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");


            }

        }
        
        public static void RunANNMiner()
        {

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------


                double avgQualityBefore = 0;
                double avgSizeBefore = 0;

                double avgQulityAfter = 0;
                double avgSizeAfter = 0;

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality_before = 0.0;
                    double quality_final = 0.0;

                    double size_before = 0.0;
                    double size_final = 0.0;

                    IClassificationMeasure testMeasure = new AccuracyMeasure();
                    ILearningMethod learningMethod = new BackPropagation(_acoLearningRateW, _acoEpochsW, 0.9,false);

                    //int hiddenUnitCount = trainingSet.Metadata.Attributes.Length * trainingSet.Metadata.Target.Length;
                    int hiddenUnitCount = (trainingSet.Metadata.Attributes.Length + trainingSet.Metadata.Target.Length);
                    
                    IActivationFunction activationFunction = new SigmoidActivationFunction();

                    IClassificationMeasure trainingMeasure = new AccuracyMeasure();
                    NNClassificationQualityEvaluator evaluator = new NNClassificationQualityEvaluator(trainingMeasure, learningMethod, hiddenUnitCount, activationFunction);
                    NNConnectionHeuristicCalculator calculator = new NNConnectionHeuristicCalculator(0.7);
                    DefaultRemovalLocalSearch<ConnectionDC> localSearch = new DefaultRemovalLocalSearch<ConnectionDC>(evaluator);
                    NNConnectorInvalidator invalidator = new NNConnectorInvalidator();

                    Problem<ConnectionDC> problem = new Problem<ConnectionDC>(invalidator, calculator, evaluator, localSearch);

                    NeuralNetwork network_before = null;

                    try
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        NeuralNetwork network_final = SingleTest.CreateNeuralNet_ANNMiner(problem, hiddenUnitCount, true, false, trainingSet, ref network_before);
                        stopWatch.Stop();

                        quality_before = SingleTest.TestClassifier(network_before, testingSet, testMeasure);
                        quality_before = Math.Round(quality_before * 100, 2);
                        avgQualityBefore += quality_before;

                        quality_final = SingleTest.TestClassifier(network_final, testingSet, testMeasure);
                        quality_final = Math.Round(quality_final * 100, 2);
                        avgQulityAfter += quality_final;

                        size_before = network_before.Size;
                        size_final = network_final.Size;

                        avgSizeBefore += size_before;
                        avgSizeAfter += avgSizeAfter;

                        //----------------------------------------
                        Console.WriteLine("ANNMiner - before:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_before.ToString());
                        Console.WriteLine("ANNMiner - final:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_final.ToString());
                        Console.WriteLine("---------------------------------------------------");
                        //----------------------------------------
                 
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        break;

                    }


                }

                avgQualityBefore /= _folds;
                avgQulityAfter /= _folds;

                avgSizeBefore/=_folds;
                avgSizeAfter /= _folds;

                SaveResults(dataset, "wANNMiner - before", avgQualityBefore.ToString(), avgSizeBefore.ToString(), stopWatch.ElapsedMilliseconds.ToString());
                SaveResults(dataset, "wANNMiner - final", avgQulityAfter.ToString(), avgSizeAfter.ToString(), stopWatch.ElapsedMilliseconds.ToString());

                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");


            }

        }

        public static void RunANNMiner_NoWis()
        {

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                double avgQualityBefore = 0;
                double avgSizeBefore = 0;

                double avgQulityAfter = 0;
                double avgSizeAfter = 0;

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality_before = 0.0;
                    double quality_final = 0.0;

                    double size_before = 0.0;
                    double size_final = 0.0;

                    IClassificationMeasure testMeasure = new AccuracyMeasure();
                    ILearningMethod learningMethod = new BackPropagation(_acoLearningRateNW, _acoEpochsNW, 0.9, false);

                    //int hiddenUnitCount = trainingSet.Metadata.Attributes.Length * trainingSet.Metadata.Target.Length;
                    int hiddenUnitCount = (trainingSet.Metadata.Attributes.Length + trainingSet.Metadata.Target.Length);

                    IActivationFunction activationFunction = new SigmoidActivationFunction();

                    IClassificationMeasure trainingMeasure = new QLFunction();
                    NNClassificationQualityEvaluator evaluator = new NNClassificationQualityEvaluator(testMeasure, learningMethod, hiddenUnitCount, activationFunction);
                    NNConnectionHeuristicCalculator calculator = new NNConnectionHeuristicCalculator(0.7);
                    DefaultRemovalLocalSearch<ConnectionDC> localSearch = new DefaultRemovalLocalSearch<ConnectionDC>(evaluator);
                    NNConnectorInvalidator invalidator = new NNConnectorInvalidator();

                    Problem<ConnectionDC> problem = new Problem<ConnectionDC>(invalidator, calculator, evaluator, localSearch);

                    NeuralNetwork network_before = null;

                    try
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        NeuralNetwork network_final = SingleTest.CreateNeuralNet_ANNMiner(problem, hiddenUnitCount, false, false, trainingSet, ref network_before);
                        stopWatch.Stop();

                        quality_before = SingleTest.TestClassifier(network_before, testingSet, testMeasure);
                        quality_before = Math.Round(quality_before * 100, 2);
                        avgQualityBefore += quality_before;

                        quality_final = SingleTest.TestClassifier(network_final, testingSet, testMeasure);
                        quality_final = Math.Round(quality_final * 100, 2);
                        avgQulityAfter += quality_final;

                        size_before = network_before.Size;
                        size_final = network_final.Size;

                        avgSizeBefore += size_before;
                        avgSizeAfter += avgSizeAfter;

                        //----------------------------------------
                        Console.WriteLine("ANNMiner - before:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_before.ToString());
                        Console.WriteLine("ANNMiner - final:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_final.ToString());
                        Console.WriteLine("---------------------------------------------------");
                        //----------------------------------------
             
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        break;

                    }


                }

                avgQualityBefore /= _folds;
                avgQulityAfter /= _folds;

                avgSizeBefore /= _folds;
                avgSizeAfter /= _folds;

                SaveResults(dataset, "ANNMiner - before", avgQualityBefore.ToString(), avgSizeBefore.ToString(), stopWatch.ElapsedMilliseconds.ToString());
                SaveResults(dataset, "ANNMiner - final", avgQulityAfter.ToString(), avgSizeAfter.ToString(), stopWatch.ElapsedMilliseconds.ToString());


                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");


            }

        }

        public static void RunANNMiner_ACOR()
        {

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                double avgQualityBefore = 0;
                double avgSizeBefore = 0;

                double avgQulityAfter = 0;
                double avgSizeAfter = 0;

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality_before = 0.0;
                    double quality_final = 0.0;

                    double size_before = 0.0;
                    double size_final = 0.0;

                    IClassificationMeasure testMeasure = new AccuracyMeasure();
                    ILearningMethod learningMethod = new BackPropagation(_acoLearningRateW, _acoEpochsW, 0.9, false);

                    //int hiddenUnitCount = trainingSet.Metadata.Attributes.Length * trainingSet.Metadata.Target.Length;
                    int hiddenUnitCount = (trainingSet.Metadata.Attributes.Length + trainingSet.Metadata.Target.Length);

                    IActivationFunction activationFunction = new SigmoidActivationFunction();

                    IClassificationMeasure trainingMeasure = new QLFunction();
                    NNClassificationQualityEvaluator evaluator = new NNClassificationQualityEvaluator(trainingMeasure, learningMethod, hiddenUnitCount, activationFunction);
                    NNConnectionHeuristicCalculator calculator = new NNConnectionHeuristicCalculator(0.7);
                    DefaultRemovalLocalSearch<ConnectionDC> localSearch = new DefaultRemovalLocalSearch<ConnectionDC>(evaluator);
                    NNConnectorInvalidator invalidator = new NNConnectorInvalidator();

                    Problem<ConnectionDC> problem = new Problem<ConnectionDC>(invalidator, calculator, evaluator, localSearch);

                    NeuralNetwork network_before = null;

                    try
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        NeuralNetwork network_final = SingleTest.CreateNeuralNet_ANNMiner_ACOR(problem, hiddenUnitCount, true, false, trainingSet, ref network_before);
                        stopWatch.Stop();

                        quality_before = SingleTest.TestClassifier(network_before, testingSet, testMeasure);
                        quality_before = Math.Round(quality_before * 100, 2);
                        avgQualityBefore += quality_before;

                        quality_final = SingleTest.TestClassifier(network_final, testingSet, testMeasure);
                        quality_final = Math.Round(quality_final * 100, 2);
                        avgQulityAfter += quality_final;

                        size_before = network_before.Size;
                        size_final = network_final.Size;

                        avgSizeBefore += size_before;
                        avgSizeAfter += avgSizeAfter;

                        //----------------------------------------
                        Console.WriteLine("ANNMiner - before:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_before.ToString());
                        Console.WriteLine("ANNMiner - final:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_final.ToString());
                        Console.WriteLine("---------------------------------------------------");
                        //----------------------------------------
                  
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        break;

                    }


                }

                avgQualityBefore /= _folds;
                avgQulityAfter /= _folds;

                avgSizeBefore /= _folds;
                avgSizeAfter /= _folds;

                SaveResults(dataset, "ANNMiner_ACOR - before", avgQualityBefore.ToString(), avgSizeBefore.ToString(), stopWatch.ElapsedMilliseconds.ToString());
                SaveResults(dataset, "ANNMiner_ACOR - final", avgQulityAfter.ToString(), avgSizeAfter.ToString(), stopWatch.ElapsedMilliseconds.ToString());



                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");


            }

        }

        public static void RunGHCNN()
        {

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                double avgQualityBefore = 0;
                double avgSizeBefore = 0;

                double avgQulityAfter = 0;
                double avgSizeAfter = 0;


                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality_before = 0.0;
                    double quality_final = 0.0;

                    double size_before = 0.0;
                    double size_final = 0.0;

                    IClassificationMeasure testMeasure = new AccuracyMeasure();
                    ILearningMethod learningMethod = new BackPropagation(_acoLearningRateNW, _acoEpochsNW, 0.9, false);

                    //int hiddenUnitCount = trainingSet.Metadata.Attributes.Length * trainingSet.Metadata.Target.Length;
                    int hiddenUnitCount = (trainingSet.Metadata.Attributes.Length + trainingSet.Metadata.Target.Length);

                    IActivationFunction activationFunction = new SigmoidActivationFunction();

                    IClassificationMeasure trainingMeasure = new QLFunction();
                    NNClassificationQualityEvaluator evaluator = new NNClassificationQualityEvaluator(trainingMeasure, learningMethod, hiddenUnitCount, activationFunction);
                    NNConnectionHeuristicCalculator calculator = new NNConnectionHeuristicCalculator(0.7);
                    DefaultRemovalLocalSearch<ConnectionDC> localSearch = new DefaultRemovalLocalSearch<ConnectionDC>(evaluator);
                    NNConnectorInvalidator invalidator = new NNConnectorInvalidator();

                    Problem<ConnectionDC> problem = new Problem<ConnectionDC>(invalidator, calculator, evaluator, localSearch);

                    NeuralNetwork network_before = null;

                    try
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        NeuralNetwork network_final = SingleTest.CreateNeuralNet_GHCNN(problem, hiddenUnitCount, true, false, trainingSet, ref network_before);
                        stopWatch.Stop();

                        quality_before = SingleTest.TestClassifier(network_before, testingSet, testMeasure);
                        quality_before = Math.Round(quality_before * 100, 2);
                        avgQualityBefore += quality_before;

                        quality_final = SingleTest.TestClassifier(network_final, testingSet, testMeasure);
                        quality_final = Math.Round(quality_final * 100, 2);
                        avgQulityAfter += quality_final;

                        size_before = network_before.Size;
                        size_final = network_final.Size;

                        avgSizeBefore += size_before;
                        avgSizeAfter += avgSizeAfter;

                        //----------------------------------------
                        Console.WriteLine("GHCNN - before:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_before.ToString());
                        Console.WriteLine("GHCNN - final:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + testMeasure.ToString() + ":" + quality_final.ToString());
                        Console.WriteLine("---------------------------------------------------");
                        //----------------------------------------
        
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        break;

                    }


                }

                avgQualityBefore /= _folds;
                avgQulityAfter /= _folds;

                avgSizeBefore /= _folds;
                avgSizeAfter /= _folds;

                SaveResults(dataset, "GHCNN - before", avgQualityBefore.ToString(), avgSizeBefore.ToString(), stopWatch.ElapsedMilliseconds.ToString());
                SaveResults(dataset, "GHCNN - final", avgQulityAfter.ToString(), avgSizeAfter.ToString(), stopWatch.ElapsedMilliseconds.ToString());



                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");


            }

        }
        
        public static void RunANNMiner_QEM()
        {
           
            AccuracyMeasure testMeasure = new AccuracyMeasure();

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                foreach (IClassificationMeasure measure in GetMeasures())
                {

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine(dataset + " - Fold:" + _currentFold.ToString() + " - " + measure.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        double quality_before = 0.0;
                        double quality_final = 0.0;

                        double size_before = 0.0;
                        double size_final = 0.0;

                        ILearningMethod learningMethod = new BackPropagation(0.05, 10, 0.9, false);

                        int hiddenUnitCount = (trainingSet.Metadata.Attributes.Length + trainingSet.Metadata.Target.Length);

                        IActivationFunction activationFunction = new SigmoidActivationFunction();

                        NNClassificationQualityEvaluator evaluator = new NNClassificationQualityEvaluator(measure, learningMethod, hiddenUnitCount, activationFunction);
                        NNConnectionHeuristicCalculator calculator = new NNConnectionHeuristicCalculator(0.7);
                        DefaultRemovalLocalSearch<ConnectionDC> localSearch = new DefaultRemovalLocalSearch<ConnectionDC>(evaluator);
                        NNConnectorInvalidator invalidator = new NNConnectorInvalidator();

                        Problem<ConnectionDC> problem = new Problem<ConnectionDC>(invalidator, calculator, evaluator, localSearch);

                        NeuralNetwork network_before = null;

                        try
                        {
                            NeuralNetwork network_final = SingleTest.CreateNeuralNet_ANNMiner(problem, hiddenUnitCount, true, false, trainingSet, ref network_before);

                            quality_before = SingleTest.TestClassifier(network_before, testingSet, testMeasure);
                            quality_before = Math.Round(quality_before * 100, 2);

                            quality_final = SingleTest.TestClassifier(network_final, testingSet, testMeasure);
                            quality_final = Math.Round(quality_final * 100, 2);

                            size_before = network_before.Size;
                            size_final = network_final.Size;

                            //----------------------------------------
                            Console.WriteLine("ANNMiner - before:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + measure.ToString() + ":" + quality_before.ToString());
                            Console.WriteLine("ANNMiner - final:" + dataset + "- Fold:" + _currentFold.ToString() + "=>" + measure.ToString() + ":" + quality_final.ToString());
                            Console.WriteLine("---------------------------------------------------");
                            //----------------------------------------
                            SaveResults(dataset, "ANNMiner - before", measure.ToString(), quality_before.ToString(), size_before.ToString());
                            SaveResults(dataset, "ANNMiner - final", measure.ToString(), quality_final.ToString(), size_final.ToString());
                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                            break;

                        }
                    }


                }

                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");


            }

        }

        public static DataMining.Data.Dataset[] LoadTrainingAndTestingData(string dataSetName, int fold)
        {

            string trFile = DatasetFolderPath + @"\" + dataSetName + @"\TR" + fold.ToString() + "_" + dataSetName + ".arff";
            string tsFile = DatasetFolderPath + @"\" + dataSetName + @"\TS" + fold.ToString() + "_" + dataSetName + ".arff";

            DataMining.Data.Dataset trainingSetDataTable = DataMining.IO.ArffHelper.LoadDatasetFromArff(trFile);
            DataMining.Data.Dataset testingSetDataTable = DataMining.IO.ArffHelper.LoadDatasetFromArff(tsFile);


            DataMining.Data.Dataset[] result = new DataMining.Data.Dataset[2];
            result[0] = trainingSetDataTable;
            result[1] = testingSetDataTable;
            return result;
        }

        public static List<string> GetDatasetFolds()
        {
            List<string> result = new List<string>();
            foreach (string directory in System.IO.Directory.GetDirectories(DatasetFolderPath))
            {
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(directory);
                if (info.Name != "Results")
                    result.Add(info.Name);

            }

            return result;
        }

        public static List<string> GetDatasetFolds(string file)
        {
            List<string> result = new List<string>();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                        result.Add(line);
                }
            }

            return result;
        }

        private static void SaveResults(params string[] args)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < args.Length; i++)
                builder.Append(args[i] + ",");
            System.IO.StreamWriter writer = new System.IO.StreamWriter("results\\" + args[0] + "_results.txt", true);
            writer.WriteLine(builder.ToString());
            writer.Flush();
            writer.Close();
        }

        private static void LogError(Exception ex)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter("results\\error.txt", true);
            writer.WriteLine(ex.Message);
            writer.WriteLine(ex.StackTrace);
            writer.Flush();
            writer.Close();
        }

        public static void ValidateDatasets()
        {

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                bool ok = true;
                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    try
                    {
                        //if (dataset == "horse")
                        {
                            DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                            DataMining.Data.Dataset trainingSet = tables[0];
                            DataMining.Data.Dataset testingSet = tables[1];
                        }

                    }
                    catch
                    {
                        ok = false;
                        break;
                    }


                    //Console.WriteLine(dataset + "- Fold:"+_currentFold.ToString());
                }

                //----------------------------------------
                Console.WriteLine(dataset + "- " + ok);
                Console.WriteLine("---------------------------------------------------");
                //----------------------------------------


            }
        }

        public static List<IClassificationMeasure> GetMeasures()
        {
            return new List<IClassificationMeasure>
            {
                new MSError(),
                new MAError(),
                new MedAError(),
                new QLFunction(),
                new CrossEntropy(),
                new BIReward()
            };
        }

    }
}
