VAR playerName = ""
This is some random ass dialogue stuff
Stop! Who would cross the bridge of death must answer me these questions three, ere the other side he see.
->Choices

==Choices==
* [Ask me the questions bridge keeper, im not afraid!] -> Name

==Name==
What, is your name?
* [Sir Lancelot of Camelot]
~ playerName = "Lancelot"   //tilde ~ sets variable values
->Quest
* [Sir Robin of Camelot]
~ playerName = "Robin"
-> Quest
* [Sir Galahad of Camelot]
~ playerName = "Galahad"
-> Quest
* [It is Arthur, King of the Britons]
~ playerName = "Arthur, King of the Britons"
-> Quest


==Quest==
What is your quest?
* [To seek the holy grail]
-> END