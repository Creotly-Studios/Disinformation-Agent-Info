VAR npcEmotion = "Neutral"

Agent: Yo! #speaker:Agent
-> Start

=== Start ===
Politician: "Who are you? How'd you get into my office?" #speaker:Agent
-> AgentReaction0

=== AgentReaction0 ===
Agent: "This might sound cringe, but I'm I.M.A, here to stop you from spreading disinformation" #speaker:Agent
-> PoliticianResponse

=== PoliticianResponse ===
Politician: "Oh, you’re the one trying to stop this? You’ve got it all wrong. I’m just having a little fun. Spreading a few rumors about the mayor to make sure my guy wins. It’s all part of the game, really." #speaker:Politician
-> AgentReaction1

=== AgentReaction1 ===
Agent: "This isn’t a game! People are actually believing these lies. You’re turning an election into a circus." #speaker:Agent
-> PoliticianReaction1

=== PoliticianReaction1 ===
Politician: "And what’s the harm in that? Everyone does it—spinning the truth a little to get ahead. Why not have a little fun while I’m at it? It’s just politics." #speaker:Politician
-> AgentReaction2

=== AgentReaction2 ===
Agent: "It’s more than just politics. You’re spreading disinformation, and that’s dangerous. People trust what they read, and when they’re fed lies, they make decisions based on those lies. You’re messing with people’s lives, their choices." #speaker:Agent
-> PoliticianReaction2

=== PoliticianReaction2 ===
Politician: "Come on, you’re really going to tell me that a few rumors here and there are going to ruin everything? It’s just some harmless fun." #speaker:Politician
-> AgentReaction3

=== AgentReaction3 ===
Agent: "It might seem harmless now, but think about the bigger picture. When people can’t trust what they hear anymore, everything gets messy. The truth gets lost, and the system becomes a joke. You’re not just playing around—you’re messing with the foundation of trust." #speaker:Agent
-> PoliticianReaction3

=== PoliticianReaction3 ===
Politician: "But what’s the point of all this if I don’t win? My guy deserves to take this, and if I’ve got to bend a few facts to make it happen, so be it." #speaker:Politician
-> AgentReaction4

=== AgentReaction4 ===
Agent: "You don’t need to manipulate things to win. You can win by showing people your candidate’s real strengths, not by tearing someone else down with lies. You’re better than that." #speaker:Agent
-> PoliticianRealization

=== PoliticianRealization ===
Politician: "I never really thought about it like that… Maybe I’ve been too focused on the game and not the consequences." #speaker:Politician
-> AgentFinal

=== AgentFinal ===
Agent: "It’s not too late to change. You don’t have to use lies to win—people will respect you more if you play fair." #speaker:Agent
-> PoliticianChange

=== PoliticianChange ===
Politician: "Alright, alright. I get it now. I guess I got carried away. I’ll stop with the lies. No more disinformation. Time to do it the right way." #speaker:Politician
-> End

=== End ===
Agent: "Thanks. That’s the kind of change we need." #speaker:Agent
Politician: "Guess I was playing dirty without even realizing it. No more tricks, just the truth." #speaker:Politician
-> END