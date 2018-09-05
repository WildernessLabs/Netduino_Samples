using System;
using Microsoft.SPOT;

namespace Simone
{
    public enum GameState
    {
        Start,
        NextLevel,
        Win,
        GameOver,
    }

    public class SimonEventArgs : EventArgs
    {
        public GameState GameState { get; set; }

        public SimonEventArgs (GameState state)
        {
            GameState = state;
        }
    }

    public class SimonGame
    {
        public delegate void GameStateChangedDelegate(object sender, SimonEventArgs e);

        public event GameStateChangedDelegate OnGameStateChanged = delegate { };

        static int MAX_LEVELS = 25;
        static int NUM_BUTTONS = 4;

        int[] Steps = new int[MAX_LEVELS];

        public int Level { get; set; }

        int currentStep;

        Random rand = new Random((int)DateTime.Now.Ticks);

        public void Reset ()
        {
            OnGameStateChanged(this, new SimonEventArgs(GameState.Start));

            Level = 1;
            currentStep = 0;

            NextLevel();
        }

        public int[] GetStepsForLevel ()
        {
            var steps = new int[Level];

            for (int i = 0; i < Level; i++)
                steps[i] = Steps[i];

            return steps;
        }

        public void EnterStep(int step)
        {
            if (Steps[currentStep] == step)
            {
                Debug.Print("Correct step: " + step + " Current step: " + currentStep);

                currentStep++;
            }
            else
            {
                Debug.Print("wrong step: " + step + " should be: " + Steps[currentStep]);

                OnGameStateChanged(this, new SimonEventArgs(GameState.GameOver));

                Reset();
            }

            if(currentStep == Level)
            {
                NextLevel();
            }
        }

        void NextLevel ()
        {
            currentStep = 0;
            Level++;

            if(Level >= MAX_LEVELS)
            {
                OnGameStateChanged(this, new SimonEventArgs(GameState.Win));
                Reset();
                return;
            }

            var level = string.Empty;
            for (int i = 0; i < Level; i++)
            {
                Steps[i] = rand.Next(NUM_BUTTONS);
                level += Steps[i] + ", ";
            }

            Debug.Print(level);

            OnGameStateChanged(this, new SimonEventArgs(GameState.NextLevel));
        }
    }
}