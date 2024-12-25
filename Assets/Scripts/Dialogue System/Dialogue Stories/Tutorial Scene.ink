VAR playerScore = 0
VAR npcEmotion = "Angry"

Ah, Mr. Wade Kim, welcome to the I.M.A. I'm Miss Maggie #speaker:Maggie
-> Start

=== Start ===
Agent: Hello, Good Day Miss Maggie, I was told to meet you? #speaker:Player

Yes, I'd be your handler at the I.M.A. I'd like to know if you have been briefed? #speaker:Maggie

    * Not Really #speaker:Player
    -> Briefing
    * Sort of... I guess? #speaker:Player
    Alright then, you would need to get through the basic training program to test your levels. #speaker:Maggie
    -> Continue

=== Briefing ===
Well then, let me fill in the blanks. You’ve been drafted into the Information Monitoring and Managing Agency — the I.M.A. #speaker:Maggie

    Agent: Oh, thank you! That was self-explanatory. So, where do I go from here? #speaker:Player
-> Continue

=== Continue ===
Good. Here we use phone booths as our transportaion system. Head over and interact with that phone booth and begin the simulation. Prove yourself, Agent. #speaker:Maggie
-> END