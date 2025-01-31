VAR playerScore = 0
VAR npcEmotion = "Angry"

Agent: Hi there! How’s your day going? #speaker:Agent
-> Start

=== Start ===
NPC1: Oh, hey! Pretty good. Have you heard the news? #speaker:NPC1
Agent: What news? #speaker:Agent
-> NewsHeard

=== NewsHeard ===
NPC1: People are saying the mayor is stepping down because of some big scandal! #speaker:NPC1
Agent: Actually, that’s not true. That rumor’s been proven false. #speaker:Agent
-> RumorProvenFalse

=== RumorProvenFalse ===
NPC1: Really? But so many people are talking about it! My friend even sent me a post about it. #speaker:NPC1
Agent: That’s what makes rumors tricky—they spread quickly, especially when they sound exciting or shocking. People often believe them without checking if they’re true. #speaker:Agent
-> RumorReasons

=== RumorReasons ===
NPC1: But why do people start rumors like that? #speaker:NPC1
Agent: Good question. Sometimes it’s just to get attention. Other times, it’s to confuse people or make them believe something that isn’t true. That’s why we have to be careful. #speaker:Agent
NPC1: So, how do I know what’s true and what’s not? #speaker:NPC1
Agent: Here’s a tip: always check where the information is coming from. If it’s from a trusted source, like a news channel or a well-known website, it’s more likely to be true. #speaker:Agent
-> SocialMedia

=== SocialMedia ===
NPC1: What if it’s something I see on social media? #speaker:NPC1
Agent: Be careful there. Social media is where most rumors spread. If you see something surprising, check if other trusted places are saying the same thing before you believe it. #speaker:Agent
-> CheckEverything

=== CheckEverything ===
NPC1: That makes sense. But isn’t it hard to check everything? #speaker:NPC1
Agent: It can feel like extra work, but it’s important. If we share something that’s not true, it could mislead others or even cause problems. Taking a moment to check can make a big difference. #speaker:Agent
NPC1: Wow, I never thought about it like that. Thanks for explaining! #speaker:NPC1
Agent: You’re welcome! Remember, the truth is worth finding. #speaker:Agent
-> END