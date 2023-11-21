# InstagramMessageConverter
Convert your Instagram message Jsons to a more readable formant.

Donload: https://beanmaster0790.itch.io/instagram-message-converter

Absolutely thrilled about the prospect of reliving those memorable Instagram conversations? Ever found yourself on a nostalgia trip, scrolling tirelessly to the top of your messages, only to be met with frustrating loading times every few scrolls?
We get it—waiting 5-15 seconds for more messages can be agonizing, especially when you have a treasure trove of memories with that special someone.

But fear not! Say goodbye to the sluggish scroll and endless loading screens. Introducing our game-changing tool that lets you retrieve all your cherished messages in a matter of seconds.
That's right, seconds! All it takes is a mere 15 minutes of setup, and you'll have instant access to your entire message history, ready to evoke those sentimental moments whenever you desire. 
(Thx GPT for that description!)

Story:

I wanted to see my old messages with my girlfried but was very dissapointed when I had to sit there waiting for 10 messages to load every few seconds so I looked online 
for a way to see all messages without scrolling for hours because of a loading screen. The most common solution was to download your data from meta and read from the jsons provided.
While that worked fine it was kinda overly complicated when I just wanted to go on a nostalgia trip. So I coded the first version of this project that only decoded me and my gfs 
messages. But then my mate sudgessted I should make it more intuitive so other people could use it. And now here we are.

How it works:

1. Asks the user to find the 'message_1' json then uses that directory to find how many other jsons are needed for that chat.

2. Once it has found out how many jsons are needed it opens each one in decending order and sorts the messages inside each json in decending order aswell.

3. After everything is sorted the program adds each message to a list and then starts writing to the file.

4. While writing the file the program will ask the user to give each username in the file a nickname as some usernames may be undesirable. (This process does not use the 'participants' list in the json as in group chats if a person has left the chat they are no longer in that list which causes errors)

5. Before writing each line has to be converted to UTF8 as if not emojis will not be shown at all.
