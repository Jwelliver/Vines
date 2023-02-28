# TODO FINAL


Game:
	- add win text and move to credits.
	

UI:
	
	- Font added for UI text
	- Start Menu decorated (add foliage and vines from the Title)
	- Add Credits and story about how/why we made the game

Performance:
	- review vine length
	- locate memory leak
	- look into seralized obj issue
	- disable RB and light or gameobject out of sight.

-PostProcessing



=======
# if there's time
========

	- IF THERE'S TIME:
		- arrows after getting amulet
		- random death result text
		- animate foliage
		- player: when swinging, disable additional movement forces above a given velocity
		- player: try auto jump on release of vine grab (might make smoother traversal)
		- fix grab point on player
		- make vines with weak points brown
		- add tree variants
		- retrieve amulet
		




================
# DONE #
===============

- GOAL IMPLEMENTED (Done)
- populate ground foilage
	- Fail
		- camera halt
		- You Died text
		- death sound
		- water splash effect & sound
		- floating hat anim
- JUNGLE SOUND & MUSIC CONTINUOUS ACROSS SCENES
	- add music gameobj with DontDestroyOnLoad script

- text shares same color across all instances. - fix! (DONE?)
- Add wearable amulet to player (DONE)
================
# SKIPPED #
===============
	- Stamina and/or Enemy
	

	- locate memory leak
	

Saturday: 
	- MENUS


=================================

# 022123

- fix/prevent infinite jump while singing
- RAYCAST LIGHT SHAFTS for length
- gracefully reorient when landing on platform sideways
- endless mode
	- button on main menu
	- level generators need to generate themselves
		- probs need to consolidate foliage and paralax generators into level gen script (or parent script)

- test rigid body legs
- find  out about flipping rb with transform 
- test 

# 022423

- copy existing anims and rename for physics player
	- duplicate existing anims and re-construct for orig player (may need to tweak/re-add leg positions)

- To deal with legs.. consider trying two sets.. one normal for anims, and a second for physics.. just enable/disable on the ground