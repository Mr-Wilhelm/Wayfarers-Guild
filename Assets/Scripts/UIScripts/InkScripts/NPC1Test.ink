VAR playerName = ""
Stop!
Who would cross the bridge of death must answer me these questions three, ere the other side he see.
->Choices

==Choices==
* [Ask me the questions bridge keeper, im not afraid!] -> Name

==Name==
What, is your name?
* [Sir Lancelot of Camelot]
~ playerName = "Lancelot"   //tilde ~ sets variable values
->Lancelot
* [Sir Robin of Camelot]
~ playerName = "Robin"
-> Robin
* [It is Arthur, King of the Britons]
~ playerName = "Arthur, King of the Britons"
-> Arthur

==Lancelot==
You are Sir Lancelot
-> END

==Robin==
You are Sir Robin
-> END

==Arthur==
My Leige!
-> END