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

namespace iFourmi.Main
{
    public static class BatchTest
    {
        public static string DatasetFolderPath
        {
            get;
            set;
        }

        static int _folds = 1;
        static int _currentFold = 0;

        public static void RunACOIBL()
        {
            int k = 9;
            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();
            
            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    //Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    Dataset datasetFull = Dataset.Merge(trainingSet, testingSet);

                    double quality = 0;

                   try
                    {
                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNClassifier(k, datasetFull, false);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("KNN: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "KNN", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knnWV = SingleTest.CreateKNNClassifier(k, datasetFull, true);
                            quality = SingleTest.TestClassifier(knnWV, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("KNN-WV: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "KNN-WV", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");

                        }

                        {
                            NearestClassClassifier ncc = SingleTest.CreateNCClassifier(datasetFull);
                            quality = SingleTest.TestClassifier(ncc, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("NNC: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "NNC", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNAntIBMinerClassifier(k, datasetFull, false);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-KNN: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-KNN", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNAntIBMinerClassifier(k, datasetFull, true);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-KNN-WV: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-KNN-WV", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNAntIBMinerClassifier_ClassBasedWeights(k, datasetFull, false);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-KNN-CB: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-KNN-CB", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNAntIBMinerClassifier_ClassBasedWeights(k, datasetFull, true);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-KNN-CB-WV: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-KNN-CB-WV", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            NearestClassClassifier ncc = SingleTest.CreateNCCAntIBMinerClassifier(datasetFull);
                            quality = SingleTest.TestClassifier(ncc, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-NCC: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-NCC", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            NearestClassClassifier ncc = SingleTest.CreateNCCAntIBMinerClassifier_ClassBasedWeights(datasetFull);
                            quality = SingleTest.TestClassifier(ncc, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-NCC-CB: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-NCC-CB",  quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            GaussianKernelEstimator GKC = SingleTest.CreateGKAntIBMinerClassifier(datasetFull);
                            quality = SingleTest.TestClassifier(GKC, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-GKC: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-GKC", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            GaussianKernelEstimator GKC = SingleTest.CreateGKAntIBMinerClassifier_ClassBaseWeights(datasetFull);
                            quality = SingleTest.TestClassifier(GKC, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("ACO-GKC-CB: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "ACO-GKC-CB", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }
                    }
                    catch (Exception ex)
                    {
                      LogError(ex);
                      //  Console.WriteLine(ex.Message);
                    }



                }
            }
        }

        public static void RunPSOIBL()
        {
            int k = 9;
            AccuracyMeasure accuracyMeasure = new AccuracyMeasure();

            foreach (string dataset in GetDatasetFolds("datasets.txt"))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                {
                    //----------------------------------------
                    //Console.WriteLine("Fold:" + _currentFold.ToString());
                    //----------------------------------------

                    DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                    DataMining.Data.Dataset trainingSet = tables[0];
                    DataMining.Data.Dataset testingSet = tables[1];

                    Dataset datasetFull = Dataset.Merge(trainingSet, testingSet);

                    double quality = 0;

                    try
                    {
                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNClassifier(k, datasetFull, false);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("KNN: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "KNN", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knnWV = SingleTest.CreateKNNClassifier(k, datasetFull, true);
                            quality = SingleTest.TestClassifier(knnWV, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("KNN-WV: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "KNN-WV", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");

                        }

                        {
                            NearestClassClassifier ncc = SingleTest.CreateNCClassifier(datasetFull);
                            quality = SingleTest.TestClassifier(ncc, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("NNC: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "NNC", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNPSOIBMinerClassifier(k, datasetFull, false);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-KNN: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-KNN", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNPSOIBMinerClassifier(k, datasetFull, true);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-KNN-WV: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-KNN-WV", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNPSOIBMinerClassifier_ClassBasedWeights(k, datasetFull, false);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-KNN-CB: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-KNN-CB", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            KNearestNeighbours knn = SingleTest.CreateKNNPSOIBMinerClassifier_ClassBasedWeights(k, datasetFull, true);
                            quality = SingleTest.TestClassifier(knn, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-KNN-CB-WV: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-KNN-CB-WV", k.ToString(), quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            NearestClassClassifier ncc = SingleTest.CreateNCCPSOIBMinerClassifier(datasetFull);
                            quality = SingleTest.TestClassifier(ncc, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-NCC: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-NCC", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            NearestClassClassifier ncc = SingleTest.CreateNCCPSOIBMinerClassifier_ClassBasedWeights(datasetFull);
                            quality = SingleTest.TestClassifier(ncc, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-NCC-CB: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-NCC-CB", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            GaussianKernelEstimator GKC = SingleTest.CreateGKPSOIBMinerClassifier(datasetFull);
                            quality = SingleTest.TestClassifier(GKC, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-GKC: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-GKC", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }

                        {
                            GaussianKernelEstimator GKC = SingleTest.CreateGKPSOIBMinerClassifier_ClassBaseWeights(datasetFull);
                            quality = SingleTest.TestClassifier(GKC, datasetFull, accuracyMeasure);
                            quality = Math.Round(quality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine("PSO-GKC-CB: " + dataset + " - Accuracy=" + quality);
                            SaveResults(dataset, "PSO-GKC-CB", quality.ToString());
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine("-------------------------------------------");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        //  Console.WriteLine(ex.Message);
                    }



                }
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

    }
}
