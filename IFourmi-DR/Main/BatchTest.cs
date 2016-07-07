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

namespace iFourmi.Main
{
    public static class BatchTest
    {
        public static string DatasetFolderPath
        {
            get;
            set;
        }
        public static string DatasetNamesFile
        {
            get;
            set;
        }
        static int _folds = 1;
        static int _currentFold = 0;

        public static void Run_WekaClassifier()
        {


            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------


                foreach (string algorithm in GetAlgorithms())
                {
                    double quality = 0;

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        double currentQuality = 0;
                        //----------------------------------------
                        //Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        string trFile = DatasetFolderPath + @"\" + dataset + @"\TR" + _currentFold.ToString() + "_" + dataset + ".arff";
                        string tsFile = DatasetFolderPath + @"\" + dataset + @"\TS" + _currentFold.ToString() + "_" + dataset + ".arff";


                        try
                        {

                            currentQuality = SingleTest.EvaluateWekaClassifier(algorithm, trFile, tsFile);
                            currentQuality = Math.Round(currentQuality * 100, 2);
                            //------------------------------------------------------------------
                            Console.WriteLine(algorithm + ": " + dataset + " - Accuracy = " + currentQuality);

                            quality += currentQuality;


                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                            //  Console.WriteLine(ex.Message);
                        }


                    }
                    //end folds loop

                    quality /= _folds;
                    SaveResults(dataset, algorithm, quality.ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                }
                //end folds loop
            }
            //end dataset loop
        }

