using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class Program
    {
        static void Main(string[] args)
        {
            DataMining.Utilities.RandomUtility.Initialize(1);
            BatchTest2.DatasetFolderPath = GetFilePath();
            BatchTest2.DatasetNamesFile = "datasets.txt";
           //BatchTest2.RunConventional();
           // //BatchTest2.RunACOIBL_WeightOutputs();
           //BatchTest2.RunACOIBL();

           //ResultsParser.PopulateDataTable();

           Console.ReadLine();


         

        }

        public static void Test1()
        {
            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();
            IEnsembleClassificationStrategy majorityVote = new MajorityVoteStrategy();
            IEnsembleClassificationStrategy weightedVote = new WeightedVoteStrategy();


            DataMining.Data.Dataset[] tables = BatchTest2.LoadTrainingAndTestingData("audiology", 0);
            DataMining.Data.Dataset trainingSet = tables[0];
            DataMining.Data.Dataset testingSet = tables[1];

            

            //EnsembleClassifier ensemble = SingleTest.CreateGKAntIBMinerClassifier_ClassBaseWeights_Ensemble(trainingSet);
            EnsembleClassifier ensemble = SingleTest.CreateNCCAntIBMinerClassifier_ClassBasedWeights_Ensemble(trainingSet);
            //EnsembleClassifier ensemble = SingleTest.CreateKNNAntIBMinerClassifier_ClassBasedWeights_Ensemble(trainingSet,false);


            double quality1 = 0;
            double quality2 = 0;
            double quality3 = 0;


            quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

            ensemble.Stratgy = majorityVote;
            quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

            ensemble.Stratgy = weightedVote;
            quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);





            quality1 = Math.Round((quality1 / 1) * 100, 2);
            quality2 = Math.Round((quality2 / 1) * 100, 2);
            quality3 = Math.Round((quality3 / 1) * 100, 2);

            //------------------------------------------------------------------
            Console.WriteLine("ACO-GKC-CB-CB: Accuracy=" + quality1);

            Console.WriteLine("ACO-GKC-CB-ens-MV: Accuracy=" + quality2);

            Console.WriteLine("ACO-GKC-CB-ens-WV: Accuracy=" + quality3);

            //Console.WriteLine(((KNearestNeighbours)ensemble[0]).KNeighbours.ToString());

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("-------------------------------------------");

        }

        public static void Test2()
        {
            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();
            IEnsembleClassificationStrategy majorityVote = new MajorityVoteStrategy();
            IEnsembleClassificationStrategy weightedVote = new WeightedVoteStrategy();


            DataMining.Data.Dataset[] tables = BatchTest2.LoadTrainingAndTestingData("audiology", 0);
            DataMining.Data.Dataset trainingSet = tables[0];
            DataMining.Data.Dataset testingSet = tables[1];

            //EnsembleClassifier ensemble = SingleTest.CreateGKAntIBMinerClassifier_ClassBaseWeights_Ensemble(trainingSet);

            GaussianKernelEstimator GCE = new GaussianKernelEstimator(-0.5, new DefaultDistanceMeasure(1), trainingSet);




            double quality = 0;
            quality = SingleTest.TestClassifier(GCE, testingSet, accuracyMeasure);




            Console.WriteLine("Accuracy=" + quality);

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("-------------------------------------------");
 
        }

        private static string GetFilePath()
        {
            return System.IO.File.ReadAllLines("Config.txt")[0];
        }





    }
}
