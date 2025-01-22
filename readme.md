### README

---

# AIML Group 20: Soccer Environment with ML-Agents

**Group Members:**  
Malo Coquin, Maarten Kooij, Chrystalla Fella, Laurens Heithecker, Aukje Heijthuijzen, Cristina Stroescu

## Overview  
This project is a soccer simulation environment built with Unity ML-Agents. Agents are trained to play soccer by learning to score goals, interact with the ball, and work as a team using reinforcement learning.

---

## How to Run  
1. Open the scene **`SoccerTemp`** located in:  
   `Assets/ML-Agents/Examples/Soccer/Scenes/`
2. Run the simulation in Unity's Play mode.

---

## How to Train
To train the agents, export the scene to an executable and run it outside of unity for best performance.
An instruction can be found here: https://github.com/DennisSoemers/ml-agents/blob/fix-numpy-release-21-branch/docs/Learning-Environment-Executable.md

---

## Modified Scripts  

### **AgentSoccer.cs**  
Controls the behavior of soccer agents, including movement, ball interaction, and rewards. It assigns roles to agents (Striker, Goalie, or Generic) and handles their actions during the game.

### **Creator.cs**  
Generates sound objects in the environment to simulate feedback for events like ball collisions. These sounds help guide agents' decisions.

### **SoccerBallController.cs**  
Manages interactions between the ball and agents or goals. It tracks ball touches and identifies scoring events.

### **SoccerEnvController.cs**  
Oversees the entire simulation, including score tracking, resetting the scene, and assigning rewards or penalties to agents based on their actions.
