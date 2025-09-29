VAR baseValue = 0
VAR lastDelta = 0
VAR responseIndex = 0
-> Start

=== Start ===
#speaker:Player
Hey Samantha, you’ve been scrolling news nonstop. Something bothering you?

#speaker:Samantha
Yeah. People online are saying the government hides major scandals and only releases what they want us to see.

#speaker:Player
That’s a serious claim. What makes you think that?

# speaker:Samantha
Should be common sense, No ?

* Governments do withhold things, but total cover-ups are hard to maintain.
  ~ responseIndex = 0
  ~ baseValue = 3
  -> React1

* Where are you reading this—official reports or just social media?
  ~ responseIndex = 1
  ~ baseValue = 2
  -> React2

* History has leaks that prove cover-ups are rare to stay hidden.
  ~ responseIndex = 2
  ~ baseValue = 2
  -> React3

* Maybe people spread these claims because distrust feels powerful.
  ~ responseIndex = 3
  ~ baseValue = 1
  -> React4

=== React1 ===
{(lastDelta < -10):
#speaker:Samantha
    You think I’m exaggerating?
- else:
    {(lastDelta > 0):
    #speaker:Samantha
        True. Total secrecy usually falls apart.
    - else:
    #speaker:Samantha
        Still, some scandals only came out decades later.
    }
}
-> SecondWave

=== React2 ===
{(lastDelta < -10):
    So you think my sources are worthless? #speaker:Samantha
- else:
    {(lastDelta > 0):
        Fair. Social media isn’t the same as evidence. #speaker:Samantha
    - else:
        Most of it is from forums and posts. Not official. #speaker:Samantha
    }
}
-> SecondWave

=== React3 ===
{(lastDelta < -10):
    But what about things like Watergate? #speaker:Samantha
- else:
    {(lastDelta > 0):
        Yeah, leaks usually bring truth to light. #speaker:Samantha
    - else:
        True, but some leaks take too long. #speaker:Samantha
    }
}
-> SecondWave

=== React4 ===
{(lastDelta < -10):
    So you think distrust is just a mood? #speaker:Samantha
- else:
    {(lastDelta > 0):
        Maybe. People spread claims to feel like insiders. #speaker:Samantha
    - else:
        It does feel good to feel like you know the truth. #speaker:Samantha
    }
}
-> SecondWave

=== SecondWave ===
#speaker:Player
So why does this story grip you so much?

#speaker:Samantha
Because I feel powerless. Like the truth is always filtered.

* Feeling powerless doesn’t mean the truth is lost.
  ~ responseIndex = 0
  ~ baseValue = 3
  -> RespondA

* Isn’t it better to rely on verified journalism than rumors?
  ~ responseIndex = 1
  ~ baseValue = 2
  -> RespondB

* Transparency groups exist to challenge secrecy.
  ~ responseIndex = 2
  ~ baseValue = 2
  -> RespondC

* Maybe rumors spread because people crave accountability they don’t see.
  ~ responseIndex = 3
  ~ baseValue = 4
  -> RespondD

=== RespondA ===
#speaker:Samantha
{(lastDelta < -10):
    Easy for you to say. #speaker:Samantha
- else:
    {(lastDelta > 0):
        True. Powerlessness doesn’t mean hopelessness. #speaker:Samantha
    - else:
        Maybe. Still feels like truth is hidden. #speaker:Samantha
    }
}
-> FinalPush

=== RespondB ===
#speaker:Samantha
{(lastDelta < -10):
    Journalists can be corrupt too. #speaker:Samantha
- else:
    {(lastDelta > 0):
        Yeah. Journalists are more reliable than random posts. #speaker:Samantha
    - else:
        But even media has its flaws. #speaker:Samantha
    }
}
-> FinalPush

=== RespondC ===
#speaker:Samantha
{(lastDelta < -10):
    Groups can be silenced. #speaker:Samantha
- else:
    {(lastDelta > 0):
        True. Transparency is built to challenge secrets. #speaker:Samantha
    - else:
        But they don’t always win. #speaker:Samantha
    }
}
-> FinalPush

=== RespondD ===
{(lastDelta < -10):
    That’s too cynical. #speaker:Samantha
- else:
    {(lastDelta > 0):
        Maybe. Rumors fill a gap when people want justice. #speaker:Samantha
    - else:
        Feels like people spread what they wish were true. #speaker:Samantha
    }
}
-> FinalPush

=== FinalPush ===
#speaker:Player
If most cover-ups get exposed eventually, does that give you hope?

{(lastDelta < -10):
    A little. But I’ll stay cautious. #speaker:Samantha
- else:
    {(lastDelta > 0):
        Yeah. Secrets rarely last forever. #speaker:Samantha
    - else:
        Maybe. Hope is hard, but I want it. #speaker:Samantha
    }
}
-> Conclusion

=== Convinced ===
#speaker:Samantha
Maybe I’ve been too cynical. You’ve given me something to think about.
-> END

=== Rejected ===
#speaker:Samantha
No. Even when things surface, I still suspect the worst.
-> END

=== Conclusion ===
#speaker:Player
Thanks for sharing this with me.

#speaker:Samantha
Yeah. Talking helps me see it differently.
-> END
