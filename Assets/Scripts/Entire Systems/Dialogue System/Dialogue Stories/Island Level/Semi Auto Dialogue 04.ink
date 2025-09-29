VAR baseValue = 0
VAR lastDelta = 0
VAR responseIndex = 0
-> Start

=== Start ===
#speaker:Player
Hey Kevin, you seem restless. What’s up?

#speaker:Kevin
It’s all this talk online about social media platforms spying on us. People say our phones are recording even when we don’t use them.

* If they were recording all the time, wouldn’t storage and bandwidth be impossible?
  ~ responseIndex = 0
  ~ baseValue = 3
  -> React1

* Do you trust the source of this claim, or just the people repeating it?
  ~ responseIndex = 1
  ~ baseValue = 2
  -> React2

* Phones ask permission for mic access. Doesn’t that limit it?
  ~ responseIndex = 2
  ~ baseValue = 2
  -> React3

* Maybe it’s not recording—just algorithms making targeted guesses.
  ~ responseIndex = 3
  ~ baseValue = 1
  -> React4

=== React1 ===
{(lastDelta < -10):
    You think I don’t understand how tech works? #speaker:Kevin
- else:
    {(lastDelta > 0):
        True. Constant recording would take insane resources. #speaker:Kevin
    - else:
        Still, they’ve got deep pockets. Maybe it’s possible. #speaker:Kevin
    }
}
-> SecondWave

=== React2 ===
{(lastDelta < -10):
    You don’t believe my sources are legit? #speaker:Kevin
- else:
    {(lastDelta > 0):
        Fair point. I should check if these people are experts. #speaker:Kevin
    - else:
        They seem convincing, but maybe I need stronger proof. #speaker:Kevin
    }
}
-> SecondWave

=== React3 ===
{(lastDelta < -10):
    Permissions don’t stop hidden code. #speaker:Kevin
- else:
    {(lastDelta > 0):
        True, permissions are there for a reason. #speaker:Kevin
    - else:
        Some say permissions are just for show. #speaker:Kevin
    }
}
-> SecondWave

=== React4 ===
{(lastDelta < -10):
    That’s what they want you to think. #speaker:Kevin
- else:
    {(lastDelta > 0):
        Maybe it’s just smarter ads, not spying. #speaker:Kevin
    - else:
        Could be guesses, but it feels too accurate. #speaker:Kevin
    }
}
-> SecondWave

=== SecondWave ===
#speaker:Player
So why does this theory stick with you?

#speaker:Kevin
Because every time I talk about something, an ad shows up for it.

* Ads track browsing, not private talk. That’s been proven.
  ~ responseIndex = 0
  ~ baseValue = 3
  -> RespondA

* Could it be coincidence and confirmation bias?
  ~ responseIndex = 1
  ~ baseValue = 2
  -> RespondB

* Maybe your phone listens for wake words, not whole convos.
  ~ responseIndex = 2
  ~ baseValue = 2
  -> RespondC

* People share these stories because they want to feel watched—it validates their concerns.
  ~ responseIndex = 3
  ~ baseValue = 4
  -> RespondD

=== RespondA ===
{(lastDelta < -10):
    You’re ignoring what I experience. #speaker:Kevin
- else:
    {(lastDelta > 0):
        Yeah. It’s probably just browsing data being tracked. #speaker:Kevin
    - else:
        Maybe. But it feels personal. #speaker:Kevin
    }
}
-> FinalPush

=== RespondB ===
{(lastDelta < -10):
    You think I’m imagining things? #speaker:Kevin
- else:
    {(lastDelta > 0):
        That’s true. I notice it more when it matches. #speaker:Kevin
    - else:
        Coincidence is possible, but it’s hard not to suspect. #speaker:Kevin
    }
}
-> FinalPush

=== RespondC ===
{(lastDelta < -10):
    That’s just another excuse. #speaker:Kevin
- else:
    {(lastDelta > 0):
        Makes sense. Wake words don’t mean constant recording. #speaker:Kevin
    - else:
        But what if wake words are just the start? #speaker:Kevin
    }
}
-> FinalPush

=== RespondD ===
{(lastDelta < -10):
    That’s cynical. People are smarter than that. #speaker:Kevin
- else:
    {(lastDelta > 0):
        Maybe. People want proof they’re not paranoid. #speaker:Kevin
    - else:
        Could be, but it still creeps me out. #speaker:Kevin
    }
}
-> FinalPush

=== FinalPush ===
#speaker:Player
If it turns out it’s just algorithms, would you feel relieved?
-> Conclusion

=== Convinced ===
#speaker:Kevin
You know what? I think you’re right. I feel like I see things clearer now.

-> END

=== Rejected ===
#speaker:Kevin
You don’t get it. You’re too sure of yourself.

-> END

=== Conclusion ===
#speaker:Kevin
Yeah. That would explain it.

#speaker:Player
Glad we talked it through.

#speaker:Kevin
Same here. At least I’m questioning it more now.

-> END
