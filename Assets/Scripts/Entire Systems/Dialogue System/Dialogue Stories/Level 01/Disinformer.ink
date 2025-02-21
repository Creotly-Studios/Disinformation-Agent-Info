VAR npcEmotion = "Angry"

#speaker: Vince
If it isn’t the truth patrol. What, here to give me a lecture?

#speaker: Player [argumentative_tone]
More like a reality check. You know the stuff you’re posting is fake.

#speaker: Vince [sarcastic_tone]
And? It gets clicks. People love drama. The Mayor wins, and I get my cut.

#speaker: Player
  So you know it’s fake, but you’re posting it anyway?
  -> VinceJustifies

=== VinceJustifies ===
#speaker: Vince [dismissive_tone]
[grins] Call it what you want. It works.

#speaker: Player [argumentative_tone]
Works for who? Because the people believing your lies? They’re the ones who lose.

#speaker: Vince [shrugs]
They should fact-check. Not my problem.

#speaker: Player [reserved_tone]
It is your problem when the truth comes out. Fake stories don’t last forever. And when people realize they’ve been played, they’ll want someone to blame.

#speaker: Vince [quiet, uncertain_tone]
…

#speaker: Player [strategic_suggestion_tone]
The Mayor? He’s protected. You? You’re disposable.

#speaker: Vince [nervous_tone]
…You think people will come after me?

#speaker: Player [reserved_tone]
I know they will. And once you lose credibility, you don’t get it back.

* [People will turn on you faster than you think. You’re just a pawn in their game.] 
    -> LogicalTone

* [You might be getting paid now, but what happens when you’re no longer useful?] 
    -> EmotionalTone

* [The truth always comes out, and when it does, no one will defend you.] 
    -> ArgumentativeTone

* [It’s your choice. Just don’t expect the Mayor to have your back when it all falls apart.] 
    -> ReservedTone

=== LogicalTone ===
#speaker: Player
People will turn on you faster than you think. You’re just a pawn in their game.

#speaker: Vince
…Guess I never thought about it like that.

#speaker: Player
Think about it now. Before it’s too late.

#speaker: Vince
Yeah… maybe I should.

-> END

=== EmotionalTone ===
#speaker: Player
You might be getting paid now, but what happens when you’re no longer useful?

#speaker: Vince
I… I don’t know. I didn’t really think about it.

#speaker: Player
Exactly. And when they’re done with you, they’ll move on like you never mattered.

#speaker: Vince
…That’s a scary thought.

-> END

=== ArgumentativeTone ===
#speaker: Player
The truth always comes out, and when it does, no one will defend you.

#speaker: Vince
You really think people will turn on me that fast?

#speaker: Player
I know they will. The same people cheering now will be the first to tear you down.

#speaker: Vince
Damn. That’s messed up.

-> END

=== ReservedTone ===
#speaker: Player
It’s your choice. Just don’t expect the Mayor to have your back when it all falls apart.

#speaker: Vince
…You think he’d drop me just like that?

#speaker: Player
I don’t think. I know.

#speaker: Vince
…Maybe I should lay low for a while.

-> END
