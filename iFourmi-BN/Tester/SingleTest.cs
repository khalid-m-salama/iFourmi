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



namespace iFourmi.Tester
{
    public static class  SingleTest
    {
        public static BayesianNetworkClassifier abcBNClassfier;
        
        public static BayesianNetworkClassifier CreateNaiveBayesianClassifier(Dataset trainingSet)
        {
            NaiveBayesAlgorithm nb = new NaiveBayesAlgorithm();
            nb.Dataset = trainingSet;
            BayesianNetworkClassifier bnClassifier = nb.CreateClassifier() as BayesianNetworkClassifier;
            return bnClassifier;
                       
        }

        public static BayesianNetworkClassifier CreateTANClassifier(Dataset trainingSet)
        {
            TAN tan = new TAN();
            tan.Dataset = trainingSet;
            BayesianNetworkClassifier bnClassifier = tan.CreateClassifier() as BayesianNetworkClassifier;
            return bnClassifier;

        }

        public static BayesianNetworkClassifier CreateABCMinerClassifier(int seed, int iterations, int colonySize, int convergence, int dependencies, Dataset trainingSet, DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityEvaluator,IHeuristicValueCalculator<Edge> calculator,bool performLocalSearch,bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);

            CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();
            BayesianClassificationQualityEvaluator evaluator = new BayesianClassificationQualityEvaluator(qualityEvaluator);
            BackwardRemovalLocalSearch localSearch = new BackwardRemovalLocalSearch(evaluator);

            Problem<Edge> problem = new Problem<Edge>(invalidator, calculator, evaluator, localSearch);

            ABCMiner abcminer = new ABCMiner(iterations, colonySize, convergence, problem, dependencies,performLocalSearch);
            abcminer.Dataset = trainingSet;

            if (fireEvents)
            {
                abcminer.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                abcminer.OnPostColonyIteration += new EventHandler(abclassifier_OnPostColonyIteration);

            }


            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = abcminer.CreateClassifier() as BayesianNetworkClassifier;
            return bnclassifier;

            
        }

        public static BayesianNetworkClassifier CreateABCClassifier(int seed, int iterations, int colonySize, int convergence,Dataset trainingSet, DataMining.ClassificationMeasures.IClassificationQualityMeasure measure, bool performLocalSearch, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);

            VariableTypeAssigmentInvalidator invalidator = new VariableTypeAssigmentInvalidator();
            BayesianClassificationQualityEvaluator evaluator = new BayesianClassificationQualityEvaluator(measure);
            VariableTypeAssignmentLocalSearch localSearch = new VariableTypeAssignmentLocalSearch(evaluator);
            DefaultHeuristicCalculator<VariableTypeAssignment> calculator = new DefaultHeuristicCalculator<VariableTypeAssignment>();
            Problem<VariableTypeAssignment> problem = new Problem<VariableTypeAssignment>(invalidator, calculator, evaluator, localSearch);

            ABC abc = new ABC(iterations, colonySize, convergence, problem,trainingSet,performLocalSearch);
            

