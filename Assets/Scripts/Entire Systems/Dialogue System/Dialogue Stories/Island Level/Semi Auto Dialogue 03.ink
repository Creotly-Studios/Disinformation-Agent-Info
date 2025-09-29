VAR baseValue = 0
VAR lastDelta = 0
VAR responseIndex = 0
-> Start

=== Start ===
#speaker:Player
Kevin, you’ve been glued to your phone all day. What’s going on?

#speaker:Kevin
I’m reading about how the stock market is just a rigged game. Insiders always win, regular folks lose.

* If it were entirely rigged, wouldn’t more people just quit investing?
  ~ responseIndex = 0
  ~ baseValue = 3
  -> React1

* Are your sources analysts or just angry traders?
  ~ responseIndex = 1
  ~ baseValue = 2
  -> React2

* Sure, some manipulation happens, but the whole system?
  ~ responseIndex = 2
  ~ baseValue = 2
  -> React3

* Sounds like frustration more than fact. Did you lose money?
  ~ responseIndex = 3
  ~ baseValue = 1
  -> React4

=== React1 ===
{(lastDelta < -10):
    People *do* quit. They just don’t make headlines. #speaker:Kevin
- else:
    {(lastDelta > 0):
        True. If it was all fake, nobody would stay in. #speaker:Kevin
    - else:
        Maybe people stay because they don’t see the truth. #speaker:Kevin
    }
}

-> SecondWave

=== React2 ===
{(lastDelta < -10):
    So you’re saying my sources are worthless? #speaker:Kevin
- else:
    {(lastDelta > 0):
        Good point. Analysts might be more reliable. #speaker:Kevin
    - else:
        They’re mostly forums and channels… maybe biased. #speaker:Kevin
    }
}

-> SecondWave

=== React3 ===
{(lastDelta < -10):
    You think manipulation isn’t enough to ruin fairness? #speaker:Kevin
- else:
    {(lastDelta > 0):
        True. Scams happen, but it doesn’t mean the entire system. #speaker:Kevin
    - else:
        I don’t know. Feels like the game is stacked. #speaker:Kevin
    }
}

-> SecondWave

=== React4 ===
{(lastDelta < -10):
    Don’t make this personal. #speaker:Kevin
- else:
    {(lastDelta > 0):
        Yeah, maybe I’m letting emotions cloud it. #speaker:Kevin
    - else:
        A little. But that doesn’t mean the system is fair. #speaker:Kevin
    }
}

-> SecondWave

=== SecondWave ===
#speaker:Player
Why does this belief stick with you?

#speaker:Kevin
Because whenever markets crash, it feels like small investors take the hit while the rich bounce back.

* Crashes hurt everyone, but the wealthy recover faster because of resources.
  ~ responseIndex = 0
  ~ baseValue = 3
  -> RespondA

* That’s inequality, not necessarily rigging.
  ~ responseIndex = 1
  ~ baseValue = 2
  -> RespondB

* Isn’t that why regulation exists—to keep markets somewhat fair?
  ~ responseIndex = 2
  ~ baseValue = 2
  -> RespondC

* Maybe people share these stories because anger spreads faster than facts.
  ~ responseIndex = 3
  ~ baseValue = 4
  -> RespondD

=== RespondA ===
{(lastDelta < -10):
    So you think I’m just jealous? #speaker:Kevin
- else:
    {(lastDelta > 0):
        True. It’s more about resources than cheating. #speaker:Kevin
    - else:
        Maybe. But it still feels unfair. #speaker:Kevin
    }
}

-> FinalPush

=== RespondB ===
{(lastDelta < -10):
    Inequality *is* rigging. #speaker:Kevin
- else:
    {(lastDelta > 0):
        Yeah, inequality doesn’t always mean crime. #speaker:Kevin
    - else:
        Maybe. But inequality is still frustrating. #speaker:Kevin
    }
}

-> FinalPush

=== RespondC ===
{(lastDelta < -10):
    Regulators are in bed with the big players. #speaker:Kevin
- else:
    {(lastDelta > 0):
        True. That’s the purpose of oversight. #speaker:Kevin
    - else:
        Maybe, but regulation doesn’t feel strong enough. #speaker:Kevin
    }
}

-> FinalPush

=== RespondD ==
{(lastDelta < -10):
    You think I’m just falling for outrage? #speaker:Kevin
- else:
    {(lastDelta > 0):
        Maybe. Anger does make these stories spread. #speaker:Kevin
    - else:
        Could be both anger and truth. #speaker:Kevin
    }
}

-> FinalPush

=== FinalPush ===
#speaker:Player
If you learned the system isn’t rigged, just unequal, would that change your view?

{(lastDelta < -10):
    Hard to swallow, but maybe. #speaker:Kevin
- else:
    {(lastDelta > 0):
        Yeah. Inequality isn’t rigging. It’s a different fight. #speaker:Kevin
    - else:
        I’d still be upset, but at least I’d see it clearer. #speaker:Kevin
    }
}

-> Conclusion

=== Convinced ===
#speaker:Kevin
You know what? I think you’re right. I see it clearer now — inequality, not a grand conspiracy. #speaker:Kevin
-> END

=== Rejected ===
#speaker:Kevin
I still think there’s more going on. You’re missing the deeper problems. #speaker:Kevin
-> END

=== Conclusion ===
#speaker:Player
Glad we talked this through.

#speaker:Kevin
Yeah. I won’t stop questioning, but I’ll rethink how I see it.
-> END