        public static void Run_Random_WekaClassifier(bool useAttributes, bool useInstances)
        {

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                foreach (string algorithm in GetAlgorithms())
                {
                    ResultObject finalResult = new ResultObject();

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        //Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        string trFile = DatasetFolderPath + @"\" + dataset + @"\TR" + _currentFold.ToString() + "_" + dataset + ".arff";
                        string tsFile = DatasetFolderPath + @"\" + dataset + @"\TS" + _currentFold.ToString() + "_" + dataset + ".arff";


                        try
                        {


                            ResultObject currentResult = SingleTest.EvaluateRandDR_WekaClassifier(algorithm, trFile, tsFile, trainingSet, useAttributes, useInstances);


                            //------------------------------------------------------------------
                            Console.WriteLine(algorithm + ": " + dataset + " - Accuracy = " + Math.Round(currentResult.Quality * 100, 2).ToString());

                            finalResult.Quality += currentResult.Quality;
                            finalResult.AttributeReduction += currentResult.AttributeReduction;
                            finalResult.InstanceReduciton += currentResult.InstanceReduciton;


                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                            //  Console.WriteLine(ex.Message);
                        }


                    }
                    //end folds loop

                    finalResult.Quality /= _folds;
                    finalResult.AttributeReduction /= _folds;
                    finalResult.InstanceReduciton /= _folds;

                    SaveResults(dataset, "ACO_DR-" + algorithm, Math.Round(finalResult.Quality * 100, 2).ToString(), Math.Round(finalResult.AttributeReduction * 100, 2).ToString(), Math.Round(finalResult.InstanceReduciton * 100, 2).ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }
                //end algorithms loop
            }
            //end datasets loop
        }

        public static void Run_Greedy_WekaClassifier(bool useAttributes, bool useInstances)
        {

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                foreach (string algorithm in GetAlgorithms())
                {
                    ResultObject finalResult = new ResultObject();

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        //Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        string trFile = DatasetFolderPath + @"\" + dataset + @"\TR" + _currentFold.ToString() + "_" + dataset + ".arff";
                        string tsFile = DatasetFolderPath + @"\" + dataset + @"\TS" + _currentFold.ToString() + "_" + dataset + ".arff";


                       // try
                        {


                            ResultObject currentResult = SingleTest.EvaluateGreedyDR_WekaClassifier(algorithm, trFile, tsFile, trainingSet, useAttributes, useInstances);


                            //------------------------------------------------------------------
                            Console.WriteLine(algorithm + ": " + dataset + " - Accuracy = " + Math.Round(currentResult.Quality * 100, 2).ToString());

                            finalResult.Quality += currentResult.Quality;
                            finalResult.AttributeReduction += currentResult.AttributeReduction;
                            finalResult.InstanceReduciton += currentResult.InstanceReduciton;


                        }
                        //catch (Exception ex)
                        {
                         //   LogError(ex);
                            //  Console.WriteLine(ex.Message);
                        }


                    }
                    //end folds loop

                    finalResult.Quality /= _folds;
                    finalResult.AttributeReduction /= _folds;
                    finalResult.InstanceReduciton /= _folds;

                    SaveResults(dataset, "ACO_DR-" + algorithm, Math.Round(finalResult.Quality * 100, 2).ToString(), Math.Round(finalResult.AttributeReduction * 100, 2).ToString(), Math.Round(finalResult.InstanceReduciton * 100, 2).ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }
                //end algorithms loop
            }
            //end datasets loop
        }

        public static void Run_ACODR_WekaClassifier(bool useAttributes, bool useInstances)
        {

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                foreach (string algorithm in GetAlgorithms())
                {
                    ResultObject finalResult = new ResultObject();

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        //Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        string trFile = DatasetFolderPath + @"\" + dataset + @"\TR" + _currentFold.ToString() + "_" + dataset + ".arff";
                        string tsFile = DatasetFolderPath + @"\" + dataset + @"\TS" + _currentFold.ToString() + "_" + dataset + ".arff";


                      //  try
                        {


                            ResultObject currentResult = SingleTest.EvaluateACOMinerDR_WekaClassifier(algorithm, trFile, tsFile, trainingSet, useAttributes, useInstances);


                            //------------------------------------------------------------------
                            Console.WriteLine(algorithm + ": " + dataset + " - Accuracy = " + Math.Round(currentResult.Quality * 100, 2).ToString());
                            
                            finalResult.Quality += currentResult.Quality;
                            finalResult.AttributeReduction += currentResult.AttributeReduction;
                            finalResult.InstanceReduciton += currentResult.InstanceReduciton;


                        }
                       // catch (Exception ex)
                        {
                         //   LogError(ex);
                            //  Console.WriteLine(ex.Message);
                        }

                        
                    }
                    //end folds loop

                    finalResult.Quality /= _folds;
                    finalResult.AttributeReduction /= _folds;
                    finalResult.InstanceReduciton /= _folds;

                    SaveResults(dataset, "ACO_DR-" + algorithm, Math.Round(finalResult.Quality * 100, 2).ToString(), Math.Round(finalResult.AttributeReduction * 100, 2).ToString(), Math.Round(finalResult.InstanceReduciton * 100, 2).ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }
                //end algorithms loop
            }
            //end datasets loop
        }

        public static void Run_ACODR2_WekaClassifier(bool attributeFirst)
        {

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                foreach (string algorithm in GetAlgorithms())
                {
                    ResultObject finalResult = new ResultObject();

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        //Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        string trFile = DatasetFolderPath + @"\" + dataset + @"\TR" + _currentFold.ToString() + "_" + dataset + ".arff";
                        string tsFile = DatasetFolderPath + @"\" + dataset + @"\TS" + _currentFold.ToString() + "_" + dataset + ".arff";


                        try
                        {


                            ResultObject currentResult = SingleTest.EvaluateACOMinerDR2_WekaClassifier(algorithm, trFile, tsFile, trainingSet, attributeFirst);


                            //------------------------------------------------------------------
                            Console.WriteLine(algorithm + ": " + dataset + " - Accuracy = " + Math.Round(currentResult.Quality * 100, 2).ToString());

                            finalResult.Quality += currentResult.Quality;
                            finalResult.AttributeReduction += currentResult.AttributeReduction;
                            finalResult.InstanceReduciton += currentResult.InstanceReduciton;


                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                            //  Console.WriteLine(ex.Message);
                        }


                    }
                    //end folds loop

                    finalResult.Quality /= _folds;
                    finalResult.AttributeReduction /= _folds;
                    finalResult.InstanceReduciton /= _folds;

                    SaveResults(dataset, "ACO_DR-" + algorithm, Math.Round(finalResult.Quality * 100, 2).ToString(), finalResult.AttributeReduction.ToString(), Math.Round(finalResult.InstanceReduciton * 100, 2).ToString());
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }
                //end algorithms loop
            }
            //end datasets loop
        }

        public static void Run_ACODR_WekaClassifier_Mulit()
        {

            foreach (string dataset in GetDatasetFolds(DatasetNamesFile))
            {

                //----------------------------------------
                Console.WriteLine("Data Table:" + dataset);
                //----------------------------------------

                foreach (string algorithm in GetAlgorithms())
                {
                    string[] algos = WekaNETBridge.WekaClassification.GetWekaAlgorithmNames();
                    List<ResultObject> finalResults = new List<ResultObject>();

                    for (_currentFold = 0; _currentFold < _folds; _currentFold++)
                    {
                        //----------------------------------------
                        //Console.WriteLine("Fold:" + _currentFold.ToString());
                        //----------------------------------------

                        DataMining.Data.Dataset[] tables = LoadTrainingAndTestingData(dataset, _currentFold);
                        DataMining.Data.Dataset trainingSet = tables[0];
                        DataMining.Data.Dataset testingSet = tables[1];

                        string trFile = DatasetFolderPath + @"\" + dataset + @"\TR" + _currentFold.ToString() + "_" + dataset + ".arff";
                        string tsFile = DatasetFolderPath + @"\" + dataset + @"\TS" + _currentFold.ToString() + "_" + dataset + ".arff";


                        try
                        {
                            List<ResultObject> currentResults = SingleTest.EvaluateACOMinerDR_WekaClassifier_Multi(algorithm, trFile, tsFile, trainingSet, false, true);

                            for (int i = 0; i < currentResults.Count; i++)
                            {
                                //------------------------------------------------------------------
                                Console.WriteLine(algorithm + "-"+algos[i]+": " + dataset + " - Accuracy = " + Math.Round(currentResults[i].Quality * 100, 2).ToString());
                                if (finalResults.Count < i + 1)
                                    finalResults.Add(currentResults[i]);
                                else
                                {
                                    finalResults[i].Quality += currentResults[i].Quality;
                                    finalResults[i].AttributeReduction += currentResults[i].AttributeReduction;
                                    finalResults[i].InstanceReduciton += currentResults[i].InstanceReduciton;
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                            //  Console.WriteLine(ex.Message);
                        }


                    }
                    //end folds loop

                    for (int i = 0; i < finalResults.Count; i++)
                    {
                        finalResults[i].Quality /= _folds;
                        finalResults[i].AttributeReduction /= _folds;
                        finalResults[i].InstanceReduciton /= _folds;

                        SaveResults(dataset, "ACO_DR-" + algorithm+"-"+algos[i], Math.Round(finalResults[i].Quality * 100, 2).ToString(), Math.Round(finalResults[i].AttributeReduction * 100, 2).ToString(), Math.Round(finalResults[i].InstanceReduciton * 100, 2).ToString());
                    }
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("-------------------------------------------");

                }
                //end algorithms loop
            }
            //end datasets loop
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

        public static List<string> GetAlgorithms()
        {
            //return new List<string>() { "KNN", "NBayes", "JRip", "J48", "NeuralNets", "SVM" };

            return System.IO.File.ReadAllLines("algorithms.txt").ToList();
       
        }
    }
}
