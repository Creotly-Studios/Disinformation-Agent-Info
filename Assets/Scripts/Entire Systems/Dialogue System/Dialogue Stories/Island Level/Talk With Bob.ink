VAR npcEmotion = "Angry"

#speaker: Player
You alright? You look like something’s eating you.

#speaker: Bob
Huh? Oh, uh… It’s nothing. Just—forget it.

#speaker: Player
Doesn’t seem like nothing. You look ready to burst.

#speaker: Bob [sighs]
Alright, fine. The Mayor hired me to post some things online. You know, get people talking, stir the pot a little. Sounded harmless at first, but now… I don’t know. Feels wrong.

#speaker: Player
What exactly did he ask you to post?
-> BobConfession

=== BobConfession ===
“Facts” about his opponent. Except I did some digging. Some of it’s… twisted. And some of it? Just flat-out lies, People are eating it up. No one even checks if it’s true. They just share it like it’s gospel. #speaker: Bob

That’s not just shady—that’s harmful maligned information.Harmful maligned information spreads because it feeds emotions—fear, anger, outrage. People react first, think later. #speaker: Player

#speaker: Bob
…Yeah. I don’t want to be part of that.

#speaker: Player
Smart call. A few clicks might seem small, but lies can do real harm.


* [History has shown us that these promises often come with strings attached. Why would this time be any different?] 
    -> LogicalTone

* [I hope so, but can we afford to take that risk when so many people are struggling?] 
    -> EmotionalTone

* [That’s exactly what they want you to think—to make you drop your guard.] 
    -> ArgumentativeTone

* [It’s worth hoping, but let’s make sure we’re not being misled.] 
    -> ReservedTone

=== LogicalTone ===
Yeah… I guess I should think twice before jumping into something just because it sounds good. #speaker: Bob

-> END

=== EmotionalTone ===
Yeah… that’s what’s really messing with me. People trust what they read, and I’m just making it worse, I need to do better. Thanks for the reality check. #speaker: Bob
-> END

=== ArgumentativeTone ===
You really think it’s that calculated?, Never thought about it like that. Guess I need to start paying more attention. #speaker: Bob

#speaker: Player
That’s the only way to stay ahead of it.

#speaker: Bob
Alright. Lesson learned.

-> END

=== ReservedTone ===
Yeah… blind trust never really worked out for anyone, I’ll keep that in mind. Thanks. #speaker: Bob

-> END
