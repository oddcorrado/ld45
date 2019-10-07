using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AiDictionnary
{
    public static string[,][] dictionnary;

    static AiDictionnary()
    {
        var nbNatures = Enum.GetNames(typeof(AiNature)).Length;
        var nbStates = Enum.GetNames(typeof(AiState)).Length;

        string[] genericIdle = new string[] { "I'm bored !", "I'm hungry !", "This is a nice place.", "I'm tired !", "*yawns*", "I'm happy !" };
        string[] genericWalk = new string[] { "I'm walking !", "I need to be over there.", "Nice day for a stroll."};
        string[] genericFlee = new string[] { "Oh come on !", "Help me !", "Have mercy !", "My father will hear about this.", "Aaaaaah !", "Run for your lives !" };
        string[] genericHide = new string[] { "I'll be safe here.", "Phew...", "Made it !"};

        dictionnary = new string[nbNatures, nbStates][];

        for(int i = 0; i < nbNatures; i++)
        {
            for(int j = 0; j < nbStates;j++)
            {
                if (j == (int)AiState.Idle) dictionnary[i, j] = genericIdle;
                else if (j == (int)AiState.Walking) dictionnary[i, j] = genericWalk;
                else if (j == (int)AiState.Fleeing || j == (int)AiState.SeekingShelter) dictionnary[i, j] = genericFlee;
                else if (j == (int)AiState.Hiding) dictionnary[i, j] = genericHide;
            }
        }
    }
}
