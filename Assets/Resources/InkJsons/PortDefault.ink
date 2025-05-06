VAR money = 0
VAR repairCost = 0

Welcome To Port Thames and Fawkes! #animate
Have at me if you need anything.#animate

->Choices

==Choices==
* [Upgrades] -> Upgrades
* [Repairs] -> Repair

==Upgrades==
Take a look, it'll blow your mind, and your enemies' brains out!
->END

==Repair==
{money >= repairCost: 
All fixed up! Now get out there and smash it!

- else:
Sorry, but you're up short.
Maybe bring your wallet next time and i'll think about it!
}
->END
