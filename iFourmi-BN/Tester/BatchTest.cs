using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.BayesianNetworks.Model;
using iFourmi.DataMining.Data;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianLearning;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;
using iFourmi.BayesianLearning.Algorithms.ACO;
using iFourmi.BayesianLearning.Algorithms.GHC;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.EnsembleStrategy;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.Algorithms.Hierarchical;
using iFourmi.DataMining.Model.Hierarchical;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.ProximityMeasures;
using System.Diagnostics;

namespace iFourmi.Tester
{
    public static class BatchTest
    {
        public static string DatasetFolderPath
        {
            get;
            set;
        }

        static int _folds = 10;
        static int _runs = 1;
        static int _currentFold = 0;
        static Stopwatch stopWatch = new Stopwatch();

        public static void RunABCMiner()
        {
            foreach (string dataset in GetDatasetFolds())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {

                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality = 0.0;
                    double edges = 0.0;

                    DataMining.ClassificationMeasures.IClassificationQualityMeasure measure = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
                    ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> calculator = new CMICalculator();

                    for (int i = 0; i < _runs; i++)
                    {
                        int seed = (int)DateTime.Now.Ticks;
                        BayesianNetworks.Model.BayesianNetworkClassifier classifier = SingleTest.CreateABCMinerClassifier(seed, 1000, 5, 10, 3, trainingSet, measure, calculator, true, true);
                        DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                        double currentQuality = SingleTest.TestClassifier(classifier, testingSet, testMeasure);
                        //----------------------------------------
                        Console.WriteLine("==>:run" + i.ToString() + " Acc=" + Math.Round(currentQuality, 2).ToString());
                        //----------------------------------------
                        quality += currentQuality;
                        edges += classifier.Edges.Length;
                    }

                    quality /= _runs;
                    edges /= _runs;

                    quality = Math.Round(quality * 100, 2);

                    Result result = new Result() { Algorithm = "ABC", Dataset = trainingSet.Metadata.DatasetName, Fold = _currentFold, Quality = quality, Edges = edges };

                    Console.WriteLine(result.ToString() + "Fold:" + _folds.ToString());
                    Console.WriteLine("---------------------------------------------------");

                    SaveResults(result);

                }

                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");

            }

        }

