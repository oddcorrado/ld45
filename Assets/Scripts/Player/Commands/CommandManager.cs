using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    List<Command> commands = new List<Command>();

    void Start()
    {
        var commandsArray = GetComponents<Command>();
        commands = new List<Command>(commandsArray);
    }

    public void StopAllCommands()
    {
        commands.ForEach(command =>
        {
            command.Stop();
        });
    }

}
