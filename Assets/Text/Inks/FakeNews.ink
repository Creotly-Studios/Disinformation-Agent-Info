VAR playerScore = 0
VAR npcEmotion = "Angry"

Agent: Hi there! How’s everything going today? #speaker:Agent
-> Start

=== Start ===
NPC2: Oh, hey! Not bad. Say, have you heard the news? #speaker:NPC2
Agent: What news? #speaker:Agent
-> NewsHeard

=== NewsHeard ===
NPC2: Apparently, the mayor is moving to Mars because Earth taxes are too high! #speaker:NPC2
Agent: Mars? That’s a new one. But nope, that’s not true. #speaker:Agent
-> RumorReaction

=== RumorReaction ===
NPC2: Really? But my cousin’s neighbor’s best friend’s dog saw it online! #speaker:NPC2
Agent: Let me guess—the dog read it on BarkBook? #speaker:Agent
-> FactCheckingIntro

=== FactCheckingIntro ===
NPC2: No, but it was on a post with a lot of likes! How can you be so sure it’s fake? #speaker:NPC2
Agent: Let me teach you how to fact-check. It’s easy once you get the hang of it. #speaker:Agent
NPC2: Okay, I’m listening! #speaker:NPC2
-> FactCheckingSteps

=== FactCheckingSteps ===
Agent: Step one: Check the source. Who’s sharing this news? Is it a trusted news outlet or an official website? If it’s just a random account, you should be cautious. #speaker:Agent
NPC2: Alright. So, if it’s from a big news site, it’s probably true? #speaker:NPC2
Agent: Not always, but it’s a good start. Even then, step two is to find other sources. Are multiple reliable places reporting the same thing? If only one random post says it, it’s suspicious. #speaker:Agent
-> EvidenceCheck

=== EvidenceCheck ===
NPC2: That makes sense. What’s next? #speaker:NPC2
Agent: Step three: Look for evidence. Does the post provide proof, like links to official announcements, interviews, or photos? If it just says, ‘Sources say,’ that’s a red flag. #speaker:Agent
NPC2: Got it. Anything else? #speaker:NPC2
Agent: Step four: Check the date. Sometimes old news gets shared as if it’s new, confusing people. #speaker:Agent
-> EmotionalHeadlines

=== EmotionalHeadlines ===
NPC2: Oh wow, I’ve fallen for that before! #speaker:NPC2
Agent: It happens to everyone. Last step: Watch out for emotional headlines. If it’s written to make you angry, scared, or shocked, it’s often clickbait. #speaker:Agent
NPC2: So, don’t let my feelings do the thinking? #speaker:NPC2
Agent: Exactly! Use your brain, not your gut. Fact-checking is like detective work—look for clues, don’t jump to conclusions. #speaker:Agent
-> Conclusion

=== Conclusion ===
NPC2: Wow, this is actually fun! Thanks for showing me how to do this. #speaker:NPC2
Agent: No problem! Now you can sniff out fake news better than your cousin’s neighbor’s dog. #speaker:Agent
-> END