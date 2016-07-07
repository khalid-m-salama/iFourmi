using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.ProximityMeasures;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model.IBLearning;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Model.Ensembles;
using iFourmi.ACOMiner;
using iFourmi.ACOMiner.HeuristicCalculators;

using weka.classifiers.evaluation;
using weka.core.converters;
using weka.filters;
using weka.filters.unsupervised.attribute;
using weka.core;
using weka.classifiers;

namespace iFourmi.Main
{
    public static class SingleTest
    {

        static int maxIterations = 1;
        static int colonySize = 1;
        static int convergenceIterations = 10;
        
        public static double EvaluateWekaClassifier(string algorithm, string trainingSetPath, string testSetPath)
        {
            double quality = 0;

            Classifier classifier = WekaNETBridge.WekaClassification.GetWekaClassifier(algorithm, trainingSetPath);
            WekaNETBridge.WekaClassifier wekaClassifier = new WekaNETBridge.WekaClassifier();
            wekaClassifier.Classifier = classifier;


            quality = WekaNETBridge.WekaClassification.EvaluateClassifier(wekaClassifier, testSetPath);

            return quality;

        }

        public static ResultObject EvaluateRandDR_WekaClassifier(string algorithm, string trainingSetPath, string testSetPath, Dataset trainingSet, bool useAttributes, bool useInstances)
        {
            Classifier classifier = WekaNETBridge.WekaClassification.GetWekaClassifier(algorithm, trainingSetPath);

            //DefaultDRHeuristicCalculator calculator = new DefaultDRHeuristicCalculator();

            DefaultHeuristicCalculator<DRComponent> calculator = new DefaultHeuristicCalculator<DRComponent>(trainingSet);
            DRComponentInvalidator invalidator = new DRComponentInvalidator();


            WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);

            WekaClassificationQualityEvaluator evaluator = new WekaClassificationQualityEvaluator(wekaClassification,true);

            DRLocalSearch localSearch = new DRLocalSearch(evaluator);

            Problem<DRComponent> problem = new Problem<DRComponent>(invalidator, calculator, evaluator, localSearch);

            RandomDR random = new RandomDR(maxIterations, colonySize, 50, problem, useAttributes, useInstances,false, trainingSet);
            random.OnPostAntSolutionContruction += OnPostAntSolutionContruction;
            random.OnPostColonyIteration += OnPostColonyIteration;



            WekaNETBridge.WekaClassifier wekcaClassifier = random.CreateWekaClassifier();

            double quality = WekaNETBridge.WekaClassification.EvaluateClassifier(wekcaClassifier, testSetPath);
            double attribueReduction = random.BestAnt.Solution.FeatureCount() / (double)trainingSet.Metadata.Attributes.Length;
            double instanceReduction = random.BestAnt.Solution.InstanceCount() / (double)trainingSet.Size;

            ResultObject result = new ResultObject() { Quality = quality, AttributeReduction = attribueReduction, InstanceReduciton = instanceReduction };

            return result;



        }


        public static ResultObject EvaluateGreedyDR_WekaClassifier(string algorithm, string trainingSetPath, string testSetPath, Dataset trainingSet, bool useAttributes, bool useInstances)
        {
            Classifier classifier = WekaNETBridge.WekaClassification.GetWekaClassifier(algorithm, trainingSetPath);



            WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);

            WekaClassificationQualityEvaluator evaluator = new WekaClassificationQualityEvaluator(wekaClassification,true);

            DRLocalSearch localSearch = new DRLocalSearch(evaluator);

            Problem<DRComponent> problem = new Problem<DRComponent>(null, null, evaluator, localSearch);

            GreedyDR greedy = new GreedyDR(maxIterations, colonySize, convergenceIterations, problem, useAttributes, useInstances,  trainingSet);
            greedy.OnPostAntSolutionContruction += OnPostAntSolutionContruction;
            greedy.OnPostColonyIteration += OnPostColonyIteration;

