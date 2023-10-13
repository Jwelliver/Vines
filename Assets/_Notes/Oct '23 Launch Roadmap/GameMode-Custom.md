# GameMode: Custom

- [] Finish LevelSettingsUIController
- [] Ability to Set game Seed
- [] Add to game mode UI

- [] Consider which controls the user should have for the level settings; E.g. at the most abstract, we could just add "difficulty: easy, medium hard" and "length: short, medium, long";

- [] OPTIONAL: Implement UI on the GameMain scene so player can set the settings and preview the level, and then easily drop in to play

  - To Preview:

    - Generate a small section (maybe 100 units wide, and either use a viewport to show a camera view)
    - OR, Just allow player to easily transition between the UI and the generated level behind it.

  - To Play:
    - Generate the entire level, then Start as usual.
      - NOTE: this may require a cross fade as the level loads (to account for large sizes)
