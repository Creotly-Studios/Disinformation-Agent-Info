VAR playerScore = 0
VAR npcEmotion = "Angry"

Ah, Mr. Wade Kim, welcome to the I.M.A. #speaker:Maggie
-> Start

=== Start ===
Hello, Good Day Mrs Maggie, I was told to meet you? #speaker:Player

Yes, I'd be your handler at the I.M.A. I'd like to know if you have been briefed? #speaker:Maggie

    * Not Really #speaker:Player
    -> Briefing
    * Sort of... I guess? #speaker:Player
    Alright then, you would need to get through the basic training program to test your levels. Remember, the target score is 65 — anything below that and you don't get in. #speaker:Maggie
    -> Continue

=== Briefing ===
Well then, let me fill in the blanks. You’ve been drafted into the Information Monitoring and Managing Agency — the I.M.A. #speaker:Maggie

    Oh, thank you! That was self-explanatory. So, where do I go from here? #speaker:Player
-> Continue

=== Continue ===
Good. Head through that door over there and begin the simulation. Prove yourself, Agent. #speaker:Maggie
-> Simulation

=== Simulation ===
Impressive, Agent Kim. You scored a solid {playerScore}. That’s better than most recruits. #speaker:Maggie #stage:Gameplay
-> END
