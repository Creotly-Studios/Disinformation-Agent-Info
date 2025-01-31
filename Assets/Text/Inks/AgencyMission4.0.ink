VAR playerScore = 0
VAR npcEmotion = "Neutral"

Agent: "Hey, Ms. Maggie. You called me in... again?" #speaker:Agent
-> Start
=== Start ===
Ms. Maggie: "Yes, AGENT. We've gotten the Address of the Politian spreading disinfo in Yornkle." #speaker:Ms. Maggie
-> SituationExplained

=== SituationExplained ===
Agent: "So?" #speaker:Agent
Ms. Maggie: "We need you to talk to Him, try convince Him what He's doing is not good... Before the higher up's get involved" #speaker:Ms. Maggie
-> FinalWords

=== FinalWords ===
Agent: "Hmmmm, Sure." #speaker:Agent
Ms. Maggie: "Well, go quickly." #speaker:Ms. Maggie
-> END