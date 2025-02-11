VAR playerScore = 0
VAR npcEmotion = "Angry"

Agent: I'm back #speaker:Agent
-> Start

=== Start ===
Ms. Maggie: Impressive, Agent Kim. That’s better than most recruits do. #speaker:Ms. Maggie
Agent: Not bad, huh? Thanks, boss. #speaker:Agent
-> MissionBriefing

=== MissionBriefing ===
Ms. Maggie: You’ve earned your first official mission. From this point forward, you’ll be operating in the field. Here’s your teleportation band. You’ll use it to travel to and from mission sites via specially marked phone booths. #speaker:Ms. Maggie
Agent: Got it. So, where am I headed first? #speaker:Agent
-> AssignmentDetails

=== AssignmentDetails ===
Ms. Maggie: Your first assignment takes you to the town of Yonkle Storntle. It’s election season there, and one of the main candidates is spreading disinformation about their opponents. Your job is to stop the spread by talking to a few people about disinformation. #speaker:Ms. Maggie
Agent: Understood. Yonkle Storntle, talk, disinformation. I’m on it. #speaker:Agent
-> Departure

=== Departure ===
Ms. Maggie: Good. The phone booth is waiting for you. Don’t let us down. #speaker:Ms. Maggie
-> END