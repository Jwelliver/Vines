# - 022323

- Cosmetic progression ?
    - collect different hats/outfits?


- Find & collect artifacts (varying in rarity)
    - bring them back in a bag (various sizes) to sell and buy upgrades
        -upgrade ideas:
            - larger bags?
            - armor: heavier but offers more protection from arrows?
            - hire tribes to take you to more remote/special areas (i.e. unlock areas with higher pct of rare items)

    - roguelike jungle mini-adventure?
        - buy & pack water/food.. manage stamina, hunger, health?
            - go as far as you can, try to find what you can and make it back to sell & upgrade
        - Flesh out ground platformer?
            - random levels with ground and vines?
                - allow player to enter ruins (random dungeon levels?)


=================


Entemologist

===========

- player finds and catches various bugs

- bugs can be sold to universities for research, or sold locally at town village(for food, some remote village reason), and/or studied

- starting with hard-coded mechanics in bugs, but move to modular system so players can have a new experience learning each type of bug each time. (good for roguelike)





- use "running into tribe" as risk
-- randomly determine if player is near tribe
-- play war drums when a tribe is "nearby"
-- player can hide in the palms while the tribe passes.. if not, they are "sighted" and tribe begins firing arrows.

- Rouglike feauture ideas:
    - player can have skills and attributes such as net-handling, aim, stealthiness, etc. which affect how effective they are at
        using catching devices and such.


== random ideas ==
- Perhaps require to bait some insect with some sort of other insect, or product thereof.

- net-gun, that shoots capture net attached to string. fires, then auto retracts with any catch it may have captured.

===
to get started:
    - bug that crawls on vines
    - bug that flies
    - all bugs have modular behaviors:
        - idle state
        - moving state
        - threatened state
        - attack state (optional)
        - make states extensible
    - initial method of catching bugs (handheld net)

== Step by step
- Player swings net
    - On "catch input"
        - enable net
        - play swing animation

- Bug AI
    - spawn randomly at vine segment
    - idle state where bug pauses movement and rotates randomly
    - move state
        - "crawl vision" finds objects that bug can crawl on and provides direction/distance for bug to move along
    - later:
        - add additional sensors and behaviors for "threatened" state and retreat/hide

- Player catches bug
    - net needs script that when trigger overlaps with bug, bug is caught
    - bug object disappears
    - Later:
        - play fading animation of bug icon with +1 ?
        - create inventory system
