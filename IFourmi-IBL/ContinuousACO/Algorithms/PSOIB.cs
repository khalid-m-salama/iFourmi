using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ContinuousACO.ProblemSpecifics;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.Model.IBLearning;

namespace iFourmi.ContinuousACO.Algorithms
{
    public class PSOIB:IClassificationAlgorithm
    {
        private int _problemSize;
        private int _swarmSize;
        private int _maxIterations;
        private int _currentIteration;
        private AbstractContinousOptimizationEvaluator _solutionQualityEvaluator;


        private double w = 0.729; // inertia weight. see http://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=00870279
        private double c1 = 1.49445; // cognitive/local weight
        //private double c2 = 1.49445; // social/global weight
        private double c3 = 1.49445; // social/global weight
        private double r1, r2, r3; // cognitive and social randomizations
        private double probDeath = 0.01;
       
        double minX = 0;
        double maxX = 1;

        private Particle[] _swarm;

        private double[] _bestGlobalPosition;
        private double _bestGlobalQuality;

        private double _lastQuality=-1;
        private double _convergenceCounter;
        private double _convergenceIterations;

        public new event EventHandler OnPostSwarmIteration;

        public int CurrentIteration
        {
            get{ return this._currentIteration;}
           set{this._currentIteration=value;}
        }

        public Solution<double> GlobalBest
        {
            get 
            {
                Solution<double> bestSolution = Solution<double>.FromList(_bestGlobalPosition.ToList());
                bestSolution.Quality = _bestGlobalQuality;
                return bestSolution;
            }
        }

        public PSOIB(int problemSize, int swarmSize, int maxIterations, int convergenceIterations,AbstractContinousOptimizationEvaluator evaluator)
        {
            this._problemSize = problemSize;
            this._swarmSize = swarmSize;
            this._maxIterations = maxIterations;
            this._solutionQualityEvaluator = evaluator;

            this._convergenceIterations = convergenceIterations;
        }

        public void Initialize()
        {
            
            this._swarm=new Particle[this._swarmSize];
            this._bestGlobalPosition = new double[this._problemSize];

            // swarm initialization
            for (int i = 0; i < _swarm.Length; ++i)
            {
                double[] randomPosition = new double[this._problemSize];
                for (int j = 0; j < randomPosition.Length; ++j)
                    randomPosition[j] = (maxX - minX) * RandomUtility.NextDouble() + minX; // 

                double quality = EvaluateQuality(randomPosition);
                double[] randomVelocity = new double[this._problemSize];

                for (int j = 0; j < randomVelocity.Length; ++j)
                {
                    double lo = minX * 0.1;
                    double hi = maxX * 0.1;
                    randomVelocity[j] = (hi - lo) * RandomUtility.NextDouble() + lo;
                }
                _swarm[i] = new Particle(i,this._swarmSize,randomPosition, quality, randomVelocity);

                // does current Particle have global best position/solution?
                if (_swarm[i].quality > _bestGlobalQuality)
                {
                    _bestGlobalQuality = _swarm[i].quality;
                    _swarm[i].position.CopyTo(_bestGlobalPosition, 0);
                }
            } // initialization
 
        }

        public double EvaluateQuality(double[] position)
        {
            Solution<double> solution = Solution<double>.FromList(position.ToList());
            this._solutionQualityEvaluator.EvaluateSolutionQuality(solution);
            return solution.Quality;
        }

        public void Evolve()
        {

            double[] newVelocity = new double[this._problemSize];
            double[] newPosition = new double[this._problemSize];
            double newQuality;

            // main loop
            while (this._currentIteration < this._maxIterations)
            {
                if (IsConverged())
                    break;

                for (int i = 0; i < _swarm.Length; ++i) // each Particle
                {
                    Particle currP = _swarm[i]; // for clarity
                    Particle localBest = this.GetLocalBest(i);
                    // new velocity
                    for (int j = 0; j < currP.velocity.Length; ++j) // each component of the velocity
                    {
                        r1 = RandomUtility.NextDouble();
                        //r2 = RandomUtility.NextDouble();
                        r3 = RandomUtility.NextDouble();

                        newVelocity[j] = (w * currP.velocity[j]) +
                          (c1 * r1 * (currP.bestPosition[j] - currP.position[j])) +
                          //(c2 * r2 * (bestGlobalPosition[j] - currP.position[j]))+
                          (c3 * r3 * localBest.bestPosition[j]-currP.position[j]);
                    }
                    newVelocity.CopyTo(currP.velocity, 0);

                    // new position
                    for (int j = 0; j < currP.position.Length; ++j)
                    {
                        newPosition[j] = currP.position[j] + newVelocity[j];
                        if (newPosition[j] < minX)
                            newPosition[j] = minX;
                        else if (newPosition[j] > maxX)
                            newPosition[j] = maxX;
                    }
                    newPosition.CopyTo(currP.position, 0);

                    newQuality = EvaluateQuality(newPosition);
                    currP.quality = newQuality;

                    if (newQuality > currP.bestQuality)
                    {
                        newPosition.CopyTo(currP.bestPosition, 0);
                        currP.bestQuality = newQuality;
                    }

                    if (newQuality > _bestGlobalQuality)
                    {
                        newPosition.CopyTo(_bestGlobalPosition, 0);
                        _bestGlobalQuality = newQuality;
                    }

                    // death?
                    double die = RandomUtility.NextDouble();
                    if (die < probDeath)
                    {
                        // new position, leave velocity, update error
                        for (int j = 0; j < currP.position.Length; ++j)
                            currP.position[j] = (maxX - minX) * RandomUtility.NextDouble() + minX;
                        currP.quality = EvaluateQuality(currP.position);
                        currP.position.CopyTo(currP.bestPosition, 0);
                        currP.bestQuality = currP.quality;

                        if (currP.quality > _bestGlobalQuality) // global best by chance?
                        {
                            _bestGlobalQuality = currP.quality;
                            currP.position.CopyTo(_bestGlobalPosition, 0);
                        }
                    }

                } // each Particle

                if (OnPostSwarmIteration != null)
                    this.OnPostSwarmIteration(this, null);
                ++CurrentIteration;
            } // while

 
        }

