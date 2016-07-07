using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO
{
    public abstract class AntColony<T>
    {
        #region Data Members

        protected int _maxIterations;
        protected int _colonySize;
        protected int _convergenceIterations;
        protected int _currentIteration;
        protected Ant<T> _bestAnt;
        protected Ant<T> _iterationBestAnt;
        protected ConstructionGraph<T> _graph;
        protected ProblemSpecifics.Problem<T> _problem;
        protected double _lastQuality;
        protected int _convergenceCounter;


        #endregion

        #region Properties

        public int MaxIterations
        {
            get { return this._maxIterations; }
        }

        public int ColonySize
        {
            get { return this._colonySize; }
        }

        public int CurrentIteration
        {
            get { return this._currentIteration; }
        }

        public Ant<T> BestAnt
        {
            get { return this._bestAnt; }
        }

        public Ant<T> IterationBestAnt
        {
            get { return this._iterationBestAnt; }
        }

        public ConstructionGraph<T> ConstructionGraph
        {
            get { return this._graph; }
        }

        public ProblemSpecifics.Problem<T> Problem
        {
            get { return this._problem; }
        }

        #endregion
                
        #region Constructor

        public AntColony(int maxIterations, int colonySize, int convergenceIterations,ConstructionGraph<T> graph, ProblemSpecifics.Problem<T> problem)
        {
            this._colonySize = colonySize;
            this._maxIterations = maxIterations;
            this._convergenceIterations = convergenceIterations;
            this._currentIteration = 0;
            this._graph = graph;            
            this._problem = problem;
            

        }

        public AntColony(int maxIterations, int colonySize, int convergenceIterations,  ProblemSpecifics.Problem<T> problem)
        {
            this._colonySize = colonySize;
            this._maxIterations = maxIterations;
            this._convergenceIterations = convergenceIterations;
            this._currentIteration = 0;            
            this._problem = problem;
            
        }


        #endregion

        #region Methods

        public  virtual void Initialize()
        {
            this.ConstructionGraph.InitializePheromone();
            this.ConstructionGraph.SetHeuristicValues(this._problem.HeuristicsCalculator,true);
            this._bestAnt = null;
            this._iterationBestAnt = null;
        }

        public abstract void Work();

        public abstract void CreateSolution();

        public abstract void EvaluateSolutionQuality(Solution<T> solution);

        public virtual void PerformLocalSearch(Ant<T> ant)
        {
            this._problem.LocalSearch.PerformLocalSearch(ant);
        }

        public virtual void UpdatePheromoneLevels()
        {

            this._iterationBestAnt.DepositPheromone(1);
            this.ConstructionGraph.EvaporatePheromone();

        }

        protected virtual void UpdateBestAnt()
        {
            if (this._bestAnt == null || _iterationBestAnt.Solution.Quality > this._bestAnt.Solution.Quality)
                this._bestAnt = _iterationBestAnt;

        }

        public abstract void PostProcessing();
      

        protected virtual bool IsConverged()
        {
            bool IsConverged = false;
            if (this._iterationBestAnt.Solution.Quality <= this._lastQuality)
                this._convergenceCounter++;
            else
            {
                this._convergenceCounter = 0;
                this._lastQuality = this._iterationBestAnt.Solution.Quality; ;
            }

            if (_convergenceCounter == this._convergenceIterations)
                IsConverged = true;
            else
                IsConverged = false;

            return IsConverged;

        }

        #endregion
    }
}
