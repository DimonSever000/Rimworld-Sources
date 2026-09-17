using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore
{
    public class GameComponent_EventsCore : GameComponent
    {
        private Game game;
        public Game Game => game;

        private static GameComponent_EventsCore currentGame;
        public static GameComponent_EventsCore CurrentGame
        {
            get
            {
                if (currentGame == null || currentGame.Game != Current.Game)
                {
                    currentGame = Current.Game.GetComponent<GameComponent_EventsCore>();
                }

                return currentGame;
            }
        }

        private GameComponent_PlanetWeatherController planetWeatherController;
        public GameComponent_PlanetWeatherController PlanetWeatherController
        {
            get
            {
                if (planetWeatherController == null)
                {
                    planetWeatherController = Current.Game.GetComponent<GameComponent_PlanetWeatherController>();
                }

                return planetWeatherController;
            }
        }

        public GameComponent_EventsCore()
        {

        }

        public GameComponent_EventsCore(Game game)
        {
            this.game = game;
        }
    }
}
