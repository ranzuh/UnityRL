# UnityRL

My reinforcement learning experiments with Unity ML Agents Toolkit.

https://github.com/Unity-Technologies/ml-agents

# Usage

Open Project folder in Unity

After project is open, press play button to see trained agent in action.

# Training

First follow these install instructions: https://github.com/Unity-Technologies/ml-agents/blob/release_12_docs/docs/Installation.md

Now you can run
```
mlagents-learn config/lunarlander_config.yaml --run-id=lunarlander
```
and after it says it, press play button in Unity

After training model is saved in a folder called "results".

You can set the model in Unity for LunarLander prefab. In the prefab under "Behavior Parameters" drag the new model to "model".

For better advice read here: https://github.com/Unity-Technologies/ml-agents/blob/release_12_docs/docs/Getting-Started.md
