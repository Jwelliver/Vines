# gameMode: EndlessMode

- EndlessMode normal (same settings across the entire level):

  - [] Setup Reusable Sections
    - [] Create a struct/class that stores the state of a placed object
      - Needs everything to place it back where it was and reapply the settings.
      - If you struggle with this for too long (>5 hours), abandon, and use a method that either replaces the sections at random, or force the player not to go backward (this is endless mode, the only direction should be forward);
        - You can use Arrows that get more accurate as player gets closer to the section they shouldn't be in.
        - Note: The Suggested system will allow for other modes of gameplay where the player may be allowed/required to go one direction or the other.
    - [] Add all Created objects to a Queue in their section/Chunk
    - [] Implement system that recycles the objects from a Section sufficiently "behind" the player and replaces the objects in front; Or Instantiates them.
  - [] Implement Game Mode Option menu

- Endless With Progressive difficulty:
  - [] Complete Level Progression task;
  - [] Complete EndlessMode
