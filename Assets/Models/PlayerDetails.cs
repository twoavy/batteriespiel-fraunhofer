using System;
using System.Linq;
using System.Net.Http;
using Helpers;
using UnityEditor;
using UnityEngine;

namespace Models
{

    public class PlayerRegistrationRequest
    {
        public string name;
        public string id;
        public string language;
        public bool finishedIntro;
        public GameState.Models current3dModel;
        
        public PlayerRegistrationRequest(string name)
        {
            this.name = name;
            id = Guid.NewGuid().ToString();
            language = Application.systemLanguage == SystemLanguage.German ? "de" : "en";
            finishedIntro = false;
            current3dModel = GameState.Models.Cells;
        }
    }
    
    [Serializable]
    public class PlayerDetails
    {
        public string id;
        public string name;
        public Language language;
        public bool finishedIntro;
        public GameState.Models current3dModel;
        public int totalScore;
        public MicrogameState[] results;

        public override string ToString()
        {
            return "PlayerDetails{" +
                   "id='" + id + '\'' +
                   ", name='" + name + '\'' +
                   ", language=" + language +
                   ", finishedIntro=" + finishedIntro +
                   ", current3dModel=" + current3dModel +
                   ", totalScore=" + totalScore +
                   ", microgames=" + string.Concat(results.Select(x => x.ToString())) +
                   '}';
        }

        public static explicit operator PlayerDetails(string v)
        {
            PlayerDetails casted = JsonUtility.FromJson<PlayerDetails>(v);
            return casted;
        }
    }
    
    [Serializable]
    public class MicrogameState
    {
        public GameState.Microgames game;
        public bool unlocked;
        public bool finished;
        public int result;

        public override string ToString()
        {
            return "MicrogameState{" +
                   "game=" + game +
                   ", unlocked=" + unlocked +
                   ", finished=" + finished +
                   ", result=" + result +
                   "}";
        }
    }

    public class PlayerRegistration : PlayerDetails
    {
        public string token;

        public override string ToString()
        {
            return "PlayerRegistration{" +
                   "id='" + id + '\'' +
                   ", name='" + name + '\'' +
                   ", language=" + language +
                   ", finishedIntro=" + finishedIntro +
                   ", current3dModel=" + current3dModel +
                   ", token='" + token + '\'' +
                   ", totalScore=" + totalScore +
                   '}';
        }

        public static explicit operator PlayerRegistration(string v)
        {
            PlayerRegistration casted = JsonUtility.FromJson<PlayerRegistration>(v);
            return casted;
        }
    }
}