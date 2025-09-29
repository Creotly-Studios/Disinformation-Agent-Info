VAR baseValue = 0
VAR lastDelta = 0
VAR responseIndex = 0
-> Start

=== Start ===
#speaker:Player
Hey Bob, you look deep in thought. Everything okay?

#speaker:Bob
Not really. I keep reading these articles saying big pharma hides natural cures so they can keep selling drugs.

* If that were true, doctors worldwide would have to be in on it. That seems impossible.
    ~ responseIndex = 0
    ~ baseValue = 3
    -> React1

* Are these articles from verified medical journals or random websites?
    ~ responseIndex = 1
    ~ baseValue = 2
    -> React2

* But if natural cures worked so well, wouldn’t there be more evidence?
    ~ responseIndex = 2
    ~ baseValue = 2
    -> React3

* It sounds like fear is driving those claims more than facts.
    ~ responseIndex = 3
    ~ baseValue = 1
    -> React4

=== React1 ===
{(lastDelta < -10):
    You think I’m gullible enough to believe all doctors? #speaker:Bob
- else:
    {(lastDelta > 0):
        True. Not every doctor could be silent about something that huge. #speaker:Bob
    - else:
        Still, companies have a lot of money at stake. #speaker:Bob
    }
}
-> SecondWave

=== React2 ===
{(lastDelta < -10):
    So you think my sources are trash? #speaker:Bob
- else:
    {(lastDelta > 0):
        Fair point. Maybe I should check if they’re peer-reviewed. #speaker:Bob
    - else:
        They aren’t journals, but they seem convincing. #speaker:Bob
    }
}
-> SecondWave

=== React3 ===
{(lastDelta < -10):
    You think I don’t see evidence when it’s there? #speaker:Bob
- else:
    {(lastDelta > 0):
        Yeah. If there was proof, it would be impossible to hide. #speaker:Bob
    - else:
        Some people say evidence is suppressed. #speaker:Bob
    }
}
-> SecondWave

=== React4 ===
{(lastDelta < -10):
    Don’t just dismiss me. This is serious. #speaker:Bob
- else:
    {(lastDelta > 0):
        Maybe fear does play a role. Hard to separate feelings from facts. #speaker:Bob
    - else:
        It feels like both fear and greed could be involved. #speaker:Bob
    }
}
-> SecondWave

=== SecondWave ===
#speaker:Player
So why do you think people spread these stories?

#speaker:Bob
Because nobody trusts corporations anymore.

* Lack of trust doesn’t equal proof of wrongdoing.
    ~ responseIndex = 0
    ~ baseValue = 3
    -> RespondA

* Maybe the distrust is real, but that doesn’t mean cures are hidden.
    ~ responseIndex = 1
    ~ baseValue = 2
    -> RespondB

* Doesn’t science thrive on challenging claims and proving them?
    ~ responseIndex = 2
    ~ baseValue = 2
    -> RespondC

* Could it be people just want simple answers to complex health problems?
    ~ responseIndex = 3
    ~ baseValue = 4
    -> RespondD

=== RespondA ===
{(lastDelta < -10):
    You’re missing the point. It’s about trust! #speaker:Bob
- else:
    {(lastDelta > 0):
        True. Lack of trust doesn’t prove a cover-up. #speaker:Bob
    - else:
        I know, but it still feels like something’s hidden. #speaker:Bob
    }
}
-> FinalPush

=== RespondB ===
{(lastDelta < -10):
    You don’t get it. Mistrust has reasons. #speaker:Bob
- else:
    {(lastDelta > 0):
        Yeah, mistrust doesn’t prove they’re guilty. #speaker:Bob
    - else:
        But the timing of these stories always feels shady. #speaker:Bob
    }
}
-> FinalPush

=== RespondC ===
{(lastDelta < -10):
    Science can be bought too. #speaker:Bob
- else:
    {(lastDelta > 0):
        True. Science is about proof, not fear. #speaker:Bob
    - else:
        Maybe some science is biased, but not all. #speaker:Bob
    }
}
-> FinalPush

=== RespondD ===
{(lastDelta < -10):
    That’s too simple. People are smarter than that. #speaker:Bob
- else:
    {(lastDelta > 0):
        Maybe. Complex problems don’t have easy fixes. #speaker:Bob
    - else:
        I don’t know. It’s hard not to wonder. #speaker:Bob
    }
}
-> FinalPush

=== FinalPush ===
#speaker:Player
If natural cures are proven safe one day, wouldn’t you want them available?

{(lastDelta < -10):
    Yeah. But until then, I’ll keep doubting. #speaker:Bob
- else:
    {(lastDelta > 0):
        Definitely. And until then, I should be careful what I believe. #speaker:Bob
    - else:
        Of course. I just want truth, not lies. #speaker:Bob
    }
}
-> Convinced

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
I’m glad we could talk this through.

#speaker:Bob
Yeah. I’ll think twice before I click next time.
-> END
