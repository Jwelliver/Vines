090823

If the player randomly stops moving on start and you can't figure out why.. check the animator.

- to test if the animator is related to the issue, check the Apply Root Motion box;

  - this may get the player walking and responding to gravity again, but will not allow momentum when jumping.

- ensure there are no unused animations sitting in there.

  - this turned out to be the fix at least one of the times.

- the other time, I'm not sure what it was. I discarded changes in git, then restored them and it was fixed.. so idk.
