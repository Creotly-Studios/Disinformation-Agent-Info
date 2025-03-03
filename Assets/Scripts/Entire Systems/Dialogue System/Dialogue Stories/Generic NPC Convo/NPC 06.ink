VAR npcEmotion = "Angry"

#speaker: NPC  
Is it just me, or does the sky look weird today?  

#speaker: Player  
What do you mean?  

#speaker: NPC  
Like… different. Some guy said on a forum that the government’s messing with the clouds.  

* [Or, y’know… it’s just weather.] -> DismissTheory  
* [What exactly did the forum say?] -> DigDeeper  
* [Sounds crazy, but I wouldn’t put it past them.] -> EntertainIdea  

=== DismissTheory ===  
#speaker: Player  
Or, y’know… it’s just weather.  

#speaker: NPC  
Pfft. You believe what you want. I’m keeping my eye on the sky.  

-> END  

=== DigDeeper ===  
#speaker: Player  
What exactly did the forum say?  

#speaker: NPC  
Something about chemical trails changing the air. Said they tested the rainwater.  

#speaker: Player  
That sounds like a reach. Any actual evidence?  

#speaker: NPC  
Uh… I dunno. I just thought it was interesting.  

-> END  

=== EntertainIdea ===  
#speaker: Player  
Sounds crazy, but I wouldn’t put it past them.  

#speaker: NPC  
Right?! That’s what I’m saying! We should be paying more attention.

#speaker: Player  
Yeah… but don’t believe everything you read.  

#speaker: NPC  
Fair. But I’ll still keep an eye out.  

-> END  