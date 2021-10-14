<br/><br/>![wowbot_targets](https://user-images.githubusercontent.com/59726045/137294839-cdab7859-377c-4977-95b8-e707ba8ae717.jpg)
<br/>
<br/>
<br/>
In the previous version, I explained how I gathered information from the game screen using an add-on we designed for this purpose.
<br/><br/><br/><br/>
for the farming purpose, I needed a path of coordination I wanted to my character go as its route.<br/>
So i designed a path recorder class that could record the character path and save the list of coordinations in a .txt file.
<br/><br/>
![wowbot_path](https://user-images.githubusercontent.com/59726045/137294928-be9ea50b-08fb-4316-9f86-812547cc9ac2.png)
<br/><br/>
After gathering those information ,i had x,y,facing and all data we needed.<br/>
I could press the 'W' key to go forward and use the mouse or 'D', 'A' keys to rotate the character's facing angle however still had to find a way to go to the each position.<br/>
So I designed a class that gives me angle numbers and counter/clockwise to turn to go to the next coordination.
<br/><br/>![collapse-quest-log-sc1](https://user-images.githubusercontent.com/59726045/137338215-cc8223eb-3193-42b8-886a-05a608d4b5d1.png)
<br/><br/><br/><br/>
Turning the character's camera keys was too slow, so I figured out a way to get the number of pixels for the mouse to change place to turn the character's face. 
<br/><br/>
After these steps, I had a great farming bot that could play game continuously for days instead of me.<br/>
But because I used a toy in the game to collect stuffs that was dropped from corps, the character had to stop at specific positions in order to collect stuffs and that caused the lack of performance in the farming process!<br/><br/>
To avoid those continuous stops I used the mouse as a radar that moved in a circle around my character so by reading the mouse icon by using some window API classes, I could see when the icon changes, and then I could decide what action to take...
<br/><br/>

![wowbot_Loot](https://user-images.githubusercontent.com/59726045/137294673-6e29a970-cc33-4658-b207-da43912d0237.gif)

