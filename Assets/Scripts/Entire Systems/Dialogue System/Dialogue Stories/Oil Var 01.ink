=== start ===
# You approach the NPC, trying to talk sense into them about the oil drilling project.

#speaker: Player  
Hey, have you seen the recent news about the government's oil drilling project? They're claiming it’s a step toward energy independence, but there’s something off about it.

* [It makes no sense. We can't sacrifice the environment for temporary gains.] -> logical_tone
* [Don’t you feel worried? It could destroy everything we care about.] -> emotional_tone
* [But think about it. They’re not showing the full picture. There's more at stake here than just energy.] -> argumentative_tone
* [I understand how you feel, but maybe we need more time to process this.] -> reserved_tone

=== logical_tone ===
#speaker: Player  
It makes no sense. We can't sacrifice the environment for temporary gains. The risks outweigh the benefits, and there are better, cleaner alternatives we could pursue.

#speaker: other  
Hmm, but energy independence is crucial. Aren’t we too reliant on other countries? Maybe we have to take risks.

* [It's about the future of our planet, not just short-term solutions.] -> logical_tone
* [I understand that, but have you thought about the long-term damage?] -> emotional_tone
* [If we start down this road, there will be no turning back. The damage to the environment will be irreversible.] -> argumentative_tone
* [We might be jumping to conclusions. Let’s gather more information.] -> reserved_tone

=== emotional_tone ===
#speaker: Player  
Don’t you feel worried? It could destroy everything we care about. The wildlife, the water, the air—it’s all at risk. Is that worth it?

#speaker: other  
I get that, but we need the energy. The world is changing fast, and we have to keep up.

* [We can keep up without destroying everything. There are other solutions.] -> logical_tone
* [But what about the people living near the drilling sites? How would they feel if their homes are destroyed?] -> emotional_tone
* [We should question the government’s motives here. This isn’t just about energy—it’s about control.] -> argumentative_tone
* [I just want you to think this through. Take some time before making up your mind.] -> reserved_tone

=== argumentative_tone ===
#speaker: Player  
But think about it. They’re not showing the full picture. There's more at stake here than just energy. This project is about corporate interests, not the common good.

#speaker: other  
But if we don’t do something now, we could fall behind other nations. How do we stay competitive without new resources?

* [At what cost? We can’t sacrifice the planet for power. That’s not true progress.] -> logical_tone
* [I know you're worried, but sacrificing everything else for energy? That’s not a fair trade.] -> emotional_tone
* [What if the real goal isn’t energy independence, but something else entirely? Maybe it's about power, control, and profit.] -> argumentative_tone
* [Maybe we don’t have all the facts yet. I’ll reserve judgment until we do.] -> reserved_tone

=== reserved_tone ===
#speaker: Player  
I understand how you feel, but maybe we need more time to process this. Let’s gather more information before jumping to conclusions.

#speaker: other  
You might be right. We need to know the full story before making any decisions. But I still believe we need the energy.

* [It’s okay to be unsure, but just keep an open mind. The truth might surprise you.] -> logical_tone
* [I hope you’ll reconsider when you see the bigger picture. It’s not just about energy—it’s about our future.] -> emotional_tone
* [Sometimes it’s hard to see the truth, but we can’t ignore it. We need to question everything.] -> argumentative_tone
* [Take your time. I’m not here to rush you, just to help you see the full picture.] -> reserved_tone