        private Particle GetLocalBest(int index)
        {
            int max = 0;
            List<int> list=this._swarm[index].neighbourIndexList;

            for (int i = 0; i < list.Count; i++)
            {
                if (this._swarm[list[i]].bestQuality > this._swarm[list[max]].bestQuality)
                    max = i;
            }

            return this._swarm[list[max]];

            //Particle n1 = index == 0 ? this._swarm[this._swarm.Length - 1] : this._swarm[index - 1];
            //Particle n2 = index == this._swarm.Length - 1 ? this._swarm[0] : this._swarm[index + 1];
            //return n1.quality > n2.quality ? n1 : n2;
        }

        protected bool IsConverged()
        {
            bool IsConverged = false;
            if (this._bestGlobalQuality == this._lastQuality)
                this._convergenceCounter++;
            else
            {
                this._convergenceCounter = 0;
                this._lastQuality = this._bestGlobalQuality;
            }

            if (_convergenceCounter == this._convergenceIterations)
                IsConverged = true;
            else
                IsConverged = false;

            return IsConverged;
        }


        public DataMining.Data.Dataset Dataset
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DataMining.Model.IClassifier CreateClassifier()
        {
            this.Initialize();
            this.Evolve();
            this._solutionQualityEvaluator.EvaluateSolutionQuality(this.GlobalBest);
            AbstractLazyClassifier clasifier = ((IBClassificationQualityEvaluator)this._solutionQualityEvaluator).Classifier;
            return clasifier;
        }

        public DataMining.Model.Ensembles.EnsembleClassifier CreateEnsembleClassifier()
        {
            this.Initialize();
            this.Evolve();
   
            List<DataMining.Model.ClassifierInfo> ensemble = new List<DataMining.Model.ClassifierInfo>();
            for (int i = 0; i < this._swarmSize; i++)
            {
                Solution<double> solution = Solution<double>.FromList(this._swarm[i].bestPosition.ToList());
                this._solutionQualityEvaluator.EvaluateSolutionQuality(solution);
                AbstractLazyClassifier clasifier = ((IBClassificationQualityEvaluator)this._solutionQualityEvaluator).Classifier;
                DataMining.Model.ClassifierInfo classifierInfo = new DataMining.Model.ClassifierInfo() { Desc = clasifier.Desc + "-" + i.ToString(), Classifier = clasifier, Quality = solution.Quality };
                ensemble.Add(classifierInfo);

            }

            DataMining.Model.Ensembles.EnsembleClassifier ensmebleClassifier = new DataMining.Model.Ensembles.EnsembleClassifier(ensemble);
            return ensmebleClassifier;
        }
    }
    
    public class Particle
    {         
        public double[] position;
        public double quality;

        public double[] bestPosition;
        public double bestQuality;
                
        public double[] velocity;

        public List<int> neighbourIndexList=new List<int>();

        public Particle(int index, int swarmSize, double[] pos, double quality, double[] vel)
        {
            this.position = new double[pos.Length];
            pos.CopyTo(this.position, 0);
        
            this.velocity = new double[vel.Length];
            vel.CopyTo(this.velocity, 0);

            this.bestPosition = new double[pos.Length];
            pos.CopyTo(this.position, 0);

            this.quality = quality;
            this.bestQuality = quality;

            for (int i = 0; i < swarmSize; i++)
            {
                if (index != i)
                {
                    if (((double)(index + 1) % (double)(i + 1) == 0) || ((double)(i + 1) % (double)(i + 1) == 0))
                        neighbourIndexList.Add(i);
                }
            } 
           
        }

        public Solution<double> ToSolution()
        {
            return Solution<double>.FromList(bestPosition.ToList());
        }
    }
}
