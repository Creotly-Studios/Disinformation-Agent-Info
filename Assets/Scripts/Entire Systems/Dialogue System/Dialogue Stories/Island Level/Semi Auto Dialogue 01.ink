VAR baseValue = 0
VAR lastDelta = 0
VAR responseIndex = 0
-> Start

=== Start ===
#speaker:Player
Hey, I do not think we have talked much. I am Kim. You look like something is weighing on you.

#speaker:Bob
Yeah. It is this election stuff. Rumors keep popping up online and it is messing with my head.

#speaker:Player
What kind of rumors?

#speaker:Bob
People are saying officials rigged votes through remote software. That they had a backdoor. Videos, screenshots, the works.

* Sounds like a lot of smoke and mirrors. Where is the real proof?
    ~ responseIndex = 0
    ~ baseValue = 2
    -> React1
    
* Videos and screenshots can be faked. You know that, right?
    ~ responseIndex = 3
    ~ baseValue = 1
    -> React3

* And you think the entire election system would just let that happen?
    ~ responseIndex = 1
    ~ baseValue = 3
    -> React2


* Even if some data leaked, that does not mean there was a plot.
    ~ responseIndex = 3
    ~ baseValue = 2
    -> React4

=== React1 ===
{(lastDelta < -10):
#speaker:Bob
    So you think I am just making things up?
- else:
{(lastDelta > 0):
#speaker:Bob
    Maybe you are right. I have not seen any verified source.
- else:
#speaker:Bob
    Still, it all adds up in weird ways. Can not just ignore that.
}
}
-> SecondWave

=== React2 ===
{(lastDelta < -10):
#speaker:Bob
    That is naive. Power does what it wants when no one watches.
- else:
{(lastDelta > 0):
#speaker:Bob
    True... You would think someone would catch it if it were real.
- else:
#speaker:Bob
    Maybe the system is too big to catch everything.
}
}
-> SecondWave

=== React3 ===
{(lastDelta < -10):
#speaker:Bob
    You think I can not tell real from fake? That is insulting.
- else:
{(lastDelta > 0):
#speaker:Bob
    Yeah, fair. Stuff like that can be misleading.
- else:
#speaker:Bob
    I know they can be faked. But why would so many people share them?
}
}
-> SecondWave

=== React4 ===
{(lastDelta < -10):
#speaker:Bob
    So you are okay with just brushing off a possible scandal?
- else:
{(lastDelta > 0):
#speaker:Bob
    Right. If it was a plan, we would see bigger consequences.
- else:
#speaker:Bob
    I guess, but how can we really know?
}
}
-> SecondWave

=== SecondWave ===
#speaker:Player
So what makes you believe it? What made this story stick with you?

#speaker:Bob
It is not just one story. It is the pattern. The way these things always surface right before results.

* People want to feel cheated instead of facing hard truths.
    ~ responseIndex = 3
    ~ baseValue = 4
    -> RespondD
    
* Is not it weird how it is always social media, not experts, who break these stories?
    ~ responseIndex = 2
    ~ baseValue = 2
    -> RespondC

* That pattern exists because losing sides need someone to blame.
    ~ responseIndex = 0
    ~ baseValue = 3
    -> RespondA

* But does pattern mean proof?
    ~ responseIndex = 1
    ~ baseValue = 2
    -> RespondB


=== RespondA ===
{(lastDelta < -10):
#speaker:Bob
    You do not get it. It is not about losing. It is about being lied to.
- else:
{(lastDelta > 0):
#speaker:Bob
    Huh. That does explain a lot of the noise every cycle.
- else:
#speaker:Bob
    Still feels different this time, though.
}
}
-> FinalPush

=== RespondB ===
{(lastDelta < -10):
#speaker:Bob
    That is dismissive. Patterns can mean something.
- else:
{(lastDelta > 0):
#speaker:Bob
    No... I guess pattern without facts is not enough.
- else:
#speaker:Bob
    It might not be proof. But it is still worth looking into, right?
}
}
-> FinalPush

=== RespondC ===
{(lastDelta < -10):
#speaker:Bob
    Experts can be biased too. That is how they get away with it.
- else:
{(lastDelta > 0):
#speaker:Bob
    You have a point. Too many sketchy posts out there.
- else:
#speaker:Bob
    Maybe people share what they *want* to believe.
}
}
-> FinalPush

=== RespondD ===
#speaker:Bob
{(lastDelta < -10):
    You think people are just sore losers? That is cold.
- else:
{(lastDelta > 0):
    Maybe... maybe it *is* easier to blame than accept.
- else:
    I do not know. It just all feels so uncertain.
}
}
-> FinalPush

=== FinalPush ===
#speaker:Player
If none of it turns out real, what then? Would you accept that?

{(lastDelta < -10):
    Guess I was just another pawn. That stings. #speaker:Bob
- else:
{(lastDelta > 0):
    Then yeah. I was wrong. But at least I asked questions. #speaker:Bob
- else:
    I would be disappointed. But I want to know the truth, even if it hurts. #speaker:Bob
}
}

-> Conclusion

=== Convinced ===
#speaker:Bob
You know what ?, ... I think you are right. I feel like I see things clearer now.
-> END

=== Rejected ===
#speaker:Bob
You do not get it. You are too sure of yourself.
-> END

=== Conclusion ===
    #speaker:Player
    I think we made progress. Maybe we can talk again soon.

    #speaker:Bob
    Yeah... I am not fully there. But I am thinking.
    -> END
- else:
    #speaker:Player
    Maybe next time we can dive deeper. Worth a fresh look.

    #speaker:Bob
    Sure. I will keep reading. Maybe I missed something.
    -> END
}
}
}
