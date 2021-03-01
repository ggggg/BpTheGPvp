# Getting Started
This page will show you how to create custom battles and create a command to start them, for today’s purposes I will show you how to create a simple custom FFA battle but this can be applied for other, more advanced usages as well.
## Creating a Battle Type.
First and formost you will need to create a new empty Broke Protocol plugin ([see Broke Protocol docs](https://broke-protocol.github.io/broke-protocol/#/)) and create a new class file, for today's exmple we will call this class "FfaBattle".<br/>
![Adding a class](https://i.ibb.co/v4HqrH6/image.png)<br/>
The library loads battle types which inhirit from the `Battle` class, so the next set would be to extend from it.
Example:
```csharp
using TheGPvp.BattleTypes;

public class FfaBattle : Battle
{
}
```
**Please note that the class is public otherwise it might not be loaded by the library.**
After doing this you will see an error, this is because `Battle` is an abstract class which has abstract methods.
To fix there errors we now must implement the abstract methods by override them.
For now we will only override the abstract method `CheckForPlayers` which returns weather there are enough players to start the battle, for this case we want more then 1 player in the battle so we will use the `Players` property to check how much players are in the lobby.
Like so:
```csharp
    public class FfaBattle : Battle
    {
        public override bool CheckForPlayers()
        {
            return base.Players.Count >= 2;
        }
    }
```
Note: the `base` is not nessary in this context, I shown it here to showcase that it is using the base's property.
 