            if (fireEvents)
            {
                abc.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                abc.OnPostColonyIteration += new EventHandler(abclassifier_OnPostColonyIteration);

            }

            
            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = abc.CreateClassifier() as BayesianNetworkClassifier;
            return bnclassifier;


        }


        public static BayesianNetworkClassifier CreateABCMinerPlusClassifier(int seed, int iterations, int colonySize, int convergence,int maxParents ,Dataset trainingSet, DataMining.ClassificationMeasures.IClassificationQualityMeasure measure, bool performLocalSearch, bool fireEvents, out BayesianNetworkClassifier abcBNClassifier)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);

            VariableTypeAssigmentInvalidator invalidator1 = new VariableTypeAssigmentInvalidator();
            BayesianClassificationQualityEvaluator evaluator = new BayesianClassificationQualityEvaluator(measure);
            VariableTypeAssignmentLocalSearch localSearch1 = new VariableTypeAssignmentLocalSearch(evaluator);
            DefaultHeuristicCalculator<VariableTypeAssignment> calculator1 = new DefaultHeuristicCalculator<VariableTypeAssignment>();
            Problem<VariableTypeAssignment> problem1 = new Problem<VariableTypeAssignment>(invalidator1, calculator1, evaluator, localSearch1);

            CyclicRelationInvalidator invalidator2 = new CyclicRelationInvalidator();            
            BackwardRemovalLocalSearch localSearch2 = new BackwardRemovalLocalSearch(evaluator);
            CMICalculator calculator = new CMICalculator();
            Problem<Edge> problem2 = new Problem<Edge>(invalidator2, calculator, evaluator, localSearch2);
            
            ABCMinerPlus abcMinerPlus = new ABCMinerPlus(iterations, colonySize, convergence, problem2, problem1, maxParents, trainingSet, performLocalSearch);
   
            if (fireEvents)
            {
                abcMinerPlus.ABCAlgorithm.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                abcMinerPlus.ABCAlgorithm.OnPostColonyIteration += new EventHandler(abclassifier_OnPostColonyIteration);

                abcMinerPlus.ABCMinerAlgorithm.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                abcMinerPlus.ABCMinerAlgorithm.OnPostColonyIteration += new EventHandler(abclassifier_OnPostColonyIteration);

                abcMinerPlus.OnVariableTypeAssignmentCompleted += new EventHandler(abcMinerPlus_OnVariableTypeAssignmentCompleted);

            }


            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = abcMinerPlus.CreateClassifier() as BayesianNetworkClassifier;
            abcBNClassifier = abcBNClassfier;
            return bnclassifier;


        }

        public static BayesianNetworkClassifier CreateABCMinerPlusIClassifier(int seed, int iterations, int colonySize, int localColonySize, int convergence, int maxParents, Dataset trainingSet, DataMining.ClassificationMeasures.IClassificationQualityMeasure measure, bool performLocalSearch, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);

            VariableTypeAssigmentInvalidator invalidator1 = new VariableTypeAssigmentInvalidator();
            BayesianClassificationQualityEvaluator evaluator = new BayesianClassificationQualityEvaluator(measure);
            SimpleVariableTypeAssignmentLocalSearch localSearch1 = new SimpleVariableTypeAssignmentLocalSearch(evaluator);
            DefaultHeuristicCalculator<VariableTypeAssignment> calculator1 = new DefaultHeuristicCalculator<VariableTypeAssignment>();
            Problem<VariableTypeAssignment> problem1 = new Problem<VariableTypeAssignment>(invalidator1, calculator1, evaluator, localSearch1);

            CyclicRelationInvalidator invalidator2 = new CyclicRelationInvalidator();
            BackwardRemovalLocalSearch localSearch2 = new BackwardRemovalLocalSearch(evaluator);
            CMICalculator calculator = new CMICalculator();
            Problem<Edge> problem2 = new Problem<Edge>(invalidator2, calculator, evaluator, localSearch2);

            ABCMinerPlusI abcMinerPlusI = new ABCMinerPlusI(iterations, colonySize,localColonySize,convergence, problem2, problem1, maxParents, trainingSet, performLocalSearch);

            if (fireEvents)
            {
                abcMinerPlusI.ABCAlgorithm.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                abcMinerPlusI.ABCAlgorithm.OnPostColonyIteration += new EventHandler(abclassifier_OnPostColonyIteration);

                abcMinerPlusI.ABCMinerAlgorithm.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                abcMinerPlusI.ABCMinerAlgorithm.OnPostColonyIteration += new EventHandler(abclassifier_OnPostColonyIteration);

            }


            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = abcMinerPlusI.CreateClassifier() as BayesianNetworkClassifier;
            
            return bnclassifier;


        }

        static void abcMinerPlus_OnVariableTypeAssignmentCompleted(object sender, EventArgs e)
        {
            BayesianNetworkClassifier abcBNC = sender as BayesianNetworkClassifier;
            abcBNClassfier = abcBNC;
        }


        public static BayesianMultinetClassifier CreateLMNAntBayesianClassification(int seed, int iterations, int colonySize, int convergence, int dependencies, Dataset trainingSet, ISolutionQualityEvaluator<Edge> qualityEvaluator, IHeuristicValueCalculator<Edge> calculator, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);

            CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();            
            BackwardRemovalLocalSearch localSearch = new BackwardRemovalLocalSearch(qualityEvaluator);

            Problem<Edge> problem = new Problem<Edge>(invalidator, calculator, qualityEvaluator, localSearch);

            ABCMinerLMN mnabcminer = new ABCMinerLMN(iterations, colonySize, convergence, problem, dependencies, trainingSet);

            if (fireEvents)
            {
                mnabcminer.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                mnabcminer.OnPostColonyIteration += new EventHandler(abclassifier_OnPostColonyIteration);

            }

            mnabcminer.Work();

            BayesianNetworks.Model.BayesianMultinetClassifier mnbclassifier = mnabcminer.MultinetBayesianClassifier;
            return mnbclassifier;


        }

        public static BayesianMultinetClassifier CreateGMNAntBayesianClassification(int seed, int iterations, int colonySize, int convergence, int dependencies, Dataset trainingSet, ISolutionQualityEvaluator<Edge> qualityEvaluator, IHeuristicValueCalculator<Edge> calculator, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);

            CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();
            BackwardRemovalLocalSearch localSearch = new BackwardRemovalLocalSearch(qualityEvaluator);

            Problem<Edge> problem = new Problem<Edge>(invalidator, calculator, qualityEvaluator, localSearch);

            ABCMinerGMN mnabcminer = new ABCMinerGMN(iterations, colonySize, convergence, problem, dependencies, trainingSet);

            if (fireEvents)
            {
                mnabcminer.OnPostAntSolutionContruction += new EventHandler(abclassifier_OnPostAntSolutionContruction);
                mnabcminer.OnPostColonyIteration += new EventHandler(GMNabclassifier_OnPostColonyIteration);

            }

            mnabcminer.Work();

            BayesianNetworks.Model.BayesianMultinetClassifier mnbclassifier = mnabcminer.MultinetBayesianClassifier;
            return mnbclassifier;


        }

        public static BayesianNetworkClassifier CreateGreedyBayesianClassifier(int maxDependencies, int maxEvaluations, Dataset trainingSet, DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityEvaluator, bool fireEvents)
        {
            CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();
            invalidator.MaxDependencies = maxDependencies;
            BayesianClassificationQualityEvaluator evaluator = new BayesianClassificationQualityEvaluator( qualityEvaluator);
            evaluator.ValidationSet = trainingSet;
            BackwardRemovalLocalSearch localSearch = new BackwardRemovalLocalSearch(evaluator);

            Problem<Edge> problem = new Problem<Edge>(invalidator, null, evaluator, localSearch);           

            GHC hcblassifier = new GHC(0, 0, 0, problem, maxEvaluations, trainingSet, trainingSet);


            if (fireEvents)
            {
                hcblassifier.OnPostEvaluation += new EventHandler(hcblassifier_OnPostEvaluation);
                hcblassifier.OnProgress += new EventHandler(hcblassifier_OnProgress);
            }


            hcblassifier.Work();

            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = hcblassifier.BayesianNetworkClassifier;
            return bnclassifier;
        }

        public static BayesianNetworkClassifier CreateK2BayesianClassifier(int maxDependencies, int maxEvaluations, Dataset trainingSet, bool fireEvents)
        {
            CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();
            invalidator.MaxDependencies = maxDependencies;
                       

            Problem<Edge> problem = new Problem<Edge>(invalidator, null, null, null);

            K2Algorithm k2blassifier = new K2Algorithm(0, 0, 0, problem, maxEvaluations, trainingSet, trainingSet);


            if (fireEvents)
            {
                k2blassifier.OnPostEvaluation += new EventHandler(hcblassifier_OnPostEvaluation);
                k2blassifier.OnProgress += new EventHandler(hcblassifier_OnProgress);
            }


            k2blassifier.Work();

            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = k2blassifier.BayesianNetworkClassifier;            
            return bnclassifier;
        }

        public static IHierarchicalClassifier CreateLocalPerNodeHierarchicalClassifier(IClassificationAlgorithm algorithm, List<DataMining.Data.Dataset> dataRepresentations, IEnsembleClassificationStrategy ensembleStrategy, IClassificationQualityMeasure evaluator, bool serialize, bool fireEvents)
        {

            DataMining.Utilities.RandomUtility.Initialize();

            LocalPerNodeClassificationAlgorithm hClassificatoinAlgorithm = new LocalPerNodeClassificationAlgorithm(dataRepresentations, algorithm, ensembleStrategy, evaluator, serialize,fireEvents);

            if (fireEvents)
            {
                hClassificatoinAlgorithm.onPostClassifierConstruction += new EventHandler(hClassificatoinAlgorithm_onPostClassifierConstruction);
                hClassificatoinAlgorithm.onPostNodeProcessing += new EventHandler(hClassificatoinAlgorithm_onPostNodeProcessing);

            } 


            return hClassificatoinAlgorithm.CreateClassifier();
        }

        public static ClusteringSolution CreateKMeansClusters(int seed, Dataset dataset, int clustersNumber, ISimilarityMeasure similarityMeasure, int maxIterations, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);
            KMeans kmeans = new KMeans(dataset, clustersNumber, similarityMeasure, maxIterations, fireEvents);
            kmeans.Initialize();            
            kmeans.OnPostIteration += new EventHandler(kmeans_OnPostIteration);
            return kmeans.CreateClusters();

        }

        public static ClusteringSolution CreateACOClusters_IB(int seed,Dataset dataset, int clustersNumber, ISimilarityMeasure similarityMeasure, int maxIterations, int colonySize,int convergenceIterations,bool fireEvents,bool performLocalSearch)
        {

            DataMining.Utilities.RandomUtility.Initialize(seed);
            DefaultHeuristicCalculator<ClusterExampleAssignment> calculator = new DefaultHeuristicCalculator<ClusterExampleAssignment>();
            ClusteringIBInvalidator invalidator = new ClusteringIBInvalidator(); 
            DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
            ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
            KMeansLocalSearch localSearch = new KMeansLocalSearch(dataset, 1,similarityMeasure,cohesionEvaluator);                        
            ACO.ProblemSpecifics.ISolutionQualityEvaluator<DataMining.Model.ClusterExampleAssignment> evaluator = new ClusteringQualityEvaluator(measure);
            Problem<DataMining.Model.ClusterExampleAssignment> problem = new Problem<DataMining.Model.ClusterExampleAssignment>(invalidator, calculator, evaluator, localSearch);
            
            ACOClustering_IB antClustering = new ACOClustering_IB(maxIterations, colonySize, convergenceIterations, problem, clustersNumber, similarityMeasure,dataset,performLocalSearch);
            antClustering.OnPostColonyIteration += new EventHandler(antClustering_OnPostColonyIteration);

            return antClustering.CreateClusters();

            

        }

        public static ClusteringSolution CreateACOClusters_MB(int seed, Dataset dataset, int clustersNumber, ISimilarityMeasure similarityMeasure, int maxIterations, int colonySize, int convergenceIterations, bool fireEvents, bool performLocalSearch)
        {

            DataMining.Utilities.RandomUtility.Initialize(seed);
            DefaultHeuristicCalculator<int> calculator = new DefaultHeuristicCalculator<int>();
            ClusteringMBInvalidator invalidator = new ClusteringMBInvalidator();
            DataMining.ProximityMeasures.IClusteringQualityMeasure measure = new CohesionClusteringMeasure();
            ClusteringQualityEvaluator cohesionEvaluator = new ClusteringQualityEvaluator(measure);
            KMeansLocalSearch localSearch = new KMeansLocalSearch(dataset, 1, similarityMeasure, cohesionEvaluator);
            ACO.ProblemSpecifics.ISolutionQualityEvaluator<int> evaluator = new ClusteringQualityEvaluator(measure);
            Problem<int> problem = new Problem<int>(invalidator, calculator, evaluator, localSearch);            


            ACOClustering_MB antClustering = new ACOClustering_MB(maxIterations, colonySize, convergenceIterations, problem, clustersNumber, similarityMeasure, dataset, performLocalSearch);
            antClustering.OnPostColonyIteration += new EventHandler(antClustering_OnPostColonyIteration);

            return antClustering.CreateClusters();



        }


        public static BayesianClusterMultinetClassifier CreateClusteringBMNClassifier(int seed, int clusterNumber,Dataset dataset, ISimilarityMeasure similarityMeasure, IClassificationQualityMeasure accuracy, IClusteringAlgorithm algorithm, IClassificationAlgorithm naive, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);
            if (fireEvents)
            {
                if (algorithm is ACOClustering_IB)
                    ((ACOClustering_IB)algorithm).OnPostColonyIteration += new EventHandler(antClustering_OnPostColonyIteration);
                if (algorithm is ACOClustering_MB)
                    ((ACOClustering_MB)algorithm).OnPostColonyIteration += new EventHandler(antClustering_OnPostColonyIteration);
            }

            ClusterBMN cBMN = new ClusterBMN(dataset, clusterNumber, similarityMeasure, accuracy, algorithm, naive);            
            return cBMN.CreateClassifier() as BayesianClusterMultinetClassifier;
        }

        public static BayesianClusterMultinetClassifier CreateAntClustBMNClassifier_IB(int seed, Dataset dataset, int maxIterations, int colonySize, int convergence, int clustersNumber, ISimilarityMeasure similarityMeasure, IClassificationQualityMeasure classificationMeasure, IClassificationAlgorithm algorithm, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);
            DefaultHeuristicCalculator<ClusterExampleAssignment> calculator = new DefaultHeuristicCalculator<ClusterExampleAssignment>();
            ClusteringIBInvalidator invalidator = new ClusteringIBInvalidator();
            ClusteringClassificationQualityEvaluator evaluator = new ClusteringClassificationQualityEvaluator(classificationMeasure, algorithm);
            evaluator.Dataset = dataset;
            KMeansLocalSearch localSearch = new KMeansLocalSearch(dataset, 1,similarityMeasure,evaluator);
            Problem<DataMining.Model.ClusterExampleAssignment> problem = new Problem<DataMining.Model.ClusterExampleAssignment>(invalidator, calculator, evaluator, localSearch);

            AntClustBMN_IB antClustBMN = new AntClustBMN_IB(maxIterations, colonySize, convergence, problem, clustersNumber, similarityMeasure, dataset,true, algorithm, classificationMeasure);
            antClustBMN.OnPostColonyIteration += new EventHandler(antClustering_OnPostColonyIteration);
            return antClustBMN.CreateClassifier() as BayesianClusterMultinetClassifier;

        }

        public static BayesianClusterMultinetClassifier CreateAntClustBMNClassifier_MB(int seed, Dataset dataset, int maxIterations, int colonySize, int convergence, int clustersNumber, ISimilarityMeasure similarityMeasure, IClassificationQualityMeasure classificationMeasure, IClassificationAlgorithm algorithm, bool fireEvents)
        {
            DataMining.Utilities.RandomUtility.Initialize(seed);
            DefaultHeuristicCalculator<int> calculator = new DefaultHeuristicCalculator<int>();
            ClusteringMBInvalidator invalidator = new ClusteringMBInvalidator();
            ClusteringClassificationQualityEvaluator evaluator = new ClusteringClassificationQualityEvaluator(classificationMeasure, algorithm);
            evaluator.Dataset = dataset;
            KMeansLocalSearch localSearch = new KMeansLocalSearch(dataset, 1, similarityMeasure, evaluator);
            Problem<int> problem = new Problem<int>(invalidator, calculator, evaluator, localSearch);

            AntClustBMN_MB antClustBMN = new AntClustBMN_MB(maxIterations, colonySize, convergence, problem, clustersNumber, similarityMeasure, dataset, true, algorithm, classificationMeasure);
            antClustBMN.OnPostColonyIteration += new EventHandler(antClustering_OnPostColonyIteration);
            return antClustBMN.CreateClassifier() as BayesianClusterMultinetClassifier;

        }


        #region Event Handlers

        static void antClustering_OnPostColonyIteration(object sender, EventArgs e)
        {
            if (sender is AntColony<DataMining.Model.ClusterExampleAssignment>)
            {
                AntColony<DataMining.Model.ClusterExampleAssignment> colony = sender as AntColony<DataMining.Model.ClusterExampleAssignment>;
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration [" + colony.CurrentIteration.ToString() + "]"+colony.ToString());
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration Best: " + Math.Round(colony.IterationBestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("Global Best: " + Math.Round(colony.BestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("------------------------");
            }
            else
            { 
                AntColony<int> colony = sender as AntColony<int>;
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration [" + colony.CurrentIteration.ToString() + "]" + colony.ToString());
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration Best: " + Math.Round(colony.IterationBestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("Global Best: " + Math.Round(colony.BestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("------------------------");
            }
            


        }

        static void antClustering_OnPostAntSolutionContruction(object sender, EventArgs e)
        {
            
        }
                
        static void hClassificatoinAlgorithm_onPostClassifierConstruction(object sender, EventArgs e)
        {
            DataMining.Algorithms.Hierarchical.ClassifierInfoEventArgs args = e as DataMining.Algorithms.Hierarchical.ClassifierInfoEventArgs;
            Console.WriteLine(((Node)sender).Name+":"+args.Info.Desc);
        }

        static void hClassificatoinAlgorithm_onPostNodeProcessing(object sender, EventArgs e)
        {
            Console.WriteLine(((Node)sender).Name + ":" + ((NodeCounterEventArgs)e).Counter.ToString());
        }
                
        public static double TestClassifier(DataMining.Model.IClassifier classifier, Dataset testingSet, DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityTester)
        {
           return qualityTester.CalculateMeasure(DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(classifier, testingSet));
           
        }

        public static double TestClassifier(DataMining.Model.Hierarchical.IHierarchicalClassifier classifier, Dataset testingSet, DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityTester)
        {
            return qualityTester.CalculateMeasure(DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(classifier, testingSet));

        }

        public static double TestClassifier(DataMining.Model.Hierarchical.IHierarchicalClassifier classifier, List<Dataset> testingSets, DataMining.ClassificationMeasures.IClassificationQualityMeasure qualityTester)
        {

            return qualityTester.CalculateMeasure(DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(classifier, testingSets));

        }
       
        private static void abclassifier_OnPostAntSolutionContruction(object sender, EventArgs e)
        {
            if (sender is Ant<Edge>)
            {
                Ant<Edge> ant = sender as Ant<Edge>;
                Console.WriteLine("[" + ant.Index.ToString() + "]: " + Math.Round(ant.Solution.Quality, 5).ToString());
            }
            if (sender is Ant<VariableTypeAssignment>)
            {

                Ant<VariableTypeAssignment> ant = sender as Ant<VariableTypeAssignment>;
                Console.WriteLine("[" + ant.Index.ToString() + "]: " + Math.Round(ant.Solution.Quality, 5).ToString());
            }
        }

        static void kmeans_OnPostIteration(object sender, EventArgs e)
        {
            
            KMeans kmeans = sender as KMeans;
            double quality = kmeans.ClusteringSolution.CalculateCohesion();
            Console.WriteLine("Iteration "+kmeans.CurrentIteration.ToString()+": "+quality.ToString());
            foreach (Cluster cluster in kmeans.ClusteringSolution.Clusters)
                Console.WriteLine(cluster.Size);
            Console.WriteLine(kmeans.ClusteringSolution.IsValid);
        }

        private static void abclassifier_OnPostColonyIteration(object sender, EventArgs e)
        {
            if (sender is AntColony<Edge>)
            {
                AntColony<Edge> colony = sender as AntColony<Edge>;
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration [" + colony.CurrentIteration.ToString() + "]");
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration Best: " + Math.Round(colony.IterationBestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("Global Best: " + Math.Round(colony.BestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("------------------------");
            }
            if (sender is AntColony<VariableTypeAssignment>)
            {
                AntColony<VariableTypeAssignment> colony = sender as AntColony<VariableTypeAssignment>;
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration [" + colony.CurrentIteration.ToString() + "]");
                Console.WriteLine("------------------------");
                Console.WriteLine("Iteration Best: " + Math.Round(colony.IterationBestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("Global Best: " + Math.Round(colony.BestAnt.Solution.Quality, 5).ToString());
                Console.WriteLine("------------------------");
            }
              



        }

        private static void GMNabclassifier_OnPostColonyIteration(object sender, EventArgs e)
        {
            ABCMinerGMN miner = sender as ABCMinerGMN ;
            Console.WriteLine("------------------------");
            Console.WriteLine("Iteration [" + miner.CurrentIteration.ToString() + "]");
            Console.WriteLine("------------------------");
            Console.WriteLine("Iteration Best: " + Math.Round(miner.IterationBestQuality, 5).ToString());
            Console.WriteLine("Global Best: " + Math.Round(miner.BestQuality, 5).ToString());
            Console.WriteLine("------------------------");

        }

        private static void hcblassifier_OnPostEvaluation(object sender, EventArgs e)
        {
            if (sender is GHC)
            {
                GHC classifier = sender as GHC;
                Console.WriteLine("Counter: " + classifier.EvaluationsCounter.ToString());
            }
            else
            {
                K2Algorithm classifier = sender as K2Algorithm;
                Console.WriteLine("Counter: " + classifier.EvaluationsCounter.ToString());
 
            }
        }

        static void hcblassifier_OnProgress(object sender, EventArgs e)
        {

            if (sender is GHC)
            {
                GHC classifier = sender as GHC;
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Quality: " + classifier.BestSolution.Quality);
                Console.WriteLine("-----------------------------------");
            }
            else
            {
                K2Algorithm classifier = sender as K2Algorithm;
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Quality: " + classifier.BestSolution.Quality);
                Console.WriteLine("-----------------------------------");

            }




        }


        #endregion


    }
}