            WekaNETBridge.WekaClassifier wekcaClassifier = greedy.CreateWekaClassifier();

            double quality = WekaNETBridge.WekaClassification.EvaluateClassifier(wekcaClassifier, testSetPath);
            double attribueReduction = greedy.BestAnt.Solution.FeatureCount() / (double)trainingSet.Metadata.Attributes.Length;
            double instanceReduction = greedy.BestAnt.Solution.InstanceCount() / (double)trainingSet.Size;

            ResultObject result = new ResultObject() { Quality = quality, AttributeReduction = attribueReduction, InstanceReduciton = instanceReduction };

            return result;



        }

        public static ResultObject EvaluateACOMinerDR_WekaClassifier(string algorithm, string trainingSetPath, string testSetPath, Dataset trainingSet, bool useAttributes, bool useInstances)
        {
            Classifier classifier = WekaNETBridge.WekaClassification.GetWekaClassifier(algorithm, trainingSetPath);

            //DefaultDRHeuristicCalculator calculator = new DefaultDRHeuristicCalculator();

            ADRHeuristicCalculator calculator = new ADRHeuristicCalculator();
            DRComponentInvalidator invalidator = new DRComponentInvalidator();


            WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);

            WekaClassificationQualityEvaluator evaluator = new WekaClassificationQualityEvaluator(wekaClassification,!useAttributes);

            DRLocalSearch localSearch = new DRLocalSearch(evaluator);

            Problem<DRComponent> problem = new Problem<DRComponent>(invalidator, calculator, evaluator, localSearch);

            ADRMiner acominer = new ADRMiner(maxIterations, colonySize, convergenceIterations, problem, useAttributes, useInstances, false, trainingSet);
            acominer.OnPostAntSolutionContruction += OnPostAntSolutionContruction;
            acominer.OnPostColonyIteration += OnPostColonyIteration;

            //acominer.Initialize();
            //acominer.Work();
            //weka.classifiers.Classifier final = evaluator.CreateClassifier(acominer.BestAnt.Solution);

            WekaNETBridge.WekaClassifier wekcaClassifier =acominer.CreateWekaClassifier();

            double quality = WekaNETBridge.WekaClassification.EvaluateClassifier(wekcaClassifier, testSetPath);
            double attribueReduction = acominer.BestAnt.Solution.FeatureCount() / (double)trainingSet.Metadata.Attributes.Length;
            double instanceReduction = acominer.BestAnt.Solution.InstanceCount() / (double)trainingSet.Size;

            ResultObject result = new ResultObject() { Quality = quality, AttributeReduction =attribueReduction, InstanceReduciton = instanceReduction };

            return result;



        }


        public static ResultObject EvaluateACOMinerDR2_WekaClassifier(string algorithm, string trainingSetPath, string testSetPath, Dataset trainingSet, bool attributeFirst)
        {
            Classifier classifier = WekaNETBridge.WekaClassification.GetWekaClassifier(algorithm, trainingSetPath);

            ADRHeuristicCalculator calculator = new ADRHeuristicCalculator();
            DRComponentInvalidator invalidator = new DRComponentInvalidator();


            WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);

            WekaClassificationQualityEvaluator evaluator = new WekaClassificationQualityEvaluator(wekaClassification,false);

            DRLocalSearch localSearch = new DRLocalSearch(evaluator);

            Problem<DRComponent> problem = new Problem<DRComponent>(invalidator, calculator, evaluator, localSearch);

            ADRMiner2 acominer = new ADRMiner2(maxIterations, colonySize, convergenceIterations, problem, attributeFirst, false, trainingSet);
            acominer.ACO1.OnPostAntSolutionContruction += OnPostAntSolutionContruction;
            acominer.ACO1.OnPostColonyIteration += OnPostColonyIteration;
            acominer.ACO2.OnPostAntSolutionContruction += OnPostAntSolutionContruction;
            acominer.ACO2.OnPostColonyIteration += OnPostColonyIteration;


            WekaNETBridge.WekaClassifier wekcaClassifier = acominer.CreateWekaClassifier();

            double quality = WekaNETBridge.WekaClassification.EvaluateClassifier(wekcaClassifier, testSetPath);

            double attribueReduction = acominer.BestSolution.AttributesToRemove().Length;          
            double instanceReduction = acominer.BestSolution.InstanceCount() / (double)trainingSet.Size;

            ResultObject result = new ResultObject() { Quality = quality, AttributeReduction = attribueReduction, InstanceReduciton = instanceReduction };

            return result;



        }


        public static List<ResultObject> EvaluateACOMinerDR_WekaClassifier_Multi(string algorithm, string trainingSetPath, string testSetPath, Dataset trainingSet, bool useAttributes, bool useInstances)
        {
            Classifier classifier = WekaNETBridge.WekaClassification.GetWekaClassifier(algorithm, trainingSetPath);

            DefaultDRHeuristicCalculator calculator = new DefaultDRHeuristicCalculator();
            DRComponentInvalidator invalidator = new DRComponentInvalidator();


            WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);

            WekaClassificationQualityEvaluator evaluator = new WekaClassificationQualityEvaluator(wekaClassification,!useAttributes);

            DRLocalSearch localSearch = new DRLocalSearch(evaluator);

            Problem<DRComponent> problem = new Problem<DRComponent>(invalidator, calculator, evaluator, localSearch);

            ADRMiner acominer = new ADRMiner(maxIterations, colonySize, convergenceIterations, problem, useAttributes, useInstances, false, trainingSet);
            acominer.OnPostAntSolutionContruction += OnPostAntSolutionContruction;
            acominer.OnPostColonyIteration += OnPostColonyIteration;

            acominer.Initialize();
            acominer.Work();
      

            List<ResultObject> results = new List<ResultObject>();

            foreach (string algo in WekaNETBridge.WekaClassification.GetWekaAlgorithmNames())
            {
                Classifier currentClassifier = WekaNETBridge.WekaClassification.GetWekaClassifier(algo, trainingSetPath);

                WekaNETBridge.WekaClassifier final = evaluator.CreateWekaClassifier(currentClassifier,acominer.BestAnt.Solution);

                double quality = WekaNETBridge.WekaClassification.EvaluateClassifier(final, testSetPath);
                double attribueReduction = acominer.BestAnt.Solution.FeatureCount() / (double)trainingSet.Metadata.Attributes.Length;
                double instanceReduction = acominer.BestAnt.Solution.InstanceCount() / (double)trainingSet.Size;

                ResultObject result = new ResultObject() { Quality = quality, AttributeReduction = attribueReduction, InstanceReduciton = instanceReduction };

                results.Add(result);
            }

            return results;



        }


        static void OnPostColonyIteration(object sender, EventArgs e)
        {
            AntColony<DRComponent> colony = sender as AntColony<DRComponent>;
            Console.WriteLine("------------------------");
            Console.WriteLine("Iteration [" + colony.CurrentIteration.ToString() + "]");
            Console.WriteLine("------------------------");
            Console.WriteLine("Iteration Best: " + Math.Round(colony.IterationBestAnt.Solution.Quality, 2).ToString());
            Console.WriteLine("Global Best: " + Math.Round(colony.BestAnt.Solution.Quality, 2).ToString());
            Console.WriteLine("------------------------");
        }

        static void OnPostAntSolutionContruction(object sender, EventArgs e)
        {
            Ant<DRComponent> ant = sender as Ant<DRComponent>;
            Console.WriteLine("[" + ant.Index.ToString() + "]: " + Math.Round(ant.Solution.Quality, 2).ToString());
        }

    }

    public class ResultObject
    {
        public double Quality;
        public double AttributeReduction;
        public double InstanceReduciton;

    }
}
