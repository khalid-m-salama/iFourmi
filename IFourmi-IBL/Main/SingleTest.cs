using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ContinuousACO.Algorithms;
using iFourmi.ContinuousACO.ProblemSpecifics;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.ProximityMeasures;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model.IBLearning;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Model.Ensembles;

namespace iFourmi.Main
{
    public static class SingleTest
    {
        static int maxIterations = 10000;
        static int colonySize = 1;
        static int convergenceIterations = 100;
        static int archive = 25;
        static double q = 0.25;
        static double segma = 0.85;



        public static KNearestNeighbours CreateKNNClassifier(int k,Dataset trainingSet, bool useWeightedVote)
        {
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(k, distanceMeasure, trainingSet, useWeightedVote);
            return knn;

        }

        public static NearestClassClassifier CreateNCClassifier(Dataset trainingSet, double distanceThreshold)
        {
            DefaultDistanceMeasure distanceMeasure=new DefaultDistanceMeasure(2);
            NearestClassClassifier NCClassifier = new NearestClassClassifier(distanceMeasure, trainingSet, distanceThreshold);
            return NCClassifier;
        }

        public static GaussianKernelEstimator CreateGKClassifier(Dataset trainingSet, double kernelParameter)
        {
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator GKClassifier = new GaussianKernelEstimator(kernelParameter, distanceMeasure, trainingSet);
            return GKClassifier;
        }

        #region ACO

        public static KNearestNeighbours CreateKNNAntIBMinerClassifier(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount=trainingSet.Metadata.Target.Values.Length;
            int attributesCount=trainingSet.Metadata.Attributes.Length;
            
            int problemSize=attributesCount+1;

            AccuracyMeasure measure=new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator=new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn,measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem=new Problem<double>(null,null,evaluator,null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            KNearestNeighbours acoknn =  antminer.CreateClassifier() as KNearestNeighbours;
            return acoknn;
        }
       
        public static KNearestNeighbours CreateKNNAntIBMinerClassifier_ClassBasedWeights(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount*classCount)+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            KNearestNeighbours acoknn = antminer.CreateClassifier() as KNearestNeighbours;
            return acoknn;
        }

        public static NearestClassClassifier CreateNCCAntIBMinerClassifier(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            NearestClassClassifier aconcc = antminer.CreateClassifier() as NearestClassClassifier;
            return aconcc;
        }

        public static NearestClassClassifier CreateNCCAntIBMinerClassifier_ClassBasedWeights(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (classCount * attributesCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            NearestClassClassifier aconcc = antminer.CreateClassifier() as NearestClassClassifier;
            return aconcc;
        }

        public static GaussianKernelEstimator CreateGKAntIBMinerClassifier(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(1,distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            GaussianKernelEstimator acogke = antminer.CreateClassifier() as GaussianKernelEstimator;
            return acogke;
        }

        public static GaussianKernelEstimator CreateGKAntIBMinerClassifier_ClassBaseWeights(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount * classCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(1, distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            GaussianKernelEstimator acogke = antminer.CreateClassifier() as GaussianKernelEstimator;
            return acogke;
        }

        //-----------------------------------------------------------------------------------------------------------

        public static EnsembleClassifier CreateKNNAntIBMinerClassifier_Ensemble(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            EnsembleClassifier acoknn = antminer.CreateEnsembleClassifier();
            return acoknn;
        }

        public static EnsembleClassifier CreateKNNAntIBMinerClassifier_ClassBasedWeights_Ensemble(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount * classCount)+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            EnsembleClassifier acoknn = antminer.CreateEnsembleClassifier();
            return acoknn;
        }

        public static EnsembleClassifier CreateNCCAntIBMinerClassifier_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            EnsembleClassifier aconcc = antminer.CreateEnsembleClassifier();
            return aconcc;
        }

        public static EnsembleClassifier CreateNCCAntIBMinerClassifier_ClassBasedWeights_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (classCount * attributesCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            EnsembleClassifier aconcc = antminer.CreateEnsembleClassifier();
            return aconcc;
        }

        public static EnsembleClassifier CreateGKAntIBMinerClassifier_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(1, distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            EnsembleClassifier acogke = antminer.CreateEnsembleClassifier();
            return acogke;
        }

        public static EnsembleClassifier CreateGKAntIBMinerClassifier_ClassBaseWeights_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount * classCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(0.5, distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            AntIBMiner antminer = new AntIBMiner(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, q, segma, trainingSet);
            antminer.OnPostColonyIteration += OnPostColonyIteration;

            EnsembleClassifier acogke = antminer.CreateEnsembleClassifier();
            return acogke;
        }

        #endregion

        #region PSO

        public static KNearestNeighbours CreateKNNPSOIBMinerClassifier(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;


            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations,evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            KNearestNeighbours psoknn = psoIB.CreateClassifier() as KNearestNeighbours;
            return psoknn;
        }

        public static KNearestNeighbours CreateKNNPSOIBMinerClassifier_ClassBasedWeights(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount * classCount)+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            
            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations ,evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            KNearestNeighbours psoknn = psoIB.CreateClassifier() as KNearestNeighbours;
            return psoknn;
        }

