VAR baseValue = 0
VAR lastDelta = 0
VAR responseIndex = 0
-> Start

=== Start ===
# speaker:Player
Samantha, you look upset. Is it about sports again?

# speaker:Samantha
Yeah. I’ve been reading that major leagues cover up doping and money laundering scandals to protect their image.

* If they covered everything, wouldn’t athletes and staff eventually expose it?
  ~ responseIndex = 0
  ~ baseValue = 3
  -> React1

* Do your sources come from investigative reports or just rumor sites?
  ~ responseIndex = 1
  ~ baseValue = 2
  -> React2

* Some cases of doping were exposed publicly. Doesn’t that show cover-ups don’t last?
  ~ responseIndex = 2
  ~ baseValue = 2
  -> React3

* Maybe fans spread these stories because outrage keeps sports exciting.
  ~ responseIndex = 3
  ~ baseValue = 1
  -> React4

=== React1 ===
{(lastDelta < -10):
# speaker:Samantha
You think I’m naïve?

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  True. Whistleblowers do speak up eventually.
- else:
# speaker:Samantha
  Still, what if many are paid to stay silent?
  }
}
-> SecondWave

=== React2 ===
{(lastDelta < -10):
# speaker:Samantha
You’re dismissing my sources?

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  Fair point. Investigative journalism carries more weight.
- else:
# speaker:Samantha
  Some of it’s from blogs and fan channels… maybe biased.
  }
}
-> SecondWave

=== React3 ===
{(lastDelta < -10):
# speaker:Samantha
That doesn’t prove everything is clean.

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  Right. Exposure means not everything can stay hidden.
- else:
# speaker:Samantha
  Maybe, but some scandals never fully surface.
  }
}
-> SecondWave

=== React4 ===
{(lastDelta < -10):
# speaker:Samantha
That’s a cheap take.

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  Maybe outrage keeps fans engaged.
- else:
# speaker:Samantha
  Could be, but this feels deeper than hype.
  }
}
-> SecondWave

=== SecondWave ===
# speaker:Player
So why does this one stick with you?

# speaker:Samantha
Because every time an athlete is caught, it feels like a distraction from bigger scandals.

* Could it be that small scandals are exposed to show accountability?
  ~ responseIndex = 0
  ~ baseValue = 3
  -> RespondA

* That sounds like perception, not necessarily fact.
  ~ responseIndex = 1
  ~ baseValue = 2
  -> RespondB

* Regulators exist to protect integrity in sports.
  ~ responseIndex = 2
  ~ baseValue = 2
  -> RespondC

* Maybe the cycle of scandal keeps fans emotionally hooked.
  ~ responseIndex = 3
  ~ baseValue = 4
  -> RespondD

=== RespondA ===
{(lastDelta < -10):
# speaker:Samantha
Or it’s a smokescreen.

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  True. Sometimes small stories are used to show action.
- else:
# speaker:Samantha
  Could be. Hard to know the intent.
  }
}
-> FinalPush

=== RespondB ===
{(lastDelta < -10):
# speaker:Samantha
My perception matters too.

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  Fair. Perception doesn’t always equal truth.
- else:
# speaker:Samantha
  Maybe. But perception shapes trust.
  }
}
-> FinalPush

=== RespondC ===
{(lastDelta < -10):
# speaker:Samantha
Regulators can be bought.

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  True. Regulation exists for fairness.
- else:
# speaker:Samantha
  Sometimes regulation feels weak.
  }
}
-> FinalPush

=== RespondD ===
{(lastDelta < -10):
# speaker:Samantha
You think it’s all about drama?

- else:
  {(lastDelta > 0):
  # speaker:Samantha
  Maybe. Scandal fuels the industry too.
- else:
# speaker:Samantha
  Possibly, but money laundering feels beyond hype.
  }
}
-> FinalPush

=== FinalPush ===
# speaker:Player
If future scandals show leagues punishing offenders transparently, would that ease your doubts?
  -> Conclusion

=== Convinced ===
# speaker:Samantha
Okay… maybe I’ve been too cynical. You’ve given me something to think about.
-> END

=== Rejection ===
# speaker:Samantha
No. Even with transparency, I’ll always believe the system is rotten.
-> END

=== Conclusion ===
# speaker:Samantha
Maybe. It would help, at least.

# speaker:Player
Glad you shared this with me.

# speaker:Samantha
Thanks. I’m not fully convinced, but I’ll look at it with fresh eyes.
-> END
