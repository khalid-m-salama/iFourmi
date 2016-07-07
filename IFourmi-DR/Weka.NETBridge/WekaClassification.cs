using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weka.classifiers.evaluation;
using weka.core.converters;
using weka.filters;
using weka.filters.unsupervised.attribute;
using weka.core;
using weka.classifiers;

namespace iFourmi.WekaNETBridge
{
    public class WekaClassification
    {
        public static string[] GetWekaAlgorithmNames()
        {
            return new List<string>() { "KNN", "NBayes", "JRip", "J48", "SVM" }.ToArray();
        }

        public static Classifier GetWekaClassifier(string algorithm, string trainingSetPath)
        {
            Classifier classifier = null;

            switch (algorithm)
            {
                case "KNN":
                    {
                        classifier = new weka.classifiers.lazy.IB1();
                        WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);
                        classifier = wekaClassification.CreateClassifier();
                    }
                    break;

                case "NBayes":
                    {
                        classifier = new weka.classifiers.bayes.NaiveBayes();
                        WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);
                        classifier = wekaClassification.CreateClassifier();
                    }
                    break;

                case "JRip":
                    {
                        classifier = new weka.classifiers.rules.JRip();
                        WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);
                        classifier = wekaClassification.CreateClassifier();
                    }
                    break;

                case "J48":
                    {
                        classifier = new weka.classifiers.trees.J48();
                        WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);
                        classifier = wekaClassification.CreateClassifier();
                    }
                    break;

                case "NeuralNets":
                    {
                        classifier = new weka.classifiers.functions.MultilayerPerceptron();
                        WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);
                        classifier = wekaClassification.CreateClassifier();
                    }
                    break;

                case "SVM":
                    {
                        classifier = new weka.classifiers.functions.SMO();
                        WekaNETBridge.WekaClassification wekaClassification = new WekaNETBridge.WekaClassification(trainingSetPath, classifier);
                        classifier = wekaClassification.CreateClassifier();
                    }
                    break;
            }

            return classifier;
        }

        /// <summary>
        /// Original, unfiltered copy of the data set.
        /// </summary>
        public Instances OriginalDataset { get; set; }
        /// <summary>
        /// Data set after attribute and/or instance selection.
        /// </summary>
        public Classifier Classifier { get; set; }
        /// <summary>
        /// Initialize an instance of the evaluator with a training set and a Weka classifier.
        /// </summary>
        /// <param name="trainingSetPath">The path of the ARFF file to be used as the training set.</param>
        /// <param name="classifier">The Weka classifer to be used during evaluation.</param>
        public WekaClassification(string trainingSetPath, Classifier classifier)
        {
            //Load Training Set from file
            OriginalDataset = new ConverterUtils.DataSource(trainingSetPath).getDataSet();
            OriginalDataset.setClassIndex(OriginalDataset.numAttributes() - 1);
            //Assign classifier
            Classifier = classifier;

        }
        private WekaClassification()
        {
            throw new Exception("This operation is not allowed.");
        }
        /// <summary>
        /// Evaluates the quality of the classifier on a data set post reduction.
        /// </summary>
        /// <param name="attributesToRemove">The 0-based indices of the attributes to remove from the data set.</param>
        /// <param name="instancesToRemove">The 0-based indicies of the instances to remove from the data set.</param>
        /// <returns>Accuracy of the classifier post evaluation.</returns>
        public static Instances GetReducedDataset(Instances dataset, int[] attributesToRemove, int[] instancesToRemove)
        {
            
            #region Validation
            if (dataset == null)
            {
                throw new Exception("Class has not been properly initialized. Please reconstruct the class and try again.");
            }
            #endregion
            //build attributes filter 
            string attributesFilterString ="";
            if (attributesToRemove!=null && attributesToRemove.Length > 0)
            {
                attributesFilterString = "-R ";
                for (int i = 0; i < attributesToRemove.Length; i++)
                {
                    attributesFilterString += (attributesToRemove[i]+1).ToString();
                    if (i < attributesToRemove.Length - 1)
                    {
                        attributesFilterString += ",";
                    }
                }
            }
            //build instances filter
            string instancesFilterString = "";
            if (instancesToRemove!=null && instancesToRemove.Length > 0)
            {
                instancesFilterString = "-R ";
                for (int i = 0; i < instancesToRemove.Length; i++)
                {
                    instancesFilterString += (instancesToRemove[i]+1).ToString();
                    if (i < instancesToRemove.Length - 1)
                    {
                        instancesFilterString += ",";
                    }
                }
            }
                        

            //get reduced set
            Instances reducedDataset = new ConverterUtils.DataSource(dataset).getDataSet();//OriginalDataset;
            reducedDataset.setClassIndex(reducedDataset.numAttributes() - 1);

            if (!String.IsNullOrWhiteSpace(attributesFilterString))
            {
                Remove attributesFilter = new Remove();
                attributesFilter.setOptions(Utils.splitOptions(attributesFilterString));                
                attributesFilter.setInputFormat(reducedDataset);
                reducedDataset = Filter.useFilter(reducedDataset, attributesFilter);
            }
                        

            if (!String.IsNullOrWhiteSpace(instancesFilterString))
            {
                weka.filters.unsupervised.instance.RemoveRange instancesFilter = new weka.filters.unsupervised.instance.RemoveRange();
                instancesFilter.setOptions(Utils.splitOptions(instancesFilterString));
                instancesFilter.setInputFormat(reducedDataset);
                reducedDataset = Filter.useFilter(reducedDataset, instancesFilter);
            }

          


            return reducedDataset;
        }

        public Classifier CreateClassifier()
        {

            Classifier.buildClassifier(OriginalDataset);

            return Classifier;
        }

        public Classifier CreateClassifier(Instances reducedDataset)
        {

            Classifier.buildClassifier(reducedDataset);

            return Classifier;
        }

        public Classifier CreateClassifier(Instances reducedDataset, Classifier classifier)
        {

            classifier.buildClassifier(reducedDataset);

            return classifier;
        }

        public static double EvaluateClassifier(WekaClassifier wekaClassifier, string datasetPath)
        {
            //classifier.i

            Instances dataset = new ConverterUtils.DataSource(datasetPath).getDataSet();
            dataset.setClassIndex(dataset.numAttributes() - 1);

            if (wekaClassifier.AttributesToRemove != null && wekaClassifier.AttributesToRemove.Length != 0)
                dataset = GetReducedDataset(dataset, wekaClassifier.AttributesToRemove, null);


            double quality = 0;
            //evaluate classifier performance
            Evaluation eval = new Evaluation(dataset);
            eval.evaluateModel(wekaClassifier.Classifier, dataset);
            //return result based on evaluation
            quality = eval.correct() / dataset.numInstances(); //TODO: I'm assuming that this is accuracy. If not please tell me, and I can modify it.
            return quality;
        }

        public static double EvaluateClassifier(Classifier classifier, Instances dataset)
        {

            double quality = 0;
            //evaluate classifier performance
            Evaluation eval = new Evaluation(dataset);                                    
            eval.evaluateModel(classifier, dataset);
            //return result based on evaluation
            quality = eval.correct() / dataset.numInstances(); //TODO: I'm assuming that this is accuracy. If not please tell me, and I can modify it.
            return quality;
        }


    
    }
}
