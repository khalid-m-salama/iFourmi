using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.IO;
using iFourmi.BayesianNetworks.Model;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianLearning;
using iFourmi.BayesianLearning.Algorithms.ACO;
using iFourmi.BayesianLearning.Algorithms.GHC;
using iFourmi.DataMining.ProximityMeasures;
using iFourmi.DataMining.Model;

namespace iFourmi.Tester
{
    class Program
    {
        public static string folderPath = @"C:\0 - Khalid\Academics\Datasets\Datasets - ARFF - D";
        public static string datasetName = "voting";
        public static string datasetFilePath = folderPath + "\\" + datasetName + ".arff";

        static void Main(string[] args)
        {

            BatchTest.DatasetFolderPath = "Datasets";

            //TestABC();
            TestABCMiner();
            TestABCMinerPlus();
            //TestABCMinerPlusI();

            //BatchTest.RunABCMinerPlus_0();
            //BatchTest.RunABCMiner_0();
            //BatchTest.RunABCMinerPlusI_0();
            //Console.WriteLine("QEF");
            //BatchTest.RunABCQEF();
            

        }

        public static void TestANTClustBMN_MB()
        {
            int seed = (int)DateTime.Now.Ticks;
            Console.WriteLine("Start");

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFile);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFile);

            DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
            //DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.BinaryMatchingSimilarityMeasure();

            DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
            DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();
            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateAntClustBMNClassifier_MB(seed, trainingSet, 1, 1, 10, 3, similarityMeasure, accuracy, naive, true);
            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, accuracy);
            Console.WriteLine("Quality: " + quality.ToString());


        }

        public static void TestANTClustBMN_IB()
        {
            int seed = (int)DateTime.Now.Ticks;
            Console.WriteLine("Start");

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFile);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFile);

            DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
            //DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.BinaryMatchingSimilarityMeasure();

            DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
            DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();
            DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateAntClustBMNClassifier_IB(seed, trainingSet, 1, 1, 10, 3, similarityMeasure, accuracy, naive, true);
            double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, accuracy);
            Console.WriteLine("Quality: " + quality.ToString());


        }

        public static void TestKmeanClusteringBMN()
        {
            int seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Start");

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFile);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFile);

            double avgQualiy = 0;

            int k = 8;

            for (int i = 0; i < 10; i++)
            {
                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                //DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.BinaryMatchingSimilarityMeasure();
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();
                DataMining.Algorithms.IClusteringAlgorithm kmeans = new DataMining.Algorithms.KMeans(trainingSet, k, similarityMeasure, 100, true);
                DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, k, trainingSet, similarityMeasure, accuracy, kmeans, naive, false);
                double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, accuracy);
                Console.WriteLine("Quality: " + quality.ToString());
                avgQualiy += quality;
            }

            Console.WriteLine(avgQualiy / 10);
            Console.WriteLine("End");

        }

        public static void TestACOCluster_IBThenBMN()
        {
            int seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Start");

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFile);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFile);

            double avgQualiy = 0;

            for (int i = 0; i < 1; i++)
            {
                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                //DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.BinaryMatchingSimilarityMeasure();
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();

                DefaultHeuristicCalculator<ClusterExampleAssignment> calculator = new DefaultHeuristicCalculator<ClusterExampleAssignment>();
                ClusteringIBInvalidator invalidator = new ClusteringIBInvalidator();
                DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
                ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
                KMeansLocalSearch localSearch = new KMeansLocalSearch(trainingSet, 1, similarityMeasure, cohesionEvaluator);
                ACO.ProblemSpecifics.ISolutionQualityEvaluator<DataMining.Model.ClusterExampleAssignment> evaluator = new ClusteringQualityEvaluator(measure);
                Problem<DataMining.Model.ClusterExampleAssignment> problem = new Problem<DataMining.Model.ClusterExampleAssignment>(invalidator, calculator, evaluator, localSearch);

                DataMining.Algorithms.IClusteringAlgorithm AntClustering = new ACOClustering_IB(1000, 10, 10, problem, 10, similarityMeasure, true);
                DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, 10, trainingSet, similarityMeasure, accuracy, AntClustering, naive, true);
                double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, accuracy);
                Console.WriteLine("Quality: " + quality.ToString());
                avgQualiy += quality;
            }

            Console.WriteLine(avgQualiy / 10);
            Console.WriteLine("End");

        }

        public static void TestACOCluster_MBThenBMN()
        {
            int seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Start");

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFile);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFile);

            double avgQualiy = 0;

            for (int i = 0; i < 1; i++)
            {
                DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
                DataMining.ClassificationMeasures.IClassificationQualityMeasure accuracy = new DataMining.ClassificationMeasures.AccuracyMeasure();
                DataMining.Algorithms.IClassificationAlgorithm naive = new NaiveBayesAlgorithm();

                DefaultHeuristicCalculator<int> calculator = new DefaultHeuristicCalculator<int>();
                ClusteringMBInvalidator invalidator = new ClusteringMBInvalidator();
                DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
                ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
                KMeansLocalSearch localSearch = new KMeansLocalSearch(trainingSet, 1, similarityMeasure, cohesionEvaluator);
                ACO.ProblemSpecifics.ISolutionQualityEvaluator<int> evaluator = new ClusteringQualityEvaluator(measure);
                Problem<int> problem = new Problem<int>(invalidator, calculator, evaluator, localSearch);

                DataMining.Algorithms.IClusteringAlgorithm AntClustering = new ACOClustering_MB(1000, 10, 10, problem, 6, similarityMeasure, true);
                DataMining.Model.IClassifier cBMNClassifier = SingleTest.CreateClusteringBMNClassifier(seed, 6, trainingSet, similarityMeasure, accuracy, AntClustering, naive, true);
                double quality = SingleTest.TestClassifier(cBMNClassifier, testingSet, accuracy);
                Console.WriteLine("Quality: " + quality.ToString());
                avgQualiy += quality;
            }

            Console.WriteLine(avgQualiy / 10);
            Console.WriteLine("End");

        }

        public static void TestACOCluster_IB()
        {
            int seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Start");

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFile);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFile);

            DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
            DataMining.ProximityMeasures.IClusteringQualityMeasure cohesionEvaluator = new DataMining.ProximityMeasures.CohesionClusteringMeasure();
            DataMining.Model.ClusteringSolution solution = SingleTest.CreateACOClusters_IB(seed, trainingSet, 3, similarityMeasure, 10, 1, 10, true, true);
            double quality = cohesionEvaluator.CalculateQuality(solution);
            Console.WriteLine("Final Quality:" + quality.ToString());

            Console.WriteLine("End");

        }

        public static void TestACOCluster_MB()
        {
            int seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Start");

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFile);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFile);

            DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.ClassBasedSimilarityMeasure(trainingSet);
            DataMining.ProximityMeasures.IClusteringQualityMeasure cohesionEvaluator = new DataMining.ProximityMeasures.CohesionClusteringMeasure();
            DataMining.Model.ClusteringSolution solution = SingleTest.CreateACOClusters_MB(seed, trainingSet, 3, similarityMeasure, 1000, 10, 10, true, true);
            double quality = cohesionEvaluator.CalculateQuality(solution);
            Console.WriteLine("Final Quality:" + quality.ToString());

            Console.WriteLine("End");

        }

        public static void TestNaive()
        {
            Console.WriteLine("Start");

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);

            DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();

            BayesianNetworkClassifier naive = SingleTest.CreateNaiveBayesianClassifier(trainingSet);
            double quality = SingleTest.TestClassifier(naive, testingSet, qualityEvaluator);
            quality = Math.Round(quality * 100, 2);
            Console.WriteLine("Naive Quality: " + quality.ToString());
            Console.WriteLine("End");
        }

        public static void TestTAN()
        {
            Console.WriteLine("Start");

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);

            DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();


            BayesianNetworkClassifier tan = SingleTest.CreateTANClassifier(trainingSet);
            double quality = SingleTest.TestClassifier(tan, testingSet, qualityEvaluator);
            quality = Math.Round(quality * 100, 2);
            Console.WriteLine("TAN Quality: " + quality.ToString());


            Console.WriteLine("End");


        }

        public static void TestABCMiner()
        {
            Console.WriteLine("Start");

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);



            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure1 = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure2 = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure3 = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure4 = new DataMining.ClassificationMeasures.ProbabilityReducedErrorMeasure();

            IHeuristicValueCalculator<Edge> calculator = new CMICalculator();

            int seed = (int)DateTime.Now.Ticks;
            BayesianNetworkClassifier abclassifier = SingleTest.CreateABCMinerClassifier(seed, 100, 10, 10, 3, trainingSet, measure1, calculator, false, true);

            //double quality1 = SingleTest.TestClassifier(abclassifier, testingSet, measure1);
            //quality1 = Math.Round(quality1 * 100, 2);

            //double quality2 = SingleTest.TestClassifier(abclassifier, testingSet, measure2);
            //quality2 = Math.Round(quality2 * 100, 2);

            //double quality3 = SingleTest.TestClassifier(abclassifier, testingSet, measure3);
            //quality3 = Math.Round(quality3 * 100, 2);

            double quality4 = SingleTest.TestClassifier(abclassifier, testingSet, measure4);
            quality4 = Math.Round(quality4 * 100, 2);

            //Console.WriteLine("ABC Quality1: " + quality1.ToString());
            //Console.WriteLine("ABC Quality2: " + quality2.ToString());
            //Console.WriteLine("ABC Quality3: " + quality3.ToString());
            Console.WriteLine("ABC Quality4: " + quality4.ToString());
            Console.WriteLine("End");


            string xml = BayesianNetworks.Utilities.GraphExporter.ExportToGaphSharpXml(abclassifier);
            System.IO.File.WriteAllText(@"C:\0 - Khalid\Academics\" + datasetName + "1.xml", xml);


            Console.ReadLine();


        }

        public static void TestABC()
        {
            Console.WriteLine("Start");

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);

            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure1 = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure2 = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure3 = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure4 = new DataMining.ClassificationMeasures.ProbabilityReducedErrorMeasure();


            int seed = (int)DateTime.Now.Ticks;
            BayesianNetworkClassifier abclassifier = SingleTest.CreateABCClassifier(seed, 10, 5, 10, trainingSet, measure1, false, true);
            double quality1 = SingleTest.TestClassifier(abclassifier, testingSet, measure1);
            quality1 = Math.Round(quality1 * 100, 2);

            double quality2 = SingleTest.TestClassifier(abclassifier, testingSet, measure2);
            quality2 = Math.Round(quality2 * 100, 2);

            double quality3 = SingleTest.TestClassifier(abclassifier, testingSet, measure3);
            quality3 = Math.Round(quality3 * 100, 2);

            double quality4 = SingleTest.TestClassifier(abclassifier, testingSet, measure4);
            quality4 = Math.Round(quality4 * 100, 2);

            Console.WriteLine("ABC Quality1: " + quality1.ToString());
            Console.WriteLine("ABC Quality2: " + quality2.ToString());
            Console.WriteLine("ABC Quality3: " + quality3.ToString());
            Console.WriteLine("ABC Quality4: " + quality4.ToString());
            Console.WriteLine("End");

            Console.ReadLine();


        }

        public static void TestABCMinerPlus()
        {
            Console.WriteLine("Start");

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);

            //DataMining.ClassificationMeasures.IClassificationQualityMeasure measure1 = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure2 = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();
            //DataMining.ClassificationMeasures.IClassificationQualityMeasure measure3 = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure4 = new DataMining.ClassificationMeasures.ProbabilityReducedErrorMeasure();

            BayesianNetworkClassifier abcBNClassifier = null;

            int seed = (int)DateTime.Now.Ticks;
            BayesianNetworkClassifier abcMinerPlusclassifier = SingleTest.CreateABCMinerPlusClassifier(seed, 100, 10, 10, 3, trainingSet, measure2, false, true, out abcBNClassifier);

            //double quality11 = SingleTest.TestClassifier(abcBNClassifier, testingSet, measure1);
            //double quality21 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure1);

            //double quality12 = SingleTest.TestClassifier(abcBNClassifier, testingSet, measure2);
            //double quality22 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure2);

            //double quality13 = SingleTest.TestClassifier(abcBNClassifier, testingSet, measure3);
            //double quality23 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure3);

            //double quality14 = SingleTest.TestClassifier(abcBNClassifier, testingSet, measure4);
            double quality24 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure4);



            //Console.WriteLine("ABC Quality: " + quality11.ToString());
            //Console.WriteLine("ABCMinerPlus Quality: " + quality21.ToString());

            //Console.WriteLine("ABC Quality: " + quality12.ToString());
            //Console.WriteLine("ABCMinerPlus Quality: " + quality22.ToString());

            //Console.WriteLine("ABC Quality: " + quality13.ToString());
            //Console.WriteLine("ABCMinerPlus Quality: " + quality23.ToString());

            //Console.WriteLine("ABC Quality: " + quality14.ToString());
            Console.WriteLine("ABCMinerPlus Quality: " + quality24.ToString());

            string xml = BayesianNetworks.Utilities.GraphExporter.ExportToGaphSharpXml(abcMinerPlusclassifier);
            System.IO.File.WriteAllText(@"C:\0 - Khalid\Academics\" + datasetName + "2.xml",xml);

            //Console.WriteLine("ABC Size: " + abcBNClassifier.Edges.Length);
            //Console.WriteLine("ABCMinerPlus Size: " + abcMinerPlusclassifier.Edges.Length);


            

            Console.WriteLine("End");

            Console.ReadLine();
        }

        public static void TestABCMinerPlusI()
        {
            Console.WriteLine("Start");

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(datasetFilePath);

            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure1 = new DataMining.ClassificationMeasures.MicroAccuracyMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure2 = new DataMining.ClassificationMeasures.ProbabilityAccuracyMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure3 = new DataMining.ClassificationMeasures.ReducedErrorMeasure();
            DataMining.ClassificationMeasures.IClassificationQualityMeasure measure4 = new DataMining.ClassificationMeasures.ProbabilityReducedErrorMeasure();


            int seed = (int)DateTime.Now.Ticks;
            BayesianNetworkClassifier abcMinerPlusclassifier = SingleTest.CreateABCMinerPlusIClassifier(seed, 10, 1, 5, 10, 2, trainingSet, measure1, false, true);

            double quality1 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure1);
            quality1 = Math.Round(quality1 * 100, 2);

            double quality2 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure2);
            quality2 = Math.Round(quality2 * 100, 2);

            double quality3 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure3);
            quality3 = Math.Round(quality3 * 100, 2);

            double quality4 = SingleTest.TestClassifier(abcMinerPlusclassifier, testingSet, measure4);
            quality4 = Math.Round(quality4 * 100, 2);

            Console.WriteLine("ABCMinerPlusI Quality1: " + quality1.ToString());
            Console.WriteLine("ABCMinerPlusI Quality2: " + quality2.ToString());
            Console.WriteLine("ABCMinerPlusI Quality3: " + quality3.ToString());
            Console.WriteLine("ABCMinerPlusI Quality4: " + quality4.ToString());
            Console.WriteLine("End");


            Console.WriteLine("End");

            Console.ReadLine();
        }

        public static void TestGBMNABC()
        {
            Console.WriteLine("Start");

            string dsname = "car";

            string trainingSetPath = @"C:\0 - Khalid\Academics\Datasets\" + dsname + @"\TR0_" + dsname + ".arff";
            string testingSetPath = @"C:\0 - Khalid\Academics\Datasets\" + dsname + @"\TS0_" + dsname + ".arff";

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(trainingSetPath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(testingSetPath);

            DataMining.ClassificationMeasures.AccuracyMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();
            IHeuristicValueCalculator<Edge> calculator = new MICalculator();
            int seed = (int)DateTime.Now.Ticks;
            BayesianMultinetClassifier mnabclassifier = SingleTest.CreateGMNAntBayesianClassification(seed, 10, 5, 10, 3, trainingSet, null, calculator, true);
            double quality = SingleTest.TestClassifier(mnabclassifier, testingSet, qualityEvaluator);
            quality = Math.Round(quality * 100, 2);

            Console.WriteLine("GMNABC Quality: " + quality.ToString());
            Console.WriteLine("End");

            //Console.ReadLine();


        }

        public static void TestLBMNABC()
        {
            Console.WriteLine("Start");

            string dsname = "car";

            string trainingSetPath = @"C:\0 - Khalid\Academics\Datasets\" + dsname + @"\TR0_" + dsname + ".arff";
            string testingSetPath = @"C:\0 - Khalid\Academics\Datasets\" + dsname + @"\TS0_" + dsname + ".arff";

            Dataset trainingSet = ArffHelper.LoadDatasetFromArff(trainingSetPath);
            Dataset testingSet = ArffHelper.LoadDatasetFromArff(testingSetPath);

            DataMining.ClassificationMeasures.AccuracyMeasure qualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();
            ISolutionQualityEvaluator<Edge> trainingQualityEvaluator = new likelihoodQualityEvaluator(trainingSet);

            IHeuristicValueCalculator<Edge> calculator = new MICalculator();
            int seed = (int)DateTime.Now.Ticks;
            BayesianMultinetClassifier lmnabclassifier = SingleTest.CreateLMNAntBayesianClassification(seed, 10, 5, 10, 3, trainingSet, trainingQualityEvaluator, calculator, true);
            double quality = SingleTest.TestClassifier(lmnabclassifier, testingSet, qualityEvaluator);
            quality = Math.Round(quality * 100, 2);

            Console.WriteLine("LMNABC Quality: " + quality.ToString());
            Console.WriteLine("End");

            //Console.ReadLine();


        }

        public static void TestEnsembleH()
        {
            string folderPath = @"C:\0 - Khalid\Academics\Datasets\ageing\test";
            //string folderPath = @"C:\0 - Khalid\Academics\Datasets\ageing";

            iFourmi.DataMining.Algorithms.IClassificationAlgorithm algorithm = new NaiveBayesAlgorithm();
            iFourmi.DataMining.ClassificationMeasures.IClassificationQualityMeasure evaluator = new iFourmi.DataMining.ClassificationMeasures.AccuracyMeasure();

            List<DataMining.Data.Dataset> dataRepresentations = new List<Dataset>();
            foreach (string filePath in System.IO.Directory.GetFiles(folderPath))
                dataRepresentations.Add(ArffHelper.LoadHierarchicalDatasetFromTxt(filePath, true));

            Console.WriteLine("Begin");
            iFourmi.DataMining.EnsembleStrategy.IEnsembleClassificationStrategy ensembleStrategy = new iFourmi.DataMining.EnsembleStrategy.EnsembleBestClassificationStrategy();
            iFourmi.DataMining.Model.Hierarchical.IHierarchicalClassifier hClassifier = SingleTest.CreateLocalPerNodeHierarchicalClassifier(algorithm, dataRepresentations, ensembleStrategy, evaluator, true, true);

            double quality = SingleTest.TestClassifier(hClassifier, dataRepresentations, evaluator);
            quality = Math.Round(quality * 100, 2);
            Console.WriteLine("Quality: " + quality.ToString());
            Console.WriteLine("End");

        }

        public static void TestKMeans()
        {

            string datasetFile = folderPath + "\\" + datasetName + ".arff";
            Dataset dataset = ArffHelper.LoadDatasetFromArff(datasetFile);

            DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure = new DataMining.ProximityMeasures.BinaryMatchingSimilarityMeasure();
            DataMining.ProximityMeasures.IClusteringQualityMeasure cohesionEvaluator = new DataMining.ProximityMeasures.CohesionClusteringMeasure();

            double maxQuality = 0;
            double avgQuality = 0;

            for (int i = 0; i < 1; i++)
            {
                int seed = (int)DateTime.Now.Ticks;

                DataMining.Model.ClusteringSolution solution = SingleTest.CreateKMeansClusters(seed, dataset, 5, similarityMeasure, 1000, true);
                double currentQuality = cohesionEvaluator.CalculateQuality(solution);
                Console.WriteLine("Iteration " + i.ToString() + ":" + currentQuality.ToString());
                avgQuality += currentQuality;
                if (currentQuality > maxQuality)
                    maxQuality = currentQuality;
            }

            // avgQuality /= 10;

            // Console.WriteLine("Maximum:" + maxQuality.ToString());
            // Console.WriteLine("Average:" + avgQuality.ToString());
        }

    }
}