        public static void RunABCMiner_0()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                BayesianNetwork.cache = new Dictionary<string, string>();

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                try
                {

                    stopWatch.Reset();
                    stopWatch.Start();


                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    //DataMining.ClassificationMeasures.IClassificationQualityMeasure trainingMeasure = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure trainingMeasure = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();

                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure1 = new DataMining.ClassificationMeasures.ReducedErrorMeasure();                    
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure2 = new DataMining.ClassificationMeasures.ProbabilityReducedErrorMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure3 = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();

                    ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> heuristics = new CMICalculator();

                    int seed = (int)DateTime.Now.Ticks;

                    BayesianNetworks.Model.BayesianNetworkClassifier classifier = SingleTest.CreateABCMinerClassifier(seed, 1000, 5, 10, 3, trainingSet, trainingMeasure, heuristics, true, true);

                    //ExportGraph(dataset, classifier);
                    

                    double quality1 = SingleTest.TestClassifier(classifier, testingSet, testingMeasure1);
                    double quality2 = SingleTest.TestClassifier(classifier, testingSet, testingMeasure2);
                    double quality3 = SingleTest.TestClassifier(classifier, testingSet, testingMeasure3);

                    

                    Result result1 = new Result() { Algorithm = "ABCMiner - Acc", Dataset = trainingSet.Metadata.DatasetName, Quality = quality1, Edges = classifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };
                    Result result2 = new Result() { Algorithm = "ABCMiner Prob_Acc", Dataset = trainingSet.Metadata.DatasetName, Quality = quality2, Edges = classifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };
                    Result result3 = new Result() { Algorithm = "ABCMiner Prob_Acc2", Dataset = trainingSet.Metadata.DatasetName, Quality = quality3, Edges = classifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };

                    Console.WriteLine(result1.ToString());
                    //Console.WriteLine(result2.ToString());

                    SaveResults(result1);
                    SaveResults(result2);
                    SaveResults(result3);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogError(ex);
                }
                finally
                {
                    stopWatch.Stop();
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
 
                }



              

            }

        }

        public static void RunABCMinerPlus_0()
        {
            foreach (string dataset in GetDatasetFiles())
            {
                BayesianNetwork.cache= new Dictionary<string, string>();

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                try
                {
                    stopWatch.Reset();
                    stopWatch.Start();


                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];


                    //DataMining.ClassificationMeasures.IClassificationQualityMeasure trainingMeasure = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure trainingMeasure = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();

                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure1 = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure2 = new DataMining.ClassificationMeasures.ProbabilityReducedErrorMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure3 = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();

                    ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> heuristics = new CMICalculator();

                    int seed = (int)DateTime.Now.Ticks;

                    BayesianNetworkClassifier abcBNClassifier = null;


                    BayesianNetworks.Model.BayesianNetworkClassifier abcminerPlus = SingleTest.CreateABCMinerPlusClassifier(seed, 500, 5, 10, 3, trainingSet, trainingMeasure, false, true, out abcBNClassifier);


                    double abcQuality1 = SingleTest.TestClassifier(abcBNClassifier, testingSet, testingMeasure1);
                    double abcminerPlusQuality1 = SingleTest.TestClassifier(abcminerPlus, testingSet, testingMeasure1);


                    double abcQuality2 = SingleTest.TestClassifier(abcBNClassifier, testingSet, testingMeasure2);
                    double abcminerPlusQuality2 = SingleTest.TestClassifier(abcminerPlus, testingSet, testingMeasure2);

                    double abcQuality3 = SingleTest.TestClassifier(abcBNClassifier, testingSet, testingMeasure3);
                    double abcminerPlusQuality3 = SingleTest.TestClassifier(abcminerPlus, testingSet, testingMeasure3);



                    Result result1 = new Result() { Algorithm = "ABC - Acc", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = abcQuality1, Edges = abcBNClassifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };
                    Result result2 = new Result() { Algorithm = "ABC - Prob_Acc", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = abcQuality2, Edges = abcBNClassifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };
                    Result result3 = new Result() { Algorithm = "ABC - Prob_Acc2", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = abcQuality3, Edges = abcBNClassifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };

                    Result result4 = new Result() { Algorithm = "ABCMiner+ - Acc", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = abcminerPlusQuality1, Edges = abcminerPlus.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };                                      
                    Result result5 = new Result() { Algorithm = "ABCMiner+ - Prob_Acc", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = abcminerPlusQuality2, Edges = abcminerPlus.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };
                    Result result6 = new Result() { Algorithm = "ABCMiner+ - Prob_Acc2", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = abcminerPlusQuality3, Edges = abcminerPlus.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };

                    Console.WriteLine(result1.ToString());
                    //Console.WriteLine(result2.ToString());

                    SaveResults(result1);
                    SaveResults(result2);
                    SaveResults(result3);
                    SaveResults(result4);
                    SaveResults(result5);
                    SaveResults(result6);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogError(ex);
                }
                finally
                {

                    stopWatch.Stop();
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
                }

            }

        }

        public static void RunABCMinerPlusI_0()
        {
            foreach (string dataset in GetDatasetFiles())
            {
                BayesianNetwork.cache = new Dictionary<string, string>();

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                try
                {
                    stopWatch.Reset();
                    stopWatch.Start();


                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];


                    //DataMining.ClassificationMeasures.IClassificationQualityMeasure trainingMeasure = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure trainingMeasure = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();

                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure1 = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure2 = new DataMining.ClassificationMeasures.ProbabilityReducedErrorMeasure();
                    DataMining.ClassificationMeasures.IClassificationQualityMeasure testingMeasure3 = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();

                    ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> heuristics = new CMICalculator();

                    int seed = (int)DateTime.Now.Ticks;


                    BayesianNetworks.Model.BayesianNetworkClassifier classifier = SingleTest.CreateABCMinerPlusIClassifier(seed, 1000, 1, 5, 10, 3, trainingSet, trainingMeasure, false, true);


                    double quality1 = SingleTest.TestClassifier(classifier, testingSet, testingMeasure1);
                    double quality2 = SingleTest.TestClassifier(classifier, testingSet, testingMeasure2);
                    double quality3 = SingleTest.TestClassifier(classifier, testingSet, testingMeasure3);



                    Result result1 = new Result() { Algorithm = "ABCMinerPlusI - Acc", Dataset = trainingSet.Metadata.DatasetName, Quality = quality1, Edges = classifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };
                    Result result2 = new Result() { Algorithm = "ABCMinerPlusI Prob_Acc", Dataset = trainingSet.Metadata.DatasetName, Quality = quality2, Edges = classifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };
                    Result result3 = new Result() { Algorithm = "ABCMinerPlusI Prob_Acc2", Dataset = trainingSet.Metadata.DatasetName, Quality = quality3, Edges = classifier.Edges.Length, Seconds = stopWatch.Elapsed.Seconds };

                    Console.WriteLine(result1.ToString());
                    //Console.WriteLine(result2.ToString());

                    SaveResults(result1);
                    SaveResults(result2);
                    SaveResults(result3);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogError(ex);
                }
                finally
                {

                    stopWatch.Stop();
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
                }

            }

        }

        public static void RunLMNABC()
        {
            foreach (string dataset in GetDatasetFolds())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {

                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality = 0.0;
                    double edges = 0.0;


                    DataMining.ClassificationMeasures.AccuracyMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();
                    likelihoodQualityEvaluator trainingQualityEvaluator = new likelihoodQualityEvaluator(trainingSet);
                    ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> calculator = new MICalculator();

                    for (int i = 0; i < _runs; i++)
                    {
                        int seed = (int)DateTime.Now.Ticks;
                        BayesianNetworks.Model.BayesianMultinetClassifier classifier = SingleTest.CreateLMNAntBayesianClassification(seed, 100, 5, 10, 3, trainingSet, trainingQualityEvaluator, calculator, true);
                        double currentQuality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);
                        //----------------------------------------
                        Console.WriteLine("==>:run" + i.ToString() + " Acc=" + Math.Round(currentQuality, 2).ToString());
                        //----------------------------------------
                        quality += currentQuality;
                        //edges += classifier.
                    }

                    quality /= _runs;
                    edges /= _runs;

                    quality = Math.Round(quality * 100, 2);

                    Result result = new Result() { Algorithm = "LMNABC", Dataset = trainingSet.Metadata.DatasetName, Fold = _currentFold, Quality = quality, Edges = edges };

                    Console.WriteLine(result.ToString() + "Fold:" + _folds.ToString());
                    Console.WriteLine("---------------------------------------------------");

                    SaveResults(result);

                }

                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");

            }

        }

        public static void RunLMNABC_0()
        {
            foreach (string dataset in GetDatasetFiles())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------


                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

             
                DataMining.ClassificationMeasures.ReducedErrorMeasure qualityEvaluator = new DataMining.ClassificationMeasures.ReducedErrorMeasure();


                likelihoodQualityEvaluator trainingQualityEvaluator = new likelihoodQualityEvaluator(trainingSet);
                ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> calculator = new MICalculator();
       
                int seed = (int)DateTime.Now.Ticks;
                BayesianNetworks.Model.BayesianMultinetClassifier classifier = SingleTest.CreateLMNAntBayesianClassification(seed, 100, 5, 10, 3, trainingSet, trainingQualityEvaluator, calculator, true);
                double quality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);

                double edges = classifier.EdgesCount.Average();

                Result result = new Result() { Algorithm = "LMNABC", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = quality, Edges = edges };

                Console.WriteLine(result.ToString());
                Console.WriteLine("---------------------------------------------------");

                SaveResults(result);

            }

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("---------------------------------------------------");



        }

        public static void RunGMNABC()
        {
            foreach (string dataset in GetDatasetFolds())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {

                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality = 0.0;
                    double edges = 0.0;


                    DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();

                    ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> calculator = new MICalculator();

                    for (int i = 0; i < _runs; i++)
                    {
                        int seed = (int)DateTime.Now.Ticks;
                        BayesianNetworks.Model.BayesianMultinetClassifier classifier = SingleTest.CreateGMNAntBayesianClassification(seed, 100, 5, 10, 3, trainingSet, null, calculator, true);
                        double currentQuality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);
                        //----------------------------------------
                        Console.WriteLine("==>:run" + i.ToString() + " Acc=" + Math.Round(currentQuality, 2).ToString());
                        //----------------------------------------
                        quality += currentQuality;
                        //edges += classifier.
                    }

                    quality /= _runs;
                    //edges /= _runs;

                    quality = Math.Round(quality * 100, 2);

                    Result result = new Result() { Algorithm = "GMNABC", Dataset = trainingSet.Metadata.DatasetName, Fold = _currentFold, Quality = quality, Edges = edges };

                    Console.WriteLine(result.ToString() + "Fold:" + _folds.ToString());
                    Console.WriteLine("---------------------------------------------------");

                    SaveResults(result);

                }

                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");

            }

        }

        public static void RunGMNABC_0()
        {
            foreach (string dataset in GetDatasetFiles())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------


                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

                DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityEvaluator = new DataMining.ClassificationMeasures.ReducedErrorMeasure();


                ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> calculator = new MICalculator();

                int seed = (int)DateTime.Now.Ticks;
                BayesianNetworks.Model.BayesianMultinetClassifier classifier = SingleTest.CreateGMNAntBayesianClassification(seed, 100, 5, 10, 3, trainingSet, null, calculator, true);
                double quality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);
                double edges = classifier.EdgesCount.Average();

                Result result = new Result() { Algorithm = "GMNABC", Dataset = trainingSet.Metadata.DatasetName, Fold = 0, Quality = quality, Edges = edges };

                Console.WriteLine(result.ToString());
                Console.WriteLine("---------------------------------------------------");

                SaveResults(result);

            }

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("---------------------------------------------------");



        }

        public static void RunHBC()
        {
            foreach (string dataset in GetDatasetFolds())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {

                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];


                    DataMining.ClassificationMeasures.AccuracyMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();

                    BayesianNetworks.Model.BayesianNetworkClassifier classifier = SingleTest.CreateGreedyBayesianClassifier(3, 250, trainingSet, qualityEvaluator, true);
                    double quality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);
                    
                    Result result = new Result() { Algorithm = "HBC", Dataset = trainingSet.Metadata.DatasetName, Fold = _currentFold, Quality = quality, Edges = classifier.Edges.Length };

                    //----------------------------------------                    
                    Console.WriteLine(result.ToString() + "Fold:" + _folds.ToString());
                    Console.WriteLine("---------------------------------------------------");



                    SaveResults(result);



                }



                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");

            }

        }

        public static void RunHBC_0()
        {
            foreach (string dataset in GetDatasetFiles())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

                DataMining.ClassificationMeasures.AccuracyMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();

                BayesianNetworks.Model.BayesianNetworkClassifier classifier = SingleTest.CreateGreedyBayesianClassifier(1, 10000, trainingSet, qualityEvaluator, true);
                double quality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);

                quality = Math.Round(quality * 100, 2);

                Result result = new Result() { Algorithm = "SP-TAN", Dataset = trainingSet.Metadata.DatasetName, Quality = quality };

                //----------------------------------------                    
                Console.WriteLine(result.ToString());
                SaveResults(result);


            }



            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("---------------------------------------------------");



        }

        public static void RunK2C()
        {
            foreach (string dataset in GetDatasetFolds())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {

                    //----------------------------------------
                    Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];


                    DataMining.ClassificationMeasures.AccuracyMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();

                    BayesianNetworks.Model.BayesianNetworkClassifier classifier = SingleTest.CreateK2BayesianClassifier(3, 250, trainingSet, true);
                    double quality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);

                    quality = Math.Round(quality * 100, 2);

                    Result result = new Result() { Algorithm = "K2", Dataset = trainingSet.Metadata.DatasetName, Fold = _currentFold, Quality = quality, Edges = classifier.Edges.Length };

                    //----------------------------------------                    
                    Console.WriteLine(result.ToString() + "Fold:" + _folds.ToString());
                    Console.WriteLine("---------------------------------------------------");


                    SaveResults(result);


                }



                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("---------------------------------------------------");

            }

        }

        public static void RunNaive()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];



                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);


                Console.WriteLine("Start");


                stopWatch.Reset();
                stopWatch.Start();

                BayesianNetworkClassifier naive = SingleTest.CreateNaiveBayesianClassifier(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                double quality = SingleTest.TestClassifier(naive, testingSet, testMeasure);


                stopWatch.Stop();
                Result result = new Result() { Algorithm = "Naive", Dataset = dataset, Quality = quality, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                Console.WriteLine(result.ToString());
                SaveResults(result);



            }

        }

        public static void RunTAN()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];



                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);


                Console.WriteLine("Start");


                stopWatch.Reset();
                stopWatch.Start();

                BayesianNetworkClassifier tan = SingleTest.CreateTANClassifier(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                double quality = SingleTest.TestClassifier(tan, testingSet, testMeasure);


                stopWatch.Stop();
                Result result = new Result() { Algorithm = "TAN", Dataset = dataset, Quality = quality, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                Console.WriteLine(result.ToString());
                SaveResults(result);



            }

        }

        public static void RunABCQEF()
        {
            foreach (string dataset in GetDatasetFiles())
            {
                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                foreach (IClassificationQualityMeasure measure in GetQualityEvaluators())
                {

                    //----------------------------------------
                    Console.WriteLine("Measure :" + measure.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    double quality = 0.0;
                    double edges = 0.0;

                    DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityEvaluator = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                    //DataMining.ClassificationMeasures.IClassificationQualityEvaluator qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyEvaluator();
                    ACO.ProblemSpecifics.IHeuristicValueCalculator<BayesianNetworks.Model.Edge> calculator = new DefaultHeuristicCalculator<Edge>();

                    for (int i = 0; i < _runs; i++)
                    {
                        int seed = (int)DateTime.Now.Ticks;
                        BayesianNetworks.Model.BayesianNetworkClassifier classifier = SingleTest.CreateABCMinerClassifier(seed, 500, 5, 10, 3, trainingSet, measure, calculator, false, true);
                        double currentQuality = SingleTest.TestClassifier(classifier, testingSet, qualityEvaluator);
                        //----------------------------------------
                        Console.WriteLine("==>:run" + i.ToString() + "-" + dataset + "-" + measure.ToString() + "=" + Math.Round(currentQuality, 2).ToString());
                        //----------------------------------------
                        quality += currentQuality;
                        edges += classifier.Edges.Length;

                    }


                    edges /= _runs;
                    quality /= _runs;

                    Result result = new Result() {Algorithm = "ABC-" + measure.ToString(), Dataset = trainingSet.Metadata.DatasetName, Quality = quality, Edges = edges };

                    Console.WriteLine(result.ToString() + "Fold:" + _folds.ToString());
                    Console.WriteLine("---------------------------------------------------");

                    SaveResults(result);



                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("---------------------------------------------------");
                }
            }


        }

        private static IEnumerable<IClassificationQualityMeasure> GetQualityEvaluators()
        {
            List<IClassificationQualityMeasure> list = new List<IClassificationQualityMeasure>();
            list.Add(new MicroAccuracyMeasure());
            list.Add(new FmeasureMeasure(1));
            list.Add(new JaccardMeasure());
            list.Add(new MestimateMeasure(10));
            list.Add(new RCostMeasure());
            list.Add(new SenstivitySpecificityMeasure());
            list.Add(new KlosgenMeasure());
            list.Add(new CertaintyFactorMeasure());
            return list;
        }

        public static void RunNaiveHlocal(iFourmi.DataMining.ClassificationMeasures.IClassificationQualityMeasure measure)
        {
            iFourmi.DataMining.Algorithms.IClassificationAlgorithm algorithm = new NaiveBayesAlgorithm();
            foreach (string filePath in System.IO.Directory.GetFiles(DatasetFolderPath))
            {
                Dataset trainingSet = iFourmi.DataMining.IO.ArffHelper.LoadHierarchicalDatasetFromTxt(filePath, true);
                List<Dataset> representation = new List<Dataset>() { trainingSet };

                Console.WriteLine("Begin");
                iFourmi.DataMining.Model.Hierarchical.IHierarchicalClassifier hClassifier = SingleTest.CreateLocalPerNodeHierarchicalClassifier(algorithm, representation, new DataMining.EnsembleStrategy.EnsembleBestClassificationStrategy(), measure, false, true);

                double quality = SingleTest.TestClassifier(hClassifier, representation, measure);
                quality = Math.Round(quality * 100, 2);
                Console.WriteLine("Quality: " + quality.ToString());
                Console.WriteLine("End");

                Result result = new Result();
                result.Algorithm = "Naive";
                result.Dataset = trainingSet.Metadata.DatasetName;
                result.Quality = quality;
                SaveResults(result);
            }

        }

        public static void RunNaiveHlocal(DataMining.EnsembleStrategy.IEnsembleClassificationStrategy ensembleStrategy, iFourmi.DataMining.ClassificationMeasures.IClassificationQualityMeasure measure)
        {
            iFourmi.DataMining.Algorithms.IClassificationAlgorithm algorithm = new NaiveBayesAlgorithm();
            List<DataMining.Data.Dataset> dataRepresentations = new List<DataMining.Data.Dataset>();
            foreach (string filePath in System.IO.Directory.GetFiles(DatasetFolderPath))
                dataRepresentations.Add(iFourmi.DataMining.IO.ArffHelper.LoadHierarchicalDatasetFromTxt(filePath, true));

            Console.WriteLine("Begin");
            iFourmi.DataMining.Model.Hierarchical.IHierarchicalClassifier hClassifier = SingleTest.CreateLocalPerNodeHierarchicalClassifier(algorithm, dataRepresentations, ensembleStrategy, measure, false, true);

            double quality = SingleTest.TestClassifier(hClassifier, dataRepresentations, measure);
            quality = Math.Round(quality * 100, 2);
            Console.WriteLine("Quality: " + quality.ToString());
            Console.WriteLine("End");

            Result result = new Result();
            result.Algorithm = "Naive";
            result.Dataset = ensembleStrategy.GetType().ToString();
            result.Quality = quality;

            SaveResults(result);

        }

        public static void RunABCHlocal(iFourmi.DataMining.ClassificationMeasures.IClassificationQualityMeasure measure)
        {
            foreach (string filePath in System.IO.Directory.GetFiles(DatasetFolderPath))
            {
                CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();
                BayesianClassificationQualityEvaluator qualityEvaluator = new BayesianClassificationQualityEvaluator(measure);
                BackwardRemovalLocalSearch localSearch = new BackwardRemovalLocalSearch(qualityEvaluator);
                IHeuristicValueCalculator<Edge> calculator = new CMICalculator();

                Problem<Edge> problem = new Problem<Edge>(invalidator, calculator, qualityEvaluator, localSearch);

                ABCMiner abcminer = new ABCMiner(25, 3, 3, problem, 2, false);

                Dataset trainingset = iFourmi.DataMining.IO.ArffHelper.LoadHierarchicalDatasetFromTxt(filePath, true);
                List<DataMining.Data.Dataset> dataRepresentations = new List<DataMining.Data.Dataset>();
                dataRepresentations.Add(trainingset);

                Console.WriteLine("Begin");
                iFourmi.DataMining.Model.Hierarchical.IHierarchicalClassifier hClassifier = SingleTest.CreateLocalPerNodeHierarchicalClassifier(abcminer, dataRepresentations, new DataMining.EnsembleStrategy.EnsembleBestClassificationStrategy(), measure, true, true);

                double quality = SingleTest.TestClassifier(hClassifier, dataRepresentations, measure);
                quality = Math.Round(quality * 100, 2);
                Console.WriteLine("Quality: " + quality.ToString());
                Console.WriteLine("End");

                Result result = new Result();
                result.Algorithm = "ABC";
                result.Dataset = trainingset.Metadata.DatasetName;
                result.Quality = quality;

                SaveResults(result);
            }

        }

        public static void RunABCHlocal(DataMining.EnsembleStrategy.IEnsembleClassificationStrategy ensembleStrategy, iFourmi.DataMining.ClassificationMeasures.IClassificationQualityMeasure measure)
        {

            CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();
            BayesianClassificationQualityEvaluator qualityEvaluator = new BayesianClassificationQualityEvaluator(measure);
            BackwardRemovalLocalSearch localSearch = new BackwardRemovalLocalSearch(qualityEvaluator);
            IHeuristicValueCalculator<Edge> calculator = new CMICalculator();

            Problem<Edge> problem = new Problem<Edge>(invalidator, calculator, qualityEvaluator, localSearch);

            ABCMiner abcminer = new ABCMiner(25, 3, 3, problem, 2, false);


            List<DataMining.Data.Dataset> dataRepresentations = new List<DataMining.Data.Dataset>();
            foreach (string filePath in System.IO.Directory.GetFiles(DatasetFolderPath))
                dataRepresentations.Add(iFourmi.DataMining.IO.ArffHelper.LoadHierarchicalDatasetFromTxt(filePath, true));

            Console.WriteLine("Begin");
            iFourmi.DataMining.Model.Hierarchical.IHierarchicalClassifier hClassifier = SingleTest.CreateLocalPerNodeHierarchicalClassifier(abcminer, dataRepresentations, ensembleStrategy, measure, true, true);

            double quality = SingleTest.TestClassifier(hClassifier, dataRepresentations, measure);
            quality = Math.Round(quality * 100, 2);
            Console.WriteLine("Quality: " + quality.ToString());
            Console.WriteLine("End");


            Result result = new Result();
            result.Algorithm = "ABC";
            result.Dataset = ensembleStrategy.GetType().ToString();
            result.Quality = quality;

            SaveResults(result);

        }

        public static void RunKMeans_Naive()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];



                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();

                Console.WriteLine("Start");

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    for (int i = 0; i < 10; i++)
                    {
                        int seed = (int)DateTime.Now.Ticks;

                        DataMining.Algorithms.IClusteringAlgorithm kmeans = new DataMining.Algorithms.KMeans(trainingSet, clusterNmber, similarityMeasure, 1000, true);
                        DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, clusterNmber, trainingSet, similarityMeasure, accuracy, kmeans, naive, false);

                        DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                        double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                        avgQualiy += quality;
                    }
                    avgQualiy /= 10;
                    stopWatch.Stop();
                    Result result = new Result() { Algorithm = "KMEANS_NAIVE", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                    Console.WriteLine(result.ToString());
                    // SaveResults(result);
                }


            }
        }

        public static void RunKMeans_TAN()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];



                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm tan = new TAN();

                Console.WriteLine("Start");

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    for (int i = 0; i < 10; i++)
                    {
                        int seed = (int)DateTime.Now.Ticks;

                        DataMining.Algorithms.IClusteringAlgorithm kmeans = new DataMining.Algorithms.KMeans(trainingSet, clusterNmber, similarityMeasure, 1000, true);
                        DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, clusterNmber, trainingSet, similarityMeasure, accuracy, kmeans, tan, false);

                        DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                        double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                        avgQualiy += quality;
                    }
                    avgQualiy /= 10;
                    stopWatch.Stop();
                    Result result = new Result() { Algorithm = "KMEANS_TAN", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                    Console.WriteLine(result.ToString());
                    SaveResults(result);
                }


            }
        }

        public static void RunACOClustIB_Naive()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];



                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    try
                    {

                        for (int i = 0; i < _runs; i++)
                        {

                            int seed = (int)DateTime.Now.Ticks;

                            DefaultHeuristicCalculator<ClusterExampleAssignment> calculator = new DefaultHeuristicCalculator<ClusterExampleAssignment>();
                            ClusteringIBInvalidator invalidator = new ClusteringIBInvalidator();
                            DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
                            ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
                            KMeansLocalSearch localSearch = new KMeansLocalSearch(trainingSet, 1, similarityMeasure, cohesionEvaluator);
                            ACO.ProblemSpecifics.ISolutionQualityEvaluator<DataMining.Model.ClusterExampleAssignment> evaluator = new ClusteringQualityEvaluator(measure);
                            Problem<DataMining.Model.ClusterExampleAssignment> problem = new Problem<DataMining.Model.ClusterExampleAssignment>(invalidator, calculator, evaluator, localSearch);

                            DataMining.Algorithms.IClusteringAlgorithm AntClustering = new ACOClustering_IB(1000, 10, 10, problem, clusterNmber, similarityMeasure, true);
                            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, clusterNmber, trainingSet, similarityMeasure, accuracy, AntClustering, naive, true);

                            DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                            avgQualiy += quality;
                        }
                        avgQualiy /= _runs;
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustIB_NAIVE", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustIB_NAIVE" + ex.Message, Dataset = dataset, Clusters = clusterNmber, Quality = -1, Seconds = -1 };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);

                    }
                }


            }
        }

        public static void RunACOClustIB_TAN()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];



                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm tan = new TAN();

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    try
                    {

                        for (int i = 0; i < _runs; i++)
                        {
                            int seed = (int)DateTime.Now.Ticks;

                            DefaultHeuristicCalculator<ClusterExampleAssignment> calculator = new DefaultHeuristicCalculator<ClusterExampleAssignment>();
                            ClusteringIBInvalidator invalidator = new ClusteringIBInvalidator();
                            DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
                            ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
                            KMeansLocalSearch localSearch = new KMeansLocalSearch(trainingSet, 1, similarityMeasure, cohesionEvaluator);
                            ACO.ProblemSpecifics.ISolutionQualityEvaluator<DataMining.Model.ClusterExampleAssignment> evaluator = new ClusteringQualityEvaluator(measure);
                            Problem<DataMining.Model.ClusterExampleAssignment> problem = new Problem<DataMining.Model.ClusterExampleAssignment>(invalidator, calculator, evaluator, localSearch);

                            DataMining.Algorithms.IClusteringAlgorithm AntClustering = new ACOClustering_IB(1000, 10, 10, problem, clusterNmber, similarityMeasure, true);
                            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, clusterNmber, trainingSet, similarityMeasure, accuracy, AntClustering, tan, true);

                            DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                            avgQualiy += quality;
                        }
                        avgQualiy /= _runs;
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustIB_TAN", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustIB_TAN" + ex.Message, Dataset = dataset, Clusters = clusterNmber, Quality = -1, Seconds = -1 };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);

                    }
                }


            }
        }

        public static void RunACOClustMB_Naive()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();


                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    try
                    {

                        for (int i = 0; i < _runs; i++)
                        {
                            int seed = (int)DateTime.Now.Ticks;

                            DefaultHeuristicCalculator<int> calculator = new DefaultHeuristicCalculator<int>();
                            ClusteringMBInvalidator invalidator = new ClusteringMBInvalidator();
                            DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
                            ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
                            KMeansLocalSearch localSearch = new KMeansLocalSearch(trainingSet, 1, similarityMeasure, cohesionEvaluator);
                            ACO.ProblemSpecifics.ISolutionQualityEvaluator<int> evaluator = new ClusteringQualityEvaluator(measure);
                            Problem<int> problem = new Problem<int>(invalidator, calculator, evaluator, localSearch);

                            DataMining.Algorithms.IClusteringAlgorithm AntClustering = new ACOClustering_MB(1000, 100, 10, problem, clusterNmber, similarityMeasure, true);
                            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, clusterNmber, trainingSet, similarityMeasure, accuracy, AntClustering, naive, true);

                            DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                            avgQualiy += quality;
                        }

                        avgQualiy /= _runs;

                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustMB_NAIVE", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustMB_NAIVE" + ex.Message, Dataset = dataset, Clusters = clusterNmber, Quality = -1, Seconds = -1 };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);

                    }
                }


            }
        }

        public static void RunACOClustMB_TAN()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm tan = new TAN();


                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    try
                    {

                        for (int i = 0; i < _runs; i++)
                        {
                            int seed = (int)DateTime.Now.Ticks;

                            DefaultHeuristicCalculator<int> calculator = new DefaultHeuristicCalculator<int>();
                            ClusteringMBInvalidator invalidator = new ClusteringMBInvalidator();
                            DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
                            ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
                            KMeansLocalSearch localSearch = new KMeansLocalSearch(trainingSet, 1, similarityMeasure, cohesionEvaluator);
                            ACO.ProblemSpecifics.ISolutionQualityEvaluator<int> evaluator = new ClusteringQualityEvaluator(measure);
                            Problem<int> problem = new Problem<int>(invalidator, calculator, evaluator, localSearch);

                            DataMining.Algorithms.IClusteringAlgorithm AntClustering = new ACOClustering_MB(1000, 100, 10, problem, clusterNmber, similarityMeasure, true);
                            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, clusterNmber, trainingSet, similarityMeasure, accuracy, AntClustering, tan, true);

                            DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                            avgQualiy += quality;
                        }

                        avgQualiy /= _runs;

                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustMB_TAN", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "ACOClustMB_TAN" + ex.Message, Dataset = dataset, Clusters = clusterNmber, Quality = -1, Seconds = -1 };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);

                    }
                }


            }
        }

        public static void RunAntClustBMNIB_Naive()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    for (int i = 0; i < _runs; i++)
                    {
                        int seed = (int)DateTime.Now.Ticks;

                        DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateAntClustBMNClassifier_IB(seed, trainingSet, 1000, 10, 10, clusterNmber, similarityMeasure, accuracy, naive, true);
                        DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                        double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                        avgQualiy += quality;

                    }

                    avgQualiy /= _runs;

                    stopWatch.Stop();
                    Result result = new Result() { Algorithm = "AntClustBMNIB_NAIVE", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                    Console.WriteLine(result.ToString());
                    SaveResults(result);
                }


            }
        }

        public static void RunAntClustBMNIB_TAN()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm tan = new TAN();

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    try
                    {
                        for (int i = 0; i < _runs; i++)
                        {
                            int seed = (int)DateTime.Now.Ticks;

                            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateAntClustBMNClassifier_IB(seed, trainingSet, 1000, 10, 10, clusterNmber, similarityMeasure, accuracy, tan, true);
                            DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                            avgQualiy += quality;

                        }


                        avgQualiy /= _runs;

                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "AntClustBMNIB_TAN", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "AntClustBMNIB_TAN" + ex.Message, Dataset = dataset, Clusters = clusterNmber, Quality = -1, Seconds = -1 };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);

                    }
                }


            }
        }

        public static void RunAntClustBMNMB_Naive()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];


                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    try
                    {
                        for (int i = 0; i < _runs; i++)
                        {
                            int seed = (int)DateTime.Now.Ticks;

                            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateAntClustBMNClassifier_MB(seed, trainingSet, 1000, 100, 10, clusterNmber, similarityMeasure, accuracy, naive, true);
                            DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                            avgQualiy += quality;

                        }

                        avgQualiy /= _runs;

                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "AntClustBMNMB_NAIVE", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "AntClustBMNMB_NAIVE" + ex.Message, Dataset = dataset, Clusters = clusterNmber, Quality = -1, Seconds = -1 };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);

                    }
                }


            }
        }

        public static void RunAntClustBMNMB_TAN()
        {
            foreach (string dataset in GetDatasetFiles())
            {

                DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset);
                DataMining.Data.Dataset trainingSet = tables[0];
                DataMining.Data.Dataset testingSet = tables[1];

                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm tan = new TAN();

                for (int clusterNmber = 2; clusterNmber <= 10; clusterNmber += 2)
                {
                    Console.WriteLine("Start");
                    stopWatch.Reset();
                    stopWatch.Start();

                    double avgQualiy = 0.0;

                    try
                    {
                        for (int i = 0; i < _runs; i++)
                        {
                            int seed = (int)DateTime.Now.Ticks;

                            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateAntClustBMNClassifier_MB(seed, trainingSet, 1000, 100, 10, clusterNmber, similarityMeasure, accuracy, tan, true);
                            DataMining.ClassificationMeasures.IClassificationQualityMeasure testMeasure = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
                            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, testMeasure);
                            avgQualiy += quality;

                        }

                        avgQualiy /= _runs;

                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "AntClustBMNMB_TAN", Dataset = dataset, Clusters = clusterNmber, Quality = avgQualiy, Seconds = (int)(stopWatch.ElapsedMilliseconds / 1000) };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Result result = new Result() { Algorithm = "AntClustBMNMB_TAN" + ex.Message, Dataset = dataset, Clusters = clusterNmber, Quality = -1, Seconds = -1 };
                        Console.WriteLine(result.ToString());
                        SaveResults(result);

                    }
                }


            }
        }

        private static void SaveResults(Result result)
        {
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(DatasetFolderPath + "\\results\\" + result.Algorithm + "-" + result.Dataset + "_results.txt", true);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(DatasetFolderPath + "\\results\\" + result.Dataset + "_results.txt", true);
            writer.WriteLine(result.ToString());
            writer.Flush();
            writer.Close();
        }

        public static void ExportGraph(string datasetName,BayesianNetworks.Model.BayesianNetworkClassifier network)
        {
            string filePath = DatasetFolderPath + "\\results\\" + datasetName + ".xml";
            string xml = BayesianNetworks.Utilities.GraphExporter.ExportToGaphSharpXml(network);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);
            writer.Write(xml);
            writer.Flush();
            writer.Close();
        }

        private static void LogError(Exception ex)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(DatasetFolderPath + "\\results\\error.txt", true);
            writer.WriteLine(ex.Message);
            writer.WriteLine(ex.StackTrace);
            writer.Flush();
            writer.Close();
        }

        private static List<string> GetDatasetFolds()
        {
            List<string> result = new List<string>();
            foreach (string directory in System.IO.Directory.GetDirectories(DatasetFolderPath))
            {
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(directory);

                result.Add(info.Name);

            }

            return result;
        }

        private static List<string> GetDatasetFiles()
        {
            List<string> result = new List<string>();
            foreach (string file in System.IO.Directory.GetFiles(DatasetFolderPath))
            {
                System.IO.FileInfo info = new System.IO.FileInfo(file);

                result.Add(info.Name);

            }

            return result;
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

        public static DataMining.Data.Dataset[] LoadTrainingAndTestingData(string dataSetFile)
        {

            string trFile = DatasetFolderPath + @"\" + dataSetFile;


            DataMining.Data.Dataset trainingSetDataTable = DataMining.IO.ArffHelper.LoadDatasetFromArff(trFile);
            DataMining.Data.Dataset testingSetDataTable = DataMining.IO.ArffHelper.LoadDatasetFromArff(trFile);


            DataMining.Data.Dataset[] result = new DataMining.Data.Dataset[2];
            result[0] = trainingSetDataTable;
            result[1] = testingSetDataTable;
            return result;
        }
    }

}
