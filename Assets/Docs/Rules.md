# Documentation of rules to follow

### Static Instance Initializations and Reference to them
All static Instances that are to be created need to be assigned on the `Awake()` method.
The reason behind it is that we don't get a null error when getting a yet to declare Instance.