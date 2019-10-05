using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Persistent
{
    public class Fighter
    {
        public GameObject prefab;
        public string name;
        public int playerId;
        public int life = 100;
        public bool alive = true;
    }

    static class PersistentData
    {

        public class Player
        {
            public string name;
            public int playerId;
            public List<Fighter> fighters = new List<Fighter>();
            public Fighter currentFighter;
        }
        private static List<Fighter> selectedFighers = new List<Fighter>();
        public static List<Fighter> SelectedFighters { get { return selectedFighers; } }
        public static int Level { get; set; }
        private static List<Player> players = new List<Player>();
        public static List<Player> Players { get { return players; } }
        public static int playerCount = 2;
    }
}
