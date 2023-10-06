# Cloud Player Data Persistance

- [] Determine Approach:

  - Option 1: Free "Leaderboard" based data
    - [] Setup save and load to leaderboard
  - Option 2: Aws-based
    - [] Figure out calling links from ingame
    - [] Setup the backend (API + Lambda + Dynamo)

- [] Setup Player Accounts

  - [] Add UI to create account; Player Name + Password
  - [] Add UI to view Player accounts; Will include Name, visual customizations and stats
    - Player's Account may be viewed via a main menu upper right link which also acts as the "signed in status" indicator.

- [] OPTIONAL: Challenges:
  - Endless Mode Challenge (distance + time)
  - Out and back Challenge (time)
  - [] Set up UI to browse challenge page
    - Will have list of Player-completed levels containing the seed, information about the level (type and difficulty coefficent), and option to Challenge
      - Each item will also contain a list of Attempted challengers, their time, and whether or not they were beaten
      - Maybe just Organize the list by Seed, and then include the associated player run data on each, So, like a mini leader board for each type and seed ?
    - Challenge Will load the associated seed to allow the player to challenge;
    - Results will then be added to the Data for that seed db item
  - [] OPTIONAL: Allow Player to "rank" the run, enabling it to be auto-uploaded on completion
    - [] OPTIONAL: may want to consider adding a start timer
