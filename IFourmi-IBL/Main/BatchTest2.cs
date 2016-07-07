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
    public static class BatchTest2
    {
        public static string DatasetFolderPath
        { get; set; }

        public static string DatasetNamesFile 
        { get; set; }

        static int _folds = 10;
        static int _currentFold = 0;

        public static void RunConventional()
        {

            
            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                //try
                {
                    double quality1 = 0;
                    double quality2 = 0;
                    double quality3 = 0;

                    double quality4 = 0;
                    double quality5 = 0;
                    double quality6 = 0;

                    double quality7 = 0;
                    double quality8 = 0;
                    double quality9 = 0;


                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        //Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];



                        KNearestNeighbours knn1 = SingleTest.CreateKNNClassifier(1, trainingSet, false);
                        quality1 += SingleTest.TestClassifier(knn1, testingSet, accuracyMeasure);
                        //------------------------------------------------------------------

                        KNearestNeighbours knn11 = SingleTest.CreateKNNClassifier(11, trainingSet, false);
                        quality2 += SingleTest.TestClassifier(knn11, testingSet, accuracyMeasure);
                        //------------------------------------------------------------------

                        KNearestNeighbours knn21 = SingleTest.CreateKNNClassifier(21, trainingSet, false);
                        quality3 += SingleTest.TestClassifier(knn21, testingSet, accuracyMeasure);
                        //------------------------------------------------------------------

                        //------------------------------------------------------------------
                        //------------------------------------------------------------------

                        NearestClassClassifier ncc0 = SingleTest.CreateNCClassifier(trainingSet, 0);
                        quality4 += SingleTest.TestClassifier(ncc0, testingSet, accuracyMeasure);
                        //------------------------------------------------------------------

                        NearestClassClassifier ncc5 = SingleTest.CreateNCClassifier(trainingSet, 0.5);
                        quality5 += SingleTest.TestClassifier(ncc5, testingSet, accuracyMeasure);
                        //------------------------------------------------------------------

                        NearestClassClassifier ncc1 = SingleTest.CreateNCClassifier(trainingSet, 0.9);
                        quality6 += SingleTest.TestClassifier(ncc1, testingSet, accuracyMeasure);
                        ////------------------------------------------------------------------

                        ////------------------------------------------------------------------
                        ////------------------------------------------------------------------

                        GaussianKernelEstimator gcc0 = SingleTest.CreateGKClassifier(trainingSet, 0);
                        quality7 += SingleTest.TestClassifier(gcc0, testingSet, accuracyMeasure);
                        //------------------------------------------------------------------

                        GaussianKernelEstimator gcc5 = SingleTest.CreateGKClassifier(trainingSet, 0.25);
                        quality8 += SingleTest.TestClassifier(gcc5, testingSet, accuracyMeasure);
                        //------------------------------------------------------------------

                        GaussianKernelEstimator gcc1 = SingleTest.CreateGKClassifier(trainingSet, 0.5);
                        quality9 += SingleTest.TestClassifier(gcc1, testingSet, accuracyMeasure);

                    }

                    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                    quality2 = Math.Round((quality2 / _folds) * 100, 2);
                    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                    quality4 = Math.Round((quality4 / _folds) * 100, 2);
                    quality5 = Math.Round((quality5 / _folds) * 100, 2);
                    quality6 = Math.Round((quality6 / _folds) * 100, 2);

                    quality7 = Math.Round((quality7 / _folds) * 100, 2);
                    quality8 = Math.Round((quality8 / _folds) * 100, 2);
                    quality9 = Math.Round((quality9 / _folds) * 100, 2);

                    Console.WriteLine("1NN: " + dataset + " - Accuracy=" + quality1);
                    SaveResults(dataset, "1NN", quality1.ToString());

                    Console.WriteLine("11NN: " + dataset + " - Accuracy=" + quality2);
                    SaveResults(dataset, "11NN", quality2.ToString());

                    Console.WriteLine("21NN: " + dataset + " - Accuracy=" + quality3);
                    SaveResults(dataset, "21NN", quality3.ToString());


                    Console.WriteLine("NCC-0: " + dataset + " - Accuracy=" + quality4);
                    SaveResults(dataset, "NCC-0", quality4.ToString());

                    Console.WriteLine("NCC-0.5: " + dataset + " - Accuracy=" + quality5);
                    SaveResults(dataset, "NCC-0.5", quality5.ToString());

                    Console.WriteLine("NCC-1: " + dataset + " - Accuracy=" + quality6);
                    SaveResults(dataset, "NCC-1", quality6.ToString());


                    Console.WriteLine("GKE-0: " + dataset + " - Accuracy=" + quality7);
                    SaveResults(dataset, "GKE-0", quality7.ToString());

                    Console.WriteLine("GKE-0.25: " + dataset + " - Accuracy=" + quality8);
                    SaveResults(dataset, "GKE-0.25", quality8.ToString());

                    Console.WriteLine("GKE-0.5: " + dataset + " - Accuracy=" + quality9);
                    SaveResults(dataset, "GKE-0.5", quality9.ToString());

                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");


                }
                //catch (Exception ex)
                {
                    //LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }
            }

        }

        public static void RunACOIBL()
        {
            
            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();
            IEnsembleClassificationStrategy majorityVote = new MajorityVoteStrategy();
            IEnsembleClassificationStrategy weightedVote = new WeightedVoteStrategy();

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                #region ACO-KNN-CB

                try
                {
                    double quality1 = 0;
                    //double quality2 = 0;
                    double quality3 = 0;


                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        EnsembleClassifier ensemble = SingleTest.CreateKNNAntIBMinerClassifier_Ensemble(trainingSet, false);

                        double trQuality = SingleTest.TestClassifier(ensemble[0], trainingSet, accuracyMeasure);
                        double tsQuality = SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                        Console.WriteLine(trQuality);
                        Console.WriteLine(tsQuality);
                        
                        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                        //ensemble.Stratgy = majorityVote;
                        //quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                        ensemble.Stratgy = weightedVote;
                        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                    }

                    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                    //quality2 = Math.Round((quality2 / _folds) * 100, 2);
                    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                    //------------------------------------------------------------------
                    Console.WriteLine("ACO-KNN-CB: " + dataset + " - Accuracy=" + quality1);
                    SaveResults(dataset, "ACO-KNN-CB", quality1.ToString());

                   // Console.WriteLine("ACO-KNN-CB-ens-MV: " + dataset + " - Accuracy=" + quality2);
                   // SaveResults(dataset, "ACO-KNN-CB-ens-MV",  quality2.ToString());

                    Console.WriteLine("ACO-KNN-CB-ens-WV: " + dataset + " - Accuracy=" + quality3);
                    SaveResults(dataset, "ACO-KNN-CB-ens-WV",  quality3.ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }

                catch (Exception ex)
                {
                    LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }

                #endregion

                return;

                #region ACO-KNN-CB-WV

                //try
                //{
                //    double quality1 = 0;
                //    double quality2 = 0;
                //    double quality3 = 0;


                //    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                //    {
                //        //----------------------------------------
                //        Console.WriteLine("Fold:" + _currentFold.ToString());
                //        //----------------------------------------

                //        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                //        DataMining.Data.Dataset trainingSet = tables[0];
                //        DataMining.Data.Dataset testingSet = tables[1];

                //        EnsembleClassifier ensemble = SingleTest.CreateKNNAntIBMinerClassifier_ClassBasedWeights_Ensemble(trainingSet, true);

                //        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                //        ensemble.Stratgy = majorityVote;
                //        quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                //        ensemble.Stratgy = weightedVote;
                //        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                //    }

                //    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                //    quality2 = Math.Round((quality2 / _folds) * 100, 2);
                //    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                //    //------------------------------------------------------------------
                //    Console.WriteLine("ACO-KNN-CB-WV: " + dataset + " - Accuracy=" + quality1);
                //    SaveResults(dataset, "ACO-KNN-CB-WV", k.ToString(), quality1.ToString());

                //    Console.WriteLine("ACO-KNN-CB-WV-ens-MV: " + dataset + " - Accuracy=" + quality2);
                //    SaveResults(dataset, "ACO-KNN-CB-WV-ens-MV", k.ToString(), quality2.ToString());

                //    Console.WriteLine("ACO-KNN-CB-WV-ens-WV: " + dataset + " - Accuracy=" + quality3);
                //    SaveResults(dataset, "ACO-KNN-CB-WV-ens-WV", k.ToString(), quality3.ToString());

                //    Console.WriteLine("-------------------------------------------");
                //    Console.WriteLine("-------------------------------------------");
                //    Console.WriteLine("-------------------------------------------");

                //}

                //catch (Exception ex)
                //{
                //    LogError(ex);
                //    //  Console.WriteLine(ex.Message);
                //}

                #endregion

                #region ACO-NCC-CB

                try
                {
                    double quality1 = 0;
                    //double quality2 = 0;
                    double quality3 = 0;


                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        EnsembleClassifier ensemble = SingleTest.CreateNCCAntIBMinerClassifier_ClassBasedWeights_Ensemble(trainingSet);

                        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                        //ensemble.Stratgy = majorityVote;
                        //quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                        ensemble.Stratgy = weightedVote;
                        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                    }

                    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                    //quality2 = Math.Round((quality2 / _folds) * 100, 2);
                    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                    //------------------------------------------------------------------
                    Console.WriteLine("ACO-NCC-CB: " + dataset + " - Accuracy=" + quality1);
                    SaveResults(dataset, "ACO-NNC-CB", quality1.ToString());

                    //Console.WriteLine("ACO-NNC-CB-ens-MV: " + dataset + " - Accuracy=" + quality2);
                    //SaveResults(dataset, "ACO-NNC-CB-ens-MV",  quality2.ToString());

                    Console.WriteLine("ACO-NNC-CB-ens-WV: " + dataset + " - Accuracy=" + quality3);
                    SaveResults(dataset, "ACO-NNC-CB-ens-WV",  quality3.ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }

                catch (Exception ex)
                {
                    LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }

                #endregion

                #region ACO-GKC-CB

                try
                {
                    double quality1 = 0;
                    //double quality2 = 0;
                    double quality3 = 0;


                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        EnsembleClassifier ensemble = SingleTest.CreateGKAntIBMinerClassifier_ClassBaseWeights_Ensemble(trainingSet);

                        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                        //ensemble.Stratgy = majorityVote;
                        //quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                        ensemble.Stratgy = weightedVote;
                        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                    }

                    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                    //quality2 = Math.Round((quality2 / _folds) * 100, 2);
                    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                    //------------------------------------------------------------------
                    Console.WriteLine("ACO-GKC-CB-CB: " + dataset + " - Accuracy=" + quality1);
                    SaveResults(dataset, "ACO-GKC-CB", quality1.ToString());

                    //Console.WriteLine("ACO-GKC-CB-ens-MV: " + dataset + " - Accuracy=" + quality2);
                    //SaveResults(dataset, "ACO-GKC-CB-ens-MV", quality2.ToString());

                    Console.WriteLine("ACO-GKC-CB-ens-WV: " + dataset + " - Accuracy=" + quality3);
                    SaveResults(dataset, "ACO-GKC-CB-ens-WV",  quality3.ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }

                catch (Exception ex)
                {
                    LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }

                #endregion


            }
        }

        public static void RunACOIBL_WeightOutputs()
        {

            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();
  

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                #region ACO-KNN-CB

                //try
                {
                    

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];



                        KNearestNeighbours knnclassifier = SingleTest.CreateKNNAntIBMinerClassifier_ClassBasedWeights(trainingSet, false);
                        //------------------------------------------------------------------
                        Console.WriteLine("ACO-KNN-CB: " + dataset);
                        SaveWeights(trainingSet, knnclassifier);


                    }

         

                   

                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }

                //catch (Exception ex)
                {
                    //LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }

                #endregion

            }
        }

        public static void RunPSOIBL()
        {
            
            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();
            IEnsembleClassificationStrategy majorityVote = new MajorityVoteStrategy();
            IEnsembleClassificationStrategy weightedVote = new WeightedVoteStrategy();

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                #region PSO-KNN-CB

                try
                {
                    double quality1 = 0;
                    //double quality2 = 0;
                    double quality3 = 0;


                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        EnsembleClassifier ensemble = SingleTest.CreateKNNPSOIBMinerClassifier_ClassBasedWeights_Ensemble(trainingSet, false);

                        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                        ensemble.Stratgy = majorityVote;
                        //quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                        ensemble.Stratgy = weightedVote;
                        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                    }

                    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                   // quality2 = Math.Round((quality2 / _folds) * 100, 2);
                    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                    //------------------------------------------------------------------
                    Console.WriteLine("PSO-KNN-CB: " + dataset + " - Accuracy=" + quality1);
                    SaveResults(dataset, "PSO-KNN-CB",  quality1.ToString());

                    //Console.WriteLine("PSO-KNN-CB-ens-MV: " + dataset + " - Accuracy=" + quality2);
                    //SaveResults(dataset, "PSO-KNN-CB-ens-MV",quality2.ToString());

                    Console.WriteLine("PSO-KNN-CB-ens-WV: " + dataset + " - Accuracy=" + quality3);
                    SaveResults(dataset, "PSO-KNN-CB-ens-WV",  quality3.ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }

                catch (Exception ex)
                {
                    //   LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }

                #endregion

                #region PSO-KNN-CB-WV

                //try
                //{
                //    double quality1 = 0;
                //    double quality2 = 0;
                //    double quality3 = 0;


                //    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                //    {
                //        //----------------------------------------
                //        Console.WriteLine("Fold:" + _currentFold.ToString());
                //        //----------------------------------------

                //        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                //        DataMining.Data.Dataset trainingSet = tables[0];
                //        DataMining.Data.Dataset testingSet = tables[1];

                //        EnsembleClassifier ensemble = SingleTest.CreateKNNPSOIBMinerClassifier_ClassBasedWeights_Ensemble(trainingSet, true);

                //        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                //        ensemble.Stratgy = majorityVote;
                //        quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                //        ensemble.Stratgy = weightedVote;
                //        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                //    }

                //    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                //    quality2 = Math.Round((quality2 / _folds) * 100, 2);
                //    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                //    //------------------------------------------------------------------
                //    Console.WriteLine("PSO-KNN-CB-WV: " + dataset + " - Accuracy=" + quality1);
                //    SaveResults(dataset, "PSO-KNN-CB-WV", k.ToString(), quality1.ToString());

                //    Console.WriteLine("PSO-KNN-CB-WV-ens-MV: " + dataset + " - Accuracy=" + quality2);
                //    SaveResults(dataset, "PSO-KNN-CB-WV-ens-MV", k.ToString(), quality2.ToString());

                //    Console.WriteLine("PSO-KNN-CB-WV-ens-WV: " + dataset + " - Accuracy=" + quality3);
                //    SaveResults(dataset, "PSO-KNN-CB-WV-ens-WV", k.ToString(), quality3.ToString());

                //    Console.WriteLine("-------------------------------------------");
                //    Console.WriteLine("-------------------------------------------");
                //    Console.WriteLine("-------------------------------------------");

                //}

                //catch (Exception ex)
                //{
                //    LogError(ex);
                //    //  Console.WriteLine(ex.Message);
                //}

                #endregion

                #region PSO-NCC-CB

                try
                {
                    double quality1 = 0;
                    //double quality2 = 0;
                    double quality3 = 0;


                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        EnsembleClassifier ensemble = SingleTest.CreateNCCPSOIBMinerClassifier_ClassBasedWeights_Ensemble(trainingSet);

                        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                        //ensemble.Stratgy = majorityVote;
                        //quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                        ensemble.Stratgy = weightedVote;
                        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                    }

                    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                    //quality2 = Math.Round((quality2 / _folds) * 100, 2);
                    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                    //------------------------------------------------------------------
                    Console.WriteLine("PSO-NCC-CB: " + dataset + " - Accuracy=" + quality1);
                    SaveResults(dataset, "PSO-NNC-CB",  quality1.ToString());

                    //Console.WriteLine("PSO-NNC-CB-ens-MV: " + dataset + " - Accuracy=" + quality2);
                    //SaveResults(dataset, "PSO-NNC-CB-ens-MV",  quality2.ToString());

                    Console.WriteLine("PSO-NNC-CB-ens-WV: " + dataset + " - Accuracy=" + quality3);
                    SaveResults(dataset, "PSO-NNC-CB-ens-WV",  quality3.ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }

                catch (Exception ex)
                {
                    LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }

                #endregion

                #region PSO-GKC-CB

                try
                {
                    double quality1 = 0;
                    //double quality2 = 0;
                    double quality3 = 0;


                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        EnsembleClassifier ensemble = SingleTest.CreateGKPSOIBMinerClassifier_ClassBaseWeights_Ensemble(trainingSet);

                        quality1 += SingleTest.TestClassifier(ensemble[0], testingSet, accuracyMeasure);

                        //ensemble.Stratgy = majorityVote;
                        //quality2 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);

                        ensemble.Stratgy = weightedVote;
                        quality3 += SingleTest.TestClassifier(ensemble, testingSet, accuracyMeasure);



                    }

                    quality1 = Math.Round((quality1 / _folds) * 100, 2);
                    //quality2 = Math.Round((quality2 / _folds) * 100, 2);
                    quality3 = Math.Round((quality3 / _folds) * 100, 2);

                    //------------------------------------------------------------------
                    Console.WriteLine("PSO-GKC-CB-CB: " + dataset + " - Accuracy=" + quality1);
                    SaveResults(dataset, "PSO-GKC-CB",  quality1.ToString());

                    //Console.WriteLine("PSO-GKC-CB-ens-MV: " + dataset + " - Accuracy=" + quality2);
                    //SaveResults(dataset, "PSO-GKC-CB-ens-MV",  quality2.ToString());

                    Console.WriteLine("PSO-GKC-CB-ens-WV: " + dataset + " - Accuracy=" + quality3);
                    SaveResults(dataset, "PSO-GKC-CB-ens-WV",  quality3.ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }

                catch (Exception ex)
                {
                    LogError(ex);
                    //  Console.WriteLine(ex.Message);
                }

                #endregion
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

        private static void SaveWeights(Dataset dataset,AbstractLazyClassifier lazyClassifier)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter("results\\" + dataset.Metadata.DatasetName + ".txt", true);
            string header = "att\\class,";
            foreach (string classlabel in dataset.Metadata.Target.Values)
                header += classlabel + ",";
            header = header.Substring(0, header.Length - 1);
            writer.WriteLine(header);

            for (int classIndex = 0; classIndex < dataset.Metadata.Target.Values.Length; classIndex++)
            {
                double sum = 0;
                for (int attributeIndex = 0; attributeIndex < dataset.Metadata.Attributes.Length; attributeIndex++)
                    sum += lazyClassifier.Weights[classIndex][attributeIndex];

                for (int attributeIndex = 0; attributeIndex < dataset.Metadata.Attributes.Length; attributeIndex++)
                    lazyClassifier.Weights[classIndex][attributeIndex]/=sum;                
            }



            for (int attributeIndex = 0; attributeIndex < dataset.Metadata.Attributes.Length; attributeIndex++)
            {
                string line = dataset.Metadata.Attributes[attributeIndex].Name + ",";
                double[] weights = new double[dataset.Metadata.Target.Values.Length];
                
                for (int classIndex = 0; classIndex < dataset.Metadata.Target.Values.Length; classIndex++)
                    weights[classIndex] = Math.Round((lazyClassifier.Weights[classIndex][attributeIndex]) * 100, 1);

                line += string.Join(",", weights);

                writer.WriteLine(line);
               
            }

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


      
    }
}
