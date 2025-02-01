//notes about the player

Everything about the player is handled in a hierarchical state machine.

<h1>Locomotion</h1>
Player has camera relative movement and jumping.

<h1>Dialogue</h1>
Player will be sent into the dialogue state after interacting with a dialogue object

<h1>Combat</h1>
Although separate from the state machine, the player can only fight when the attack function is called via the state machine (PS: this uses free flow combat)