        public static NearestClassClassifier CreateNCCPSOIBMinerClassifier(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            NearestClassClassifier psoncc = psoIB.CreateClassifier() as NearestClassClassifier;
            return psoncc;
        }

        public static NearestClassClassifier CreateNCCPSOIBMinerClassifier_ClassBasedWeights(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (classCount * attributesCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            NearestClassClassifier psoncc = psoIB.CreateClassifier() as NearestClassClassifier;
            return psoncc;
        }

        public static GaussianKernelEstimator CreateGKPSOIBMinerClassifier(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(1, distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            GaussianKernelEstimator psogke = psoIB.CreateClassifier() as GaussianKernelEstimator;
            return psogke;
        }

        public static GaussianKernelEstimator CreateGKPSOIBMinerClassifier_ClassBaseWeights(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount * classCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(1, distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            GaussianKernelEstimator psogke = psoIB.CreateClassifier() as GaussianKernelEstimator;
            return psogke;
        }


        //--------------------------------------------------------


        public static EnsembleClassifier CreateKNNPSOIBMinerClassifier_Ensemble(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;


            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            EnsembleClassifier psoknn = psoIB.CreateEnsembleClassifier();
            return psoknn;
        }

        public static EnsembleClassifier CreateKNNPSOIBMinerClassifier_ClassBasedWeights_Ensemble(Dataset trainingSet, bool useWeightedVote)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount * classCount)+1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            KNearestNeighbours knn = new KNearestNeighbours(distanceMeasure, trainingSet, useWeightedVote);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(knn, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            EnsembleClassifier psoknn = psoIB.CreateEnsembleClassifier();
            return psoknn;
        }

        public static EnsembleClassifier CreateNCCPSOIBMinerClassifier_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            EnsembleClassifier psoncc = psoIB.CreateEnsembleClassifier();
            return psoncc;
        }

        public static EnsembleClassifier CreateNCCPSOIBMinerClassifier_ClassBasedWeights_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (classCount * attributesCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(2);
            NearestClassClassifier ncc = new NearestClassClassifier(distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(ncc, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;
            Problem<double> problem = new Problem<double>(null, null, evaluator, null);

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            EnsembleClassifier psoncc = psoIB.CreateEnsembleClassifier();
            return psoncc;
        }

        public static EnsembleClassifier CreateGKPSOIBMinerClassifier_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = attributesCount + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(1, distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            EnsembleClassifier psogke = psoIB.CreateEnsembleClassifier();
            return psogke;
        }

        public static EnsembleClassifier CreateGKPSOIBMinerClassifier_ClassBaseWeights_Ensemble(Dataset trainingSet)
        {
            int classCount = trainingSet.Metadata.Target.Values.Length;
            int attributesCount = trainingSet.Metadata.Attributes.Length;

            int problemSize = (attributesCount * classCount) + 1;

            AccuracyMeasure measure = new AccuracyMeasure();
            DefaultDistanceMeasure distanceMeasure = new DefaultDistanceMeasure(1);
            GaussianKernelEstimator gke = new GaussianKernelEstimator(1, distanceMeasure, trainingSet);
            IBClassificationQualityEvaluator evaluator = new ContinuousACO.ProblemSpecifics.IBClassificationQualityEvaluator(gke, measure);
            evaluator.LearningSet = trainingSet;
            evaluator.ValidationSet = trainingSet;

            PSOIB psoIB = new PSOIB(problemSize, archive, maxIterations / archive, convergenceIterations, evaluator);
            psoIB.OnPostSwarmIteration += OnPostColonyIteration;

            EnsembleClassifier psogke = psoIB.CreateEnsembleClassifier();
            return psogke;
        }


        #endregion

        public static double TestClassifier(IClassifier classifier, Dataset testingSet, IClassificationMeasure measure)
        {
            return measure.CalculateMeasure(classifier, testingSet);

        }

        static void OnPostColonyIteration(object sender, EventArgs e)
        {
            if (sender is AntIBMiner)
            {
                AntIBMiner colony = sender as AntIBMiner;
                Console.WriteLine("Iteration [" + colony.CurrentIteration.ToString() + "] - Fitness: " + Math.Round(colony.GlobalBest.Quality*100, 4).ToString());
            }
            else
            {
                PSOIB swarm = sender as PSOIB;
                Console.WriteLine("Iteration [" + swarm.CurrentIteration.ToString() + "] - Fitness: " + Math.Round(swarm.GlobalBest.Quality*100, 4).ToString());
            }
        }



        public static IDistanceMeasure distanceMeasure { get; set; }
    }
}
