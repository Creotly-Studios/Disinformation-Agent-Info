VAR npcEmotion = "Angry"

#speaker: Player
You alright? You look like something’s eating at you.

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
#speaker: Bob
“Facts” about his opponent. Except I did some digging. Some of it’s… twisted. And some of it? Just flat-out lies.

#speaker: Player
That’s not just shady—that’s harmful maligned information. False info, spread on purpose to sway people.

#speaker: Bob
Yeah… and the worst part? People are eating it up. No one even checks if it’s true. They just share it like it’s gospel.

#speaker: Player
That’s how it works. Harmful maligned information spreads because it feeds emotions—fear, anger, outrage. People react first, think later.

#speaker: Bob
…Yeah. I don’t want to be part of that.

#speaker: Player
Smart call. A few clicks might seem small, but lies can do real harm.

#speaker: Bob
Guess I should return the money. Or… maybe donate it somewhere that actually helps people?

* [History has shown us that these promises often come with strings attached. Why would this time be any different?] 
    -> LogicalTone

* [I hope so, but can we afford to take that risk when so many people are struggling?] 
    -> EmotionalTone

* [That’s exactly what they want you to think—to make you drop your guard.] 
    -> ArgumentativeTone

* [It’s worth hoping, but let’s make sure we’re not being misled.] 
    -> ReservedTone

=== LogicalTone ===
#speaker: Player
History has shown us that these promises often come with strings attached. Why would this time be any different?

#speaker: Bob
Yeah… I guess I should think twice before jumping into something just because it sounds good.

#speaker: Player
It’s always worth questioning the motive behind these things.

#speaker: Bob
Good point. I’ll keep that in mind. Thanks.

-> END

=== EmotionalTone ===
#speaker: Player
I hope so, but can we afford to take that risk when so many people are struggling?

#speaker: Bob
Yeah… that’s what’s really messing with me. People trust what they read, and I’m just making it worse.

#speaker: Player
Exactly. It’s not just about words—it’s about real people being affected.

#speaker: Bob
Yeah… I need to do better. Thanks for the reality check.

-> END

=== ArgumentativeTone ===
#speaker: Player
That’s exactly what they want you to think—to make you drop your guard.

#speaker: Bob
You really think it’s that calculated?

#speaker: Player
It usually is. The whole point is to manipulate people before they even realize it.

#speaker: Bob
Damn. Never thought about it like that. Guess I need to start paying more attention.

#speaker: Player
That’s the only way to stay ahead of it.

#speaker: Bob
Alright. Lesson learned.

-> END

=== ReservedTone ===
#speaker: Player
It’s worth hoping, but let’s make sure we’re not being misled.

#speaker: Bob
Yeah… blind trust never really worked out for anyone, huh?

#speaker: Player
Not really. Hope is good, but facts matter more.

#speaker: Bob
I’ll keep that in mind. Thanks.

-> END
